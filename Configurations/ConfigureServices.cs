using Configurations.Interfaces;
using Configurations.Policies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Globalization;
using System.Text;

namespace Configurations
{
    public static class ConfigureServices
    {
        public static void AddInjectionApplication(this IServiceCollection services, IConfiguration configuration)
        {

            var assemblies = DependencyInjectionConfig.GetAssemblies() ?? [];

            #region Injection Services
            var interfaces = assemblies.SelectMany(x => x.GetTypes()).ToList()
                .Where(t => t.IsInterface && (typeof(IScopedService).IsAssignableFrom(t))).ToList();

            var classes = assemblies.SelectMany(x => x.GetTypes()).ToList()
                .Where(t => t.IsClass && (typeof(IScopedService).IsAssignableFrom(t))).ToList();
            interfaces.ForEach(inter =>
            {
                var cls = classes.FirstOrDefault(x => inter.IsAssignableFrom(x));

                if (cls != null)
                    services.AddScoped(inter, cls);
            });
            #endregion

            #region Injection Repositories
            var repoInterfaces = assemblies.SelectMany(x => x.GetTypes()).ToList()
                .Where(t => t.IsInterface && typeof(IScopedRepository).IsAssignableFrom(t)).ToList();

            var repoClasses = assemblies.SelectMany(x => x.GetTypes()).ToList()
                .Where(t => !t.IsAbstract && t.IsClass && typeof(IScopedRepository).IsAssignableFrom(t)).ToList();
            repoInterfaces.ForEach(inter =>
            {
                var cls = repoClasses.FirstOrDefault(x => inter.IsAssignableFrom(x));

                if (cls != null)
                    services.AddScoped(inter, cls);
            });
            #endregion

            #region Add Logger
            var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(logger);
            });
            #endregion

            #region Add Localization
            services.AddLocalization(option => option.ResourcesPath = "");
            services.Configure<RequestLocalizationOptions>(option =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("ar"),
                    new CultureInfo("en")
                };

                option.SupportedCultures = supportedCultures;
                option.SupportedUICultures = supportedCultures;

                option.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
                {
                    var userLangs = context.Request.Headers["Accept-Language"].ToString();
                    var defaultLang = string.IsNullOrEmpty(userLangs) ? "en" : userLangs == "ar-SA" ? "ar" : "en";

                    return Task.FromResult(new ProviderCultureResult("en", defaultLang))!;
                }));
            });
            #endregion

            services.RegisterAuthentication(configuration);
        }

        private static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = configuration["Token:Issuer"],
                        ValidAudience = configuration["Token:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"]))
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.SuperAdmin, policy => policy.RequireRole("SuperAdmin"));
                options.AddPolicy(AuthorizationPolicies.Admin, policy => policy.RequireRole("Admin"));
                options.AddPolicy(AuthorizationPolicies.SystemUser, policy => policy.RequireRole("SystemUser"));
            });
        }
    }
}
