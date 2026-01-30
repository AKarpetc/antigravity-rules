using CSharpFunctionalExtensions;
using SD.TestApi.Application.Models.External;
using SD.TestApi.Domain.Common;

namespace SD.TestApi.Application.Interfaces;

public interface ICartAssistantExternalService
{
    Task<Result<ExternalDataResponse, Error>> GetOptionSetListAsync(ExternalDataRequest request, CancellationToken cancellationToken);
}
