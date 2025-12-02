using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Models
{
    [Table("historial_medico")]
    public class HistorialMedicoSupabase : BaseModel
    {
        [PrimaryKey("historial_medico_id")]
        public int HistorialMedicoId { get; set; }

        [Column("paciente_id")]
        public int PacienteId { get; set; }

        [Column("nombre_paciente")]
        public string? NombrePaciente { get; set; }

        [Column("diagnostico")]
        public string? Diagnostico { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column("tratamiento")]
        public string? Tratamiento { get; set; }

        [Column("alergias")]
        public string? Alergias { get; set; }

        [Column("enfermedades")]
        public string? Enfermedades { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
