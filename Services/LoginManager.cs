using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SocialClubNI.Models;
using Blobr;

namespace SocialClubNI.Services
{
    public class LoginManager
    {
        private readonly StorageWrapper storageWrapper;
        private const string USER_BLOB = "users";

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
            var users = await storageWrapper.GetPageAsync<User>(USER_BLOB);

            var existingUser = users.Items
                                .FirstOrDefault(u => u.Username == username);
            
            if(existingUser != null)
            {
                var saltedPassword = SaltProvider.GenerateHashedPassword(password, existingUser.Salt);
                
                if(saltedPassword == existingUser.Password)
                {
                    return existingUser;
                }
            }
            
            return existingUser;
        }

        public async Task<User> RegisterUserAsync(string username, string email, string password)
        {
            var users = await storageWrapper.GetPageAsync<User>(USER_BLOB);

            var salt = SaltProvider.GenerateSalt();
            var hashedPassword = SaltProvider.GenerateHashedPassword(password, salt);

            Console.WriteLine(salt);
            Console.WriteLine(hashedPassword);

            var user = new User()
            {
                Username = username,
                Password = hashedPassword,
                Salt = salt,
                Email = email,
                Id = Guid.NewGuid().ToString()
            };

            users.AddItem(user);
            await storageWrapper.SavePageAsync(users);

            return user;
        }

        public async Task<User> GetUser(ClaimsPrincipal principal)
        {
            var users = await storageWrapper.GetPageAsync<User>(USER_BLOB);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier);
            
            return userId != null ? users.Items.FirstOrDefault(u => u.Id == userId.Value) : null;
        }

        public async Task<bool> IsExistingUsername(string username)
        {
            var users = await storageWrapper.GetPageAsync<User>(USER_BLOB);

            return users.Items.Any(u => string.Compare(u.Username, username, true) == 0);
        }

        public async Task<bool> IsExistingEmail(string email)
        {
            var users = await storageWrapper.GetPageAsync<User>(USER_BLOB);

            return users.Items.Any(u => string.Compare(u.Email, email, true) == 0);
        }
    }
}