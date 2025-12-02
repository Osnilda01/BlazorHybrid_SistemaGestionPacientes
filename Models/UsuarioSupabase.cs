using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Models
{
    [Table("usuario")]
    public class UsuarioSupabase : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }

        [Column("rol")]
        public string? Rol { get; set; }

        [Column("nombre")]
        public string? Nombre { get; set; }

        [Column("contrasena")] 
        public string? Contrasena { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
