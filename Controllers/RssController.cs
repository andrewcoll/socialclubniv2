using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using SocialClubNI.Services;
using Blobr;
using PodFeedr;
using Microsoft.Extensions.Caching.Memory;

namespace SocialClubNI
{
    public class RssController : Controller
    {
        private readonly TelemetryClient telemetryClient;
        private readonly StorageWrapper storageWrapper;
        private readonly PodcastFileProvider fileProvider;
        private readonly PodcastSeasons podcastSeasons;
        private readonly IMemoryCache cache;

        public RssController(
            StorageWrapper storageWrapper, 
            PodcastFileProvider fileProvider, 
            TelemetryClient telemetryClient, 
            SeasonProviderFactory seasonProviderFactory, 
            IMemoryCache cache)
        {
            this.storageWrapper = storageWrapper;
            this.fileProvider = fileProvider;
            this.telemetryClient = telemetryClient;
            this.podcastSeasons = seasonProviderFactory.GetPodcastSeasons(DateTime.UtcNow);
            this.cache = cache;
        }

        public async Task<string> Index()
        {
            telemetryClient.TrackEvent("rssrequest");
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

            pod.Image = new Uri("https://storage.thesocialclubni.com/static/logo.jpg");
            
            pod.Category = new Category()
            {
                Name = "Sports & Recreation",
                SubCategory = "Professional"
            };

            pod.Explicit = false;
            pod.Items = new List<PodFeedr.Episode>();
            
            var page = await GetAllPodcasts();      

            foreach(var episode in page.OrderBy(p => p.Published))
            {
                var ep = new PodFeedr.Episode();
                ep.Title = episode.Title;
                ep.Summary = episode.Summary;
                ep.Subtitle = episode.SubTitle;
                ep.PubDate = episode.Published; 
                ep.Author = "Laure James, Keith Bailie, Conor McLaughlin, Mark McIntosh";
                ep.Url = new Uri($"http://thesocialclubni.com/download/{episode.Filename}?store=itunes");
                ep.FileSize = episode.FileSize;
                ep.FileType = "audio/mpeg";
                ep.Category = "Sports & Recreation";
                ep.Guid = $"http://irishfantasyleague.com/podcast/{episode.Filename}";
                
                // really ugly hack
                var duration = episode.Duration;
                if(duration.Count(c => c == ':') == 1)
                {
                    duration = "00:" + duration;
                }

                ep.Duration = TimeSpan.Parse(duration);

                pod.Items.Add(ep);
            }

            var podFeedr = new PodFeedr.PodFeedr(pod);

            string response = string.Empty;
            using(var stream = new MemoryStream())
            {
                podFeedr.WriteStream(stream);
                response = Encoding.UTF8.GetString(stream.ToArray());
            }

            HttpContext.Response.ContentType = "application/xml";
            return response;
        }

        private async Task<ICollection<SocialClubNI.Models.Podcast>> GetAllPodcasts()
        {
            var podcasts = new List<SocialClubNI.Models.Podcast>();

            var seasons = podcastSeasons.Seasons.OrderBy(s => s.StartDate);
            foreach(var s in seasons)
            {
                try
                {
                    var page = await GetPage<SocialClubNI.Models.Podcast>($"podcasts-{s.Abbreviation}");
                    podcasts.AddRange(page.Items);
                }
                catch(BlobrLoadException)
                {
                    continue;
                }
            }

            return podcasts;
        }

        private async Task<Page<T>> GetPage<T>(string pageName)
        {
            //  return await cache.GetOrCreateAsync(pageName, async entry => 
            //  {
            //      entry.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15);

            //      return await storageWrapper.GetPageAsync<T>(pageName);
            //  });

            return await storageWrapper.GetPageAsync<T>(pageName);
        }
    }
}
