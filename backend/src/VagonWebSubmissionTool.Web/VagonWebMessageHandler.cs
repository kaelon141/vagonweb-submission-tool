using System.Net;
using Microsoft.Extensions.Primitives;
using VagonWebSubmissionTool.Application;

namespace VagonWebSubmissionTool.Web;

public class VagonWebMessageHandler(IHttpContextAccessor httpContextAccessor) : HttpClientHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var language = httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("Accept-Language", out var value) == true
            ? ParseAndPickLanguageFromHttpHeader(value)
            : "en";
        
        CookieContainer.Add(new Cookie("vagonweb[lang]", language, "/", Constants.VagonWebDomain));
        
        return base.SendAsync(request, cancellationToken);
    }

    static string ParseAndPickLanguageFromHttpHeader(StringValues headerValues)
    {
        List<(string Language, decimal Priority)?> acceptedLanguages = [];
    
        foreach (var entry in headerValues.Where(x => x is not null).SelectMany(x => x!.Split(',')))
        {
            var entryParts = entry.Split(';');
            var language = entryParts[0];
            if (language.Contains('-'))
                language = language.Split('-')[0];

       
            var priority = entryParts.Length > 1 && entryParts[1].StartsWith("q=") && decimal.TryParse(entryParts[1][2..], out var q)
                ? q : 1m;
            acceptedLanguages.Add((language, priority));
        }

        return acceptedLanguages.OrderByDescending(x => x!.Value.Priority).FirstOrDefault()?.Language ?? "en";
    }
}