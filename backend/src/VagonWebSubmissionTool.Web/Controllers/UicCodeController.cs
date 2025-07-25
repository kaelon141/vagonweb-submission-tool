using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Web.Controllers;

[ApiController]
[Route("api/v1/uic-wagon-numbers")]
public class UicCodeController(IUicWagonNumbersService uicWagonNumbersService) : BaseController
{
    
    
    [HttpGet("{uicWagonNumber}/country")]
    [EndpointSummary("Attempt to get the country code from a UIC wagon number.")]
    [ProducesResponseType(typeof(IUicWagonNumbersService.CountryCodeDto), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetCountryCodeByUicNumber(string uicWagonNumber)
    {
        var result = uicWagonNumbersService.GetCountryCodeByUicNumber(uicWagonNumber);

        return result switch
        {
            { IsSuccess: true, Value: var countryCode } => Ok(new { CountryCode = countryCode }),
            { IsFailure: true, Error: var error } => StatusCode(StatusCodes.Status400BadRequest, error),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
