using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialClubNI.Models;
using SocialClubNI.Services;
using SocialClubNI.ViewModels;
using Blobr;
using Microsoft.Extensions.Caching.Memory;

namespace SocialClubNI.Controllers
{
    public class HomeController : Controller
    {
        private const string MIXCLOUD_URL = "https://www.mixcloud.com/oembed/?url=https%3A//www.mixcloud.com/ardskeith/{0}/&format=json";
        private readonly StorageWrapper storageWrapper;
        private readonly LoginManager claimsManager;
        private readonly MixCloudProvider mixCloudProvider;
        private readonly PodcastSeasons podcastSeasons;
        private readonly IMemoryCache cache;

        public HomeController(
            StorageWrapper storageWrapper, 
            IMemoryCache cache, 
            LoginManager claimsManager, 
            MixCloudProvider mixCloudProvider, 
            SeasonProviderFactory SeasonProviderFactory)
        {
            this.storageWrapper = storageWrapper;
            this.claimsManager = claimsManager;
            this.mixCloudProvider = mixCloudProvider;
            this.podcastSeasons = SeasonProviderFactory.GetPodcastSeasons(DateTimeOffset.Now);
            this.cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Home";
            var currentSeason = podcastSeasons.CurrentSeason.Abbreviation;
            Page<Podcast> page;
            try
            {
                page = await GetPage<Podcast>($"podcasts-{currentSeason}");
            }
            catch(BlobrLoadException)
            {
                currentSeason = podcastSeasons.Seasons.OrderByDescending(s => s.StartDate).Skip(1).Take(1).First().Abbreviation;
                page = await GetPage<Podcast>($"podcasts-{currentSeason}");
            }
            return View(page.Items.OrderByDescending(p => p.Published).First());
        }

        private async Task<Page<T>> GetPage<T>(string pageName)
        {
            return await cache.GetOrCreateAsync(pageName, async entry => 
            {
                entry.AbsoluteExpiration = TimeSpan.FromMinutes(15);

                return await storageWrapper.GetPageAsync<T>(pageName);
            });
        }

        public IActionResult Team()
        {
            ViewBag.Title = "Team";
            return View();
        }

        public async Task<IActionResult> Seasons(string season)
        {
            if(string.IsNullOrWhiteSpace(season))
            {
                season = podcastSeasons.CurrentSeason.Abbreviation;
            }
            ViewBag.Title = "Episodes";
            Page<Podcast> page;
            try
            {
                page = await GetPage<Podcast>($"podcasts-{season}");
            }
            catch(BlobrLoadException)
            {
                season = podcastSeasons.Seasons.OrderByDescending(s => s.StartDate).Skip(1).Take(1).First().Abbreviation;
                page = await GetPage<Podcast>($"podcasts-{season}");
            }
            ViewBag.Seasons = podcastSeasons.Seasons.OrderByDescending(s => s.StartDate);
            return View(page.Items.OrderByDescending(p => p.Published));
        }

        public IActionResult Error()
        {
            ViewBag.Title = "Error";
            return View();
        }

        public async Task<IActionResult> Episode(string season, string stub)
        {
            var page = await GetPage<Podcast>($"podcasts-{season}");
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
