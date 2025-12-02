using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Domain.Dtos
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        [Required(ErrorMessage = "El nombre del doctor es requerido")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "El Apellido del doctor es requerido")]
        public string? Apellido { get; set; }

        [MaxLength(12, ErrorMessage = "El campo excede la cantidad de caracteres necesarios")]
        //[Required(AllowEmptyStrings = true)]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Formato esperado: XXX-XXX-XXXX")]
        public string? Telefono { get; set; }
        public string? Sexo { get; set; }
        public string? Correo { get; set; }
        [Required(ErrorMessage = "Necesita especificar la especialidad")]
        public string? Especialidad { get; set; }
    }
}
