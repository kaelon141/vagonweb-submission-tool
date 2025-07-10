using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.WebUtilities;

namespace VagonWebSubmissionTool.Application.Services;

public class BaseService(IHttpClientFactory httpClientFactory)
{
    protected async Task<Result<HtmlAgilityPack.HtmlDocument, string>> GetHtml(string url)
    {
        var httpClient = httpClientFactory.CreateVagonWebClient();
        var response = await httpClient.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<HtmlAgilityPack.HtmlDocument, string>($"Error encountered querying vagonWEB for coaches: {response.StatusCode}, {responseBody}");
        }
        
        var document = new HtmlAgilityPack.HtmlDocument();
        document.LoadHtml(responseBody);
        return Result.Success<HtmlAgilityPack.HtmlDocument, string>(document);
    }

    protected IDictionary<string, string> ParseQueryParams(string url)
    {
        if (!url.Contains('?')) return new Dictionary<string, string>();

        var queryString = url.Split('?')[1];
        return QueryHelpers.ParseQuery(queryString).ToDictionary(x => x.Key, x => x.Value.First()!);
    }
}