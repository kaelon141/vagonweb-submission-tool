using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.WebUtilities;

namespace VagonWebSubmissionTool.Application.Services;

public interface ICoachCatalogService
{
    public record CountryCodeDto(string Code, string Description);
    
    public record OperatorDto(string Code);
    
    Task<Result<List<CountryCodeDto>, string>> GetUicCountryCodes(int? year = null);
    
    Task<Result<List<OperatorDto>, string>> GetOperatorsForCountry(string countryCode, int? year = null);
}

public class CoachCatalogService(IHttpClientFactory httpClientFactory) : BaseService(httpClientFactory), ICoachCatalogService
{
    private const string CatalogEndpoint = "admin/ajax_dopravci.php";
    private const string SearchEndpoint = "admin/vozy2.php";
    
    public async Task<Result<List<ICoachCatalogService.CountryCodeDto>, string>> GetUicCountryCodes(int? year = null)
    {
        year ??= DateTime.Now.Year;

        var html = await GetHtml(GetCatalogUrl(year.Value));
        if (html.IsFailure)
        {
            return html.Error;
        }
        
        var countryCodes = new List<ICoachCatalogService.CountryCodeDto>();
        
        var links = html.Value.DocumentNode.Elements("a");
        
        foreach (var link in links)
        {
            var description = link.InnerText;
            if (string.IsNullOrWhiteSpace(description))
                continue;
            
            var onClickJs = link.GetAttributeValue("onclick", string.Empty);
            var zemeIndex = onClickJs.IndexOf("&zeme=", StringComparison.Ordinal);
            var zemeEndIndex = onClickJs.IndexOf("&", zemeIndex + 6, StringComparison.Ordinal);
            var zeme = onClickJs[(zemeIndex + 6)..zemeEndIndex];
            
            countryCodes.Add(new(zeme, description));
        }

        return countryCodes;
    }

    public async Task<Result<List<ICoachCatalogService.OperatorDto>, string>> GetOperatorsForCountry(string countryCode, int? year = null)
    {
        year ??= DateTime.Now.Year;
        
        var html = await GetHtml(GetCatalogUrl(year.Value, countryCode));
        if (html.IsFailure)
        {
            return html.Error;
        }
        
        var links = html.Value.DocumentNode.Elements("a");
        return links.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
            .Select(x => new ICoachCatalogService.OperatorDto(x.InnerText))
            .ToList();
    }

    private static string GetCatalogUrl(int year, string? countryCode = null, string? owner = null)
    {
        var searchParams = new Dictionary<string, string?>
        {
            { "rok", year.ToString() },
            { "typ", "2012" }
        };

        if (!string.IsNullOrWhiteSpace(countryCode))
        {
            searchParams.Add("zeme", countryCode);
        }

        if (string.IsNullOrWhiteSpace(owner))
            return QueryHelpers.AddQueryString(CatalogEndpoint, searchParams);
        
        searchParams.Add("d", owner);
        return QueryHelpers.AddQueryString(SearchEndpoint, searchParams);
    }

    
    
}