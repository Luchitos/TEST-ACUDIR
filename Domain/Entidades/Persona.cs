using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidades
{
    public class Persona
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public int Edad { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Profesion { get; set; }
    }
}
