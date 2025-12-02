using MauiBlazorHybrid.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Services
{
    public class AuthService
    {
        private ClaimsPrincipal _usuarioActual;
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LoginAsync(string nombre, string contraseña)
        {
            System.Diagnostics.Debug.WriteLine($"Intentando login con usuario: {nombre}");

            try
            {
                var usuario = await _context.Usuario
                    .FirstOrDefaultAsync(u => u.Nombre == nombre && u.Contraseña == contraseña);

                if (usuario == null)
                {
                    System.Diagnostics.Debug.WriteLine("Usuario no encontrado o contraseña incorrecta.");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Login exitoso: {usuario.Nombre} con rol {usuario.Rol}");

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

                var identity = new ClaimsIdentity(claims, "offline");
                _usuarioActual = new ClaimsPrincipal(identity);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en LoginAsync: {ex.Message}");
                return false;
            }
        }


        public void Logout()
        {
            _usuarioActual = new ClaimsPrincipal(new ClaimsIdentity());
        }

        public ClaimsPrincipal UsuarioActual => _usuarioActual ?? new ClaimsPrincipal(new ClaimsIdentity());
    }
}
