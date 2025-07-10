namespace VagonWebSubmissionTool.Application;

public static class HttpClientFactoryExtensions
{
    public static HttpClient CreateVagonWebClient(this IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient(Constants.HttpClientName);
    }
}