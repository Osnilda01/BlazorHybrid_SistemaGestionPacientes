using CommunityToolkit.Maui;
using MauiBlazorHybrid.Data;
using MauiBlazorHybrid.Domain.Entities;
using MauiBlazorHybrid.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Supabase;

namespace MauiBlazorHybrid
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

			builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.AddDebug();
#endif
            //builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IPacienteService, PacienteService>();
            builder.Services.AddScoped<ICitaService, CitaService>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IHistorialService, HistorialService>();


            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddAuthorizationCore();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var rutaDb = Path.Combine(FileSystem.AppDataDirectory, "Gestion.db");
                options.UseSqlite($"Data Source={rutaDb}");
            });

            //Servicio de sincronizacion
            builder.Services.AddSingleton<GlobalSyncService>();
            //Segundo plano
            builder.Services.AddSingleton<SyncBackgroundService>();

            // Inicializar Supabase
            var url = "https://zvrrqxmxgimyzedokmsu.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inp2cnJxeG14Z2lteXplZG9rbXN1Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjQwMzU0NzEsImV4cCI6MjA3OTYxMTQ3MX0.RMTYN_oTL8LNBzIwCenrjevcpDGKn_MmROhEGNAxibk";

            var supabase = new Supabase.Client(url, key);
            supabase.InitializeAsync().Wait(); // inicialización

            // Registrar como servicio singleton
            builder.Services.AddSingleton(supabase);

            var app = builder.Build();

            

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();

                if (!context.Usuario.Any())
                {
                    context.Usuario.Add(new Usuario
                    {
                        Id = 1,
                        Nombre = "admin",
                        Contraseña = "admin123",
                        Rol = "Admin"
                    });
                    context.SaveChanges();
                }
            }

            // Iniciar sincronización en segundo plano
            var syncBackground = app.Services.GetRequiredService<SyncBackgroundService>();
            var cts = new CancellationTokenSource();
            _ = syncBackground.StartAsync(cts.Token);


            return app;
		}
	}
}
