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
    public interface IHistorialService
    {
        Task CrearNuevoHistorial(HistorialMedicoDto dto);
        Task EliminarHistorial(int Id);
        Task ModificarHistorial(HistorialMedicoDto dto);
        Task<List<CitaDto>> ObtenerCitasPorPacienteAsync(int pacienteId);
        Task<List<HistorialMedicoDto>> ObtenerHistorialesAsync();
    }

    public class HistorialService : IHistorialService
    {
        private readonly AppDbContext _context;
        private readonly Client _supabase;

        public HistorialService(AppDbContext context, Client supabase)
        {
            _context = context;
            _supabase = supabase;
        }

        public async Task<List<HistorialMedicoDto>> ObtenerHistorialesAsync()
        {
            return await _context.HistorialesMedicos
                .Include(h => h.Paciente)
                .Where(h => h.Paciente != null)
                .Select(h => new HistorialMedicoDto
                {
                    HistorialMedicoId = h.HistorialMedicoId,
                    NombrePaciente = h.Paciente.Nombre +" "+ h.Paciente.Apellido,
                    PacienteId = h.PacienteId,
                    Fecha = h.Fecha,
                    Diagnostico = h.Diagnostico,
                    Tratamiento = h.Tratamiento,
                    Enfermedades = h.Enfermedades,
                    Alergias = h.Alergias,
                    Observaciones = h.Observaciones
                })
                .ToListAsync();
        }

        public async Task CrearNuevoHistorial(HistorialMedicoDto dto)
        {
            var pacienteExiste = await _context.Pacientes.AnyAsync(p => p.PacienteId == dto.PacienteId);
            if (!pacienteExiste)
                throw new InvalidOperationException("El paciente no existe.");

            var historial = new HistorialMedico
            {
                PacienteId = dto.PacienteId,
                HistorialMedicoId = dto.HistorialMedicoId,
                NombrePaciente = dto.NombrePaciente,
                Fecha = dto.Fecha,
                Diagnostico = dto.Diagnostico,
                Tratamiento = dto.Tratamiento,
                Enfermedades = dto.Enfermedades,
                Alergias = dto.Alergias,
                Observaciones = dto.Observaciones,
                UpdatedAt = DateTime.UtcNow
            };
            _context.HistorialesMedicos.Add(historial);
            await _context.SaveChangesAsync();
        }

        public async Task ModificarHistorial(HistorialMedicoDto dto)
        {
            var historial = await _context.HistorialesMedicos.FindAsync(dto.HistorialMedicoId);
            if (historial != null)
            {
                historial.PacienteId = dto.PacienteId;
                historial.HistorialMedicoId = dto.HistorialMedicoId;
                historial.NombrePaciente = dto.NombrePaciente;
                historial.Fecha = dto.Fecha;
                historial.Diagnostico = dto.Diagnostico;
                historial.Tratamiento = dto.Tratamiento;
                historial.Enfermedades = dto.Enfermedades;
                historial.Alergias = dto.Alergias;
                historial.Observaciones = dto.Observaciones;
                historial.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarHistorial(int Id)
        {
            var historial = await _context.HistorialesMedicos
                .FirstOrDefaultAsync(c => c.HistorialMedicoId == Id);
            if (historial != null)
            {
                _context.HistorialesMedicos.Remove(historial);
                await _context.SaveChangesAsync();

                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    await _supabase
                        .From<HistorialMedicoSupabase>()
                        .Where(h => h.HistorialMedicoId == Id)
                        .Delete();
                }
            }
        }


        //quiero obtener citas por paciente (falta logica en el front)
        public async Task<List<CitaDto>> ObtenerCitasPorPacienteAsync(int pacienteId)
        {
            return await _context.Citas
                .Where(c => c.PacienteId == pacienteId)
                .Select(c => new CitaDto
                {
                    CitaId = c.CitaId,
                    Fecha = c.Fecha,
                    Estado = c.Estado,
                    Motivo = c.Motivo
                })
                .ToListAsync();
        }


    }
}
