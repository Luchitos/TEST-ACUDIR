using FluentValidation;

namespace Application.Personas.Queries.GetPersonas;

public sealed class GetPersonasQueryValidator : AbstractValidator<GetPersonasQuery>
{
    private static readonly string[] AllowedSortBy = { "Id", "Nombre", "NombreCompleto", "Edad", "Profesion" };
    private static readonly string[] AllowedSortDir = { "asc", "desc" };

    public GetPersonasQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Size).InclusiveBetween(1, 100);
        RuleFor(x => x.SortBy).Must(x => x is null || AllowedSortBy.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortBy)}");
        RuleFor(x => x.SortDir).Must(x => AllowedSortDir.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortDir must be 'asc' or 'desc'");

        RuleFor(x => x)
            .Must(x => !x.MinEdad.HasValue || !x.MaxEdad.HasValue || x.MinEdad <= x.MaxEdad)
            .WithMessage("MinEdad must be <= MaxEdad");
    }
}
