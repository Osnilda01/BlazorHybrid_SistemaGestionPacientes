using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Domain.Entities
{
    [Table("HistorialesMedicos")]
    public class HistorialMedico
    {
        public int HistorialMedicoId { get; set; }
        public int PacienteId { get; set; }
        [ForeignKey("PacienteId")]
        public Paciente? Paciente { get; set; }
        public string? NombrePaciente { get; set; }

        public string? Diagnostico { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? Tratamiento { get; set; }
        public string? Alergias { get; set; }
        public string? Enfermedades { get; set; }
        public string? Observaciones { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
