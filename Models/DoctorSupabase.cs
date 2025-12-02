using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MauiBlazorHybrid.Models
{
    [Table("doctor")]
    public class DoctorSupabase : BaseModel
    {
        [PrimaryKey("doctor_id")]
        public int DoctorId { get; set; }

        [Column("nombre")]
        public string? Nombre { get; set; }

        [Column("apellido")]
        public string? Apellido { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("sexo")]
        public string? Sexo { get; set; }

        [Column("correo")]
        public string? Correo { get; set; }

        [Column("especialidad")]
        public string? Especialidad { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // EF Core navigation properties no se usan en Supabase
        // public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
