using VagonWebSubmissionTool.Application;
using VagonWebSubmissionTool.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
VagonWebSubmissionTool.Application.ServiceRegistration.RegisterServices(builder.Services);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient(Constants.HttpClientName, client =>
{
    client.BaseAddress = Constants.VagonWebHost;
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent",
        "VagonWebSubmissionTool/1.0.0 tool for submitting consists to vagonWEB more easily (https://github.com/jordy-de-koning/vagonweb-submission-tool/)");
}).ConfigurePrimaryHttpMessageHandler((serviceProvider) =>
{
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    return new VagonWebMessageHandler(httpContextAccessor);
});

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