namespace ProyectoClaseG4.DTOs;

public class RegisterDto
{
    // Lo que el frontend manda cuando alguien se quiere registrar
    public string FullName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string Password {get; set;} = string.Empty;
}