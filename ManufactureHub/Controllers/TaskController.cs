using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManufactureHub.Data;
using ManufactureHub.Models;

namespace ManufactureHub.Controllers
{
    public class TaskController : Controller
    {
        private readonly ManufactureHubContext _context;

        public TaskController(ManufactureHubContext context)
        {
            _context = context;
        }

        // GET: TaskViewModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tasks.ToListAsync());
        }

        // GET: TaskViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskViewModel = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            return View(taskViewModel);
        }

        // GET: TaskViewModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaskViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Deadline,Created,Priority,FileUrl")] TaskViewModel taskViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskViewModel);
        }

        // GET: TaskViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskViewModel = await _context.Tasks.FindAsync(id);
            if (taskViewModel == null)
            {
                return NotFound();
            }
            return View(taskViewModel);
        }

        // POST: TaskViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Deadline,Created,Priority,FileUrl")] TaskViewModel taskViewModel)
        {
            if (id != taskViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskViewModelExists(taskViewModel.Id))
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
            return View(taskViewModel);
        }

        // GET: TaskViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskViewModel = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            return View(taskViewModel);
        }

        // POST: TaskViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskViewModel = await _context.Tasks.FindAsync(id);
            if (taskViewModel != null)
            {
                _context.Tasks.Remove(taskViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskViewModelExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
