using Application.Personas.Commands.CreatePersona;

using FluentValidation;

namespace Application.Personas.Validators
{
    public class CreatePersonaCommandValidator : AbstractValidator<CreatePersonaCommand>
    {
        public CreatePersonaCommandValidator()
        {
            RuleFor(x => x.NombreCompleto)
                .NotEmpty()
                .WithMessage("El nombre completo es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.Edad)
                .GreaterThanOrEqualTo(0).WithMessage("La edad no puede ser negativa.");

            RuleFor(x => x.Domicilio)
                .NotEmpty().WithMessage("El domicilio es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .MaximumLength(50)
                .Matches(@"^\+?[0-9]*$").WithMessage("El teléfono solo puede contener números y el carácter '+'.");

            RuleFor(x => x.Profesion)
                .NotEmpty().WithMessage("La profesión es obligatoria.")
                .MaximumLength(50);
        }
    }
}
