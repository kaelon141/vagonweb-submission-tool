using System.ComponentModel;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Web.Controllers;

[ApiController]
[Route("api/v1/coaches")]
public class CoachesController(ICoachCatalogService coachCatalogService) : BaseController
{
    [HttpGet("countries")]
    [EndpointSummary("Get a list of country codes for browsing coaches in the global coach catalog.")]
    [ProducesResponseType(typeof(List<ICoachCatalogService.CountryCodeDto>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCoachCountries()
    {
        var result = await coachCatalogService.GetUicCountryCodes();

        return result switch
        {
            { IsSuccess: true, Value: var countryCodes } => Ok(countryCodes),
            { IsFailure: true, Error: var error } => StatusCode(StatusCodes.Status500InternalServerError, error),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("countries/{countryCode}")]
    [EndpointSummary("Get a list of coaches by country.")]
    [ProducesResponseType(typeof(List<ICoachCatalogService.OperatorDto>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOperatorsForCountry(
        [Description("The country for which to get a listing of coached. The country code can be gotten from the GetCoachCountries endpoint.")] string countryCode
    )
    {
        var result = await coachCatalogService.GetOperatorsForCountry(countryCode);

        return result switch
        {
            { IsSuccess: true, Value: var operators } => Ok(operators),
            { IsFailure: true, Error: var error } => StatusCode(StatusCodes.Status500InternalServerError, error),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}