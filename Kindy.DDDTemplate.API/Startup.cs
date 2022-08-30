using Kindy.Core.Nacos;
using Kindy.DDDTemplate.API.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kindy.DDDTemplate.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptionsExt(Configuration);
            services.AddHealthChecks();
            services.AddNacosServices(Configuration);
            services.AddCustomerControllers();
            services.AddModelMapper();
            services.AddEFDBContext(Configuration);
            services.AddSqlSugarScopeMutilDatabase(Configuration);
            services.AddMediatRServices();
            services.AddRepositoryServices();
            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwaggerExt();
                app.UseDeveloperExceptionPage();
            }


            //using (var scope = app.ApplicationServices.CreateScope())
            //{
            //    var dc = scope.ServiceProvider.GetService<MasterContext>();
            //    dc.Database.EnsureCreated();  //模型创建数据库
            //}


            //app.UseHttpsRedirection();

            app.UseHealthChecks("/healthChecks");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
