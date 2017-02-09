using System;
using System.IO;
using System.Xml;

namespace PodFeedr
{
    public class PodFeedr
    {
        private readonly Podcast podcast;

        private const string ITUNES_NAMESPACE = "http://www.itunes.com/dtds/podcast-1.0.dtd";

        public PodFeedr(Podcast podcast)
        {
            if(podcast == null)
            {
                throw new ArgumentNullException(nameof(podcast));
            }

            this.podcast = podcast;
        }

        /// <summary>
        /// Write XML stream
        /// </summary>
        /// <param name="stream"></param>
        public void WriteStream(Stream stream)
        {
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;

            using(var xmlWriter = XmlWriter.Create(stream, xmlSettings))
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("rss");
                xmlWriter.WriteAttributeString("xmlns", "itunes", null, ITUNES_NAMESPACE);
                xmlWriter.WriteAttributeString("version", "2.0");

                var prefix = xmlWriter.LookupPrefix(ITUNES_NAMESPACE);

                // start channel
                xmlWriter.WriteStartElement("channel");

                xmlWriter.WriteElementString(prefix, "new-feed-url", null, "http://thesocialclubni.com/rss?store=iTunes");

                xmlWriter.WriteElementString("title", podcast.Title);
                xmlWriter.WriteElementString("link", podcast.Link.ToString());
                xmlWriter.WriteElementString("language", podcast.Language);
                xmlWriter.WriteElementString("copyright", podcast.Copyright);

                xmlWriter.WriteElementString(prefix, "subtitle", null, podcast.Subtitle );

                xmlWriter.WriteElementString(prefix, "summary", null, podcast.Summary);
                xmlWriter.WriteElementString("description", podcast.Description);
                xmlWriter.WriteElementString(prefix, "explicit", null, podcast.Explicit ? "yes" : "ho");

                xmlWriter.WriteStartElement(prefix, "image", null);
                xmlWriter.WriteAttributeString("href", podcast.Image.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement(prefix, "owner", null);
                xmlWriter.WriteElementString(prefix, "name", null, podcast.Owner.Name);
                xmlWriter.WriteElementString(prefix, "email", null, podcast.Owner.Email);
                xmlWriter.WriteEndElement();
                
                xmlWriter.WriteStartElement(prefix, "category", null);
                xmlWriter.WriteAttributeString("text", podcast.Category.Name);
                xmlWriter.WriteElementString(prefix, "category", null, podcast.Category.SubCategory);
                xmlWriter.WriteEndElement();

                foreach(var episode in podcast.Items)
                {
                    xmlWriter.WriteStartElement("item");

                    xmlWriter.WriteElementString("title", episode.Title);
                    xmlWriter.WriteElementString(prefix, "author", null, episode.Author);

                    xmlWriter.WriteStartElement(prefix, "category", null);
                    xmlWriter.WriteAttributeString("text", episode.Category);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteElementString(prefix, "subtitle", null, episode.Subtitle);
                    xmlWriter.WriteElementString(prefix, "summary", null, episode.Summary);

                    xmlWriter.WriteStartElement("enclosure");
                    xmlWriter.WriteAttributeString("url", episode.Url.ToString());
                    xmlWriter.WriteAttributeString("length", episode.FileSize.ToString());
                    xmlWriter.WriteAttributeString("type", episode.FileType);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteElementString("guid", episode.Guid);
                    //Tue, 9 Aug 2011 21:00:00 00:00
                    xmlWriter.WriteElementString("pubDate", episode.PubDate.ToString("ddd, d MMM yyyy HH:mm:ss zzz"));
                    xmlWriter.WriteElementString(prefix, "duration", null, episode.Duration.ToString());
                    xmlWriter.WriteElementString(prefix, "keywords", null, episode.Keywords);

                    xmlWriter.WriteEndElement();
                }

                // channel
                xmlWriter.WriteEndElement();

                // rss
                xmlWriter.WriteEndElement();

                // document
                xmlWriter.WriteEndDocument();

                xmlWriter.Flush();
            }
        }
    }
}