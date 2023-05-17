using CarConnectBusinessLogic.BusinessLogics;
using CarConnectContracts.BusinessLogicsContracts;
using CarConnectContracts.StorageContracts;
using CarConnectDataBase.Implements;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace CarConnectServer
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
            services.AddScoped<CarConnectDataBase.CarConnectDataBase>();

            services.AddTransient<IUserStorage, UserStorage>();
            services.AddTransient<IUserLogic, UserLogic>();

            services.AddTransient<ICarLogic, CarLogic>();
            services.AddTransient<ICarStorage, CarStorage>();

            services.AddTransient<IReviewLogic, ReviewLogic>();
            services.AddTransient<IReviewStorage, ReviewStorage>();

            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CarConnectServer", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RepairRestApi v1"));
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
