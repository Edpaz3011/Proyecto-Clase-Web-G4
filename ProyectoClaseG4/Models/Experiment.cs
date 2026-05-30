namespace ProyectoClaseG4.Models;

public class Experiment
{
    // Representa un experimento o prueba que un usuario esta realizando
    // Funcionalidad principal despues de hacer el login
    
    public string Id { get; set; }
    
    // Titulo de lo que intento hacer
    public string Title { get; set; } = string.Empty;
    
    // El resultado de si funciono o no
    public string Result { get; set; } = string.Empty;
    
    // Usuario que creo el experimento / prueba
    public string UserId { get; set; } = string.Empty;
    
    // Exito o fracaso
    public bool Success { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}