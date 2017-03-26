using Microsoft.AspNetCore.Builder;

namespace Castle.Windsor.MicroKernel.Lifestyle
{
    public static class PerWebRequestLifestyleMiddlewareExtensions
    {
        public static IApplicationBuilder UseCastleWindsor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PerWebRequestLifestyleMiddleware>();
        }
    }
}