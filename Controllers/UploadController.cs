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

        public async Task<object> Upload()
        {
           
             if(!IsMultiPartContentType(HttpContext.Request.ContentType))
             {
                 return BadRequest();
             }

             if(HttpContext.Request.Form.Files.Count != 1)
             {
                 return BadRequest();
             }
            
            FlowChunk chunkInfo = FlowChunk.ParseForm(HttpContext.Request.Form);
            string chunkNumber = chunkInfo.Number.ToString("000000");
            string tempFilePath = $"~tmp0/{chunkInfo.Identifier}.part{chunkNumber}";
            string directory = Path.GetDirectoryName(tempFilePath);
            
            int remainder;
            int totalChunks = Math.DivRem(chunkInfo.TotalSize, chunkInfo.Size, out remainder);

            if(remainder > 0)
            {
                totalChunks++;
            }
            
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            System.IO.File.Delete(tempFilePath);
            
            byte[] buffer = new byte[chunkInfo.Size];
            
            using(var stream = new FileStream(tempFilePath, FileMode.CreateNew))
            {
                int readBytes = 0;
                using(var readStream = Request.Form.Files[0].OpenReadStream())
                {
                    readBytes = await readStream.ReadAsync(buffer, 0, chunkInfo.Size);
                }

                await stream.WriteAsync(buffer, 0, readBytes);
            }

            var files = Directory.GetFiles(directory, $"{chunkInfo.Identifier}.part*");
            Console.WriteLine($"Chunk {chunkInfo.Number} found {files.Length} files");
            if(files.Length == totalChunks)
            {
                string filePath = Path.Combine(directory, chunkInfo.Filename);

                List<byte> fileData = new List<byte>();
                foreach(string file in files.OrderBy(x => x))
                {
                    Console.WriteLine($"Writing {file}");
                    var contents = await System.IO.File.ReadAllBytesAsync(file);
                    fileData.AddRange(contents);
                    System.GC.Collect(); 
                    System.GC.WaitForPendingFinalizers(); 
                    System.IO.File.Delete(file);
                }   

                using(var stream = new MemoryStream(fileData.ToArray()))
                {
                    await fileProvider.UploadFileAsync(stream, chunkInfo.Filename);
                }
            }

            return Ok();
        }

        private bool IsMultiPartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) 
                && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}