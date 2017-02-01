using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialClubNI.Services;
using SocialClubNI.Models;
using Blobr;
using PodFeedr;

namespace SocialClubNI
{
    public class RssController
    {
        private readonly StorageWrapper storageWrapper;

        public RssController(StorageWrapper storageWrapper)
        {
            this.storageWrapper = storageWrapper;
        }

        public async Task<string> Index()
        {
            var pod = new PodFeedr.Podcast();
            pod.Title = "The Social Club";
            pod.Link = new Uri("http://thesocialclubni.com");
            pod.Copyright = "Copyright 2011-2017 The Social Club";
            pod.Language = "en-us";
            pod.Subtitle = "Northern Ireland's Football Podcast";
            pod.Author = "Laure James, Keith Bailie, Conor McLaughlin, Mark McIntosh";
            pod.Summary = "Fortnightly podcast for fans of Northern Irish football.";
            pod.Description = "Northern Ireland's football podcast. Every other Tuesday Conor, Keith, Laure and Mark bring you a shiny new podcast to gorge your ears on. We cover everything from the international scene to domestic action.";

            pod.Owner = new PodFeedr.Owner()
            {
                Name = "Podcast Team",
                Email = "podcast@thesocialclubni.com"
            };

            pod.Image = new Uri("http://irishfantasyleague.com/podcast/logo.jpg");
            
            pod.Category = new Category()
            {
                Name = "Sports & Recreation",
                SubCategory = "Professional"
            };

            var page = await this.storageWrapper.GetPageAsync<SocialClubNI.Models.Podcast>("podcasts-1617");            

            pod.Explicit = false;
            pod.Items = new List<PodFeedr.Episode>();

            foreach(var episode in page.Items.OrderBy(p => p.Published))
            {
                var ep = new PodFeedr.Episode();
                ep.Title = episode.Title;
                ep.Summary = episode.Summary;
                ep.Subtitle = episode.SubTitle;
                //Console.WriteLine(episode.Duration);
                //ep.Duration = TimeSpan.Parse(episode.Duration);
                
                ep.PubDate = episode.Published; 

                ep.Url = new Uri($"http://thesocialclubni.com/download/{episode.Filename}?store=itunes");
                ep.FileSize = 0;
                ep.FileType = "audio/mpeg";

                ep.Category = "Sports & Recreation";
                ep.Guid = $"http://irishfantasyleague.com/podcast/{episode.Filename}";

                pod.Items.Add(ep);
            }

            var podFeedr = new PodFeedr.PodFeedr(pod);

            string response = string.Empty;

            using(var stream = new MemoryStream())
            {
                podFeedr.WriteStream(stream);
                response = Encoding.UTF8.GetString(stream.ToArray());
            }

            return response;
        }
    }
}