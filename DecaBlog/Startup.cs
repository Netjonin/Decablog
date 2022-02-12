using System;
using DecaBlog.Data;
using DecaBlog.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Mailjet.Client;
using DecaBlog.Configurations;
using DecaBlog.Services.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DecaBlog
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
            services.AddControllers()
                .ConfigureApiBehaviorOptions(opt => opt.SuppressModelStateInvalidFilter = true)
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling =
                     Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddNewtonsoftJson(x =>
                     x.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddNewtonsoftJson(x =>
                x.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.Configure<DataProtectionTokenProviderOptions>(option =>
                option.TokenLifespan = TimeSpan.FromHours(3));
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinaryConfig"));
            services.AddAutoMapper();
            services.AddControllers();
            services.AddDbContextExtension(Configuration);
            services.AddTransient<Seeder>();
            services.AddIdentityExtension();
            services.AddSwaggerExtension();
            services.ConfigureCoreServices();
            services.AddHttpClient<IMailjetClient, MailjetClient>(client =>
            {
                client.UseBasicAuthentication(Configuration.GetSection("MailJetKeys")["ApiKey"], Configuration.GetSection("MailJetKeys")["ApiSecret"]);
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("JWT:Key").Value))
                };
            }).AddGoogle(options =>
            {
                options.ClientId = Configuration.GetSection("GoogleOAUTH")["ClientId"];
                options.ClientSecret = Configuration.GetSection("GoogleOAUTH")["ClientSecret"];
            }).AddCookie();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Seeder seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            seeder.SeedMe(env.EnvironmentName).Wait();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            x.SwaggerEndpoint("/swagger/v1/swagger.json", "DecaBlog Api v1"));
        }
    }
}
