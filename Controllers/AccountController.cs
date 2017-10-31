using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SocialClubNI.Services;
using SocialClubNI.ViewModels;
using System.Threading.Tasks;

namespace SocialClubNI.Controllers
{
    public class AccountController : Controller
    {
        private readonly LoginManager loginManager;
        private readonly ClaimsManager claimsManager;

        public AccountController(LoginManager loginManager, ClaimsManager claimsManager)
        {
            this.loginManager = loginManager;
            this.claimsManager = claimsManager;
        }

        public IActionResult Login()
        {
            ViewBag.Title = "Login";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if(ModelState.IsValid)
            {
                var result = await loginManager.VerifyLoginAsync(loginViewModel.Username, loginViewModel.Password);
                if(result != null)
                {
                    var userPrincipal = claimsManager.CreatePrincipalAsync(result);
                    await HttpContext.SignInAsync("TscniCookieMiddlewareInstance", userPrincipal, new AuthenticationProperties() { IsPersistent = true });
                    return RedirectToAction("Profile", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to find a user with that username or password.");
                    return View(loginViewModel);
                }
            }

            ViewBag.Title = "Login";
            return View(loginViewModel);
        }

        public IActionResult Register()
        {
            ViewBag.Title = "Register";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if(ModelState.IsValid)
            {
                if(await loginManager.GetRegistrationSecret() != registerViewModel.RegistrationKey)
                {
                    ModelState.AddModelError(string.Empty, "Incorrect registration secret.");
                    return View(registerViewModel);
                }

                if(await loginManager.IsExistingUsername(registerViewModel.Username))
                {
                    ModelState.AddModelError(string.Empty, "That username is already taken");
                }

                if(await loginManager.IsExistingEmail(registerViewModel.Email))
                {
                    ModelState.AddModelError(string.Empty, "An account has already been registered with that email address.");
                }

                if(registerViewModel.Password != registerViewModel.PasswordConfirm)
                {
                    ModelState.AddModelError(string.Empty, "Passwords do not match");
                }

                if(ModelState.ErrorCount > 0)
                {
                    return View(registerViewModel);
                }

                var registeredUser = await loginManager.RegisterUserAsync(registerViewModel.Username, registerViewModel.Email, registerViewModel.Password);
                var userPrincipal = claimsManager.CreatePrincipalAsync(registeredUser);
                await HttpContext.SignInAsync("TscniCookieMiddlewareInstance", userPrincipal, new AuthenticationProperties() { IsPersistent = true } );

                return RedirectToAction("Profile", "Home");
            }

            ViewBag.Title = "Register";
            return View(registerViewModel);
        }

        public async Task<IActionResult> Logout()
        {
            var x = HttpContext.User;
            await HttpContext.SignOutAsync("TscniCookieMiddlewareInstance");
            var y = HttpContext.User;
            return RedirectToAction("Index", "Home");
        }
    }
}