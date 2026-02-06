namespace Doctor_Payments_API.Models;

public sealed class AppointmentMessage
{
    public string AppointmentId { get; init; } = string.Empty;
    public string DoctorId { get; init; } = string.Empty;
    public string PatientName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
