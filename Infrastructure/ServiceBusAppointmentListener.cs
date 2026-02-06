using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Doctor_Payments_API.Models;
using Doctor_Payments_API.Options;
using Doctor_Payments_API.Services;
using Microsoft.Extensions.Options;

namespace Doctor_Payments_API.Infrastructure;

public sealed class ServiceBusAppointmentListener : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<ServiceBusAppointmentListener> _logger;

    public ServiceBusAppointmentListener(
        ServiceBusClient client,
        IOptions<ServiceBusOptions> options,
        IPaymentService paymentService,
        ILogger<ServiceBusAppointmentListener> logger)
    {
        _paymentService = paymentService;
        _logger = logger;

        _processor = client.CreateProcessor(options.Value.QueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 4
        });

        _processor.ProcessMessageAsync += HandleMessageAsync;
        _processor.ProcessErrorAsync += HandleErrorAsync;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => _processor.StartProcessingAsync(stoppingToken);

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        var payload = args.Message.Body.ToString();
        try
        {
            var appointment = JsonSerializer.Deserialize<AppointmentMessage>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (appointment is null || string.IsNullOrWhiteSpace(appointment.AppointmentId))
            {
                await args.DeadLetterMessageAsync(args.Message, "InvalidPayload", "Unable to deserialize appointment message.");
                return;
            }

            await _paymentService.HandleAppointmentAsync(appointment, args.CancellationToken);
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process appointment message.");
            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task HandleErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Service Bus processor error. Entity: {EntityPath}", args.EntityPath);
        return Task.CompletedTask;
    }
}
