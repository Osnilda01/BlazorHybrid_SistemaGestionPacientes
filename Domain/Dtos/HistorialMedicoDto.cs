using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Domain.Dtos
{
    public class HistorialMedicoDto
    {
        public int HistorialMedicoId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un paciente válido.")]
        public int PacienteId { get; set; }
        public string? NombrePaciente { get; set; }
        public string? Diagnostico { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? Tratamiento { get; set; }
        public string? Alergias { get; set; }
        public string? Enfermedades { get; set; }
        public string? Observaciones { get; set; }
    }
}
