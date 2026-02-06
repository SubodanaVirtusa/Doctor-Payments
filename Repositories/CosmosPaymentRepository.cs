using Doctor_Payments_API.Models;
using Doctor_Payments_API.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Doctor_Payments_API.Repositories;

public sealed class CosmosPaymentRepository : IPaymentRepository
{
    private readonly Container _container;

    public CosmosPaymentRepository(CosmosClient client, IOptions<CosmosDbOptions> options)
    {
        var cosmos = options.Value;
        _container = client.GetContainer(cosmos.DatabaseName, cosmos.ContainerName);
    }

    public async Task<PaymentRecord> UpsertAsync(PaymentRecord record, CancellationToken cancellationToken)
    {
        var response = await _container.UpsertItemAsync(record, new PartitionKey(record.DoctorId), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<PaymentRecord?> GetByIdAsync(string id, string doctorId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _container.ReadItemAsync<PaymentRecord>(id, new PartitionKey(doctorId), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<PaymentRecord>> GetAllAsync(CancellationToken cancellationToken)
    {
        var query = _container.GetItemQueryIterator<PaymentRecord>(
            new QueryDefinition("SELECT * FROM c ORDER BY c.createdAtUtc DESC"));

        var results = new List<PaymentRecord>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync(cancellationToken);
            results.AddRange(response.Resource);
        }

        return results;
    }
}
