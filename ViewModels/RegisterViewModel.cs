using System.ComponentModel.DataAnnotations;

namespace SocialClubNI.ViewModels
{
    public class RegisterViewModel
    {
        /// <summary>
        /// Username
        /// </summary>
        /// <returns></returns>
        public string Username { get; set; }
        
        /// <summary>
        /// User email
        /// </summary>
        /// <returns></returns>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        /// <returns></returns>
        [DataTypeAttribute(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Password confirmation
        /// </summary>
        /// <returns></returns>
        [DataTypeAttribute(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}