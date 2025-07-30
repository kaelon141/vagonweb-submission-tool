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

    public static string ToVagonwebLanguageCode(this Language language)
        => language switch
        {
            Language.English => "en",
            Language.Czech => "cs",
            Language.German => "de",
            Language.Polish => "pl",
            Language.Hungarian => "hu",
            Language.Spanish => "es",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
}