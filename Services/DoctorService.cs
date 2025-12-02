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
    public interface IDoctorService
    {
        Task CrearNuevoDoctor(DoctorDto dto);
        Task EliminarDoctor(int Id);
        Task<List<DoctorDto>> GetDoctoresAsync();
        Task ModificarDoctor(DoctorDto dto);
    }

    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _context;
        private readonly GlobalSyncService _syncservice;
        private readonly Client _supabase;

        public DoctorService(AppDbContext context, GlobalSyncService syncservice, Client supabase)
        {
            _context = context;
            _syncservice = syncservice;
            _supabase = supabase;

        }

        public async Task<List<DoctorDto>> GetDoctoresAsync()
        {
            return await _context.Doctores
                .Select(d => new DoctorDto
                {
                    DoctorId = d.DoctorId,
                    Nombre = d.Nombre,
                    Apellido = d.Apellido,
                    Sexo = d.Sexo,
                    Correo = d.Correo,
                    Telefono = d.Telefono,
                    Especialidad = d.Especialidad
                }).ToListAsync();
        }

        public async Task CrearNuevoDoctor(DoctorDto dto)
        {

            var doctor = new Doctor
            {
                DoctorId = dto.DoctorId,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Sexo = dto.Sexo,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                Especialidad = dto.Especialidad,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Doctores.Add(doctor);
            await _context.SaveChangesAsync();

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _syncservice.SyncDoctoresAsync();
            }
        }

        public async Task ModificarDoctor(DoctorDto dto)
        {
            var doctor = await _context.Doctores.FindAsync(dto.DoctorId);
            if (doctor != null)
            {
                doctor.Nombre = dto.Nombre;
                doctor.Apellido = dto.Apellido;
                doctor.Sexo = dto.Sexo;
                doctor.Correo = dto.Correo;
                doctor.Telefono = dto.Telefono;
                doctor.Especialidad = dto.Especialidad;
                doctor.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    await _syncservice.SyncDoctoresAsync();
                }
            }
        }

        public async Task EliminarDoctor(int id)
        {
            var doctor = await _context.Doctores.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctores.Remove(doctor);
                await _context.SaveChangesAsync();

                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    await _supabase
                        .From<DoctorSupabase>()
                        .Where(d => d.DoctorId == id)
                        .Delete();
                }
            }
        }


    }
}
