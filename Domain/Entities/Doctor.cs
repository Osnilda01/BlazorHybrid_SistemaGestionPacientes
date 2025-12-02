using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MauiBlazorHybrid.Domain.Entities
{
    public class Doctor
    {

        [Key]
        public int DoctorId { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? Sexo { get; set; }
        [Column(TypeName = "varchar(13)")]
        public string? Correo { get; set; }
        public string? Especialidad { get; set; }
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
