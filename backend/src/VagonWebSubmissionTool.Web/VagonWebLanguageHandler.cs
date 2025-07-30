using VagonWebSubmissionTool.Application;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Web;

/// <summary>
/// HTTP message handler for HTTP requests sent to vagonweb.cz, that sets the vagonweb[lang] cookie which the website uses for localisation purposes.
/// </summary>
public class VagonWebLanguageHandler(ILanguageResolver languageResolver) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var language = languageResolver.Resolve();
        
        request.Headers.Add("Cookie", $"vagonweb[lang]={language.ToVagonwebLanguageCode()}");
        
        return base.SendAsync(request, cancellationToken);
    }
}