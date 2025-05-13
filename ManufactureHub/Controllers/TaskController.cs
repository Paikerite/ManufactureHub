using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManufactureHub.Data;
using ManufactureHub.Models;
using ManufactureHub.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ManufactureHub.Controllers
{
    public class TaskController : Controller
    {
        private readonly ManufactureHubContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private string[] unValidExtensionsForUpload = [".exe", ".msi"];

        public TaskController(ManufactureHubContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TaskViewModels
        public async Task<IActionResult> Index()
        {
            if (User.Identity == null)
            {
                return View();
            }

            var userName = User.Identity.Name;

            if (!User.Identity.IsAuthenticated || userName == null)
            {
                return View();
            }
            var resUser = await _userManager.Users
                .Include(s => s.Sections)
                .ThenInclude(t => t.Tasks)
                .FirstOrDefaultAsync(nm => nm.UserName == userName);

            if (resUser == null)
            {
                return BadRequest();
            }

            //return View(await _context.Tasks.Include(s => s.Section).ToListAsync());
            var checkAdminRoleInUser = await _userManager.GetRolesAsync(resUser);
            if (checkAdminRoleInUser.Contains(Roles.Admin.ToString()) || 
                checkAdminRoleInUser.Contains(Roles.TeamLeadWorkstation.ToString()) ||
                checkAdminRoleInUser.Contains(Roles.HeadFacility.ToString()))
            {
                return View(await _context.Tasks.Include(s => s.Section).ToListAsync());
            }
            var tasksToShow = (from section in resUser.Sections
                               from task in section.Tasks
                               select task).ToList();

            return View(tasksToShow);
        }

        // GET: TaskViewModels/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskViewModel = await _context.Tasks
                .Include(s => s.Section)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            return View(taskViewModel);
        }

        // GET: TaskViewModels/Create
        [Authorize(Roles = "Admin,TeamLeadWorkstation")]
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
        [Authorize(Roles = "Admin,TeamLeadWorkstation")]
        public async Task<IActionResult> Create([Bind("Name,Description,Deadline,Priority,SectionId,FormFile")] TaskModelPost taskModelPost)
        {
            ModelState.Remove("FormFile");
            if (ModelState.IsValid)
            {
                string? pathToFiles = string.Empty;

                if (taskModelPost.FormFile != null)
                {
                    try
                    {
                        pathToFiles = await UploadFile(taskModelPost.FormFile);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Помилка на сервері - {ex.Message}");
                        return View(taskModelPost);
                    }
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
                    FileUrl = pathToFiles,
                    //taskViewModel.ProfilePictureUploader = "user.svg";
                };

                _context.Add(taskViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskModelPost);
        }

        // GET: TaskViewModels/Edit/5
        [Authorize(Roles = "Admin,TeamLeadWorkstation")]
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

            List<SectionViewModel> sections = await _context.Sections.ToListAsync();
            List<SelectListItem> sectionsSelect = new List<SelectListItem>();
            foreach (var section in sections)
            {
                sectionsSelect.Add(new SelectListItem { Text = section.Name, Value = section.Id.ToString() });
            }

            TaskModelEdit taskModelEdit = new TaskModelEdit()
            {
                Id = taskViewModel.Id,
                Name = taskViewModel.Name,
                Description = taskViewModel.Description,
                Deadline = taskViewModel.Deadline,
                Created = taskViewModel.Created,
                Priority = taskViewModel.Priority,
                UploadNewFiles = false,
                SectionId = taskViewModel.SectionId.ToString(),
                SectionSelect = sectionsSelect
            };

            return View(taskModelEdit);
        }

        // POST: TaskViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TeamLeadWorkstation")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Deadline,Priority,SectionId,UploadNewFiles,FormFile")] TaskModelEdit taskModelEdit)
        {
            if (id != taskModelEdit.Id)
            {
                return NotFound();
            }

            if (!taskModelEdit.UploadNewFiles)
            {
                ModelState.Remove("FormFile");
            }

            if (ModelState.IsValid)
            {
                var taskViewModel = await _context.Tasks.FirstOrDefaultAsync(lol => lol.Id == id);

                if (taskViewModel == null)
                {
                    ModelState.AddModelError("", "Невдалося знайти таск");
                    return View();
                }

                string? pathToFiles = string.Empty;

                if (taskModelEdit.FormFile != null)
                {
                    try
                    {
                        if (taskModelEdit.UploadNewFiles)
                        {
                            pathToFiles = await UploadFile(taskModelEdit.FormFile);
                            if (taskViewModel.FileUrl != null)
                            {
                                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", taskViewModel.FileUrl);
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                    Thread.Sleep(20);
                                }
                            }
                        }
                        else
                        {
                            pathToFiles = taskViewModel.FileUrl;
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Помилка на сервері - {ex.Message}");
                        return View(taskModelEdit);
                    }
                }

                var convertResIdId = int.TryParse(taskModelEdit.SectionId, out int sectionId);
                if (!convertResIdId)
                {
                    ModelState.AddModelError("", "Помилка при зчитування id цеху");
                    return View(taskModelEdit);
                }

                taskViewModel.Name = taskModelEdit.Name;
                taskViewModel.Description = taskModelEdit.Description;
                taskViewModel.Priority = taskModelEdit.Priority;
                taskViewModel.Deadline = taskModelEdit.Deadline;
                taskViewModel.SectionId = sectionId;
                taskViewModel.FileUrl = pathToFiles;

                try
                {
                    //_context.Update(taskViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskViewModelExists(taskViewModel.Id))
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
                    return View(taskModelEdit);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(taskModelEdit);
        }
        
        [HttpGet]
        public IActionResult Test()
        {
            return Json(new { message = "TaskController is reachable" });
        }

        /* [ValidateAntiForgeryToken]*/ // Require CSRF token for POST
        [HttpPost]
        [Authorize(Roles = "Admin,TeamLeadSection")]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return Json(new { success = false, error = "Invalid input" });
            }

            // Map hyphenated status values to enum values
            string mappedStatus = status switch
            {
                "in-progress" => "inprogress",
                "under-review" => "underreview",
                "done" => "done",
                "rejected" => "rejected",
                _ => status // Fallback to original value if no mapping
            };

            if (!Enum.TryParse<StatusTask>(mappedStatus, true, out var statusTask))
            {
                return Json(new { success = false, error = $"Invalid status value: {status}" });
            }

            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return Json(new { success = false, error = "Task not found" });
            }

            if (!User.IsInRole("Admin") && !User.IsInRole("TeamLeadSection"))
            {
                return Json(new { success = false, error = "Unauthorized" });
            }

            task.StatusTask = statusTask;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task status updated successfully" });
        }

        // GET: TaskViewModels/Delete/5
        [Authorize(Roles = "Admin,TeamLeadWorkstation")]
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
        [Authorize(Roles = "Admin,TeamLeadWorkstation")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskViewModel = await _context.Tasks.FindAsync(id);
            if (taskViewModel != null)
            {
                _context.Tasks.Remove(taskViewModel);
                if (taskViewModel.FileUrl != null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", taskViewModel.FileUrl);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        Thread.Sleep(20);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DownloadFile([FromQuery] string fileName)
        {
            if (fileName.Contains("..") || fileName.Contains('/'))
            {
                return BadRequest("Invalid file name.");
            }
            // Path to the file on the server
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // Read the file as a byte array
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Set the content type (MIME type) based on the file extension
            string contentType = "application/octet-stream"; // Default for unknown types
            if (fileName.EndsWith(".pdf"))
                contentType = "application/pdf";
            else if (fileName.EndsWith(".jpg"))
                contentType = "image/jpeg";
            // Add more MIME types as needed

            // Return the file with the appropriate headers
            return File(fileBytes, contentType, fileName);
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
