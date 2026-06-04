using System.ComponentModel.DataAnnotations;

namespace ProyectoClaseG4.DTOs;

public class LoginDto
{
    // Lo que el frontend manda cuando alguien quiere entrar
    
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Email no valido")]
    public string Email {get; set;} = string.Empty;
    
    
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    public string Password {get; set;} = string.Empty;
}