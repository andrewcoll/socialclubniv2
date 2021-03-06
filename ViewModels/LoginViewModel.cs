using System.ComponentModel.DataAnnotations;

namespace SocialClubNI.ViewModels
{
    public class LoginViewModel
    {
        /// <summary>
        /// Username
        /// </summary>
        /// <returns></returns>
        public string Username { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        /// <returns></returns>
        [DataTypeAttribute(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Remember me
        /// </summary>
        /// <returns></returns>
        public bool RememberMe { get; set; }
    }
}