using Castle.Windsor;
using Microsoft.AspNetCore.Builder;

namespace Castle.MicroKernel.Lifestyle
{
    public static class PerWebRequestLifestyleMiddlewareExtensions
    {
        internal static IWindsorContainer Container = null;

        public static IApplicationBuilder UseCastleWindsor(this IApplicationBuilder builder, IWindsorContainer container)
        {
            Container = container;
            return builder.UseMiddleware<PerWebRequestLifestyleMiddleware>();
        }
    }
}