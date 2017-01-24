using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;


namespace SocialClubNI.Services
{
    public class PodcastFileProvider
    {
        private readonly CloudBlobContainer container;

        public PodcastFileProvider(CloudBlobContainer container)
        {
            if(container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            this.container = container;
        }

        /// <summary>
        /// Get the Url for a provided podcast filename
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <returns>The podcast url</returns>
        public string GetPodcastUrl(string filename)
        {
            var sasPolicy = new SharedAccessBlobPolicy();
            sasPolicy.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(60);
            sasPolicy.Permissions = SharedAccessBlobPermissions.Read;

            var blob = container.GetBlobReference(filename);
            var signature = blob.GetSharedAccessSignature(sasPolicy);

            return blob.Uri + signature;
        }

        /// <summary>
        /// Upload a new podcast to blob storage
        /// </summary>
        /// <param name="stream">Stream of the file</param>
        /// <param name="filename">The name to give the file</param>
        /// <returns>The blob uri</returns>
        public async Task<string> UploadFileAsync(Stream stream, string filename)
        {
            var blob = container.GetBlockBlobReference(filename);

            await blob.UploadFromStreamAsync(stream);
            blob.Properties.ContentType = "audio/mpeg";
            await blob.SetPropertiesAsync();
            return blob?.Uri.ToString();
        }


        public async Task<ICollection<string>> GetPodcastFilenames()
        {
            var blobs = await container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.Metadata, null, null, null, null);

            return blobs.Results.Select(b => Uri.UnescapeDataString(b.Uri.Segments.Last())).ToList();
        }
    }
}