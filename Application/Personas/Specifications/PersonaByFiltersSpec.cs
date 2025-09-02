using Domain.Entidades;

using System.Linq;

namespace Application.Personas.Specifications
{
    public sealed class PersonaByFiltersSpec
    {
        private readonly string? _nombre;
        private readonly int? _edad;
        private readonly int? _minEdad;
        private readonly int? _maxEdad;
        private readonly string? _domicilio;
        private readonly string? _telefono;
        private readonly string? _profesion;
        private readonly string _sortBy;
        private readonly bool _desc;
        private readonly int _page;
        private readonly int _size;

        public PersonaByFiltersSpec(
            string? nombre, int? edad, int? minEdad, int? maxEdad,
            string? domicilio, string? telefono, string? profesion,
            string? sortBy, string sortDir, int page, int size)
        {
            _nombre = nombre;
            _edad = edad;
            _minEdad = minEdad;
            _maxEdad = maxEdad;
            _domicilio = domicilio;
            _telefono = telefono;
            _profesion = profesion;
            _sortBy = (sortBy ?? "Id").Trim();
            _desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            _page = page;
            _size = size;
        }

        public IQueryable<Persona> ApplyFiltersAndSort(IQueryable<Persona> query)
        {
            if (!string.IsNullOrWhiteSpace(_nombre))
                query = query.Where(p => p.NombreCompleto.Contains(_nombre!, StringComparison.OrdinalIgnoreCase));
            if (_edad.HasValue)
                query = query.Where(p => p.Edad == _edad.Value);
            if (_minEdad.HasValue)
                query = query.Where(p => p.Edad >= _minEdad.Value);
            if (_maxEdad.HasValue)
                query = query.Where(p => p.Edad <= _maxEdad.Value);
            if (!string.IsNullOrWhiteSpace(_domicilio))
                query = query.Where(p => p.Domicilio.Contains(_domicilio!, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(_telefono))
                query = query.Where(p => p.Telefono.Contains(_telefono!, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(_profesion))
                query = query.Where(p => p.Profesion.Contains(_profesion!, StringComparison.OrdinalIgnoreCase));

            query = (_sortBy.ToLowerInvariant()) switch
            {
                "nombre" or "nombrecompleto" => _desc ? query.OrderByDescending(p => p.NombreCompleto) : query.OrderBy(p => p.NombreCompleto),
                "edad" => _desc ? query.OrderByDescending(p => p.Edad) : query.OrderBy(p => p.Edad),
                "profesion" => _desc ? query.OrderByDescending(p => p.Profesion) : query.OrderBy(p => p.Profesion),
                _ => _desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
            };

            return query;
        }

        public IQueryable<Persona> ApplyPaging(IQueryable<Persona> query)
        {
            return query.Skip((_page - 1) * _size).Take(_size);
        }
    }
}
