using System.Collections.Generic;
using SocialClubNI.Models;

namespace SocialClubNI.ViewModels
{
    public class ManageEpisodesViewModel
    {
        public ICollection<Podcast> Episodes { get; set; }

        public ManageEpisodesViewModel()
        {
            this.Episodes = new List<Podcast>();
        }
    }
}