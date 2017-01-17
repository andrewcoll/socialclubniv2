using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SocialClubNI.Controllers
{
    public class DownloadController : Controller
    {
        private readonly CloudBlobContainer container;

        public DownloadController(CloudBlobContainer container)
        {
            this.container = container;
        }

        public void Download(string filename)
        {
            var sasPolicy = new SharedAccessBlobPolicy();
            sasPolicy.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(60);
            sasPolicy.Permissions = SharedAccessBlobPermissions.Read;
            
            var blob = container.GetBlobReference(filename);
            var signature = blob.GetSharedAccessSignature(sasPolicy);
            
            Response.Redirect(blob.Uri + signature);
        }
    }
}