using System.Text.Json.Serialization;

namespace Acudir.Test.Apis.Contracts.Auth
{
    public sealed class LoginResponse
    {
        /// <summary>Token **incluyendo** el prefijo "Bearer ".</summary>
        public string Token { get; set; } = default!;

        /// <summary>Tipo de token (siempre "Bearer").</summary>
        public string TokenType { get; set; } = "Bearer";


        public DateTime ExpiresAt { get; set; }
    }
}
