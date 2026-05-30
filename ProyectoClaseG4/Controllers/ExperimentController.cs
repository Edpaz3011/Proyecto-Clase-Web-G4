using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoClaseG4.DTOs;
using ProyectoClaseG4.Services;
using System.Security.Claims;

namespace ProyectoClaseG4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todo este controller requiere token válido
    public class ExperimentController : ControllerBase
    {
        private readonly ExperimentServices _experimentService;

        public ExperimentController(ExperimentServices experimentService)
        {
            _experimentService = experimentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExperimentDto dto)
        {
            try
            {
                // Sacamos el UserId del token JWT, no del body
                // Así el usuario no puede crear experimentos a nombre de otro
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var experiment = await _experimentService.Create(dto, userId);
                return Ok(experiment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyExperiments()
        {
            try
            {
                // Igual, el UserId viene del token no de la URL
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var experiments = await _experimentService.GetByUser(userId);
                return Ok(experiments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}