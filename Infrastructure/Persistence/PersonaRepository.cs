using Domain.Entidades;
using Domain.Interfaz;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class PersonaRepository : IPersonaRepository
    {
        private readonly string _filePath = "Test.json";

        #region GetAllAsync
        public async Task<IEnumerable<Persona>> GetAllAsync(CancellationToken ct = default)
        {
            var jsonData = await File.ReadAllTextAsync(_filePath);
            return JsonConvert.DeserializeObject<List<Persona>>(jsonData) ?? new List<Persona>();
        }
        #endregion

        #region GetByIdAsync
        public async Task<Persona?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var jsonData = await File.ReadAllTextAsync(_filePath);
            var personas = JsonConvert.DeserializeObject<List<Persona>>(jsonData) ?? new List<Persona>();
            return personas.FirstOrDefault(p => p.Id == id);
        }
        #endregion

        #region AddAsync
        public async Task AddAsync(Persona persona, CancellationToken ct = default)
        {
            var personas = (await GetAllAsync()).ToList();
            persona.Id = personas.Any() ? personas.Max(p => p.Id) + 1 : 1;
            personas.Add(persona);
            await File.WriteAllTextAsync(_filePath, JsonConvert.SerializeObject(personas, Formatting.Indented));
        }
        #endregion

        #region UpdateAsync
        public async Task UpdateAsync(Persona persona, CancellationToken ct = default)
        {
            var personas = (await GetAllAsync()).ToList();
            var index = personas.FindIndex(p => p.Id == persona.Id);
            if (index != -1)
            {
                personas[index] = persona;
                await File.WriteAllTextAsync(_filePath, JsonConvert.SerializeObject(personas, Formatting.Indented));
            }
        }
        #endregion
    }
}


