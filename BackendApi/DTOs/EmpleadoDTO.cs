using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs
{
    public class EmpleadoDTO
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required DateTime FechaIngreso { get; set; }
        public required int Sueldo { get; set; }
    }
}
