using Doctor_Payments_API.Models;
using Doctor_Payments_API.Repositories;

namespace Doctor_Payments_API.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;

    public PaymentService(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<PaymentRecord>> GetPaymentsAsync(CancellationToken cancellationToken)
        => _repository.GetAllAsync(cancellationToken);

    public Task<PaymentRecord?> GetPaymentAsync(string id, string doctorId, CancellationToken cancellationToken)
        => _repository.GetByIdAsync(id, doctorId, cancellationToken);

    public Task<PaymentRecord> HandleAppointmentAsync(AppointmentMessage appointment, CancellationToken cancellationToken)
    {
        var record = new PaymentRecord
        {
            AppointmentId = appointment.AppointmentId,
            DoctorId = appointment.DoctorId,
            PatientName = appointment.PatientName,
            Amount = appointment.Amount,
            PaymentStatus = "Pending"
        };

        return _repository.UpsertAsync(record, cancellationToken);
    }
}
