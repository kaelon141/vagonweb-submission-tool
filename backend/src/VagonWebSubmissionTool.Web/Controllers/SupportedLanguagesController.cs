using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VagonWebSubmissionTool.Application;

namespace VagonWebSubmissionTool.Web.Controllers;

[ApiController]
[Route("api/v1/supported-languages")]
public class SupportedLanguagesController : BaseController
{
    public record LanguageEntry(string bcp47Code, string Name);
    
    [HttpGet]
    [EndpointSummary("Get a list of supported languages.")]
    [ProducesResponseType(typeof(List<LanguageEntry>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetLanguages([FromQuery] int? year)
    {
        return Ok(Languages.All.Select(language => new LanguageEntry(language.ToBcp47Code(), language.ToNativeName())).ToList());
    }
}