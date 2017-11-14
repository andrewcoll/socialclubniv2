using Microsoft.AspNetCore.Builder;

namespace SocialClubNI.Services
{
    public static class RssMiddlewareExtensions
    {
        public static IApplicationBuilder UseRssFeed(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RssMiddleware>();
        }
    }
}