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
    public interface IPacienteService
    {
        Task CrearNuevoPaciente(PacienteDto dto);
        Task EliminarPaciente(int Id);
        Task<List<PacienteDto>> GetPacientesAsync();
        Task ModificarPaciente(PacienteDto dto);
    }

    public class PacienteService : IPacienteService
    {
        private readonly AppDbContext _context;
        private readonly Client _supabase;

        public PacienteService(AppDbContext context, Client supabase)
        {
            _context = context;
            _supabase = supabase;
        }

        public async Task<List<PacienteDto>> GetPacientesAsync()
        {
            return await _context.Pacientes
                .Select(p => new PacienteDto
                {
                    PacienteId = p.PacienteId,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    Edad = p.Edad,
                    Sexo = p.Sexo,
                    Cedula = p.Cedula,
                    Correo = p.Correo,
                    Telefono = p.Telefono,
                    Nacionalidad = p.Nacionalidad,
                    TipoSangre = p.TipoSangre,
                    Direccion = p.Direccion
                }).ToListAsync();
        }

        public async Task CrearNuevoPaciente(PacienteDto dto)
        {

            var paciente = new Paciente
            {
                PacienteId = dto.PacienteId,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                FechaNacimiento = dto.FechaNacimiento,
                Edad = dto.Edad,
                Sexo = dto.Sexo,
                Cedula = dto.Cedula,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                Nacionalidad = dto.Nacionalidad,
                TipoSangre = dto.TipoSangre,
                Direccion = dto.Direccion,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
        }

        public async Task ModificarPaciente(PacienteDto dto)
        {
            var paciente = await _context.Pacientes.FindAsync(dto.PacienteId);
            if (paciente != null)
            {
                paciente.Nombre = dto.Nombre;
                paciente.Apellido = dto.Apellido;
                paciente.FechaNacimiento = dto.FechaNacimiento;
                paciente.Edad = dto.Edad;
                paciente.Sexo = dto.Sexo;
                paciente.Cedula = dto.Cedula;
                paciente.Correo = dto.Correo;
                paciente.Telefono = dto.Telefono;
                paciente.Nacionalidad = dto.Nacionalidad;
                paciente.TipoSangre = dto.TipoSangre;
                paciente.Direccion = dto.Direccion;
                paciente.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarPaciente(int Id)
        {
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.PacienteId == Id);
            if (paciente != null)
            {
                _context.Pacientes.Remove(paciente);
                await _context.SaveChangesAsync();

                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    await _supabase
                        .From<PacienteSupabase>()
                        .Where(p => p.PacienteId == Id)
                        .Delete();
                }
            }
        }
    }
}
