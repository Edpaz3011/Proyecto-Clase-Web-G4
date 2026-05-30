namespace ProyectoClaseG4.DTOs;

public class ExperimentDto
{
    // Lo que el frontend manda cuando alguien crea un experimento
    // No vamos a incluir el userId porque lo vamos a sacar del token
    
    public string Title {get; set;} = string.Empty;
    public string Result {get; set;} = string.Empty;
    public bool Success {get; set;} = false; 
}