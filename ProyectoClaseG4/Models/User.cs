namespace ProyectoClaseG4.Models;

public class User
{
    // Representar un usuario en el sistema
    // Esta clase es lo que vamos a guardar / leer en FS

    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // La contraseña siempre va a ir hasheada, nunca en texto plano
    public string PasswordHash { get; set; } = string.Empty;
    
    // Colocar un rol por defecto
    public string Role { get; set; } = UserRole.Guest;
    
    // Para saber cuando se creo el registro
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}