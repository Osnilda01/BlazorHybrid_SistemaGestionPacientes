using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Domain.Dtos
{
    public class PacienteDto
    {
        public int PacienteId { get; set; }
        [Required(ErrorMessage = "El paciente es requerido")]
        [RegularExpression(@"^[a-zA-Z\s.\-']{2,}$", ErrorMessage = "El nombre no puede contener numeros ni simbolos!")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Ingrese el apellido")]
        [RegularExpression(@"^[a-zA-Z\s.\-']{2,}$", ErrorMessage = "El apellido no puede contener numeros ni simbolos!")]

        public string? Apellido { get; set; }

        public DateTime FechaNacimiento { get; set; } = new DateTime(2000, 1, 1);
        public int Edad { get; set; }
        [Required(ErrorMessage = "Seleccione el genero")]
        public string? Sexo { get; set; }
        [MaxLength(13, ErrorMessage = "El campo excede la cantidad de caracteres necesarios")]
        [RegularExpression(@"^\d{3}-\d{7}-\d{1}$", ErrorMessage = "Formato esperado: 000-0000000-0")]
        public string? Cedula { get; set; } = "Sin especificar";
        public string? Correo { get; set; }
        [MaxLength(12, ErrorMessage = "El campo excede la cantidad de caracteres necesarios")]
        //[Required(AllowEmptyStrings = true)]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Formato esperado: 809-000-0000")]
        public string? Telefono { get; set; }
        [Required(ErrorMessage = "Seleccione una nacionalidad")]
        public string? Nacionalidad { get; set; }
        public string? TipoSangre { get; set; }
        public string? Direccion { get; set; }

    }
}
