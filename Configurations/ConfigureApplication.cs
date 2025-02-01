using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Configurations
{
    public static class ConfigureApplication
    {
        public static void UseBuildExtensions(this IApplicationBuilder app)
        {
            //Configure Localization
            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("ar-SA"),
                new CultureInfo("en-US")
            };
            supportedCultures[0].NumberFormat.NumberDecimalSeparator = ".";

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
        }
    }
}
