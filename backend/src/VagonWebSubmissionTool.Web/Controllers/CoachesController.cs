using Microsoft.AspNetCore.Mvc;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Web.Controllers;

[Route("api/v1/coaches")]
public class CoachesController(ICoachCatalogService coachCatalogService) : BaseController
{
    [Route("countries")]
    [HttpGet]
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

    [Route("countries/{countryCode}")]
    [HttpGet]
    public async Task<IActionResult> GetOperatorsForCountry(string countryCode)
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