using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string? Rol { get; set; }
        public string? Nombre { get; set; }
        public string? Contraseña { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
