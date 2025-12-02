using MauiBlazorHybrid.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MauiBlazorHybrid.Domain.Entities;

namespace MauiBlazorHybrid.Services
{
    public interface ILoginService
    {
        Task<Usuario?> ValidarRol(string nombre, string contraseña);
    }

    public class LoginService : ILoginService
    {
        private readonly AppDbContext _context;

        public LoginService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Usuario?> ValidarRol(string nombre, string contraseña)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Nombre == nombre && u.Contraseña == contraseña);

            return usuario;
        }

    }
}

