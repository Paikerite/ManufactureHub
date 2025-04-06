using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManufactureHub.Data;
using ManufactureHub.Models;

namespace ManufactureHub.Controllers
{
    public class WorkstationController : Controller
    {
        private readonly ManufactureHubContext _context;

        public WorkstationController(ManufactureHubContext context)
        {
            _context = context;
        }

        // GET: Workstation
        public async Task<IActionResult> Index()
        {
            return View(await _context.Workstations.ToListAsync());
        }

        // GET: Workstation/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Workstation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workstation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,IdTeamLead")] WorkstationViewModel workstationViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workstationViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workstationViewModel);
        }

        // GET: Workstation/Edit/5
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
            return View(workstationViewModel);
        }

        // POST: Workstation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,IdTeamLead")] WorkstationViewModel workstationViewModel)
        {
            if (id != workstationViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workstationViewModel);
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
                return RedirectToAction(nameof(Index));
            }
            return View(workstationViewModel);
        }

        // GET: Workstation/Delete/5
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
