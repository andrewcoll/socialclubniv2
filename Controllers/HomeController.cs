using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialClubNI.Models;
using SocialClubNI.Services;
using SocialClubNI.ViewModels;
using Blobr;

namespace SocialClubNI.Controllers
{
    public class HomeController : Controller
    {
        private const string MIXCLOUD_URL = "https://www.mixcloud.com/oembed/?url=https%3A//www.mixcloud.com/ardskeith/{0}/&format=json";

        private readonly StorageWrapper storageWrapper;
        private readonly LoginManager claimsManager;

        private readonly MixCloudProvider mixCloudProvider;

        public HomeController(StorageWrapper storageWrapper, LoginManager claimsManager, MixCloudProvider mixCloudProvider)
        {
            this.storageWrapper = storageWrapper;
            this.claimsManager = claimsManager;
            this.mixCloudProvider = mixCloudProvider;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Home";
            var page = await storageWrapper.GetPageAsync<Podcast>($"podcasts-1617");
            return View(page.Items.OrderByDescending(p => p.Published).First());
        }

        public IActionResult Team()
        {
            ViewBag.Title = "Team";
            return View();
        }

        public async Task<IActionResult> Seasons(string season = "1617")
        {
            ViewBag.Title = "Episodes";
            var page = await storageWrapper.GetPageAsync<Podcast>($"podcasts-{season}");
            return View(page.Items.OrderByDescending(p => p.Published));
        }

        public IActionResult Error()
        {
            ViewBag.Title = "Error";
            return View();
        }

        public async Task<IActionResult> Episode(string season, string stub)
        {
            var page = await storageWrapper.GetPageAsync<Podcast>($"podcasts-{season}");
            var podcast = page.Items.FirstOrDefault(p => string.Compare(stub, p.Stub, true) == 0);

            var parsedResponse = await mixCloudProvider.GetMixCloudEmbed(stub);

            var mixcloudPodcast = MixCloudPodcast.FromPodcast(podcast, parsedResponse);

            ViewBag.Title = mixcloudPodcast.Title;
            return View(mixcloudPodcast);
        }

        public IActionResult Forbidden()
        {
            ViewBag.Title = "Forbidden";
            return View();
        }

        [Authorize(Policy = "IsLoggedIn")]
        public async Task<IActionResult> Profile()
        {
            var user = await claimsManager.GetUser(HttpContext.User);
            var vm = new ProfileViewModel() { Username = user.Username, Email = user.Email };

            ViewBag.Title = user.Username;
            return View(vm);
        }
    }
}
