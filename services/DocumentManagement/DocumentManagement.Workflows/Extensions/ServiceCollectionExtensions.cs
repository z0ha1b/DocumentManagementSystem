using System.Reflection;
using DocumentManagement.Core.Options;
using DocumentManagement.Core.Services.Implementations;
using DocumentManagement.Core.Services.Interfaces;
using DocumentManagement.Workflows.Activities;
using DocumentManagement.Workflows.Handlers;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Storage.Net;

namespace DocumentManagement.Workflows.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowServices(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: "DefaultCors",
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddElsa(elsa =>
        {
            //elsa.UseWorkflowRuntime(runtime => runtime.AddWorkflow<HelloWorldHttpWorkflow>());

            elsa.UseIdentity(identity =>
            {
                identity.TokenOptions = options => options.SigningKey = "ADADADDASDASDADADDASDASDASDASDASDADASDDAASDADADADADADASDSA";
                identity.UseAdminUserProvider();
            });

            elsa.UseDefaultAuthentication();
            elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore());
            elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore());
            elsa.UseJavaScript();
            elsa.UseLiquid();
            elsa.UseWorkflowsApi();
            elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "/workflows");
            elsa.AddActivitiesFrom<GetDocuments>();
            elsa.AddActivitiesFrom<ArchiveDocument>();
            elsa.AddActivitiesFrom<ZipFile>();
        });

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddNotificationHandlersFrom<NewDocumentReceivedHandler>();

        return services;
    }

    public static WebApplication AddWorkflowMiddlewares(this WebApplication app)
    {
        app.UseCors("DefaultCors");

        app.MapControllers();

        app.UseWorkflowsApi();
        app.UseWorkflows();

        return app;
    }
}