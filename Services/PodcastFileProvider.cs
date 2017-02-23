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
            if(string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            return $"http://storage.thesocialclubni.com/podcasts/{filename}";
        }

        /// <summary>
        /// Get the Url for a provided podcast filename
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <returns>The podcast url</returns>
        public string GetPodcastUrlWithSaS(string filename)
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

        /// <summary>
        /// Get the Filenames of all available blobs
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<string>> GetPodcastFilenames()
        {
            var blobs = await container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.Metadata, null, null, null, null);

            return blobs.Results.Select(b => Uri.UnescapeDataString(b.Uri.Segments.Last())).ToList();
        }

        /// <summary>
        /// Get the file size of a specified blob
        /// </summary>
        /// <param name="filename">Blob name</param>
        /// <returns></returns>
        public async Task<long> GetBlobSize(string filename)
        {
            var blob = container.GetBlobReference(filename);
            long length = 0;

            if(await blob.ExistsAsync())
            {
                await blob.FetchAttributesAsync();
                length = blob.Properties.Length;
            }
            
            return length;
        }

        /// <summary>
        /// Get the URL for the podcast from the fileshare
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <returns></returns>
        public string GetPodcastFileShareUrl(string filename)
        {
            return $"http://podcast.irishfantasyleague.com/{filename}";
        }
    }
}