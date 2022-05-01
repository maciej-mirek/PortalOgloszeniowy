using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortalOgloszeniowy.Models;
using PortalOgloszeniowy.Models.ViewModels;
using PortalOgloszeniowy.Services;

namespace PortalOgloszeniowy.Controllers
{

    public class AccountController : Controller
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly IAdvertService _advertService;



        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IAdvertService advertService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _advertService = advertService;
        }

        [Route("/login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/login")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,false) ;

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else 
                {
                    ModelState.AddModelError("","Nieprawidłowy login lub hasło");
                }
            }

            return View();
        }

        [Route("/register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                };
                var result = await _userManager.CreateAsync(user,model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent:false);
                    return RedirectToAction("Index", "Home",model);
                }

                foreach(var error in result.Errors)
                {
                    if(error.Description.EndsWith("is already taken."))
                    {
                        error.Description = "Ten login jest zajęty.";
                    }
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }


        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        [Authorize]
        [Route("/profile")]
        public async Task<IActionResult> Profile()
        {
            ViewBag.Adverts = _advertService.GetUsersAdverts(await _userManager.GetUserAsync(User));
            ViewBag.User = await _userManager.GetUserAsync(User);
            return View();
        }
    }
}
