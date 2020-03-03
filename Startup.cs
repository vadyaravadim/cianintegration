using CianLayerProcessing.UploaderCrm;
using CianPlatform.Interface;
using CianPlatform.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace CianPlatform
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IHostBuilder HostBuilder { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationSection crmAuthConfig = Configuration.GetSection("ConfigAuthCrm");
            IConfigurationSection cianConfig = Configuration.GetSection("ConfigCian");

            services.AddControllers();
            services.AddTransient<ResultModel>();
            services.AddTransient<IDataProcessingCian, DataProcessingCian>();

            services.AddScoped((service) =>
            {                
                string resource = crmAuthConfig["resource"];
                string login = crmAuthConfig["login"];
                string password = crmAuthConfig["password"];
                string odata = crmAuthConfig["odata"];
                return new UploaderCianData(resource, login, password, odata);
            });

            services.AddSingleton((service) =>
            {
                string basicUrlProject = cianConfig["basicUrlProject"];
                string basicUrl = cianConfig["basicUrl"];
                string basicExcelDownload = cianConfig["basicExcelDownload"];
                string basicApiOffers = cianConfig["basicApiOffers"];
                return new BasicModel(basicUrlProject, basicUrl, basicExcelDownload, basicApiOffers);
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
