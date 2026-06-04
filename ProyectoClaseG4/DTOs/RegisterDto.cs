using System.ComponentModel.DataAnnotations;

namespace ProyectoClaseG4.DTOs;

public class RegisterDto
{
    // Lo que el frontend manda cuando alguien se quiere registrar
    
    //El sistema requiere un nombre valido
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 a 50 caracteres")]
    public string FullName {get; set;} = string.Empty;
    
    
    //El sistema requiere un email válido
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no valido")]
    public string Email {get; set;} = string.Empty;
    
    
    //El sistema requiere una contraseña válida
    
    [Required(ErrorMessage = "Escriba una contraseña")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password {get; set;} = string.Empty;
}