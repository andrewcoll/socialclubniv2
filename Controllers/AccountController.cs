using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authorization;
using SocialClubNI.Models;
using SocialClubNI.ViewModels;
using SocialClubNI.Services;

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
                    await HttpContext.Authentication.SignInAsync("TscniCookieMiddlewareInstance", userPrincipal, new AuthenticationProperties() { IsPersistent = true });
                    return RedirectToAction("Profile", "Account");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to find a user with that username or password.");
                    return View(loginViewModel);
                }
            }

            return View(loginViewModel);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if(ModelState.IsValid)
            {
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
                await HttpContext.Authentication.SignInAsync("TscniCookieMiddlewareInstance", userPrincipal, new AuthenticationProperties() { IsPersistent = true } );

                return RedirectToAction("Profile", "Account");
            }

            return View(registerViewModel);
        }

        public async Task<IActionResult> Logout()
        {
            var x = HttpContext.User;
            await HttpContext.Authentication.SignOutAsync("TscniCookieMiddlewareInstance");
            var y = HttpContext.User;
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Policy = "LoggedIn")]
        public async Task<string> Profile()
        {
            return "Hello World";
        }
    }
}