using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SocialClubNI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SocialClubNI.ViewModels
{
    public class CreateEpisodeViewModel
    {

        /// <summary>
        /// Podcast Title
        /// </summary>
        /// <returns></returns>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Podcast Subtitle
        /// </summary>
        /// <returns></returns>
        [Required]
        public string SubTitle { get; set; }

        /// <summary>
        /// Summary of the Podcast
        /// </summary>
        /// <returns></returns>
        [Required]
        public string Summary { get; set; } 

        /// <summary>
        /// Published time
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset Published { get; set; }

        /// <summary>
        /// Podcast Duration
        /// </summary>
        /// <returns></returns>
        public string Duration { get; set; }

        /// <summary>
        /// Podcast season
        /// </summary>
        /// <returns></returns>
        public string Season { get; set; }

        /// <summary>
        /// Podcast Filename
        /// </summary>
        /// <returns></returns>
        [Required]
        public string Filename { get; set; }

        /// <summary>
        /// Podcast stub
        /// </summary>
        /// <returns></returns>
        public string Stub { get; set; }

        /// <summary>
        /// Existing Mp3s
        /// </summary>
        /// <returns></returns>
        public ICollection<SelectListItem> Mp3s { get; set; }

        public static CreateEpisodeViewModel FromPodcast(Podcast podcast)
        {
            var cevm = new CreateEpisodeViewModel();
            cevm.Title = podcast.Title;
            cevm.Summary = podcast.Summary;
            cevm.SubTitle = podcast.SubTitle;
            cevm.Published = podcast.Published;
            cevm.Duration = podcast.Duration;
            cevm.Season = podcast.Season;
            cevm.Filename = podcast.Filename;
            cevm.Stub = podcast.Stub;

            return cevm;
        }
    }
}