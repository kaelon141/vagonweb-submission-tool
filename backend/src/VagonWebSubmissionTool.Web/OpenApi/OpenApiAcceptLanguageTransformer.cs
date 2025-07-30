using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using VagonWebSubmissionTool.Application;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Web.OpenApi;

public class OpenApiAcceptLanguageTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var acceptLanguageHeader = new OpenApiParameter
        {
            Name = "Accept-Language",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Language to use for localisation",
            Schema = new OpenApiSchema
            {
                Type = "string",
                Enum = Languages.All.Select(language => new OpenApiString(language.ToBcp47Code())).ToList<IOpenApiAny>(),
                Default = new OpenApiString(ILanguageResolver.DefaultLanguage.ToBcp47Code())
            }
        };

        foreach (var operation in document.Paths.Values.SelectMany(x => x.Operations.Values))
        {
            operation.Parameters ??= new List<OpenApiParameter>();
            operation.Parameters.Add(acceptLanguageHeader);
        }
        
        return Task.CompletedTask;
    }
}