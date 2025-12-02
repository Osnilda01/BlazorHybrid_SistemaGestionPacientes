using MauiBlazorHybrid.Data;
using MauiBlazorHybrid.Domain.Dtos;
using MauiBlazorHybrid.Domain.Entities;
using MauiBlazorHybrid.Models;
using Microsoft.EntityFrameworkCore;
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Services
{
    public interface IUserService
    {
        Task CrearNuevoUsuario(UsuarioDto dto);
        Task EliminarUsuario(int Id);
        Task<List<UsuarioDto>> GetUsuariosAsync();
        Task ModificarUsuario(UsuarioDto dto);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly Client _supabase;

        public UserService(AppDbContext context, Client supabase)
        {
            _context = context;
            _supabase = supabase;

        }

        public async Task<List<UsuarioDto>> GetUsuariosAsync()
        {
            return await _context.Usuario
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Contraseña = u.Contraseña,
                    Rol = u.Rol

                }).ToListAsync();
        }

        public async Task CrearNuevoUsuario(UsuarioDto dto)
        {

            var usuario = new Usuario
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Contraseña = dto.Contraseña,
                Rol = dto.Rol,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task ModificarUsuario(UsuarioDto dto)
        {
            var usuario = await _context.Usuario.FindAsync(dto.Id);
            if (usuario != null)
            {
                usuario.Nombre = dto.Nombre;
                usuario.Contraseña = dto.Contraseña;
                usuario.Rol = dto.Rol;
                usuario.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarUsuario(int Id)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == Id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
                await _context.SaveChangesAsync();

                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    await _supabase
                        .From<UsuarioSupabase>()
                        .Where(u => u.Id == Id)
                        .Delete();
                }
            }
        }
    }
}
