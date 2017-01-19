using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using SocialClubNI.Models;
using Blobr;

namespace SocialClubNI.Services
{
    public class LoginManager
    {
        private readonly StorageWrapper storageWrapper;
        private const string USER_BLOB = "users";

        private static List<User> temp = new List<User>();

        public LoginManager(StorageWrapper storageWrapper)
        {
            this.storageWrapper = storageWrapper;
        }

        /// <summary>
        /// Get a user based on username and password
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns></returns>
        public async Task<User> VerifyLoginAsync(string username, string password)
        {
            //var users = await storageWrapper.GetPageAsync<User>(USER_BLOB);

            //var existingUser = users.Items
            //                    .FirstOrDefault(u => u.Username == username && u.Password == password);
            
            var existingUser = new User() { Username = "Andrew", Id = Guid.NewGuid().ToString() };
            temp.Add(existingUser);
            return existingUser;
        }

        public async Task<User> GetUser(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier);
            var user = temp.FirstOrDefault(u => u.Id == userId.Value);

            return user;
        }
    }
}