using System;

namespace SocialClubNI.Models
{
    public class Podcast
    {
        /// <summary>
        /// Podcast Title
        /// </summary>
        /// <returns></returns>
        public string Title { get; set; }

        /// <summary>
        /// Podcast Subtitle
        /// </summary>
        /// <returns></returns>
        public string SubTitle { get; set; }

        /// <summary>
        /// Summary of the Podcast
        /// </summary>
        /// <returns></returns>
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
        public string Filename { get; set; }

        /// <summary>
        /// Podcast stub
        /// </summary>
        /// <returns></returns>
        public string Stub { get; set; }
    }
}