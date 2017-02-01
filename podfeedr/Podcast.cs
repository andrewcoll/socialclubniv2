using System;
using System.Collections.Generic;

namespace PodFeedr
{
    public class Podcast
    {
        /// <summary>
        /// Podcast title
        /// </summary>
        /// <returns></returns>
        public string Title { get; set; }

        /// <summary>
        /// Podcast link
        /// </summary>
        /// <returns></returns>
        public Uri Link { get; set; }

        /// <summary>
        /// Podcast language
        /// </summary>
        /// <returns></returns>
        public string Language { get; set; }

        /// <summary>
        /// Podcast copyright
        /// </summary>
        /// <returns></returns>
        public string Copyright { get; set; }

        /// <summary>
        /// Podcast subtitle
        /// </summary>
        /// <returns></returns>
        public string Subtitle { get; set; }

        /// <summary>
        /// Podcast author
        /// </summary>
        /// <returns></returns>
        public string Author { get; set; }

        /// <summary>
        /// Podcast summary
        /// </summary>
        /// <returns></returns>
        public string Summary { get; set; }

        /// <summary>
        /// Podcast description
        /// </summary>
        /// <returns></returns>
        public string Description { get; set; }

        /// <summary>
        /// Podcast owner
        /// </summary>
        /// <returns></returns>
        public Owner Owner { get; set; }

        /// <summary>
        /// Podcast image
        /// </summary>
        /// <returns></returns>
        public Uri Image { get; set; }

        /// <summary>
        /// Podcast category
        /// </summary>
        /// <returns></returns>
        public Category Category { get; set; }

        /// <summary>
        /// Is podcast explicit
        /// </summary>
        /// <returns></returns>
        public bool Explicit { get; set; }

        /// <summary>
        /// Podcast episodes
        /// </summary>
        /// <returns></returns>
        public ICollection<Episode> Items { get; set; }

        public Podcast()
        {
            this.Items = new List<Episode>();
        }
    }
}