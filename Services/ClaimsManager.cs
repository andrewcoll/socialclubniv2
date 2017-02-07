using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SocialClubNI.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace SocialClubNI.Services
{
    public class ClaimsManager
    {
        private const string CLAIM_ISSUER = "http://thesocialclubni.com";

        public ClaimsPrincipal CreatePrincipalAsync(User user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id, ClaimValueTypes.String, CLAIM_ISSUER));
            claims.Add(new Claim(ClaimTypes.Name, user.Username, ClaimValueTypes.String, CLAIM_ISSUER));

            var identity = new ClaimsIdentity(claims, "Passport");
            var principal = new ClaimsPrincipal(identity);
            
            return principal;
        }
    }
}