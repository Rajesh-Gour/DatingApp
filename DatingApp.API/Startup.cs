using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System.Web;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using  DatingApp.API.Helper;
using AutoMapper;
using  Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

       
        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(X => X.UseSqlite
            (Configuration.GetConnectionString("DefaultConnection")));
            
            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            // Once my sql installed un-comment below 2 lines
            // services.AddDbContext<DataContext>(X => X.UseMySql
            // (Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<DataContext>(X => X.UseSqlite
            (Configuration.GetConnectionString("DefaultConnection")));
            
            ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<DataContext>(X => X.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            //Video 76
            // services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            // .AddJsonOptions ( opt => {

            //     opt.SerializerSettings.ReferenceLoopHandling = 
            //     Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            // });
            services.AddControllers().AddNewtonsoftJson();
            services.AddCors();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper(typeof(DatingRepository).Assembly);
            services.AddScoped<IAuthRepository,AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(Options => {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                ValidateIssuerSigningKey=true,
                IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                ValidateIssuer=false,
                ValidateAudience=false
            };
            });

            services.AddScoped<LogUserActivity>();
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // Use the default property (Pascal) casing
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });



            //services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(Builder =>{

                    Builder.Run(async context =>{
                    
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var error=context.Features.Get<IExceptionHandlerFeature>();

                    if (error!=null)
                    {
                        context.Response.AddApplicationError(error.Error.Message);
                        await context.Response.WriteAsync(error.Error.Message);
                    } 
                    });
                });
            }

            //app.UseHttpsRedirection();
            
            app.UseRouting();

            

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
