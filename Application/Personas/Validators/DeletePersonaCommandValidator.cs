using FluentValidation;

namespace Application.Personas.Validators
{
    public class DeletePersonaCommandValidator : AbstractValidator<Application.Personas.Commands.DeletePersona.DeletePersonaCommand>
    {
        public DeletePersonaCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor a cero.");
        }
    }
}
