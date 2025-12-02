using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Domain.Entities
{
    [Table("Citas")]
    public class Cita
    {
        [Key]
        public int CitaId { get; set; }
        public int PacienteId { get; set; }
        [ForeignKey("PacienteId")]
        public Paciente? Paciente { get; set; }
        // Segunda relacion:
        public int DoctorId { get; set; }  
        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }
        public string? NombrePaciente { get; set; }
        public string? NombreDoctor { get; set; }
        public DateTime Hora { get; set; }
        public DateTime Fecha { get; set; }
        public string? TipoConsulta { get; set; }

        public string? Motivo { get; set; }
        public string? Estado { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
