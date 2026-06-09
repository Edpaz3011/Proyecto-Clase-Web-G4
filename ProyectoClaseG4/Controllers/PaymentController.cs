using Microsoft.AspNetCore.Mvc;
using ProyectoClaseG4.Models;

namespace ProyectoClaseG4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    [HttpPost("process")]
    public IActionResult ProcessPayment([FromBody] PaymentRecord payment)
    {
        if (string.IsNullOrWhiteSpace(payment.CardNumber))
        {
            return BadRequest("Número de tarjeta requerido.");
        }

        if (payment.CardNumber.Length != 16)
        {
            return BadRequest("La tarjeta debe tener 16 dígitos.");
        }

        if (payment.CVV.Length != 3)
        {
            return BadRequest("CVV inválido.");
        }

        payment.Status = "Success";
        payment.PaymentDate = DateTime.UtcNow;

        return Ok(new
        {
            message = "Pago procesado correctamente",
            payment.Status,
            payment.PaymentDate
        });
    }
}