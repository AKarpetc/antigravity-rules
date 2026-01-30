using CSharpFunctionalExtensions;
using SD.TestApi.Domain.Common;
using SD.TestApi.Domain.Models;

namespace SD.TestApi.Application.Interfaces;

public interface ISettingsRepository
{
    Task<Result<SettingsModel, Error>> GetSettingsAsync(CancellationToken cancellationToken);
    Task<UnitResult<Error>> SaveSettingsAsync(SettingsModel settings, CancellationToken cancellationToken);
}
