using System.Collections.Immutable;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Icebear.Exceptions.Middleware
{
    public static class IcebearExceptionAppBuilder
    {
        public static void UseIcebearException(this IApplicationBuilder app)
        {
            /*app.Use(async (context, next) =>
            {
                // Do work that doesn't write to the Response.
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });*/

            app.Run(async context =>
            {
                var request = context.Request;
                var path = context.Request.Path;

                if (path.Equals("/icebear/logs"))
                {
                    await context.Response.WriteAsync("Build the error page");
                }
                else if (path.Equals("/icebear/error"))
                {
                    await context.Response.WriteAsync("Exception page");
                }
                /*var formParams = request.Form;
                var queryParams = request.Query;
                var pathParams = request.Path;*/
            });
        }
    }
}