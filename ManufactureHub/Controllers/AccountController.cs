using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using ManufactureHub.Data;
using ManufactureHub.Models;
using ManufactureHub.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ManufactureHub.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ILogger<AccountController> logger;
        private readonly IEmailSender emailService;

        private const string UrlToDefaultAvatar = "user.svg";

        private string[] validExtensionsForUpload = [".png", ".jpg", ".jpeg"];

        public AccountController(ManufactureHubContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, RoleManager<ApplicationRole> roleManager, IEmailSender emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
            this.emailService = emailService;
        }

        //GET: User
        public async Task<IActionResult> Index()
        {
            if (userManager.Users != null)
            {
                var users = await userManager.Users.OrderBy(us=>us.SurName).ToListAsync();
                var userRoles = new Dictionary<int, IList<string>>();
                var ListRoles = new List<string>();

                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    ListRoles.Clear();

                    if (roles == null)
                    {
                        userRoles[user.Id] = new List<string> { "Err. No roles" };
                        continue;
                    }

                    foreach (var item in roles)
                    {
                        var rolele = await roleManager.FindByNameAsync(item);
                        if (rolele != null)
                        {
                            ListRoles.Add(rolele.RoleName);
                        }
                        userRoles[user.Id] = new List<string>(ListRoles);
                    }
                }

                ViewBag.UserRoles = userRoles;

                return View(users);
            }
            else
            {
                return Problem("Entity set 'userManager.Users' is null.");
            }
        }


        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || userManager.Users == null)
            {
                return NotFound();
            }

            var userAccountViewModel = await userManager.FindByIdAsync(id.ToString());
            if (userAccountViewModel == null)
            {
                return NotFound("Юзер заданого id не знайдений");
            }

            if (User.Identity is null)
            {
                return Unauthorized();
            }

            if (User.Identity.Name is null)
            {
                return BadRequest();
            }

            var uussserrri = await userManager.FindByEmailAsync(User.Identity.Name);
            if (uussserrri is null)
            {
                return NotFound();
            }
            ViewBag.UserIdVisitor = uussserrri.Id;

            var roleResult = await userManager.GetRolesAsync(userAccountViewModel);

            var roles = await userManager.GetRolesAsync(userAccountViewModel);
            if (roles == null)
            {
                return NoContent();
            }

            var ListRoles = new List<string>();
            foreach (var item in roles)
            {
                var rolele = await roleManager.FindByNameAsync(item);
                if (rolele != null)
                {
                    ListRoles.Add(rolele.RoleName);
                }
            }

            ViewBag.UserRoles = ListRoles;
            return View(userAccountViewModel);
        }

        //GET: User/Login
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        // GET: User/Login
        //[AllowAnonymous]
        //public async Task<IActionResult> Login()
        //{
        //    ApplicationRole Role = new ApplicationRole()
        //    {
        //        RoleName = "Адмін",
        //        Name = "Admin",
        //        Description = "Відповідає за управління, контроль та підтримку роботи системи, організації, мережі або інфраструктури. Також, відповідає за функціонування веб-сайту, включаючи оновлення контенту, технічну підтримку та забезпечення безпеки",
        //        RoleEnum = Roles.Admin,
        //    };

        //    var resRole = await roleManager.CreateAsync(Role);

        //    Role = new ApplicationRole()
        //    {
        //        RoleName = "Директор виробництва",
        //        Name = "HeadFacility",
        //        Description = "Відповідає за організацію, управління та контроль виробничих процесів на підприємстві. Є ключовою в галузі виробництва, оскільки директор виробництва забезпечує ефективне функціонування всіх виробничих підрозділів, дотримання технологічних стандартів, своєчасне виконання замовлень та досягнення встановлених цілей.",
        //        RoleEnum = Roles.HeadFacility,
        //    };

        //    resRole = await roleManager.CreateAsync(Role);

        //    Role = new ApplicationRole()
        //    {
        //        RoleName = "Логістика",
        //        Name = "LogisticTeam",
        //        Description = "Займається організацією та оптимізацією потоків матеріалів, сировини, готової продукції та інших ресурсів, необхідних для ефективного функціонування виробництва. Основна мета логістика виробництва полягає в забезпеченні безперебійного постачання, мінімізації витрат та підвищенні ефективності виробничих процесів.",
        //        RoleEnum = Roles.LogisticTeam,
        //    };

        //    resRole = await roleManager.CreateAsync(Role);

        //    Role = new ApplicationRole()
        //    {
        //        RoleName = "Керівник цеху",
        //        Name = "TeamLeadWorkstation",
        //        Description = "Керує роботою цеху, відповідає за організацію виробничих процесів, виконання планових завдань, якість продукції та ефективне використання ресурсів. Є ключовою в структурі виробництва, оскільки керівник цеху безпосередньо впливає на результативність роботи підрозділу та досягнення загальних цілей підприємства.",
        //        RoleEnum = Roles.TeamLeadWorkstation,
        //    };

        //    resRole = await roleManager.CreateAsync(Role);

        //    Role = new ApplicationRole()
        //    {
        //        RoleName = "Керівник дільниці",
        //        Name = "TeamLeadSection",
        //        Description = "Відповідає за організацію та управління роботою конкретної дільниці (виробничої зони, цеху або відділу) у межах підприємства. Забезпечує виконання виробничих завдань, дотримання технологічних процесів, контроль якості продукції та ефективне використання ресурсів на своїй дільниці.",
        //        RoleEnum = Roles.TeamLeadSection,
        //    };

        //    resRole = await roleManager.CreateAsync(Role);

        //    Role = new ApplicationRole()
        //    {
        //        RoleName = "Робітник",
        //        Name = "Worker",
        //        Description = "Забезпечує процес обробки деталей на верстатах з програмним керуванням.",
        //        RoleEnum = Roles.Worker,
        //    };

        //    resRole = await roleManager.CreateAsync(Role);

        //    ApplicationUser user = new ApplicationUser()
        //    {
        //        Name = "Арсеній",
        //        SurName = "Норков",
        //        PatronymicName = "Сергійович",
        //        ProfilePicture = "user.svg",
        //        Email = "norkov900@gmail.com",
        //        UserName = "norkov900@gmail.com",
        //        Position = "Системний адміністратор",
        //        EmploymentDate = DateTime.Today,
        //        LastLoginDate = DateTime.Now,
        //        LastLoginIP = "0.0.0.0"
        //    };

        //    var result = await userManager.CreateAsync(user, "Arsen2003");
        //    //await userManager.SetUserNameAsync(user, userAccountViewModel.Email);
        //    //await userManager.SetEmailAsync(user, userAccountViewModel.Email);
        //    var identityResultRole = await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
        //    await signInManager.SignInAsync(user, isPersistent: false);

        //    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login([Bind("Email,Password,RememberMe")] LoginModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await userManager.FindByNameAsync(model.Email);
                if (user is null)
                {
                    ModelState.AddModelError("", "Невдала спроба ввійти, неправильний пароль або пошта");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(model.Email!, model.Password!, model.RememberMe, false);
                if (result.Succeeded)
                {
                    logger.LogInformation("User logged in.");
                    if (returnUrl is null)
                    {
                        return RedirectToAction(nameof(TaskController.Index), "Task");
                    }
                    else
                    {
                        return LocalRedirect(returnUrl);
                    }
                }

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Можливість входу до аккаунту заборонений");
                    return View(model);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout");
                }

                //var user = await userManager.FindByNameAsync(model.Email!) ?? await userManager.FindByEmailAsync(model.Email!);

                //var resultCheck = await userManager.CheckPasswordAsync(user, model.Email!);

                //if (resultCheck)
                //{
                //    await signInManager.SignInAsync(user, model.RememberMe);
                //}

                //return StatusCode(StatusCodes.Status401Unauthorized, "Incorrect username or password");
                ModelState.AddModelError("", "Невдала спроба ввійти, неправильний пароль або пошта");
            }
            return View(model);
        }

        // GET: User/Create
        [Authorize(Roles = "Admin,HeadFacility")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> Create([Bind("Name,SurName,PatronymicName,Role,Email,Position,Password,ConfirmPassword")] RegisterModel userAccountViewModel, string? returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                string? pathToImage = "user.svg";

                ApplicationUser user = new ApplicationUser()
                {
                    Name = userAccountViewModel.Name!,
                    SurName = userAccountViewModel.SurName!,
                    PatronymicName = userAccountViewModel.PatronymicName!,
                    ProfilePicture = pathToImage,
                    Email = userAccountViewModel.Email!,
                    UserName = userAccountViewModel.Email!,
                    Position = userAccountViewModel.Position!,
                    EmploymentDate = DateTime.Today,
                    LastLoginDate = DateTime.Now,
                    LastLoginIP = "0.0.0.0",
                    EmailConfirmed  = true,
                };

                var result = await userManager.CreateAsync(user, userAccountViewModel.Password!);
                //await userManager.SetUserNameAsync(user, userAccountViewModel.Email);
                //await userManager.SetEmailAsync(user, userAccountViewModel.Email);

                if (result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");
                    var resultRole = await roleManager.FindByNameAsync(userAccountViewModel.Role.ToString());

                    IdentityResult identityResultRole;
                    if (resultRole is null)
                    {
                        logger.LogError($"Fail to find role {userAccountViewModel.Role.ToString()}, adding user to role Worker");
                        identityResultRole = await userManager.AddToRoleAsync(user, Roles.Worker.ToString());
                    }
                    else
                    {
                        identityResultRole = await userManager.AddToRoleAsync(user, resultRole.Name);
                        if (!identityResultRole.Succeeded)
                        {
                            logger.LogError($"Fail to add user to role {userAccountViewModel.Role.ToString()}, adding user to role Worker");
                            identityResultRole = await userManager.AddToRoleAsync(user, Roles.Worker.ToString());
                        }
                    }

                    //await signInManager.SignInAsync(user, isPersistent: false);
                    //return LocalRedirect(returnUrl);

                    //await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(AccountController.Index), "Account");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(userAccountViewModel);
        }

        // GET: User/Edit/5
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || userManager.Users == null)
            {
                return NotFound();
            }

            var accountUser = await userManager.FindByIdAsync(id.ToString());
            if (accountUser == null)
            {
                return NotFound("Юзер заданого id не знайдений");
            }

            var userRole = await userManager.GetRolesAsync(accountUser);
            var identityRole = await roleManager.FindByNameAsync(userRole[0]);

            EditModel editModel = new EditModel()
            {
                Id = accountUser.Id,
                Name = accountUser.Name,
                SurName = accountUser.SurName,
                PatronymicName = accountUser.PatronymicName,
                Position = accountUser.Position,
                Role = identityRole.RoleEnum,
                //ProfilePicture = accountUser.ProfilePicture,
            };

            return View(editModel);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name,SurName,PatronymicName,Role,Position")] EditModel editModel)
        {
            if (id != editModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id.ToString());

                if (user is not null)
                {
                    var resultRoleFromEdit = await roleManager.FindByNameAsync(editModel.Role.ToString());
                    var resultsRolesFromUser = await userManager.GetRolesAsync(user);
                    var resultRoleFromUser = await roleManager.FindByNameAsync(resultsRolesFromUser[0]);

                    if (resultRoleFromEdit != resultRoleFromUser)
                    {
                        IdentityResult identityResultRole;
                        var resultRemove = await userManager.RemoveFromRoleAsync(user, resultRoleFromUser.Name);
                        if (!resultRemove.Succeeded)
                        {
                            ModelState.AddModelError("", "Fail to remove user from Role");
                            return View(editModel);
                        }
                        var resultAdd = await userManager.AddToRoleAsync(user, resultRoleFromEdit.Name);
                        if (!resultAdd.Succeeded)
                        {
                            logger.LogError($"Fail to add user to role {resultRoleFromEdit.Name}, adding user to role Worker");
                            identityResultRole = await userManager.AddToRoleAsync(user, Roles.Worker.ToString());
                        }
                    }

                    user.Name = editModel.Name;
                    user.SurName = editModel.SurName;
                    user.PatronymicName = editModel.PatronymicName;
                    user.Position = editModel.Position;

                    var resultupdate = await userManager.UpdateAsync(user);
                    await signInManager.RefreshSignInAsync(user);
                }
                return RedirectToAction(nameof(TaskController.Index), "Account");
            }
            return View(editModel);
        }

        //GET: User/NotHaveRights
        public IActionResult NotHaveRights()
        {
            return View();
        }

        // GET: User/Delete/5
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || userManager.Users == null)
            {
                return NotFound();
            }

            var userAccountViewModel = await userManager.FindByIdAsync(id.ToString());
            if (userAccountViewModel == null)
            {
                return NotFound("Юзер заданого id не знайдений");
            }
            return View(userAccountViewModel);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            if (userManager.Users == null)
            {
                return NotFound();
            }
            var user = await userManager.FindByIdAsync(Id.ToString());
            if (user == null)
            {
                return NotFound("Юзер заданого id не знайдений");
            }

            var result = await userManager.DeleteAsync(user);
            var userId = await userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Трапилась помилка під час видалення юзера {userId}");
            }

            //await signInManager.SignOutAsync();

            logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return RedirectToAction(nameof(TaskController.Index), "Task");
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(TaskController.Index), "Task");
        }

        public async Task<string> UploadFile(IFormFile fileAvatar)
        {
            //ImageFiles.Clear(); 
            // e.GetMultipleFiles(maxAllowedFiles) for several files
            int maxFileSize = 1024 * 1024 * 10; // 10MB
            if (fileAvatar.Length >= maxFileSize)
            {
                throw new Exception("Файл занадто великого розміру, максимум 10 mb");
            }

            string extension = Path.GetExtension(fileAvatar.FileName);

            if (!validExtensionsForUpload.Contains(extension))
            {
                throw new Exception($"Данний формат файла не підтримується, дозволяється: {string.Join(",", validExtensionsForUpload)}");
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AvatarsImages");

            string newFileName = Path.ChangeExtension(Path.GetRandomFileName(),
                                                      extension);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string absolutePath = Path.Combine(path, newFileName);
            string relativePath = Path.Combine("AvatarsImages", newFileName);

            await using FileStream fs = new(absolutePath, FileMode.Create);
            await fileAvatar.OpenReadStream().CopyToAsync(fs, maxFileSize);

            return relativePath;
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpPost, ActionName("ForgotPasswordPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([Bind("Email")] ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Account", code },
                    protocol: Request.Scheme);

                await emailService.SendEmailAsync(
                    model.Email,
                    "Скидання паролю",
                    $"Будь ласка, виконайте скидання <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>натисніть тут</a>.");

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View();
        }

        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                ResetPasswordModel input = new ResetPasswordModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return View(input);
            }
        }

        [HttpPost, ActionName("ResetPasswordPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([Bind("Email, Password, ConfirmPassword, Code")] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
