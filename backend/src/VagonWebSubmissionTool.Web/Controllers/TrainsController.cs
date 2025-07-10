using Microsoft.AspNetCore.Mvc;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Web.Controllers;

[Route("api/v1/trains")]
public class TrainsController(ITrainCatalogService trainCatalogService) : BaseController
{
    [Route("categories")]
    [HttpGet]
    public async Task<IActionResult> GetTrainCategories([FromQuery] int? year)
    {
        var categories = await trainCatalogService.GetTrainCategories(year);

        return categories switch
        {
            { IsSuccess: true, Value: var operators } => Ok(operators),
            { IsFailure: true, Error: var error } => StatusCode(StatusCodes.Status500InternalServerError, error),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> SearchTrains([FromQuery] int? year, [FromQuery] string? operatorCode,
        [FromQuery] string? categoryCode, [FromQuery] string? trainName, [FromQuery] string? lineNumber)
    {
        var trains = await trainCatalogService.SearchTrains(operatorCode, categoryCode, trainName, lineNumber, year);

        return trains switch
        {
            { IsSuccess: true, Value: var operators } => Ok(operators),
            { IsFailure: true, Error: var error } => StatusCode(StatusCodes.Status500InternalServerError, error),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}