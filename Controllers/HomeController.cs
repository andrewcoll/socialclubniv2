using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blobr;
using Newtonsoft.Json;
using SocialClubNI.Models;
using SocialClubNI.Services;
using SocialClubNI.ViewModels;
using Microsoft.AspNetCore.Authorization;


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
            var page = await storageWrapper.GetPageAsync<Podcast>($"podcasts-1617");
            return View(page.Items.OrderByDescending(p => p.Published).First());
        }

        public IActionResult Team()
        {
            return View();
        }

        public async Task<IActionResult> Seasons(string season = "1617")
        {
            var page = await storageWrapper.GetPageAsync<Podcast>($"podcasts-{season}");
            return View(page.Items.OrderByDescending(p => p.Published));
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Episode(string season, string stub)
        {
            var page = await storageWrapper.GetPageAsync<Podcast>($"podcasts-{season}");
            var podcast = page.Items.FirstOrDefault(p => string.Compare(stub, p.Stub, true) == 0);

            var parsedResponse = await mixCloudProvider.GetMixCloudEmbed(stub);

            var mixcloudPodcast = MixCloudPodcast.FromPodcast(podcast, parsedResponse);

            return View(mixcloudPodcast);
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        [Authorize(Policy = "LoggedIn")]
        public async Task<IActionResult> Profile()
        {
            var user = await claimsManager.GetUser(HttpContext.User);

            var vm = new ProfileViewModel() { Username = user.Username, Email = user.Email };
            return View(vm);
        }
    }
}
