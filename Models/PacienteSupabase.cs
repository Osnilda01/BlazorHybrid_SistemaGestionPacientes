using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Models
{
    [Table("paciente")] // 👈 nombre de la tabla en Postgres
    public class PacienteSupabase : BaseModel
    {
        [PrimaryKey("paciente_id")]
        public int PacienteId { get; set; }

        [Column("nombre")]
        public string? Nombre { get; set; }

        [Column("apellido")]
        public string? Apellido { get; set; }

        [Column("fecha_nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Column("edad")]
        public int Edad { get; set; }

        [Column("sexo")]
        public string? Sexo { get; set; }

        [Column("cedula")]
        public string? Cedula { get; set; }

        [Column("correo")]
        public string? Correo { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("nacionalidad")]
        public string? Nacionalidad { get; set; }

        [Column("tipo_sangre")]
        public string? TipoSangre { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("seguro_medico")]
        public string? SeguroMedico { get; set; }


        // EF Core navigation properties no se usan en Supabase
        // public ICollection<Cita> Citas { get; set; } = new List<Cita>();
        // public ICollection<Doctor> Doctores { get; set; } = new List<Doctor>();
        // public ICollection<HistorialMedico> HistorialesMedicos { get; set; } = new List<HistorialMedico>();
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
