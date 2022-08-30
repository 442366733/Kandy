using Microsoft.AspNetCore.Builder;

namespace Kindy.DDDTemplate.API.Extension
{
    /// <summary>
    /// 中间件扩展
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerExt(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            return app;
        }
    }
}
