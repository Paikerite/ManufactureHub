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

        // GET: User
        //public async Task<IActionResult> Index()
        //{
        //      return _context.Users != null ? 
        //                  View(await _context.Users.ToListAsync()) :
        //                  Problem("Entity set 'EScheduleDbContext.Users'  is null.");
        //}

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

            return View(userAccountViewModel);
        }

        // GET: User/Login
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

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
        public async Task<IActionResult> Create(IFormFile fileAvatar, [Bind("Name,SurName,PatronymicName,Role,Email,Password,ConfirmPassword")] RegisterModel userAccountViewModel, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                string? pathToImage = string.Empty;

                if (fileAvatar is not null)
                {
                    try
                    {
                        pathToImage = await UploadFile(fileAvatar);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Помилка на сервері - {ex.Message}");
                        return View(userAccountViewModel);
                    }
                }
                else
                {
                    pathToImage = UrlToDefaultAvatar;
                }

                ApplicationUser user = new ApplicationUser()
                {
                    Name = userAccountViewModel.Name!,
                    SurName = userAccountViewModel.SurName!,
                    PatronymicName = userAccountViewModel.PatronymicName!,
                    ProfilePicture = pathToImage,
                    Email = userAccountViewModel.Email!,
                    UserName = userAccountViewModel.Email!,
                    //Role = userAccountViewModel.Role,
                };

                var result = await userManager.CreateAsync(user, userAccountViewModel.Password!);
                await userManager.SetUserNameAsync(user, userAccountViewModel.Email);
                await userManager.SetEmailAsync(user, userAccountViewModel.Email);

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

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);

                    //await signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction(nameof(ScheduleController.Index), "Schedule");
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
                Email = accountUser.Email,
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
        public async Task<IActionResult> Edit(IFormFile fileAvatar, int id, [Bind("Id,Name,SurName,PatronymicName,Role,Email,Password")] EditModel editModel)
        {
            if (id != editModel.Id)
            {
                return BadRequest();
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
                    user.Email = editModel.Email;

                    var resultupdate = await userManager.UpdateAsync(user);
                    await signInManager.RefreshSignInAsync(user);
                }
                return RedirectToAction(nameof(TaskController.Index), "Task");
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
        [HttpPost, ActionName("Delete")]
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

            await signInManager.SignOutAsync();

            logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return RedirectToAction(nameof(TaskController.Index), "Task");
        }

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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

        //[AllowAnonymous]
        //public async Task<IActionResult> TestCreat()
        //{
        //    IdentityResult result = await roleManager.CreateAsync(new ApplicationRole { RoleName = "Студент", Name= "Student", Description= "Студент, людина яка навчається і знає кому давати хабар" });
        //    if (result.Succeeded)
        //    {
        //        IdentityResult result1 = await roleManager.CreateAsync(new ApplicationRole { RoleName = "Вчитель", Name = "Teacher", Description = "Вчитель, людина яка навчає і знає з кого брати хабар" });
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                logger.LogWarning(error.Description);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (var error in result.Errors)
        //        {
        //            logger.LogWarning(error.Description);
        //        }
        //    }

        //    return View();
        //}
    }
}
