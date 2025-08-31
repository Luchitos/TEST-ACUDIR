using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Acudir.Test.Apis.Health;

public class TestJsonHealthCheck : IHealthCheck
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _cfg;

    public TestJsonHealthCheck(IWebHostEnvironment env, IConfiguration cfg)
    {
        _env = env; _cfg = cfg;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
    {
        var path = _cfg["Data:TestJsonPath"] ?? Path.Combine(_env.ContentRootPath, "Test.json");

        try
        {
            if (!File.Exists(path))
                return Task.FromResult(HealthCheckResult.Unhealthy($"File not found: {path}"));

            // Read access
            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) { }

            // Write access
            var dir = Path.GetDirectoryName(path)!;
            var probe = Path.Combine(dir, ".readywrite.tmp");
            File.WriteAllText(probe, DateTime.UtcNow.ToString("O"));
            File.Delete(probe);

            return Task.FromResult(HealthCheckResult.Healthy($"Accessible: {path}"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Error accessing {path}: {ex.Message}"));
        }
    }
}
