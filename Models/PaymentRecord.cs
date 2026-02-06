using System.Text.Json.Serialization;

namespace Doctor_Payments_API.Models;

public sealed class PaymentRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string AppointmentId { get; set; } = string.Empty;
    public string DoctorId { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
