using Microsoft.AspNetCore.Mvc;
using POS.Services;
using POS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace POS.Controllers
{
    public class UsersController: Controller
    {
        private readonly IServiceUser serviceUser;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UsersController(IServiceUser serviceUser,
                               UserManager<User> userManager,
                               SignInManager<User> signInManager)
        {
            this.serviceUser = serviceUser;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
			{
                return View(viewModel);
			}

            var result = await signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, 
                viewModel.RememberMe, lockoutOnFailure:false);
            
            if (result.Succeeded)
			{
                return RedirectToAction("Home", "Home");
			}
			else
			{
                ModelState.AddModelError(String.Empty, "The username or password is incorrect");
                return View(viewModel);
			}

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var name = string.Concat(model.Name, " ", model.LastName);

            var user = new User() { Name = name, Email = model.Email };

            var result = await userManager.CreateAsync(user, password: model.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Home", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Login", "Users");
        }


    }
}
