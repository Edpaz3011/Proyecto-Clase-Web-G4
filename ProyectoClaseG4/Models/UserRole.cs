namespace ProyectoClaseG4.Models;

/// <summary>
/// Definimos roles disponibles para los usuarios en el sistema
/// Usaremos UserRole para controlar el acceso a funcionalidades
/// </summary>

public class UserRole
{
    /// <summary>
    /// El Usuario Admin podra gestionar alojamientos, usuarios, reservas y ver reportes.
    /// Solo se asignara manualmente desde la base de datos o por otro admin
    /// </summary>
    public const string Admin = "admin";
    
    /// <summary>
    /// Este será el rol por defecto al crar el registro.
    /// Podrá: Buscar alojamiento, hacer reservas, dejar reseñas y hacer preguntas.
    /// </summary>
    public const string Guest = "guest";
}