using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoClaseG4.DTOs;
using ProyectoClaseG4.Models;
using ProyectoClaseG4.Services;

namespace ProyectoClaseG4.Controllers;

// ApiController activa validaciones de forma automatica y ayuda al manejo de errores
[ApiController]
// La ruta se construiria: api/auth
// [controller] toma el nombre de la clase sin la palabra Controller
[Route("api/[controller]")]

public class AuthController :ControllerBase
{ 
    // Guarda el servicio en una privada readonly
        // Solo de lectura porque no deberia cambiar despues que se hace la inyeccion
        private readonly AuthService _authService;
        
        // El constructor recibe el AuthService gracias a la inyeccion de dependencias
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        
        // HttpPost indica que este endpoint responde a peticiones POST
        // POST /api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            // FromBody le dice a .NET que lea los datos del cuerpo del request
            // Desde el frontend se manda un json y .net lo convierte a DTO
            [FromBody] RegisterDto dto)
        {
            try
            {
                var user = await _authService.Register(dto);

                // OK() devuelve un 200 con el objeto que le pasemos
                // Solo campos necesarios, id, fullname, email, role
                return Ok(new { user.Id, user.FullName, user.Email, user.Role });
            }
            catch (Exception e)
            {
                // BadRequest devuelve 400 que indicaria que algo fallo por parte del cliente
                // Por ejemplo, email duplicado o datos invalidos
                return BadRequest(new { message = e.Message });
            }
        }
        
        // POST /api/Auth/register-admin
        //solo admins pueden llamar este endpoint

        [HttpPost("register-admin")]
        [Authorize(Roles = UserRole.Admin)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto dto)
        {
            try
            {
                var user = await _authService.RegisterAdmin(dto);
                return Ok(new { user.Id, user.FullName, user.Email, user.Role });
            }
            
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
        
        
        // La ruta completa seria: POST /api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            // Igual que el register, FromBody que convierte el JSO al DTO
            [FromBody] LoginDto dto
        )
        {
            try
            {
                // Si las credenciales son correctas, recibimos un JWT
                var response = await _authService.Login(dto);
                
                //Devolvemos el token al frontend para que lo guarde
                //El frontend debe mandarlo en cada peticion como Bearer Token
                return Ok( response );
            }catch(Exception e)
            {
                // Credenciales invalidas u otro error - 400
                return BadRequest(new { message = e.Message });
            }
        }
    
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                await _authService.ForgotPassword(dto);
                return Ok(new { message = "Si el correo existe recibiras un email con las instrucciones para cambiar la contraseña" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
                
            }
        }
}