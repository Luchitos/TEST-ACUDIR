using MediatR;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Application.Mapping;
using Application.Services;
using Infrastructure;
using Acudir.Test.Apis.Middlewares;
using Serilog;
using Acudir.Test.Apis.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using dotenv.net;

using Acudir.Test.Apis.Options;
using Acudir.Test.Apis.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Mapper));
builder.Services.AddMediatR(typeof(Application.Request.PersonaRequest.GetPersonaRequestHandler).Assembly);

builder.Services
    .AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddCheck<TestJsonHealthCheck>("testjson", tags: new[] { "ready" });

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IServicePersona, ServicePersona>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // v1, v1.0
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .Enrich.FromLogContext();
});

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diagCtx, http) =>
    {
        if (http.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out object? cid) && cid is string s)
            diagCtx.Set("CorrelationId", s);

        diagCtx.Set("RequestPath", http.Request.Path);
        diagCtx.Set("UserAgent", http.Request.Headers.UserAgent.ToString());
    };
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    foreach (var desc in provider.ApiVersionDescriptions)
    {
        c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"Acudir.Test.Apis {desc.GroupName.ToUpperInvariant()}");
    }
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html", permanent: false);
        return;
    }
    await next();
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = r => r.Name == "self"
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready"),
    ResponseWriter = async (ctx, rpt) =>
    {
        ctx.Response.ContentType = "application/json";
        var payload = new
        {
            status = rpt.Status.ToString(),
            checks = rpt.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                error = e.Value.Exception?.Message,
                description = e.Value.Description
            })
        };
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
