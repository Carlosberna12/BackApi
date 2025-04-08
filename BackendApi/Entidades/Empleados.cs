using System.ComponentModel.DataAnnotations;

namespace BackendApi.Entidades
{
    public class Empleados
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El camapo {0} es requerido")]
        public required string Nombre { get; set; }
        [Required(ErrorMessage = "El camapo {0} es requerido")]
        public required DateTime FechaIngreso { get; set; }
        [Required(ErrorMessage = "El camapo {0} es requerido")]
        public required int Sueldo { get; set; }

    }
}
