using Microsoft.AspNetCore.OpenApi;
using VagonWebSubmissionTool.Application;
using VagonWebSubmissionTool.Application.Services;
using VagonWebSubmissionTool.Web;
using VagonWebSubmissionTool.Web.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
VagonWebSubmissionTool.Application.ServiceRegistration.RegisterServices(builder.Services);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<OpenApiAcceptLanguageTransformer>();
});

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ILanguageResolver, HttpRequestLanguageResolver>();
builder.Services.AddTransient<VagonWebLanguageHandler>();
builder.Services.AddTransient<VagonWebCachingHandler>();

builder.Services.AddHttpClient(Constants.HttpClientName, client =>
    {
        client.BaseAddress = Constants.VagonWebHost;
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("User-Agent",
            "VagonWebSubmissionTool/1.0.0 tool for submitting consists to vagonWEB more easily (https://github.com/kaelon141/vagonweb-submission-tool/)");
    })
    .AddHttpMessageHandler<VagonWebLanguageHandler>()
    .AddHttpMessageHandler<VagonWebCachingHandler>();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.Run();