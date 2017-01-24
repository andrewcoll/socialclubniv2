using System;
using System.Threading.Tasks;
using SocialClubNI.Models;
using SocialClubNI.Services;
using Microsoft.AspNetCore.Mvc;
using Blobr;

namespace SocialClubNI.Controllers
{
    public class AdminController : Controller
    {
        private readonly StorageWrapper storageWrapper;
        private readonly MixCloudProvider mixCloudProvider;

        public AdminController(StorageWrapper storageWrapper, MixCloudProvider mixCloudProvider)
        {
            this.storageWrapper = storageWrapper;
            this.mixCloudProvider = mixCloudProvider;
        }

        public IActionResult Upload()
        {
            return View();
        }
        
        public async Task<IActionResult> Create(string stub)
        {
            if(string.IsNullOrWhiteSpace(stub))
            {
                return View("CreateStub");
            }

            var podcast = await MixCloudProvider.GetMixCloudMetata(stub);
            return View("CreateEpisode", podcast);
        }

        [HttpPost]
        public IActionResult Create(Podcast podast)
        {
            return View();
        }
    }
}