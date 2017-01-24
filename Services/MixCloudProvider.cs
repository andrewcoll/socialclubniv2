using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SocialClubNI.Models;

namespace SocialClubNI.Services
{
    public class MixCloudProvider
    {
        // URL for retrieving embed HTML from mixcloud for a podcast
        private const string EMBED_URL = "https://www.mixcloud.com/oembed/?url=https%3A//www.mixcloud.com/ardskeith/{0}/&format=json";

        // URL for retrieving podcast metadata
        private const string METADATA_URL = "http://api.mixcloud.com/ardskeith/{0}/";
        
        
        /// <summary>
        /// Retrieve the embed code for a podcast
        /// </summary>
        /// <param name="stub">The unique podcast stub</param>
        /// <returns>Embed HTML</returns>
        public async Task<string> GetMixCloudEmbed(string stub)
        {
            if(string.IsNullOrWhiteSpace(stub))
            {
                throw new ArgumentNullException(nameof(stub));
            }

            var client = new HttpClient();
            var url = string.Format(EMBED_URL, stub);

            var result = await client.GetAsync(url);
            if(!result.IsSuccessStatusCode)
            {
                throw new MixCloudException($"Error retrieving embed for stub '{stub}'");
            }

            var response = await result.Content.ReadAsStringAsync();
            var parsedResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            return parsedResponse["embed"];
        }


        /// <summary>
        /// Retrieve podcast metadata from MixCloud
        /// </summary>
        /// <param name="stub">Unique Podcast stub</param>
        /// <returns>Podcast metadata</returns>
        public async Task<Podcast> GetMixCloudMetata(string stub)
        {
            if(string.IsNullOrWhiteSpace(stub))
            {
                throw new ArgumentNullException(nameof(stub));
            }

            var client = new HttpClient();
            var url = string.Format(METADATA_URL, stub);

            var result = await client.GetAsync(url);
            if(!result.IsSuccessStatusCode)
            {
                throw new MixCloudException($"Error retrieving metadata for stub '{stub}'");
            }

            var response = await result.Content.ReadAsStringAsync();
            var converter = new ExpandoObjectConverter();
            dynamic json = JsonConvert.DeserializeObject<ExpandoObject>(response, converter);

            var podcast = new Podcast();
            podcast.Title = json.name;
            podcast.Summary = json.description;
            podcast.Published = json.created_time;
            podcast.Stub = stub;
            var duration = TimeSpan.FromSeconds(json.audio_length);
            podcast.Duration = duration.ToString();

            return podcast;
        }
    }
}