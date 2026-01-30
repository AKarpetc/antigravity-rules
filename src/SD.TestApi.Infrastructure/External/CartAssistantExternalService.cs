using System.Text.Json;
using CSharpFunctionalExtensions;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Application.Models.External;
using SD.TestApi.Domain.Common;

namespace SD.TestApi.Infrastructure.External;

internal class CartAssistantExternalService : ICartAssistantExternalService
{
    public async Task<Result<ExternalDataResponse, Error>> GetOptionSetListAsync(ExternalDataRequest request, CancellationToken cancellationToken)
    {
        // Mock implementation: Load from docs/data-response.json
        // In real world, use GrpcClient.
        
        var current = Directory.GetCurrentDirectory();
        string[] candidates = 
        {
            Path.Combine(current, "docs", "data-response.json"),
            Path.Combine(current, "..", "..", "docs", "data-response.json"),
            Path.Combine(current, "..", "..", "..", "docs", "data-response.json"),
            "/Users/artyomkarpets/RiderProjects/TestApiSolution/docs/data-response.json"
        };
        
        foreach (var path in candidates)
        {
            if (File.Exists(path))
            {
                var content = await File.ReadAllTextAsync(path, cancellationToken);
                var data = JsonSerializer.Deserialize<ExternalDataResponse>(content);
                return data != null ? Result.Success<ExternalDataResponse, Error>(data) : Result.Failure<ExternalDataResponse, Error>(new Error("External.Invalid", "Invalid JSON"));
            }
        }

        // Fallback default
        return Result.Success<ExternalDataResponse, Error>(new ExternalDataResponse { OptionSetList = [] });
    }
}
