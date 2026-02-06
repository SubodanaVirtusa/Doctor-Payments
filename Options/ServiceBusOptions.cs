namespace Doctor_Payments_API.Options;

public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    public string ConnectionString { get; init; } = string.Empty;
    public string QueueName { get; init; } = "appointments";
}
