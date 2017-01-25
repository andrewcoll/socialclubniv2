using System;
using System.Security.Claims;
using System.Threading.Tasks;
using SocialClubNI.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace SocialClubNI.Services
{
    public class ClaimsManager
    {
        
        public ClaimsPrincipal CreatePrincipalAsync(User user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claimId = new ClaimsIdentity();
            claimId.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claimId.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            claimId.AddClaim(new Claim("LastChanged", DateTime.UtcNow.ToString()));
            claimId.AddClaim(new Claim("testing", "true"));
            return new ClaimsPrincipal(claimId);
        }
    }
}