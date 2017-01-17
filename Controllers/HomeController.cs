using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blobr;
using SocialClubNI.Models;

namespace SocialClubNI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var azureWrapper = new AzureStorageWrapper("andrecolltest", "5YE5X/xOZPCKpCtdsi2eAxjnPmcoia88q6TPrLziCGS+KhhWL3LEupD1UOsdBtSqmlIOi0+jGlD2D3yArYscMw==", "testingcontainer");
            var storageWrapper = new StorageWrapper(azureWrapper);

            var page = storageWrapper.GetPageAsync<Podcast>($"podcasts-1617").Result;
            return View(page.Items.OrderByDescending(p => p.Published).First());
        }

        public IActionResult Team()
        {
            return View();
        }

        public IActionResult Episodes(string seasons = "1617")
        {
            var azureWrapper = new AzureStorageWrapper("andrecolltest", "5YE5X/xOZPCKpCtdsi2eAxjnPmcoia88q6TPrLziCGS+KhhWL3LEupD1UOsdBtSqmlIOi0+jGlD2D3yArYscMw==", "testingcontainer");
            var storageWrapper = new StorageWrapper(azureWrapper);

            var page = storageWrapper.GetPageAsync<Podcast>($"podcasts-1617").Result;
            return View(page.Items.OrderByDescending(p => p.Published));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
