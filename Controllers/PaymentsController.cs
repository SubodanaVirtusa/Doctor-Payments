using Doctor_Payments_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Doctor_Payments_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var payments = await _paymentService.GetPaymentsAsync(cancellationToken);
        return Ok(payments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, [FromQuery] string doctorId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(doctorId))
        {
            return BadRequest("doctorId is required as a query parameter.");
        }

        var payment = await _paymentService.GetPaymentAsync(id, doctorId, cancellationToken);
        return payment is null ? NotFound() : Ok(payment);
    }
}
