using Domain.Entidades;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaz
{
    public interface IPersonaRepository
    {
        Task<Persona?> GetByIdAsync(int id);
        Task<IEnumerable<Persona>> GetAllAsync();
        Task AddAsync(Persona persona);
        Task UpdateAsync(Persona persona);
    }
}

