using FluentValidation;
using Application.Personas.Commands.UpdatePersona;

namespace Application.Personas.Validators
{
    public class UpdatePersonaCommandValidator : AbstractValidator<UpdatePersonaCommand>
    {
        public UpdatePersonaCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor a cero.");

            When(x => x.NombreCompleto is not null, () =>
            {
                RuleFor(x => x.NombreCompleto!)
                    .MaximumLength(50)
                    .WithMessage("El nombre completo no puede tener más de 50 caracteres.");
            });

            When(x => x.Edad.HasValue, () =>
            {
                RuleFor(x => x.Edad.Value)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("La edad no puede ser negativa.");
            });

            When(x => x.Domicilio is not null, () =>
            {
                RuleFor(x => x.Domicilio!)
                    .MaximumLength(50)
                    .WithMessage("El domicilio no puede tener más de 50 caracteres.");
            });

            When(x => x.Telefono is not null, () =>
            {
                RuleFor(x => x.Telefono!)
                    .MaximumLength(50)
                    .Matches(@"^\+?[0-9]*$")
                    .WithMessage("El teléfono solo puede contener números y el carácter '+'.");
            });

            When(x => x.Profesion is not null, () =>
            {
                RuleFor(x => x.Profesion!)
                    .MaximumLength(50)
                    .WithMessage("La profesión no puede tener más de 50 caracteres.");
            });
        }
    }
}
