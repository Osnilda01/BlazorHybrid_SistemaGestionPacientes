using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Models
{
    [Table("cita")]
    public class CitaSupabase : BaseModel
    {
        [PrimaryKey("cita_id")]
        public int CitaId { get; set; }

        [Column("paciente_id")]
        public int PacienteId { get; set; }

        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Column("nombre_paciente")]
        public string? NombrePaciente { get; set; }

        [Column("nombre_doctor")]
        public string? NombreDoctor { get; set; }

        [Column("hora")]
        public DateTime Hora { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("tipo_consulta")]
        public string? TipoConsulta { get; set; }

        [Column("motivo")]
        public string? Motivo { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
