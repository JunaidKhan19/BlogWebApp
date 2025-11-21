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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel) 
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email or password is invalid.");
                    return View(loginViewModel);
                }

                var signInResult = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, isPersistent: false, lockoutOnFailure: false);

                if (!signInResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Email or password is invalid.");
                    return View(loginViewModel);
                }

                return RedirectToAction("Index", "Post");
            }

            return View(loginViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Post");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
}
