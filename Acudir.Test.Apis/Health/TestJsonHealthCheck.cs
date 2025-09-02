using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Acudir.Test.Apis.Health
{
    public class TestJsonHealthCheck : IHealthCheck
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _cfg;

        public TestJsonHealthCheck(IWebHostEnvironment env, IConfiguration cfg)
        {
            _env = env;
            _cfg = cfg;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
        {
            // 1) Resolver ruta desde config, con fallback al content root
            string path = _cfg["Data:TestJsonPath"] ?? Path.Combine(_env.ContentRootPath, "Test.json");

            if (string.IsNullOrWhiteSpace(path))
                return HealthCheckResult.Unhealthy("Data:TestJsonPath no está configurado y no se pudo calcular un fallback válido.");

            try
            {
                if (!File.Exists(path))
                    return HealthCheckResult.Unhealthy($"File not found: {path}");

                // 2) Comprobar LECTURA (asíncrono)
                await using (var fs = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite,
                    bufferSize: 1,
                    options: FileOptions.Asynchronous))
                {
                    // No necesitamos leer contenido; abrirlo con permisos correctos ya valida el acceso.
                    // Hacemos una operación mínima async para respetar el token.
                    await fs.FlushAsync(token).ConfigureAwait(false);
                }

                // 3) Comprobar ESCRITURA: generar un archivo temporal de forma segura
                string? dir = Path.GetDirectoryName(path);
                if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
                    return HealthCheckResult.Degraded($"Read OK, but write check skipped (invalid dir for {path}).");

                string probe = Path.Combine(dir, $".readywrite.{Guid.NewGuid():N}.tmp");
                try
                {
                    await File.WriteAllTextAsync(probe, DateTime.UtcNow.ToString("O"), token).ConfigureAwait(false);
                    File.Delete(probe);
                    return HealthCheckResult.Healthy($"Accessible (read/write): {path}");
                }
                catch (UnauthorizedAccessException uae)
                {
                    // Lectura OK, escritura denegada → Degraded (útil en entornos read-only)
                    return HealthCheckResult.Degraded($"Read OK but write denied for {dir}: {uae.Message}");
                }
                catch (IOException ioex)
                {
                    // Lectura OK, problema de IO al escribir → Degraded
                    return HealthCheckResult.Degraded($"Read OK but write failed in {dir}: {ioex.Message}");
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                return HealthCheckResult.Unhealthy($"Unauthorized to access {path}: {uae.Message}");
            }
            catch (IOException ioex)
            {
                return HealthCheckResult.Unhealthy($"I/O error accessing {path}: {ioex.Message}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Unexpected error accessing {path}: {ex.Message}");
            }
        }
    }
}
