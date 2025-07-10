using Microsoft.AspNetCore.Mvc;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Web.Controllers;

[Route("api/v1/uic-wagon-numbers")]
public class UicCodeController(IUicWagonNumbersService uicWagonNumbersService) : BaseController
{
    [Route("{uicWagonNumber}/country")]
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
