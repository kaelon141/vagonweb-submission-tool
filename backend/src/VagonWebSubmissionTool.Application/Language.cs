using CSharpFunctionalExtensions;

namespace VagonWebSubmissionTool.Application;

public enum Language
{
    English = 0,
    Czech = 1,
    German = 2,
    Polish = 3,
    Hungarian = 4,
    Spanish = 5
}

public static class Languages
{
    public static Language[] All => [Language.English, Language.Czech, Language.German, Language.Polish, Language.Hungarian, Language.Spanish];
}

public static class LanguageBuilder
{
    public static Maybe<Language> TryParseLanguageCode(string languageCode)
    {
        if (languageCode.Length != 2 && (languageCode.Length != 5 || languageCode is not [_, _, '-', _, _]))
            return Maybe<Language>.None;

        var languagePart = languageCode[..2].ToLower();
        return languagePart switch
        {
            "en" => Language.English,
            "cs" => Language.Czech,
            "de" => Language.German,
            "pl" => Language.Polish,
            "hu" => Language.Hungarian,
            "es" => Language.Spanish,
            _ => Maybe<Language>.None
        };
    }

    public static string ToVagonWebLanguageCode(this Language language) => language switch
    {
        Language.English => "en",
        Language.Czech => "cs",
        Language.German => "de",
        Language.Polish => "pl",
        Language.Hungarian => "hu",
        Language.Spanish => "es",
        _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
    };

    public static string ToBcp47Code(this Language language) => language switch
    {
        Language.English => "en-GB",
        Language.Czech => "cs-CZ",
        Language.German => "de-DE",
        Language.Polish => "pl-PL",
        Language.Hungarian => "hu-HU",
        Language.Spanish => "es-ES",
        _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
    };

    public static string ToNativeName(this Language language) => language switch
    {
        Language.English => "English",
        Language.Czech => "Český",
        Language.German => "Deutsch",
        Language.Polish => "Polski",
        Language.Hungarian => "Magyar",
        Language.Spanish => "Español",
        _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
    };
}