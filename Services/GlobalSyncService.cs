using Supabase;
using MauiBlazorHybrid.Models;
using MauiBlazorHybrid.Data;
using Microsoft.EntityFrameworkCore;
using MauiBlazorHybrid.Domain.Entities;

public class GlobalSyncService
{
    private readonly AppDbContext _db;
    private readonly Client _supabase;
    private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

    public GlobalSyncService(AppDbContext db, Client supabase)
    {
        _db = db;
        _supabase = supabase;
    }

    public async Task SyncAllAsync()
    {
        await _syncLock.WaitAsync(); //Bloquear
        try
        {
            await SyncDoctoresAsync();
            await SyncPacientesAsync();
            await SyncCitasAsync();
            await SyncHistorialesAsync();
            await SyncUsuariosAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error general en SyncAllAsync: {ex.Message}");
        }
        finally
        {
            _syncLock.Release(); //Libera el lock
        }
    }

    public async Task SyncDoctoresAsync()
    {
        try
        {

            System.Diagnostics.Debug.WriteLine("🔄 Iniciando sincronización de doctores...");

            _db.ChangeTracker.Clear();


            // 1️⃣ Descargar doctores desde Supabase
            var response = await _supabase.From<DoctorSupabase>().Get();
            var remotos = response.Models;
            System.Diagnostics.Debug.WriteLine($"📥 {remotos.Count} doctores recibidos desde Supabase.");

            var locales = _db.Doctores.ToList();

            // 2️⃣ Insertar/actualizar en SQLite según remotos
            foreach (var remoto in remotos)
            {
                var local = locales.FirstOrDefault(d => d.DoctorId == remoto.DoctorId);

                if (local == null)
                {
                    System.Diagnostics.Debug.WriteLine($"➕ Insertando doctor {remoto.Nombre} en SQLite.");
                    _db.Doctores.Add(new Doctor
                    {
                        DoctorId = remoto.DoctorId,
                        Nombre = remoto.Nombre,
                        Apellido = remoto.Apellido,
                        Telefono = remoto.Telefono,
                        Sexo = remoto.Sexo,
                        Correo = remoto.Correo,
                        Especialidad = remoto.Especialidad,
                        UpdatedAt = remoto.UpdatedAt
                    });
                }
                else if (remoto.UpdatedAt > local.UpdatedAt)
                {
                    System.Diagnostics.Debug.WriteLine($"⬇️ Actualizando doctor {local.Nombre} desde Supabase.");
                    local.Nombre = remoto.Nombre;
                    local.Apellido = remoto.Apellido;
                    local.Telefono = remoto.Telefono;
                    local.Sexo = remoto.Sexo;
                    local.Correo = remoto.Correo;
                    local.Especialidad = remoto.Especialidad;
                    local.UpdatedAt = remoto.UpdatedAt;

                    _db.Doctores.Update(local);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚖️ Doctor {remoto.Nombre} ya sincronizado o más reciente en local.");
                }
            }

            // 4️⃣ Guardar cambios en SQLite
            await _db.SaveChangesAsync();

            // 5️⃣ Subir a Supabase solo los locales nuevos o modificados
            foreach (var local in _db.Doctores.ToList())
            {
                var remoto = remotos.FirstOrDefault(r => r.DoctorId == local.DoctorId);
                if (remoto == null || local.UpdatedAt > remoto.UpdatedAt)
                {
                    System.Diagnostics.Debug.WriteLine($"⬆️ Upsert doctor {local.Nombre} en Supabase.");
                    await _supabase.From<DoctorSupabase>().Upsert(new DoctorSupabase
                    {
                        DoctorId = local.DoctorId,
                        Nombre = local.Nombre,
                        Apellido = local.Apellido,
                        Telefono = local.Telefono,
                        Sexo = local.Sexo,
                        Correo = local.Correo,
                        Especialidad = local.Especialidad,
                        UpdatedAt = local.UpdatedAt
                    });
                }
            }

            
            // 7️⃣ Eliminar doctores en Supabase que ya no existen en SQLite
            var idsLocales = _db.Doctores.Select(d => d.DoctorId).ToHashSet();
            var remotosParaEliminar = remotos.Where(r => !idsLocales.Contains(r.DoctorId)).ToList();
            foreach (var doctor in remotosParaEliminar)
            {
                System.Diagnostics.Debug.WriteLine($"🗑 Eliminando doctor {doctor.Nombre} de Supabase porque ya no existe en SQLite.");
                await _supabase.From<DoctorSupabase>()
                               .Where(d => d.DoctorId == doctor.DoctorId)
                               .Delete();
            }



            System.Diagnostics.Debug.WriteLine("✅ Sincronización de doctores completada.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error en SyncDoctoresAsync: {ex.Message}");
            throw;
        }
    }


    // 🔁 Repite el mismo patrón para Pacientes, Citas, Historiales y Usuarios
    private async Task SyncPacientesAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("🔄 Iniciando sincronización de pacientes...");

            // Limpia el ChangeTracker para evitar entidades duplicadas
            _db.ChangeTracker.Clear();

            // 1️⃣ Descargar pacientes desde Supabase
            var response = await _supabase.From<PacienteSupabase>().Get();
            var remotos = response.Models;

            System.Diagnostics.Debug.WriteLine($"📥 {remotos.Count} pacientes recibidos desde Supabase.");

            foreach (var remoto in remotos)
            {
                var local = _db.Pacientes.AsNoTracking().FirstOrDefault(p => p.PacienteId == remoto.PacienteId);

                if (local == null)
                {
                    System.Diagnostics.Debug.WriteLine($"➕ Insertando paciente {remoto.Nombre} en SQLite.");
                    _db.Pacientes.Add(new Paciente
                    {
                        PacienteId = remoto.PacienteId,
                        Nombre = remoto.Nombre,
                        Apellido = remoto.Apellido,
                        Correo = remoto.Correo,
                        Telefono = remoto.Telefono,
                        Sexo = remoto.Sexo,
                        FechaNacimiento = remoto.FechaNacimiento,
                        Edad = remoto.Edad,
                        Cedula = remoto.Cedula,
                        Nacionalidad = remoto.Nacionalidad,
                        TipoSangre = remoto.TipoSangre,
                        Direccion = remoto.Direccion,
                        SeguroMedico = remoto.SeguroMedico,
                        UpdatedAt = remoto.UpdatedAt
                    });
                }
                else if (remoto.UpdatedAt > local.UpdatedAt)
                {
                    System.Diagnostics.Debug.WriteLine($"⬇️ Actualizando paciente {local.Nombre} desde Supabase.");
                    var pacienteActualizado = local;
                    pacienteActualizado.Nombre = remoto.Nombre;
                    pacienteActualizado.Apellido = remoto.Apellido;
                    pacienteActualizado.Correo = remoto.Correo;
                    pacienteActualizado.Telefono = remoto.Telefono;
                    pacienteActualizado.Sexo = remoto.Sexo;
                    pacienteActualizado.FechaNacimiento = remoto.FechaNacimiento;
                    pacienteActualizado.Edad = remoto.Edad;
                    pacienteActualizado.Cedula = remoto.Cedula;
                    pacienteActualizado.Nacionalidad = remoto.Nacionalidad;
                    pacienteActualizado.TipoSangre = remoto.TipoSangre;
                    pacienteActualizado.SeguroMedico = remoto.SeguroMedico;
                    pacienteActualizado.Direccion = remoto.Direccion;
                    pacienteActualizado.UpdatedAt = remoto.UpdatedAt;

                    _db.Pacientes.Update(pacienteActualizado);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚖️ Paciente {remoto.Nombre} ya sincronizado o más reciente en local.");
                }
            }


            await _db.SaveChangesAsync();

            // 2️⃣ Subir pacientes desde SQLite hacia Supabase
            var locales = _db.Pacientes.AsNoTracking().ToList();

            foreach (var local in locales)
            {
                var remoto = remotos.FirstOrDefault(r => r.PacienteId == local.PacienteId);

                if (remoto == null || local.UpdatedAt > remoto.UpdatedAt)
                {
                    System.Diagnostics.Debug.WriteLine($"⬆️ Subiendo paciente {local.Nombre} a Supabase.");
                    var result = await _supabase.From<PacienteSupabase>().Upsert(new PacienteSupabase
                    {
                        PacienteId = local.PacienteId,
                        Nombre = local.Nombre,
                        Apellido = local.Apellido,
                        Correo = local.Correo,
                        Telefono = local.Telefono,
                        Sexo = local.Sexo,
                        FechaNacimiento = local.FechaNacimiento,
                        Edad = local.Edad,
                        Cedula = local.Cedula,
                        Nacionalidad = local.Nacionalidad,
                        TipoSangre = local.TipoSangre,
                        Direccion = local.Direccion,
                        SeguroMedico = local.SeguroMedico,
                        UpdatedAt = local.UpdatedAt
                    });

                    System.Diagnostics.Debug.WriteLine($"Resultado Upsert: {result.Models.Count} registros procesados.");
                }
            }


            System.Diagnostics.Debug.WriteLine("✅ Sincronización de pacientes completada.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error en SyncPacientesAsync: {ex.Message}");
            throw;
        }
    }



    public async Task SyncCitasAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("🔄 Iniciando sincronización de citas...");

            _db.ChangeTracker.Clear();

            // 1️⃣ Descargar citas desde Supabase
            var response = await _supabase.From<CitaSupabase>().Get();
            var remotos = response.Models;
            System.Diagnostics.Debug.WriteLine($"📥 {remotos.Count} citas recibidas desde Supabase.");

            var locales = _db.Citas.ToList();

            // 2️⃣ Insertar/actualizar en SQLite según remotos
            foreach (var remoto in remotos)
            {
                var local = locales.FirstOrDefault(c => c.CitaId == remoto.CitaId);

                if (local == null)
                {
                    System.Diagnostics.Debug.WriteLine($"➕ Insertando cita {remoto.CitaId} en SQLite.");
                    _db.Citas.Add(new Cita
                    {
                        CitaId = remoto.CitaId,
                        PacienteId = remoto.PacienteId,
                        DoctorId = remoto.DoctorId,
                        Fecha = remoto.Fecha,
                        Hora = remoto.Hora,
                        TipoConsulta = remoto.TipoConsulta,
                        NombrePaciente = remoto.NombrePaciente,
                        NombreDoctor = remoto.NombreDoctor,
                        Estado = remoto.Estado,
                        Motivo = remoto.Motivo,
                        UpdatedAt = remoto.UpdatedAt
                    });
                }
                else if (remoto.UpdatedAt > local.UpdatedAt)
                {
                    System.Diagnostics.Debug.WriteLine($"⬇️ Actualizando cita {local.CitaId} desde Supabase.");
                    local.PacienteId = remoto.PacienteId;
                    local.DoctorId = remoto.DoctorId;
                    local.Fecha = remoto.Fecha;
                    local.Hora = remoto.Hora;
                    local.Motivo = remoto.Motivo;
                    local.TipoConsulta = remoto.TipoConsulta;
                    local.NombrePaciente = remoto.NombrePaciente;
                    local.NombreDoctor = remoto.NombreDoctor;
                    local.Estado = remoto.Estado;
                    local.UpdatedAt = remoto.UpdatedAt;

                    _db.Citas.Update(local);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚖️ Cita {remoto.CitaId} ya sincronizada o más reciente en local.");
                }
            }

            // 4️⃣ Guardar cambios en SQLite
            await _db.SaveChangesAsync();

            // 5️⃣ Subir a Supabase solo las locales nuevas o modificadas
            foreach (var local in _db.Citas.ToList())
            {
                var remoto = remotos.FirstOrDefault(r => r.CitaId == local.CitaId);
                if (remoto == null || local.UpdatedAt > remoto.UpdatedAt)
                {
                    System.Diagnostics.Debug.WriteLine($"⬆️ Upsert cita {local.CitaId} en Supabase.");
                    await _supabase.From<CitaSupabase>().Upsert(new CitaSupabase
                    {
                        CitaId = local.CitaId,
                        PacienteId = local.PacienteId,
                        DoctorId = local.DoctorId,
                        Fecha = local.Fecha,
                        Hora = local.Hora,
                        Motivo = local.Motivo,
                        TipoConsulta = local.TipoConsulta,
                        NombrePaciente = local.NombrePaciente,
                        NombreDoctor = local.NombreDoctor,
                        Estado = local.Estado,
                        UpdatedAt = local.UpdatedAt
                    });
                }
            }

            // 6️⃣ Eliminar citas en Supabase que ya no existen en SQLite
            var idsLocales = _db.Citas.Select(c => c.CitaId).ToHashSet();
            var remotasParaEliminar = remotos.Where(r => !idsLocales.Contains(r.CitaId)).ToList();

            foreach (var cita in remotasParaEliminar)
            {
                System.Diagnostics.Debug.WriteLine($"🗑 Eliminando cita {cita.CitaId} de Supabase porque ya no existe en SQLite.");
                await _supabase.From<CitaSupabase>()
                               .Where(c => c.CitaId == cita.CitaId) // debe mapear a cita_id
                               .Delete();
            }


            System.Diagnostics.Debug.WriteLine("✅ Sincronización de citas completada.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error en SyncCitasAsync: {ex.Message}");
            throw;
        }
    }



    public async Task SyncHistorialesAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("🔄 Iniciando sincronización de historiales...");

            _db.ChangeTracker.Clear();

            // 1️⃣ Descargar historiales desde Supabase
            var response = await _supabase.From<HistorialMedicoSupabase>().Get();
            var remotos = response.Models;
            System.Diagnostics.Debug.WriteLine($"📥 {remotos.Count} historiales recibidos desde Supabase.");

            var locales = _db.HistorialesMedicos.ToList();

            // 2️⃣ Insertar/actualizar en SQLite según remotos
            foreach (var remoto in remotos)
            {
                var local = locales.FirstOrDefault(h => h.HistorialMedicoId == remoto.HistorialMedicoId);

                if (local == null)
                {
                    System.Diagnostics.Debug.WriteLine($"➕ Insertando historial {remoto.HistorialMedicoId} en SQLite.");
                    _db.HistorialesMedicos.Add(new HistorialMedico
                    {
                        HistorialMedicoId = remoto.HistorialMedicoId,
                        PacienteId = remoto.PacienteId,
                        NombrePaciente = remoto.NombrePaciente,
                        Diagnostico = remoto.Diagnostico,
                        Alergias = remoto.Alergias,
                        Enfermedades = remoto.Enfermedades,
                        Tratamiento = remoto.Tratamiento,
                        Observaciones = remoto.Observaciones,
                        Fecha = remoto.Fecha,
                        UpdatedAt = remoto.UpdatedAt
                    });
                }
                else if (remoto.UpdatedAt > local.UpdatedAt)
                {
                    System.Diagnostics.Debug.WriteLine($"⬇️ Actualizando historial {local.HistorialMedicoId} desde Supabase.");
                    local.PacienteId = remoto.PacienteId;
                    local.NombrePaciente = remoto.NombrePaciente;
                    local.Diagnostico = remoto.Diagnostico;
                    local.Alergias = remoto.Alergias;
                    local.Enfermedades = remoto.Enfermedades;
                    local.Tratamiento = remoto.Tratamiento;
                    local.Observaciones = remoto.Observaciones;
                    local.Fecha = remoto.Fecha;
                    local.UpdatedAt = remoto.UpdatedAt;

                    _db.HistorialesMedicos.Update(local);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚖️ Historial {remoto.HistorialMedicoId} ya sincronizado o más reciente en local.");
                }
            }

            // 3️⃣ Guardar cambios en SQLite
            await _db.SaveChangesAsync();

            // 4️⃣ Subir a Supabase solo los locales nuevos o modificados
            foreach (var local in _db.HistorialesMedicos.Include(h => h.Paciente).ToList())
            {
                var remoto = remotos.FirstOrDefault(r => r.HistorialMedicoId == local.HistorialMedicoId);
                if (remoto == null || local.UpdatedAt > remoto.UpdatedAt)
                {
                    System.Diagnostics.Debug.WriteLine($"⬆️ Upsert historial {local.HistorialMedicoId} en Supabase.");
                    await _supabase.From<HistorialMedicoSupabase>().Upsert(new HistorialMedicoSupabase
                    {
                        HistorialMedicoId = local.HistorialMedicoId,
                        PacienteId = local.PacienteId,
                        NombrePaciente = local.Paciente?.Nombre + " " + local.Paciente?.Apellido,
                        Diagnostico = local.Diagnostico,
                        Alergias = local.Alergias,
                        Enfermedades = local.Enfermedades,
                        Tratamiento = local.Tratamiento,
                        Observaciones = local.Observaciones,
                        Fecha = local.Fecha,
                        UpdatedAt = local.UpdatedAt
                    });
                }
            }

            // 5️⃣ Eliminar historiales en Supabase que ya no existen en SQLite
            var idsLocales = _db.HistorialesMedicos.Select(h => h.HistorialMedicoId).ToHashSet();
            var remotosParaEliminar = remotos.Where(r => !idsLocales.Contains(r.HistorialMedicoId)).ToList();
            foreach (var historial in remotosParaEliminar)
            {
                System.Diagnostics.Debug.WriteLine($"🗑 Eliminando historial {historial.HistorialMedicoId} de Supabase porque ya no existe en SQLite.");
                await _supabase.From<HistorialMedicoSupabase>()
                               .Where(h => h.HistorialMedicoId == historial.HistorialMedicoId)
                               .Delete();
            }

            System.Diagnostics.Debug.WriteLine("✅ Sincronización de historiales completada.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error en SyncHistorialesAsync: {ex.Message}");
            throw;
        }
    }


    private async Task SyncUsuariosAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("🔄 Iniciando sincronización de usuarios...");

            // Limpia el ChangeTracker para evitar entidades duplicadas
            _db.ChangeTracker.Clear();

            // 1️⃣ Descargar usuarios desde Supabase
            var response = await _supabase.From<UsuarioSupabase>().Get();
            var remotos = response.Models;

            System.Diagnostics.Debug.WriteLine($"📥 {remotos.Count} usuarios recibidos desde Supabase.");

            foreach (var remoto in remotos)
            {
                var local = _db.Usuario.AsNoTracking()
                    .FirstOrDefault(u => u.Id == remoto.Id);

                if (local == null)
                {
                    System.Diagnostics.Debug.WriteLine($"➕ Insertando usuario {remoto.Nombre} en SQLite.");
                    _db.Usuario.Add(new Usuario
                    {
                        Id = remoto.Id,
                        Nombre = remoto.Nombre,
                        Contraseña = remoto.Contrasena,
                        Rol = remoto.Rol,
                        UpdatedAt = remoto.UpdatedAt
                    });
                }
                else if (remoto.UpdatedAt > local.UpdatedAt)
                {
                    System.Diagnostics.Debug.WriteLine($"⬇️ Actualizando usuario {local.Nombre} desde Supabase.");
                    var usuarioActualizado = local;
                    usuarioActualizado.Nombre = remoto.Nombre;
                    usuarioActualizado.Contraseña = remoto.Contrasena;
                    usuarioActualizado.Rol = remoto.Rol;
                    usuarioActualizado.UpdatedAt = remoto.UpdatedAt;

                    _db.Usuario.Update(usuarioActualizado);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚖️ Usuario {remoto.Nombre} ya sincronizado o más reciente en local.");
                }
            }

            await _db.SaveChangesAsync();

            // 2️⃣ Subir usuarios desde SQLite hacia Supabase
            var locales = _db.Usuario.AsNoTracking().ToList();

            foreach (var local in locales)
            {
                var remoto = remotos.FirstOrDefault(r => r.Id == local.Id);

                if (remoto == null || local.UpdatedAt > remoto.UpdatedAt)
                {
                    System.Diagnostics.Debug.WriteLine($"⬆️ Subiendo usuario {local.Nombre} a Supabase.");
                    await _supabase.From<UsuarioSupabase>().Upsert(new UsuarioSupabase
                    {
                        Id = local.Id,
                        Nombre = local.Nombre,
                        Contrasena = local.Contraseña,
                        Rol = local.Rol,
                        UpdatedAt = local.UpdatedAt
                    });
                }
            }
           
            System.Diagnostics.Debug.WriteLine("✅ Sincronización de usuarios completada.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error en SyncUsuariosAsync: {ex.Message}");
            throw;
        }
    }



}

