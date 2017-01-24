using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using SocialClubNI.Services;

namespace SocialClubNI.Controllers
{
    public class FileController : Controller
    {

        private readonly PodcastFileProvider fileProvider;
        public FileController(PodcastFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }

        /// <summary>
        /// Download an episode from blob storage
        /// </summary>
        /// <param name="filename">Name of the file to download</param>
        public void Download(string filename)
        {
            var uri = fileProvider.GetPodcastUrl(filename);
            Response.Redirect(uri);
        }

        [HttpPost]
        public async Task<string> Upload(IFormFile file)
        {
            using(var fileStream = file.OpenReadStream())
            {
                await fileProvider.UploadFileAsync(fileStream, file.FileName);
            }
            
            return "Complete";
        }
    }
}