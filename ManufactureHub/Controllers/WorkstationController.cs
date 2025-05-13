using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManufactureHub.Data;
using ManufactureHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ManufactureHub.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManufactureHub.Controllers
{
    public class WorkstationController : Controller
    {
        private readonly ManufactureHubContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public WorkstationController(ManufactureHubContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: Workstation
        [Authorize(Roles = "Admin,HeadFacility,TeamLeadWorkstation")]
        public async Task<IActionResult> Index()
        {
            var workstations = await _context.Workstations
                .Include(w => w.Sections)
                .ThenInclude(s => s.Users)
                .Include(w => w.Sections)
                .ThenInclude(s => s.Tasks)
                .ToListAsync();
            return View(workstations);
        }

        // GET: Workstation/Details/5
        [Authorize(Roles = "Admin,HeadFacility,TeamLeadWorkstation")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workstationViewModel = await _context.Workstations
                .Include(w => w.Sections)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workstationViewModel == null)
            {
                return NotFound();
            }

            ViewBag.TeamLeadUser = await userManager.FindByIdAsync(workstationViewModel.Id.ToString());

            return View(workstationViewModel);
        }

        // GET: Workstation/Create
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> Create()
        {
            IList<ApplicationUser> userTeamLeads = await userManager.GetUsersInRoleAsync(Roles.TeamLeadWorkstation.ToString());
            List<SelectListItem> userTeamLeadsSelect = new List<SelectListItem>();
            foreach (var item in userTeamLeads)
            {
                userTeamLeadsSelect.Add(new SelectListItem { Text = $"{item.Name} {item.SurName} {item.PatronymicName}. {item.Position}", Value = item.Id.ToString() });
            }

            WorkstationModelPost workstationModelPost = new WorkstationModelPost() 
            {
                TeamLeadSelect = userTeamLeadsSelect,
            };
            return View(workstationModelPost);
        }

        // POST: Workstation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,TeamLeadId")] WorkstationModelPost workstationModelPost)
        {
            if (ModelState.IsValid)
            {
                var convertResId = int.TryParse(workstationModelPost.TeamLeadId, out int teamLeadId);
                if (!convertResId)
                {
                    ModelState.AddModelError("", "Помилка при зчитування id тімліда");
                    return View(workstationModelPost);
                }

                WorkstationViewModel workstationViewModel = new WorkstationViewModel()
                {
                    Name = workstationModelPost.Name,
                    Description = workstationModelPost.Description,
                    IdTeamLead = teamLeadId
                };

                _context.Add(workstationViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workstationModelPost);
        }

        // GET: Workstation/Edit/5
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workstationViewModel = await _context.Workstations.FindAsync(id);
            if (workstationViewModel == null)
            {
                return NotFound();
            }

            IList<ApplicationUser> userTeamLeads = await userManager.GetUsersInRoleAsync(Roles.TeamLeadWorkstation.ToString());
            List<SelectListItem> userTeamLeadsSelect = new List<SelectListItem>();
            foreach (var item in userTeamLeads)
            {
                userTeamLeadsSelect.Add(new SelectListItem { Text = $"{item.Name} {item.SurName} {item.PatronymicName}. {item.Position}", Value = item.Id.ToString() });
            }

            WorkstationModelEdit workstationModelEdit = new WorkstationModelEdit()
            {
                Id = workstationViewModel.Id,
                Name = workstationViewModel.Name,
                Description = workstationViewModel.Description,
                TeamLeadId = workstationViewModel.IdTeamLead.ToString(),
                TeamLeadSelect = userTeamLeadsSelect,
            };
            return View(workstationModelEdit);
        }
        
        // POST: Workstation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,TeamLeadId")] WorkstationModelEdit workstationModelEdit)
        
        {
            if (id != workstationModelEdit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var convertResId = int.TryParse(workstationModelEdit.TeamLeadId, out int teamLeadId);
                if (!convertResId)
                {
                    ModelState.AddModelError("", "Помилка при зчитування id тімліда");
                    return View(workstationModelEdit);
                }

                var workstationViewModel = await _context.Workstations.FirstOrDefaultAsync(s => s.Id == id);

                if (workstationViewModel == null)
                {
                    ModelState.AddModelError("", "Невдалося знайти цех");
                    return View(workstationModelEdit);
                }

                // Update scalar properties
                workstationViewModel.Name = workstationModelEdit.Name;
                workstationViewModel.Description = workstationModelEdit.Description;
                workstationViewModel.IdTeamLead = teamLeadId;

                try
                {
                    //_context.Update(workstationViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkstationViewModelExists(workstationViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception for debugging
                    Console.WriteLine(ex.InnerException?.Message);
                    ModelState.AddModelError("", "Помилка при збереженні змін. Спробуйте ще раз.");
                    return View(workstationModelEdit);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workstationModelEdit);
        }

        // GET: Workstation/Delete/5
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workstationViewModel = await _context.Workstations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workstationViewModel == null)
            {
                return NotFound();
            }

            return View(workstationViewModel);
        }

        // POST: Workstation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HeadFacility")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workstationViewModel = await _context.Workstations.FindAsync(id);
            if (workstationViewModel != null)
            {
                _context.Workstations.Remove(workstationViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkstationViewModelExists(int id)
        {
            return _context.Workstations.Any(e => e.Id == id);
        }
    }
}
