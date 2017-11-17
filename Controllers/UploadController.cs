using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SocialClubNI.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SocialClubNI.Models;

namespace SocialClubNI
{
    [Authorize(Policy="IsLoggedIn")]
    public class UploadController : Controller
    {
        private readonly PodcastFileProvider fileProvider;  

        public UploadController(PodcastFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }

        public IActionResult Initiate()
        {
            var result = new 
            {
                SaS = fileProvider.GetPodcastContainerSaS(),
                Endpoint = "https://storage.thesocialclubni.com",
                Container = "podcasts"
            };
            return new JsonResult(result);
        }
    }
}