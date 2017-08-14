using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using SocialClubNI.Services;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SocialClubNI.Controllers
{
    public class FileController : Controller
    {
        private readonly TelemetryClient telemetryClient;
        private readonly PodcastFileProvider fileProvider;
        private readonly CloudBlobContainer cloudBlobContainer;
        
        public FileController(PodcastFileProvider fileProvider, CloudBlobContainer container, TelemetryClient telemetryClient)
        {
            this.fileProvider = fileProvider;
            this.telemetryClient = telemetryClient;
            this.cloudBlobContainer = container;
        }

        /// <summary>
        /// Download an episode from blob storage
        /// </summary>
        /// <param name="filename">Name of the file to download</param>
        public void Download(string filename, string store = "web")
        {
            telemetryClient.TrackEvent($"dl-{store}-{filename}");
            var uri = fileProvider.GetPodcastUrl(filename);
            Response.Redirect(uri);
        }

        [Authorize(Policy="IsLoggedIn")]
        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var formAccumulator = new KeyValueAccumulator();
            string targetFilePath = null;
            
            var formOptions = new FormOptions();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                formOptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            while(section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if(!hasContentDispositionHeader)
                {
                    var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                    var encoding = GetEncoding(section);

                    using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            formAccumulator.Append(key, value);
                        }
                }

                section = await reader.ReadNextSectionAsync();
            }

            var results = formAccumulator.GetResults();
            
            return View("FileUploadSuccess");
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }

        private static bool IsMultipartContentType(string contentType)
        {
            return 
                !string.IsNullOrEmpty(contentType) &&
                contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string GetBoundary(string contentType)
        {
            var elements = contentType.Split(' ');
            var element = elements.Where(entry => entry.StartsWith("boundary=")).First();
            var boundary = element.Substring("boundary=".Length);
            // Remove quotes
            if (boundary.Length >= 2 && boundary[0] == '"' && 
                boundary[boundary.Length - 1] == '"')
            {
                boundary = boundary.Substring(1, boundary.Length - 2);
            }
            return boundary;
        }

        private string GetFileName(string contentDisposition)
        {
            return contentDisposition
                .Split(';')
                .SingleOrDefault(part => part.Contains("filename"))
                .Split('=')
                .Last()
                .Trim('"');
        }
    }
}