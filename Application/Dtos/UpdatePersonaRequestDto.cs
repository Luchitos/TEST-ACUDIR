using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class UpdatePersonaRequestDto
    {
        [Required]
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "El nombre completo no puede tener más de 50 caracteres.")]
        public string? NombreCompleto { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La edad no puede ser un número negativo.")]
        public int? Edad { get; set; }

        [StringLength(50, ErrorMessage = "El domicilio no puede tener más de 50 caracteres.")]
        public string? Domicilio { get; set; }

        [StringLength(50, ErrorMessage = "El teléfono no puede tener más de 50 caracteres.")]
        [RegularExpression(@"^\+?[0-9]*$", ErrorMessage = "El teléfono solo puede contener números y el carácter '+'.")]
        public string? Telefono { get; set; }

        [StringLength(50, ErrorMessage = "La profesión no puede tener más de 50 caracteres.")]
        public string? Profesion { get; set; }
    }
}
