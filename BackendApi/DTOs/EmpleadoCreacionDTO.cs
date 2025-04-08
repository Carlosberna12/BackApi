using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs
{
    public class EmpleadoCreacionDTO
    {
        [Required(ErrorMessage = "El camapo {0} es requerido")]
        public required string Nombre { get; set; }
        [Required(ErrorMessage = "El camapo {0} es requerido")]
        public required DateTime FechaIngreso { get; set; }
        [Required(ErrorMessage = "El camapo {0} es requerido")]
        public required int Sueldo { get; set; }
    }
}
