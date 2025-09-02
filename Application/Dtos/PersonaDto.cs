namespace Application.Dtos
{
    public record PersonaDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public int Edad { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Profesion { get; set; }
    }
}
