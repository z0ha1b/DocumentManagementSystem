using System.Configuration;
using System.Reflection;
using DocumentManagement.Core.Options;
using DocumentManagement.Core.Services.Implementations;
using DocumentManagement.Core.Services.Interfaces;
using DocumentManagement.Workflows.Activities;
using DocumentManagement.Workflows.Handlers;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Webhooks.Extensions;
using Esprima.Ast;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Net;

namespace DocumentManagement.Workflows.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowServices(this IServiceCollection services, IConfiguration configuration)
    {
        var sqliteConnectionString = configuration.GetConnectionString("Sqlite")!;
        var identitySection = configuration.GetSection("Identity");
        var identityTokenSection = identitySection.GetSection("Tokens");

        // Add Elsa services.
        services
            .AddElsa(elsa => elsa
                    // .UseSasTokens()
                    .UseIdentity(identity =>
                    {
                        identity.IdentityOptions = options => identitySection.Bind(options);
                        identity.TokenOptions = options => identityTokenSection.Bind(options);
                        identity.UseConfigurationBasedUserProvider(options => identitySection.Bind(options));
                        identity.UseConfigurationBasedApplicationProvider(options => identitySection.Bind(options));
                        identity.UseConfigurationBasedRoleProvider(options => identitySection.Bind(options));
                    })
                    .UseDefaultAuthentication()
                    .UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite(sqliteConnectionString)))
                    .UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseSqlite(sqliteConnectionString)))
                    .UseScheduling()
                    .UseJavaScript()
                    .UseLiquid()
                    .UseHttp(http => http.ConfigureHttpOptions = options => configuration.GetSection("Http").Bind(options))
                    .UseEmail(email => email.ConfigureOptions = options => configuration.GetSection("Smtp").Bind(options))
                    .UseWebhooks(webhooks => webhooks.WebhookOptions = options => configuration.GetSection("Webhooks").Bind(options))
                    .UseWorkflowsApi()
                    .UseRealTimeWorkflows()
                //.AddActivitiesFrom<Program>()
                //.AddWorkflowsFrom<Program>()
            );

        services.AddHealthChecks();

        services.AddCors(cors => cors.Configure(configuration.GetSection("CorsPolicy")));


        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddNotificationHandlersFrom<NewDocumentReceivedHandler>();

        return services;
    }

    public static WebApplication AddWorkflowMiddlewares(this WebApplication app)
    {
        app.UseWorkflowsApi();
        app.UseWorkflows();
        app.UseWorkflowsSignalRHubs();

        return app;
    }
}