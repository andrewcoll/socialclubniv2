using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public void Download(string filename, string store = "web")
        {
            var uri = fileProvider.GetPodcastUrl(filename);
            Response.Redirect(uri);
        }

        [Authorize(Policy="IsLoggedIn")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            using(var fileStream = file.OpenReadStream())
            {
                await fileProvider.UploadFileAsync(fileStream, file.FileName);
            }
            
            return View("FileUploadSuccess");
        }
    }
}