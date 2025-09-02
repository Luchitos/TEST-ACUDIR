using Domain.Entidades;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaz
{
    public interface IPersonaRepository
    {
        Task<Persona?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Persona>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Persona persona, CancellationToken ct = default);
        Task UpdateAsync(Persona persona, CancellationToken ct = default);
    }
}

