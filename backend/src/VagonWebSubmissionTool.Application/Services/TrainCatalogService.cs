using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.WebUtilities;

namespace VagonWebSubmissionTool.Application.Services;

public interface ITrainCatalogService
{
    public record TrainDescription(string Number, string Name, string Route, string TrainUrl);
    public record TrainCategory(string CategoryCode, string Description);
    public record TrainOperator(string OperatorCode, string Name, List<TrainCategory> Categories);
    public record Country(string CountryName, List<TrainOperator> Operators);

    public Task<Result<List<Country>, string>> GetTrainCategories(int? year = null);

    public Task<Result<List<ITrainCatalogService.TrainDescription>, string>> SearchTrains(
        string? operatorCode = null,
        string? categoryCode = null,
        string? trainName = null,
        string? lineNumber = null,
        int? year = null
    );
}

public class TrainCatalogService(IHttpClientFactory httpClientFactory) : BaseService(httpClientFactory), ITrainCatalogService
{
    private const string CategoriesListingEndpoint = "razeni/index.php";
    private const string TrainsListingEndpoint = "razeni/razeni.php";
    private const string CategoriesListClass = "obsah_razeni";
    
    public async Task<Result<List<ITrainCatalogService.Country>, string>> GetTrainCategories(int? year = null)
    {
        year ??= DateTime.Now.Year;
        
        var listPageUrl = QueryHelpers.AddQueryString(CategoriesListingEndpoint, "rok", year.Value.ToString());
        var html = await GetHtml(listPageUrl);
        if (html.IsFailure)
        {
            return html.Error;
        }
        
        var parsedCountries = new List<ITrainCatalogService.Country>();
        var countryLists = html.Value.DocumentNode.SelectNodes($"//*[@class='{CategoriesListClass}']/div")!;

        foreach (var countryList in countryLists)
        {
            var operatorNames = new Dictionary<string, string>();
            var categoriesByOperator = new Dictionary<string, List<ITrainCatalogService.TrainCategory>>();
            var countryName = countryList.SelectSingleNode("h2")!.InnerText;
            
            foreach (var categoryElement in countryList.SelectNodes("a")!)
            {
                var categoryName = SanitizeString(categoryElement.SelectSingleNode("div")!.InnerText);
                var queryParams = ParseQueryParams(categoryElement.GetAttributeValue("href", string.Empty));
                if (categoryElement.HasClass("dopravce"))
                {
                    operatorNames.Add(queryParams["zeme"], categoryName);
                    continue;
                }
                
                categoriesByOperator.TryAdd(queryParams["zeme"], []);
                categoriesByOperator[queryParams["zeme"]].Add(new ITrainCatalogService.TrainCategory(queryParams["kategorie"], categoryName));
            }
            
            var operators = operatorNames.Select(kvp => new ITrainCatalogService.TrainOperator(kvp.Key, kvp.Value, categoriesByOperator.GetValueOrDefault(kvp.Key, []))).ToList();
            parsedCountries.Add(new ITrainCatalogService.Country(countryName, operators));
        }

        return parsedCountries;
    }

    public async Task<Result<List<ITrainCatalogService.TrainDescription>, string>> SearchTrains(
        string? operatorCode = null,
        string? categoryCode = null,
        string? trainName = null,
        string? lineNumber = null,
        int? year = null
    )
    {
        year ??= DateTime.Now.Year;

        var searchParams = new Dictionary<string, string?>
        {
            { "rok", year.Value.ToString() },
            { "zeme", operatorCode },
            { "kategorie", categoryCode },
            { "jmeno", trainName },
            { "relace", lineNumber }
        };
        
        var url = QueryHelpers.AddQueryString(TrainsListingEndpoint, searchParams);
        var htmlDoc = await GetHtml(url);
        if (htmlDoc.IsFailure)
        {
            return htmlDoc.Error;
        }
        
        var parsedTrains = new List<ITrainCatalogService.TrainDescription>();
        foreach (var train in htmlDoc.Value.DocumentNode.SelectNodes("//tr[@class='tr_razeni']")!)
        {
            var trainUrl = train.SelectSingleNode("td[@class='cislo']/a")!.GetAttributeValue("href", string.Empty);
            var trainNumber = SanitizeString(train.SelectSingleNode("td[@class='cislo']/a")!.InnerText);
            var trainDescription = SanitizeString(train.SelectSingleNode("td[@class='nazev']")!.InnerText);
            var route = SanitizeString(train.SelectSingleNode("td[@class='maly']")!.InnerText);
            parsedTrains.Add(new ITrainCatalogService.TrainDescription(trainNumber, trainDescription, route, trainUrl));
        }
        
        return parsedTrains;
    }

    private string SanitizeString(string str)
    {
        return str.Replace("&nbsp;", " ").Trim();
    }
}