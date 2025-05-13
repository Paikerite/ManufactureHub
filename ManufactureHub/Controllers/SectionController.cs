using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManufactureHub.Data;
using ManufactureHub.Models;
using Microsoft.AspNetCore.Identity;
using ManufactureHub.Data.Enums;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace ManufactureHub.Controllers
{
    public class SectionController : Controller
    {
        private readonly ManufactureHubContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public SectionController(ManufactureHubContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: Section
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sections
                .Include(i => i.Tasks)
                .Include(b => b.Users)
                .ToListAsync());
        }

        // GET: Section/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sectionViewModel = await _context.Sections
                .Include(i => i.Users)
                .Include(i => i.Tasks)
                .Include(w => w.Workstation)
                .FirstOrDefaultAsync(m => m.Id == id); 

            if (sectionViewModel == null)
            {
                return NotFound();
            }

            return View(sectionViewModel);
        }

        // GET: Section/Create
        [Authorize(Roles = "Admin,HeadFacility,TeamLeadWorkstation")]
        public async Task<IActionResult> Create()
        {
            IList<ApplicationUser> userTeamLeads = await userManager.GetUsersInRoleAsync(Roles.TeamLeadSection.ToString());
            List<SelectListItem> userTeamLeadsSelect = new List<SelectListItem>();
            foreach (var item in userTeamLeads)
            {
                userTeamLeadsSelect.Add(new SelectListItem { Text = $"{item.Name} {item.SurName} {item.PatronymicName}. {item.Position}", Value = item.Id.ToString() });
            }

            List<WorkstationViewModel> workstations = await _context.Workstations.ToListAsync();
            List<SelectListItem> workstationsSelect = new List<SelectListItem>();
            foreach (var workstation in workstations)
            {
                workstationsSelect.Add(new SelectListItem { Text = workstation.Name, Value = workstation.Id.ToString() });
            }

            IList<ApplicationUser> userWorkers = await userManager.GetUsersInRoleAsync(Roles.Worker.ToString());
            List<SelectListItem> usersSelect = new List<SelectListItem>();
            foreach (var user in userWorkers)
            {
                usersSelect.Add(new SelectListItem { Text = $"{user.Name} {user.SurName} {user.PatronymicName}. {user.Position}", Value = user.Id.ToString() });
            }

            SectionModelPost sectionModelPost = new SectionModelPost()
            {
                TeamLeadSelect = userTeamLeadsSelect,
                WorkstationsSelect = workstationsSelect,
                UsersWorkersSelect = usersSelect,
            };
            return View(sectionModelPost);
        }

        // POST: Section/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HeadFacility,TeamLeadWorkstation")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,PrimaryColor,WorkstationId,TeamLeadId,UsersWorkersId")] SectionModelPost sectionModelPost)
        {
            // Debug: Log the raw form data
            //foreach (var key in Request.Form.Keys)
            //{
            //    Console.WriteLine($"Form Key: {key}, Value: {Request.Form[key]}");
            //}

            if (ModelState.IsValid)
            {
                var convertResId = int.TryParse(sectionModelPost.TeamLeadId, out int teamLeadId);
                if (!convertResId)
                {
                    ModelState.AddModelError("", "Помилка при зчитування id тімліда");
                    return View(sectionModelPost);
                }

                var convertResIdId = int.TryParse(sectionModelPost.WorkstationId, out int workstationId);
                if (!convertResIdId)
                {
                    ModelState.AddModelError("", "Помилка при зчитування id цеху");
                    return View(sectionModelPost);
                }

                List<ApplicationUser> workers = new List<ApplicationUser>();

                if (sectionModelPost.UsersWorkersId.Count() != 0)
                {
                    foreach (var item in sectionModelPost.UsersWorkersId)
                    {
                        var user = await userManager.FindByIdAsync(item);
                        if (user != null)
                        {
                            workers.Add(user);
                        }
                    }
                }

                var teamleadUser = await userManager.FindByIdAsync(sectionModelPost.TeamLeadId);
                if (teamleadUser == null)
                {
                    ModelState.AddModelError("", "Невдалося знайти Id тімліда");
                    return View();
                }

                workers.Add(teamleadUser);

                SectionViewModel sectionViewModel = new SectionViewModel()
                {
                    Name = sectionModelPost.Name,
                    Description = sectionModelPost.Description,
                    PrimaryColor = sectionModelPost.PrimaryColor,
                    IdTeamLead = teamLeadId,
                    WorkstationId = workstationId,
                    Users = workers,
                };

                _context.Add(sectionViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sectionModelPost);
        }

        // GET: Section/Edit/5
        [Authorize(Roles = "Admin,HeadFacility,TeamLeadWorkstation")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var sectionViewModel = await _context.Sections.FindAsync(id);
            var sectionViewModel = await _context.Sections
                    .Include(b => b.Users)
                    .FirstOrDefaultAsync(s=>s.Id == id);

            if (sectionViewModel == null)
            {
                return NotFound();
            }

            IList<ApplicationUser> userTeamLeads = await userManager.GetUsersInRoleAsync(Roles.TeamLeadSection.ToString());
            List<SelectListItem> userTeamLeadsSelect = new List<SelectListItem>();
            foreach (var item in userTeamLeads)
            {
                userTeamLeadsSelect.Add(new SelectListItem { Text = $"{item.Name} {item.SurName} {item.PatronymicName}. {item.Position}", Value = item.Id.ToString() });
            }

            List<WorkstationViewModel> workstations = await _context.Workstations.ToListAsync();
            List<SelectListItem> workstationsSelect = new List<SelectListItem>();
            foreach (var workstation in workstations)
            {
                workstationsSelect.Add(new SelectListItem { Text = workstation.Name, Value = workstation.Id.ToString() });
            }

            IList<ApplicationUser> userWorkers = await userManager.GetUsersInRoleAsync(Roles.Worker.ToString());
            List<SelectListItem> usersSelect = new List<SelectListItem>();
            foreach (var user in userWorkers)
            {
                usersSelect.Add(new SelectListItem { Text = $"{user.Name} {user.SurName} {user.PatronymicName}. {user.Position}", Value = user.Id.ToString() });
            }

            SectionModelEdit sectionModelEdit = new SectionModelEdit()
            {
                Id = sectionViewModel.Id,
                Name = sectionViewModel.Name,
                Description = sectionViewModel.Description,
                PrimaryColor = sectionViewModel.PrimaryColor,
                TeamLeadId = sectionViewModel.IdTeamLead.ToString(),
                TeamLeadSelect = userTeamLeadsSelect,
                WorkstationId = sectionViewModel.WorkstationId.ToString(),
                WorkstationsSelect = workstationsSelect,
                UsersWorkersSelect = usersSelect,
                //Tasks = sectionViewModel.Tasks,
            };

            if (sectionViewModel.Users.Count() != 0)
            {
                //var userTeamLeadToRemove = sectionViewModel.Users.ToList();
                int idTeamLeadSctionToExcludeFromWorkers = -1;
                foreach (var user in sectionViewModel.Users)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    if (userRoles.Any(rl => rl == Roles.TeamLeadSection.ToString()))
                    {
                        idTeamLeadSctionToExcludeFromWorkers = user.Id;
                    }
                }
                sectionModelEdit.UsersWorkersId = sectionViewModel.Users.Where(u => u.Id != idTeamLeadSctionToExcludeFromWorkers)
                    .Select(ii=>ii.Id.ToString());
            }
            return View(sectionModelEdit);
        }

        // POST: Section/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HeadFacility,TeamLeadWorkstation")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,PrimaryColor,WorkstationId,TeamLeadId,UsersWorkersId")] SectionModelEdit sectionModelEdit)
        {
            if (id != sectionModelEdit.Id)
            {
                return NotFound();
            }

            ModelState.Remove("UsersWorkersId"); //it is not good actually,but without it can't upload form with empty UsersWorkersId, should be reconsidered later

            if (ModelState.IsValid)
            {
                var convertResId = int.TryParse(sectionModelEdit.TeamLeadId, out int teamLeadId);
                if (!convertResId)
                {
                    ModelState.AddModelError("", "Помилка при зчитування id тімліда");
                    return View(sectionModelEdit);
                }

                var convertResIdId = int.TryParse(sectionModelEdit.WorkstationId, out int workstationId);
                if (!convertResIdId)
                {
                    ModelState.AddModelError("", "Помилка при зчитування id цеху");
                    return View(sectionModelEdit);
                }

                var sectionViewModel = await _context.Sections
                        .Include(s => s.Users)
                        .FirstOrDefaultAsync(s => s.Id == id);

                if (sectionViewModel == null)
                {
                    ModelState.AddModelError("", "Невдалося знайти секцію");
                    return View();
                }

                // Update scalar properties
                sectionViewModel.Name = sectionModelEdit.Name;
                sectionViewModel.Description = sectionModelEdit.Description;
                sectionViewModel.PrimaryColor = sectionModelEdit.PrimaryColor;
                sectionViewModel.IdTeamLead = teamLeadId;
                sectionViewModel.WorkstationId = workstationId;
                //sectionViewModel.Tasks = sectionModelPost.Tasks; // Be cautious with Tasks; ensure they're handled correctly

                // Handle Users collection
                var newUserIds = sectionModelEdit.UsersWorkersId?.ToList() ?? new List<string>();
                newUserIds.Add(sectionModelEdit.TeamLeadId); // Include team lead

                // Get existing user IDs
                var existingUserIds = sectionViewModel.Users.Select(u => u.Id.ToString()).ToList();

                // Determine users to add and remove
                var usersToAdd = newUserIds.Except(existingUserIds).ToList();
                var usersToRemove = existingUserIds.Except(newUserIds).ToList();

                // Remove users
                foreach (var userId in usersToRemove)
                {
                    var user = sectionViewModel.Users.FirstOrDefault(u => u.Id.ToString() == userId);
                    if (user != null)
                    {
                        sectionViewModel.Users.Remove(user);
                    }
                }

                // Add new users
                foreach (var userId in usersToAdd)
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        sectionViewModel.Users.Add(user);
                    }
                }

                //List<ApplicationUser> workers = new List<ApplicationUser>();

                //if (sectionModelPost.UsersWorkersId.Count() != 0)
                //{
                //    foreach (var item in sectionModelPost.UsersWorkersId)
                //    {
                //        var user = await userManager.FindByIdAsync(item);
                //        if (user != null)
                //        {
                //            workers.Add(user);
                //        }
                //    }
                //}

                //var teamleadUser = await userManager.FindByIdAsync(sectionModelPost.TeamLeadId);
                //if (teamleadUser == null)
                //{
                //    ModelState.AddModelError("", "Невдалося знайти Id тімліда");
                //    return View();
                //}

                //workers.Add(teamleadUser);

                //SectionViewModel sectionViewModel = new SectionViewModel()
                //{
                //    Id = sectionModelPost.Id,
                //    Name = sectionModelPost.Name,
                //    Description = sectionModelPost.Description,
                //    PrimaryColor = sectionModelPost.PrimaryColor,
                //    IdTeamLead = teamLeadId,
                //    WorkstationId = workstationId,
                //    Users = workers,
                //    Tasks = sectionModelPost.Tasks,
                //};

                try
                {
                    //_context.Update(sectionViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectionViewModelExists(sectionViewModel.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception for debugging
                    Console.WriteLine(ex.InnerException?.Message);
                    ModelState.AddModelError("", "Помилка при збереженні змін. Спробуйте ще раз.");
                    return View(sectionModelEdit);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sectionModelEdit);
        }

        // GET: Section/Delete/5
        [Authorize(Roles = "Admin,HeadFacility,TeamLeadWorkstation")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sectionViewModel = await _context.Sections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sectionViewModel == null)
            {
                return NotFound();
            }

            return View(sectionViewModel);
        }

        // POST: Section/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HeadFacility,TeamLeadWorkstation")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sectionViewModel = await _context.Sections.FindAsync(id);
            if (sectionViewModel != null)
            {
                _context.Sections.Remove(sectionViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SectionViewModelExists(int id)
        {
            return _context.Sections.Any(e => e.Id == id);
        }
    }
}
