# Medigestor
Es un sistema de gestión de pacientes que tiene como objetivo optimizar procesos medicos y facilitar el seguimiento de los pacientes con una interfaz simple y moderna.


## Modulos
Este sistema no se limita a la gestión de datos personales de los pacientes solamente por lo que esta dividido en 7 partes:

- Inicio de sesión
- Gestión de usuarios
- Gestión de pacientes
- Gestión de citas
- Gestión de doctores
- Reportes y estadisticas
- Historiales medicos

## Herramienta de desarrollo
- Visual Studio 2022

## Tecnologías utilizadas
- Blazor Hybrid con .NET MAUI
- Sqlite
- Supabase

## Consideraciones al ejecutar el proyecto
- Al clonar el repositorio o descargarlo como `.zip`, al ejecutarse el proyecto automáticamente se sincronizará con una **base de datos remota en Supabase** y creará una **base de datos local SQLite**.  
- Para entrar al sistema utiliza las credenciales de prueba:  
  - Usuario: `admin`  
  - Contraseña: `admin123`
