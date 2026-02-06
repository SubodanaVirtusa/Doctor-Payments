using Doctor_Payments_API.Models;

namespace Doctor_Payments_API.Repositories;

public interface IPaymentRepository
{
    Task<PaymentRecord> UpsertAsync(PaymentRecord record, CancellationToken cancellationToken);
    Task<PaymentRecord?> GetByIdAsync(string id, string doctorId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PaymentRecord>> GetAllAsync(CancellationToken cancellationToken);
}
