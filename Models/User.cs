using System;

namespace SocialClubNI.Models
{
    public class User
    {
        /// <summary>
        /// Unique User Identity
        /// </summary>
        /// <returns></returns>
        public string Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        /// <returns></returns>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        /// <returns></returns>
        public string Password { get; set; }

        /// <summary>
        /// Password salt
        /// </summary>
        /// <returns></returns>
        public string Salt { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        /// <returns></returns>
        public string Email { get; set; }
    }
}