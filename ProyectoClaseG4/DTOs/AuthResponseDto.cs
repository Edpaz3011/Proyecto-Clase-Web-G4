namespace ProyectoClaseG4.DTOs;

public class AuthResponseDto
{
    //Token JWT para autenticar request
    
    public string Token {get; set;} = string.Empty;
    
    //Datos del usuario que mostrara la interfaz
    
    public string UserId {get; set;} = string.Empty;
    public string FullName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    
    //El front Usara esto para redirigir al panel correcto
    public string Role {get; set;} = string.Empty;
    
    //Esto hara saber cuando expira el token y pedirle que vuelva a iniicar sesion
    
    public DateTime ExpiresAt {get; set;}
}