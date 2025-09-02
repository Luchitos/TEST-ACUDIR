using System.ComponentModel.DataAnnotations;

namespace Acudir.Test.Apis.Contracts.Auth
{
    public sealed class LoginRequest
    {
        /// <summary>Nombre de usuario. Para el challenge: "AcudirTest".</summary>
        [Required]
        public string Username { get; set; } = default!;

        /// <summary>Contraseña. Para el challenge: "AcudirTest".</summary>
        [Required]
        public string Password { get; set; } = default!;
    }
}
