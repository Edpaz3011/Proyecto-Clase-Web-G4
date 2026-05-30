namespace ProyectoClaseG4.DTOs;

public class LoginDto
{
    // Lo que el frontend manda cuando alguien quiere entrar
    public string Email {get; set;} = string.Empty;
    public string Password {get; set;} = string.Empty;
}