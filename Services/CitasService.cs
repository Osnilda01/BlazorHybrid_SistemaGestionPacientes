using DocumentFormat.OpenXml.Office2010.Excel;
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
    public interface ICitaService
    {
        Task CrearNuevaCita(CitaDto dto);
        Task EliminarCita(int Id);
        Task CitaPendiente(int Id);
        Task CancelarCita(int Id);
        Task ModificarCita(CitaDto dto);
        Task<List<CitaDto>> ObtenerCitasAsync();
        
        Task<List<(string NombrePaciente, int NumeroCitas)>> ObtenerPacientesFrecuentesAsync();// estadisticas

        Task ActualizarEstadoCitasAsync(); //Para cuando es completada.
    }

    public class CitaService : ICitaService
    {
        private readonly AppDbContext _context;
        private readonly GlobalSyncService _syncservice;
        private readonly Client _supabase;

        public CitaService(AppDbContext context, GlobalSyncService syncservice, Client supabase)
        {
            _context = context;
            _syncservice = syncservice;
            _supabase = supabase;
        }

        public async Task<List<CitaDto>> ObtenerCitasAsync()
        {

            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .Where(c => c.Paciente != null && c.Doctor != null)
                .Select(c => new CitaDto
                {
                    CitaId = c.CitaId,
                    Fecha = c.Fecha,
                    Hora = c.Hora,
                    PacienteId = c.PacienteId,
                    DoctorId = c.DoctorId,
                    NombrePaciente = c.Paciente.Nombre + " " + c.Paciente.Apellido,
                    NombreDoctor = (c.Doctor.Sexo == "Femenino"
                    ? "Dra. " + c.Doctor.Nombre + " " + c.Doctor.Apellido
                    : "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido)
                    + " - " + c.Doctor.Especialidad,
                    TipoConsulta = c.TipoConsulta,
                    Motivo = c.Motivo,
                    Estado = c.Estado

                })
                .ToListAsync();
        }

        public async Task ActualizarEstadoCitasAsync()
        {
            var citasPendientes = await _context.Citas
                .Where(c => c.Estado == "Pendiente") // solo pendientes
                .ToListAsync();

            foreach (var cita in citasPendientes)
            {
                var fechaHoraCita = new DateTime(
                    cita.Fecha.Year,
                    cita.Fecha.Month,
                    cita.Fecha.Day,
                    cita.Hora.Hour,
                    cita.Hora.Minute,
                    cita.Hora.Second
                );

                if (fechaHoraCita <= DateTime.Now)
                {
                    cita.Estado = "Completada";
                }
            }

            await _context.SaveChangesAsync();
        }


        public async Task<List<(string NombrePaciente, int NumeroCitas)>> ObtenerPacientesFrecuentesAsync()
        {
            var pacientesfrecuentes = await _context.Citas
                .GroupBy(c => new { c.PacienteId, c.Paciente.Nombre, c.Paciente.Apellido })
                .Select(g => new
                {
                    NombrePaciente = g.Key.Nombre + " " + g.Key.Apellido,
                    NumeroCitas = g.Count()
                })
                .OrderByDescending(p => p.NumeroCitas)
                .Take(10)
                .ToListAsync();

            // Convertimos a tupla para no crear DTO
            return pacientesfrecuentes
                .Select(p => (p.NombrePaciente, p.NumeroCitas))
                .ToList();
        }
        public async Task CrearNuevaCita(CitaDto dto)
        {
            var paciente = await _context.Pacientes.FindAsync(dto.PacienteId);
            var doctor = await _context.Doctores.FindAsync(dto.DoctorId);
            if (paciente == null)
            {
                throw new Exception("Paciente no encontrado");
            }
            if (doctor == null)
            {
                throw new Exception("Doctor no encontrado");
            }

            var cita = new Cita
            {
                PacienteId = dto.PacienteId,
                DoctorId = dto.DoctorId,
                NombrePaciente = $"{paciente.Nombre} {paciente.Apellido}",
                NombreDoctor = $"{doctor.Nombre} {doctor.Apellido}",
                Fecha = dto.Fecha,
                Hora = dto.Hora,
                TipoConsulta = dto.TipoConsulta,
                Motivo = dto.Motivo,
                Estado = dto.Estado,
                UpdatedAt = DateTime.UtcNow

    };

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();
        }

        public async Task ModificarCita(CitaDto dto)
        {
            var cita = await _context.Citas.FindAsync(dto.CitaId);
            if (cita != null)
            {
                cita.PacienteId = dto.PacienteId;
                cita.DoctorId = dto.DoctorId;
                cita.NombrePaciente = dto.NombrePaciente;
                cita.NombreDoctor = dto.NombreDoctor;
                cita.Fecha = dto.Fecha;
                cita.Hora = dto.Hora;
                cita.TipoConsulta = dto.TipoConsulta;
                cita.Motivo = dto.Motivo;
                cita.Estado = dto.Estado;
                cita.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarCita(int Id)
        {
            var cita = await _context.Citas
                .FirstOrDefaultAsync(c => c.CitaId == Id);
            if (cita != null)
            {
                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();

                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    await _supabase
                        .From<CitaSupabase>()
                        .Where(c => c.CitaId == Id)
                        .Delete();
                }
            }
        }

        public async Task CancelarCita(int Id)
        {
            var cita = await _context.Citas
                .FirstOrDefaultAsync(c => c.CitaId == Id);
            if (cita != null)
            {
                cita.Estado = "Cancelada";
                await _context.SaveChangesAsync();
            }
        }

        public async Task CitaPendiente(int Id)
        {
            var cita = await _context.Citas
                .FirstOrDefaultAsync(c => c.CitaId == Id);
            if (cita != null)
            {
                cita.Estado = "Pendiente";
                await _context.SaveChangesAsync();
            }
        }

    }
}