using System;
using System.Collections.Generic;
using SocialClubNI.Models;

namespace SocialClubNI.Services
{
    public class SeasonProviderFactory
    {
        private DateTimeOffset startDate = new DateTimeOffset(2011, 07, 01, 00, 00, 00, 00, TimeSpan.FromSeconds(0));

        public PodcastSeasons GetPodcastSeasons(DateTimeOffset currentDate)
        {
            if(DateTimeOffset.Compare(currentDate, startDate) < 0)
            {
                throw new ArgumentException($"Current date is invalid, it must be after {startDate.ToString()}");
            }

            List<Season> seasons = new List<Season>();

            while(startDate < currentDate)
            {
                var previousDate = startDate;
                startDate = startDate.AddYears(1);
                var prevAbbrv = previousDate.ToString("yy");
                var nextAbbrv = startDate.ToString("yy");

                // var seasonStartDate = currentDate;
                // var earlierAbbreviation = seasonStartDate.ToString("yy");
                // currentDate = currentDate.AddYears(1);
                // var laterAbbreviation = currentDate.ToString("yy");

                var seasonAbbr = $"{prevAbbrv}{nextAbbrv}";
                var seasonName = $"{prevAbbrv}/{nextAbbrv}";
                seasons.Add(new Season() { Abbreviation = seasonAbbr, StartDate = previousDate, Name = seasonName });
            }

            return new PodcastSeasons(seasons);
        }
    }
}