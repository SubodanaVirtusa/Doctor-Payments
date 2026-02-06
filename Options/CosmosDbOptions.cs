namespace Doctor_Payments_API.Options;

public sealed class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";

    public string ConnectionString { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = "doctor-payments";
    public string ContainerName { get; init; } = "payments";
}
