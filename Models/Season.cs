using System;

namespace SocialClubNI.Models
{
    public class Season
    {
        public string Abbreviation { get; set; }
        public string Name { get; set;}
        public DateTimeOffset StartDate { get; set; }
    }
}