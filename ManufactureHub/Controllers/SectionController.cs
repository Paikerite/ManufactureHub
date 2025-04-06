using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManufactureHub.Data;
using ManufactureHub.Models;
using Microsoft.AspNetCore.Identity;
using ManufactureHub.Data.Enums;

namespace ManufactureHub.Controllers
{
    public class SectionController : Controller
    {
        private readonly ManufactureHubContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public SectionController(ManufactureHubContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // GET: Section
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sections.Include(i=>i.Tasks).Include(b=>b.Users).ToListAsync());
        }

        // GET: Section/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Section/Create
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
        public async Task<IActionResult> Create([Bind("Id,Name,Description,PrimaryColor,WorkstationId,TeamLeadId,UsersWorkersId")] SectionModelPost sectionModelPost)
        {
            if (ModelState.IsValid)
            {
                var convertResId = int.TryParse(sectionModelPost.TeamLeadId, out int teamLeadId);
                if (!convertResId)
                {
                    ModelState.AddModelError("", "Помилка при зчитування id тімліда");
                    return View();
                }

                var convertResIdId = int.TryParse(sectionModelPost.WorkstationId, out int workstationId);
                if (!convertResIdId) 
                {
                    ModelState.AddModelError("", "Помилка при зчитування id цеху");
                    return View();
                }

                List<ApplicationUser> workers = new List<ApplicationUser>();

                foreach (var item in sectionModelPost.UsersWorkersSelect)
                {
                    var user = await userManager.FindByIdAsync(item.Value);
                    if (user != null) 
                    {
                        workers.Add(user);
                    }
                }

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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sectionViewModel = await _context.Sections.FindAsync(id);
            if (sectionViewModel == null)
            {
                return NotFound();
            }
            return View(sectionViewModel);
        }

        // POST: Section/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,PrimaryColor,IdTeamLead")] SectionViewModel sectionViewModel)
        {
            if (id != sectionViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sectionViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectionViewModelExists(sectionViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sectionViewModel);
        }

        // GET: Section/Delete/5
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
