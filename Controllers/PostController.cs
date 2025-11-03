using BlogWebApplication.Data;
using BlogWebApplication.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet] // Create action method to render the create post view
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
    }
}
