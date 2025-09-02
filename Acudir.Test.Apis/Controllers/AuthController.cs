using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Acudir.Test.Apis.Contracts.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Acudir.Test.Apis.Controllers
{
    /// <summary>
    /// Autenticación: emite JWT por 1h para el usuario/contraseña del challenge.
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Login (challenge). Devuelve un JWT **con prefijo "Bearer "** en el campo token.
        /// </summary>
        /// <remarks>
        /// <b>CREDENCIALES</b><br/>
        /// Usuario: <code>AcudirTest</code><br/>
        /// Contraseña: <code>AcudirTest</code><br/><br/>
        /// </remarks>
        /// <response code="200">Token emitido correctamente.</response>
        /// <response code="401">Credenciales inválidas (ProblemDetails).</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Validación "hardcodeada"
            const string validUser = "AcudirTest";
            const string validPass = "AcudirTest";

            if (!string.Equals(request.Username, validUser, StringComparison.Ordinal) ||
                !string.Equals(request.Password, validPass, StringComparison.Ordinal))
            {
                return Unauthorized(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/401",
                    Title = "Invalid credentials",
                    Detail = "Username or password is incorrect.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // Configuración de JWT (Issuer/Audience por appsettings/env; secret por env JWT_SECRET_KEY)
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer not configured");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience not configured");
            var secret = _configuration["JWT_SECRET_KEY"] ?? _configuration["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT secret not configured. Set env var JWT_SECRET_KEY.");

            var nowUtc = DateTime.UtcNow;
            var expiresUtc = nowUtc.AddHours(1);

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var signingCreds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, validUser),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(nowUtc).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = issuer,
                Audience = audience,
                NotBefore = nowUtc,
                IssuedAt = nowUtc,
                Expires = expiresUtc,
                SigningCredentials = signingCreds
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            var rawJwt = handler.WriteToken(token);
            var tokenWithPrefix = $"Bearer {rawJwt}";

            var response = new LoginResponse
            {
                Token = tokenWithPrefix,
                TokenType = "Bearer",
                ExpiresAt = expiresUtc
            };

            return Ok(response);
        }
    }
}
