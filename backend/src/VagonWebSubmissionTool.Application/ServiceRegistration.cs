using System.Net;
using Microsoft.Extensions.DependencyInjection;
using VagonWebSubmissionTool.Application.Services;

namespace VagonWebSubmissionTool.Application;

public static class ServiceRegistration
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<ITrainCatalogService, TrainCatalogService>();
        services.AddScoped<ICoachCatalogService, CoachCatalogService>();
        services.AddScoped<IUicWagonNumbersService, UicWagonNumbersService>();
    }
}