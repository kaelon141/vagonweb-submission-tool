using VagonWebSubmissionTool.Application;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Web;

/// <summary>
/// Language resolver that can statelessly determine which language to use based on the Accept-Language header in the current HTTP request.
/// Default to English when the Accept-Language header is not specified or when no supported language is included in the header.
/// </summary>
/// <param name="httpContextAccessor"></param>
public class HttpRequestLanguageResolver(IHttpContextAccessor httpContextAccessor) : ILanguageResolver
{
    public Language Resolve()
    {
        //If Accept-Language is not specified in the HTTP request, use the default language.
        if (httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("Accept-Language", out var acceptLanguageHeaderValues) == false)
            return ILanguageResolver.DefaultLanguage;

        //parse the Accept-Language header with priority-awareness as specified in RFC 3282.
        List<(Language Language, decimal Priority)?> acceptedLanguages = [];
        foreach (var entry in acceptLanguageHeaderValues.Where(x => x is not null).SelectMany(x => x!.Split(',')))
        {
            var entryParts = entry.Split(';');
            
            var languageCode = entryParts[0];
            var priority = entryParts.Length > 1 && entryParts[1].StartsWith("q=") && decimal.TryParse(entryParts[1][2..], out var q)
                ? q : 1m;

            //If the language code is not supported, the LanguageBuilder returns Maybe.None, in which case we'll skip that code.
            var language = LanguageBuilder.TryParseLanguageCode(languageCode);
            if (language.HasNoValue)
                continue;
            
            acceptedLanguages.Add((language.Value, priority));
        }
        
        //pick whatever language is supported and has the highest priority, or fall back to the default language.
        return acceptedLanguages.OrderByDescending(x => x!.Value.Priority).FirstOrDefault()?.Language ?? ILanguageResolver.DefaultLanguage;
    }
}