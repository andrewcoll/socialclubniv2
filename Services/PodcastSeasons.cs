using System;
using System.Collections.Generic;
using System.Linq;
using SocialClubNI.Models;

namespace SocialClubNI.Services
{
    public class PodcastSeasons
    {
        public Season CurrentSeason { get; private set; }
        public ICollection<Season> Seasons { get; set; }

        public PodcastSeasons(ICollection<Season> seasons)
        {
            if(seasons == null)
            {
                throw new ArgumentNullException(nameof(seasons));
            }

            this.Seasons = seasons;
            this.CurrentSeason = seasons.OrderByDescending(s => s.StartDate).First();
        }
    }
}