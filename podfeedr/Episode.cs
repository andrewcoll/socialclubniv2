using System;

namespace PodFeedr
{
    public class Episode
    {
        /// <summary>
        /// The podcast title
        /// </summary>
        /// <returns></returns>
        public string Title { get; set; }

        /// <summary>
        /// Podcast Author(s)
        /// </summary>
        /// <returns></returns>
        public string Author { get; set; }

        /// <summary>
        /// Podcast summary
        /// </summary>
        /// <returns></returns>
        public string Summary { get; set; }

        /// <summary>
        /// Podcast unique GUID
        /// </summary>
        /// <returns></returns>
        public string Guid { get; set; }

        /// <summary>
        /// Podcast published date
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset PubDate { get; set; }

        /// <summary>
        /// Podcast duration
        /// </summary>
        /// <returns></returns>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Podcast keywords, space separated
        /// </summary>
        /// <returns></returns>
        public string Keywords { get; set; }

        /// <summary>
        /// Podcast subtitle
        /// </summary>
        /// <returns></returns>
        public string Subtitle { get; set; }

        /// <summary>
        /// Podcast URL
        /// </summary>
        /// <returns></returns>
        public Uri Url { get; set;}

        /// <summary>
        /// Podcast File Size
        /// </summary>
        /// <returns></returns>
        public long FileSize { get; set; }

        /// <summary>
        /// Podcast File Type
        /// </summary>
        /// <returns></returns>
        public string FileType { get; set; }

        /// <summary>
        /// Podcast Category
        /// </summary>
        /// <returns></returns>
        public string Category { get; set; }
    }
}
