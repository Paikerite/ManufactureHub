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
    public class SectionController : Controller
    {
        private readonly ManufactureHubContext _context;

        public SectionController(ManufactureHubContext context)
        {
            _context = context;
        }

        // GET: Section
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sections.ToListAsync());
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Section/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,PrimaryColor,IdTeamLead")] SectionViewModel sectionViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sectionViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sectionViewModel);
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
