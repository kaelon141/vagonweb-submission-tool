using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Web.Controllers;

[ApiController]
[Route("api/v1/trains")]
public class TrainsController(ITrainCatalogService trainCatalogService) : BaseController
{
    [HttpGet("categories")]
    [EndpointSummary("Get a list of operators and train categories per operator.")]
    [ProducesResponseType(typeof(List<ITrainCatalogService.Country>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    [HttpGet]
    [EndpointSummary("Search for train by operator, category, name and/or line number.")]
    [ProducesResponseType(typeof(List<ITrainCatalogService.TrainDescription>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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