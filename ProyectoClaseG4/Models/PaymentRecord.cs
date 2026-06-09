namespace ProyectoClaseG4.Models;

public class PaymentRecord
{
    public string ReservationId { get; set; } = string.Empty;
    public string CardHolder { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
}