using System.ComponentModel.DataAnnotations;

namespace ProyectoClaseG4.DTOs;

public class ForgotPasswordDto
{
    
    [Required(ErrorMessage = "El campo email es obligatorio")]
    [EmailAddress(ErrorMessage = "El campo email es obligatorio")]
    public string Email { get; set; } = string.Empty;
    
}