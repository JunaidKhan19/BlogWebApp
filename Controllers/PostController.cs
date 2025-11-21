using BlogWebApplication.Data;
using BlogWebApplication.Models;
using BlogWebApplication.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApplication.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtensions = {".jpg", ".jpeg", ".png"}; //defining allowed extensions for image 

        
        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context; //injecting db dependency so that it creates db object automatically
            _webHostEnvironment = webHostEnvironment; //injecting db dependency so that it creates db object automatically
        }

        [HttpGet] // Create action method to render the Posts
        public IActionResult Index(int? categoryId)
        {
            var postQuery = _context.Posts.Include(p => p.Category).AsQueryable();
            if (categoryId.HasValue)
            {
                postQuery = postQuery.Where(p => p.CategoryId == categoryId);
            }

            var posts = postQuery.ToList();//deferred execution to get posts based on query

            ViewBag.Categories = _context.Categories.ToList();

            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var post = _context.Posts.Include(p => p.Category).Include(p => p.Comments).FirstOrDefault(p => p.Id == id);

            if(post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpGet] // Create action method to render the create post view
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var postViewModel = new Models.ViewModels.PostViewModel();
            postViewModel.Categories = _context.Categories.Select(c => 
                new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                }
            ).ToList();
            return View(postViewModel);
        }

        [HttpPost] // Create action method to handle form submission
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                //Verifying Extension of the file sent by user
                var inputFileExtension = Path.GetExtension(postViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = _allowedExtensions.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("", "Invalid Image format. Allowed Image formats are .jpg, .jpeg, .png");
                    return View(postViewModel);
                }

                //saving imagepath to db
                postViewModel.Post.FeatureImagePath = await UploadFileToFolder(postViewModel.FeatureImage);
                await _context.Posts.AddAsync(postViewModel.Post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            postViewModel.Categories = _context.Categories.Select(c =>
                new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                }
            ).ToList();

            return View(postViewModel);
        }

        private async Task<string> UploadFileToFolder(IFormFile file)
        {
            var inputFileExtension = Path.GetExtension(file.FileName);//taking the extension of input file

            var fileName = Guid.NewGuid().ToString() + inputFileExtension;// Generating unique filename

            var wwwRootPath = _webHostEnvironment.WebRootPath;//this accesses the wwwroot path

            var imagesFolderPath = Path.Combine(wwwRootPath, "images");

            if (!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);//if there is no folder for image create it
            }

            var filePath = Path.Combine(imagesFolderPath, fileName); //creating filepath

            //copying file to this filepath
            try
            {
                await using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                return "Error Uploading Images:" + ex.Message;
            }
            return "/images/" + fileName;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (postFromDb == null)
            {
                return NotFound();
            }

            EditPostViewModel editPostViewModel = new EditPostViewModel()
            {
                Post = postFromDb,
                Categories = _context.Categories.Select(c =>
                    new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name,
                    }
                ).ToList(),
            };

            return View(editPostViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditPostViewModel editPostViewModel)
        {
            if (editPostViewModel == null)
            {
                return View(editPostViewModel);
            }

            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == editPostViewModel.Post.Id);

            if(postFromDb == null)
            {
                return NotFound();
            }

            if (editPostViewModel.FeatureImage != null)
            {
                var inputFileExtension = Path.GetExtension(editPostViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = _allowedExtensions.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("", "Invalid Image format. Allowed Image formats are .jpg, .jpeg, .png");
                    return View(editPostViewModel);
                }

                // Delete existing image file if it exists
                if (!string.IsNullOrEmpty(postFromDb.FeatureImagePath))
                {
                    var existingfilePath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images",
                        Path.GetFileName(postFromDb.FeatureImagePath)
                    );

                    if (System.IO.File.Exists(existingfilePath))
                    {
                        System.IO.File.Delete(existingfilePath);
                    }
                }

                postFromDb.FeatureImagePath = await UploadFileToFolder(editPostViewModel.FeatureImage);
            }
            else 
            {
                editPostViewModel.Post.FeatureImagePath = postFromDb.FeatureImagePath;
            }

            postFromDb.Title = editPostViewModel.Post.Title;
            postFromDb.Content = editPostViewModel.Post.Content;
            postFromDb.CategoryId = editPostViewModel.Post.CategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (postFromDb == null) 
            {
                return NotFound();
            }

            return View(postFromDb);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (string.IsNullOrEmpty(postFromDb.FeatureImagePath))
            {
                var existingfilePath = Path.Combine(
                       _webHostEnvironment.WebRootPath,
                       "images",
                       Path.GetFileName(postFromDb.FeatureImagePath)
                   );

                if (System.IO.File.Exists(existingfilePath))
                {
                    System.IO.File.Delete(existingfilePath);
                }
            }
            _context.Posts.Remove(postFromDb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index"); 
        }

        [Authorize]
        public JsonResult AddComment([FromBody]Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            _context.Comments.Add(comment);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                username = comment.UserName,
                commentContent = comment.CommentContent,
                commentDate = comment.CommentDate.ToString("MMMM dd,yyyy")
            });
        }
    }
}
