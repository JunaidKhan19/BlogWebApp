using BlogWebApplication.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApplication.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                //Create Identity User Object
                var user = new IdentityUser { UserName = registerViewModel.Email, Email = registerViewModel.Email,  };

                //Create user in the database
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                //Check if user creation was successful
                if (result.Succeeded)
                {
                    if (! await _roleManager.RoleExistsAsync("User"))
                    {
                        //Create User Role if it doesn't exist
                        await _roleManager.CreateAsync(new IdentityRole("User"));

                    }

                    await _userManager.AddToRoleAsync(user, "User");

                    //Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    //isPersistent true means the user will stay logged in even after closing the browser
                    //so to handle sign out automatically see in Program.cs file  ConfigureApplicationCookie method

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(registerViewModel);
        }
    }
}
