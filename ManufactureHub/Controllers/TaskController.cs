using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManufactureHub.Data;
using ManufactureHub.Models;
using ManufactureHub.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO.Compression;

namespace ManufactureHub.Controllers
{
    public class TaskController : Controller
    {
        private readonly ManufactureHubContext _context;

        private string[] unValidExtensionsForUpload = [".exe", ".msi"];

        public TaskController(ManufactureHubContext context)
        {
            _context = context;
        }

        // GET: TaskViewModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tasks.Include(s=>s.Section).ToListAsync());
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
        public async Task<IActionResult> Create()
        {
            List<SectionViewModel> sections = await _context.Sections.ToListAsync();
            List<SelectListItem> sectionsSelect = new List<SelectListItem>();
            foreach (var section in sections)
            {
                sectionsSelect.Add(new SelectListItem { Text = section.Name, Value = section.Id.ToString() });
            }

            TaskModelPost taskModelPost = new TaskModelPost() { Deadline = DateTime.Now.Date, SectionSelect = sectionsSelect };

            return View(taskModelPost);
        }

        // POST: TaskViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Deadline,Priority,SectionId,FormFile")] TaskModelPost taskModelPost)
        {
            if (ModelState.IsValid)
            {
                string? pathToImage = string.Empty;

                try
                {
                    pathToImage = await UploadFile(taskModelPost.FormFile);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка на сервері - {ex.Message}");
                    return View(taskModelPost);
                }

                var convertResIdId = int.TryParse(taskModelPost.SectionId, out int sectionId);
                if (!convertResIdId)
                {
                    ModelState.AddModelError("", "Помилка при зчитування id цеху");
                    return View(taskModelPost);
                }

                TaskViewModel taskViewModel = new TaskViewModel() 
                {
                    Name = taskModelPost.Name,
                    Description = taskModelPost.Description,
                    Deadline = taskModelPost.Deadline,
                    Priority = taskModelPost.Priority,
                    Created = DateTime.Now,
                    StatusTask = StatusTask.underreview,
                    SectionId = sectionId,
                    FileUrl = pathToImage,
                    //taskViewModel.ProfilePictureUploader = "user.svg";
                };

                _context.Add(taskViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskModelPost);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Deadline,Created,Priority,FileUrl,StatusTask")] TaskViewModel taskViewModel)
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

        public async Task<string> UploadFile(IFormFile fileAvatar)
        {
            //ImageFiles.Clear(); 
            // e.GetMultipleFiles(maxAllowedFiles) for several files
            int maxFileSize = 1024 * 1024 * 50; // 50MB
            if (fileAvatar.Length >= maxFileSize)
            {
                throw new Exception("Файл занадто великого розміру, максимум 50 mb");
            }

            string extension = Path.GetExtension(fileAvatar.FileName);

            if (unValidExtensionsForUpload.Contains(extension))
            {
                throw new Exception($"Данний формат файла не підтримується, не дозволяється також такі: {string.Join(",", unValidExtensionsForUpload)}");
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TaskFiles");

            string newFileName = Path.ChangeExtension(Path.GetRandomFileName(),
                                                      extension);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string absolutePath = Path.Combine(path, newFileName);
            string relativePath = Path.Combine("TaskFiles", newFileName);

            await using FileStream fs = new(absolutePath, FileMode.Create);
            await fileAvatar.OpenReadStream().CopyToAsync(fs, maxFileSize);

            return relativePath;
        }

        private bool TaskViewModelExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
