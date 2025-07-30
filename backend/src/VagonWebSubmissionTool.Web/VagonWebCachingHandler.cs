using System.Text;
using Microsoft.Extensions.Caching.Memory;
using VagonWebSubmissionTool.Application;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Web;

public class VagonWebCachingHandler(IMemoryCache cache, TimeSpan? cachingPeriod = null) : DelegatingHandler
{
    private readonly TimeSpan _cachingPeriod = cachingPeriod ?? TimeSpan.FromHours(1);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method != HttpMethod.Get)
            return await base.SendAsync(request, cancellationToken);

        var cacheKey = GenerateCacheKey(request);

        if (cache.TryGetValue(cacheKey, out HttpResponseMessage? cached) && cached is not null)
            return await CloneResponseAsync(cached);

        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return response;
        
        var clone = await CloneResponseAsync(response);
        cache.Set(cacheKey, clone, _cachingPeriod);

        return response;
    }

    private static string GenerateCacheKey(HttpRequestMessage request)
    {
        var sb = new StringBuilder();

        sb.Append("GET:");
        sb.Append(request.RequestUri!);

        // Cookies: parse vagonweb[lang] value specifically as we need to cache responses on a per-language level.
        string? langCookieValue = null;

        if (request.Headers.TryGetValues("Cookie", out var cookieHeaders))
        {
            foreach (var cookieHeader in cookieHeaders)
            {
                var cookies = cookieHeader.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var cookie in cookies)
                {
                    if (!cookie.StartsWith("vagonweb[lang]="))
                        continue;
                    
                    langCookieValue = cookie["vagonweb[lang]=".Length..];
                    break;
                }
            }
        }

        sb.Append($"|langCookie:{langCookieValue ?? ILanguageResolver.DefaultLanguage.ToVagonWebLanguageCode()}");

        return sb.ToString().GetHashCode().ToString();
    }

    private static async Task<HttpResponseMessage> CloneResponseAsync(HttpResponseMessage original)
    {
        var contentString = await original.Content.ReadAsStringAsync();

        var clone = new HttpResponseMessage(original.StatusCode)
        {
            Content = new StringContent(contentString),
            ReasonPhrase = original.ReasonPhrase,
            Version = original.Version,
            RequestMessage = original.RequestMessage
        };

        foreach (var header in original.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}