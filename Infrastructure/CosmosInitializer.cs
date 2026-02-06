using Doctor_Payments_API.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Doctor_Payments_API.Infrastructure;

public sealed class CosmosInitializer : IHostedService
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;

    public CosmosInitializer(CosmosClient client, IOptions<CosmosDbOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var database = await _client.CreateDatabaseIfNotExistsAsync(_options.DatabaseName, cancellationToken: cancellationToken);
        await database.Database.CreateContainerIfNotExistsAsync(
            id: _options.ContainerName,
            partitionKeyPath: "/doctorId",
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
