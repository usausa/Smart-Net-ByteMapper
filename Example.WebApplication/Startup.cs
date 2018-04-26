namespace Example.WebApplication
{
    using Example.WebApplication.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Smart.AspNetCore.Formatters;
    using Smart.IO.ByteMapper;

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
            // TODO
            var mapperFactory = new MapperFactoryConfig()
                .CreateMapByExpression<SampleData>(10, c => { })
                .ToMapperFactory();

            services.AddMvc(options =>
            {
                var outputFormatter = new ByteMapperOutputFormatter(mapperFactory);
                outputFormatter.SupportedMediaTypes.Add("text/x-fixrecord");
                options.OutputFormatters.Add(outputFormatter);
                options.FormatterMappings.SetMediaTypeMappingForFormat("dat", "text/x-fixrecord");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
