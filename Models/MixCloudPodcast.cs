namespace SocialClubNI.Models
{
    public class MixCloudPodcast : Podcast
    {
        /// <summary>
        /// The embed code for the mixcloud player
        /// </summary>
        /// <returns></returns>
        public string Embed { get; set; }

        public static MixCloudPodcast FromPodcast(Podcast original, string embed)
        {
            var mixCloudPodcast = new MixCloudPodcast();
            mixCloudPodcast.Title = original.Title;
            mixCloudPodcast.Summary = original.Summary;
            mixCloudPodcast.SubTitle = original.SubTitle;
            mixCloudPodcast.Duration = original.Duration;
            mixCloudPodcast.Filename = original.Filename;
            mixCloudPodcast.Published = original.Published;
            mixCloudPodcast.Season = original.Season;
            mixCloudPodcast.Stub = original.Stub;
            mixCloudPodcast.Embed = embed;

            return mixCloudPodcast;
        }
    }
}