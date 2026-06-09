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

        if (!payment.CardNumber.All(char.IsDigit))
        {
            return BadRequest("La tarjeta solo debe contener números.");
        }

        if (payment.CVV.Length != 3)
        {
            return BadRequest("CVV inválido.");
        }

        if (!payment.CVV.All(char.IsDigit))
        {
            return BadRequest("El CVV solo debe contener números.");
        }

        if (payment.Amount <= 0)
        {
            return BadRequest("El monto debe ser mayor que cero.");
        }

        payment.Status = "Success";
        payment.PaymentDate = DateTime.UtcNow;

        var reference = $"PAY-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        return Ok(new
        {
            message = "Pago procesado correctamente",
            reference,
            payment.Status,
            payment.PaymentDate
        });
    }
}