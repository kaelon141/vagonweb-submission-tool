using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace VagonWebSubmissionTool.Web.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult Html(string html)
    {
        return new ContentResult
        {
            Content = html,
            ContentType = MediaTypeNames.Text.Html + "; charset=utf-8",
            StatusCode = StatusCodes.Status200OK
        };
    }
}