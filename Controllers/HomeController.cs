using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blobr;
using Newtonsoft.Json;
using SocialClubNI.Models;

namespace SocialClubNI.Controllers
{
    public class HomeController : Controller
    {
        private const string MIXCLOUD_URL = "https://www.mixcloud.com/oembed/?url=https%3A//www.mixcloud.com/ardskeith/{0}/&format=json";

        private readonly StorageWrapper storageWrapper;

        public HomeController(StorageWrapper storageWrapper)
        {
            this.storageWrapper = storageWrapper;
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

            var mixcloudUrl = string.Format(MIXCLOUD_URL, podcast.Stub);

            HttpClient client = new HttpClient();
            
            var result = await client.GetAsync(mixcloudUrl);
            if(!result.IsSuccessStatusCode)
            {
                RedirectToAction("Error");
            }

            var response = await result.Content.ReadAsStringAsync();
            var parsedResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            var mixcloudPodcast = MixCloudPodcast.FromPodcast(podcast, parsedResponse["embed"]);

            return View(mixcloudPodcast);
        }
    }
}
