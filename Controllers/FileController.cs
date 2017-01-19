using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SocialClubNI.Controllers
{
    public class FileController : Controller
    {
        private readonly CloudBlobContainer container;

        public FileController(CloudBlobContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Download an episode from blob storage
        /// </summary>
        /// <param name="filename">Name of the file to download</param>
        public void Download(string filename)
        {
            var sasPolicy = new SharedAccessBlobPolicy();
            sasPolicy.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(60);
            sasPolicy.Permissions = SharedAccessBlobPermissions.Read;
            
            var blob = container.GetBlobReference(filename);
            var signature = blob.GetSharedAccessSignature(sasPolicy);
            
            Response.Redirect(blob.Uri + signature);
        }

        [HttpPost]
        public async Task<string> Upload(IFormFile file)
        {
            using(var fileStream = file.OpenReadStream())
            {
                await UploadFileAsync(fileStream, file.FileName);
            }
            
            return "Complete";
        }

        private async Task<string> UploadFileAsync(Stream stream, string filename)
        {
            var blob = container.GetBlockBlobReference(filename);

            await blob.UploadFromStreamAsync(stream);
            blob.Properties.ContentType = "audio/mpeg";
            await blob.SetPropertiesAsync();
            return blob?.Uri.ToString();
        }
    }
}