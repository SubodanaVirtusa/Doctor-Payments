using Doctor_Payments_API.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Doctor_Payments_API.Infrastructure;

public sealed class CosmosInitializer : IHostedService
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;
    private readonly ILogger<CosmosInitializer> _logger;

    public CosmosInitializer(CosmosClient client, IOptions<CosmosDbOptions> options, ILogger<CosmosInitializer> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Initializing Cosmos DB database and container...");
            var database = await _client.CreateDatabaseIfNotExistsAsync(_options.DatabaseName, cancellationToken: cancellationToken);
            await database.Database.CreateContainerIfNotExistsAsync(
                id: _options.ContainerName,
                partitionKeyPath: "/doctorId",
                cancellationToken: cancellationToken);
            _logger.LogInformation("Cosmos DB initialization completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Cosmos DB. The application will continue without Cosmos DB initialization.");
            // Don't rethrow - allow the application to start even if Cosmos DB is not available
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
