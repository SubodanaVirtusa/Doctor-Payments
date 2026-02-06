using Doctor_Payments_API.Models;

namespace Doctor_Payments_API.Services;

public interface IPaymentService
{
    Task<PaymentRecord> HandleAppointmentAsync(AppointmentMessage appointment, CancellationToken cancellationToken);
    Task<IReadOnlyList<PaymentRecord>> GetPaymentsAsync(CancellationToken cancellationToken);
    Task<PaymentRecord?> GetPaymentAsync(string id, string doctorId, CancellationToken cancellationToken);
}
