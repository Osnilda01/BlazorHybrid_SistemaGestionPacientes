using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Domain.Entities
{
    [Table("Pacientes")]
    public class Paciente
    {
        [Key]
        public int PacienteId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? Nombre { get; set; }
        [Column(TypeName = "varchar(60)")]
        public string? Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Edad { get; set; }
        [Column(TypeName = "varchar(60)")]
        public string? Sexo { get; set; }
        [Column(TypeName = "varchar(13)")]
        public string? Cedula { get; set; }
        public string? Correo{ get; set; }
        public string? Telefono { get; set; }
        public string? Nacionalidad { get; set; }
        public string? SeguroMedico { get; set; }
        public string? TipoSangre { get; set; }
        public string? Direccion { get; set; }
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
        public ICollection<Doctor> Doctores { get; set; } = new List<Doctor>();
        public ICollection<HistorialMedico> HistorialesMedicos { get; set; } = new List<HistorialMedico>();
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
