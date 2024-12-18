using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace GamboCity_DiscordBot.Language;
public static class LanguageHelper {
    public static void SetLanguageFromConfiguration(IConfiguration configuration) {
        string? languageString = configuration["LANGUAGE"];

        if (languageString == null)
            return;

        SetLanguageFromString(configuration["LANGUAGE"]!);
    }

    public static void SetLanguageFromString(string language) {
        CultureInfo cultureInfo = new(language);

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
    }
}
