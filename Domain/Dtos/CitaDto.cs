using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Domain.Dtos
{
    public class CitaDto
    {
        public int CitaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un paciente")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un paciente válido")]
        public int PacienteId { get; set; }
        public int DoctorId { get; set; }

        public string? NombrePaciente { get; set; }
        public string? NombreDoctor{ get; set; }

        [Required(ErrorMessage = "La hora es requerida")]
        public DateTime Hora { get; set; } = DateTime.Parse("09:00");
        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Necesita ingresar un doctor")]
        public string? Doctor { get; set; }
        [Required(ErrorMessage = "El tipo de consulta es requerido")]
        public string? TipoConsulta { get; set; }
        [Required(ErrorMessage = "Debe proporcionar un motivo")]
        public string? Motivo { get; set; }
        public string? Estado { get; set; } = "Pendiente";

    }
}
