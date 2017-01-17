using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blobr;
using SocialClubNI.Models;
using Microsoft.Extensions.Configuration;

namespace SocialClubNI.Controllers
{
    public class HomeController : Controller
    {
        private readonly StorageWrapper storageWrapper;

        public HomeController(StorageWrapper storageWrapper)
        {
            this.storageWrapper = storageWrapper;
        }

        public IActionResult Index()
        {
            var page = storageWrapper.GetPageAsync<Podcast>($"podcasts-1617").Result;
            return View(page.Items.OrderByDescending(p => p.Published).First());
        }

        public IActionResult Team()
        {
            return View();
        }

        public IActionResult Seasons(string season = "1617")
        {
            var page = storageWrapper.GetPageAsync<Podcast>($"podcasts-{season}").Result;
            return View(page.Items.OrderByDescending(p => p.Published));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
