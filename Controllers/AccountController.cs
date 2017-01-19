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
                    ModelState.AddModelError(string.Empty, "Invalid login");
                    return View(loginViewModel);
                }
            }

            return View(loginViewModel);
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