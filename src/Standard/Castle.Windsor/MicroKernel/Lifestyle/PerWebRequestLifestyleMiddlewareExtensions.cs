using Castle.Windsor.Windsor;
using Microsoft.AspNetCore.Builder;

namespace Castle.Windsor.MicroKernel.Lifestyle
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