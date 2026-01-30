using System.Text.Json;
using CSharpFunctionalExtensions;
using MediatR;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Domain.Common;
using SD.TestApi.Domain.Models;

namespace SD.TestApi.Application.Features.Settings.Commands.SaveSettings;

internal class SaveSettingsCommandHandler : IRequestHandler<SaveSettingsCommand, UnitResult<Error>>
{
    private readonly ISettingsRepository _settingsRepository;

    public SaveSettingsCommandHandler(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<UnitResult<Error>> Handle(SaveSettingsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var settings = JsonSerializer.Deserialize<SettingsModel>(request.SettingsJson);
            if (settings == null)
            {
                 return UnitResult.Failure(new Error("Settings.InvalidJson", "Could not deserialize settings."));
            }

            var saveResult = await _settingsRepository.SaveSettingsAsync(settings, cancellationToken);
            if (saveResult.IsFailure)
            {
                return UnitResult.Failure(saveResult.Error);
            }

            return UnitResult.Success<Error>();
        }
        catch (JsonException)
        {
            return UnitResult.Failure(new Error("Settings.InvalidJson", "Invalid JSON format."));
        }
        catch (Exception)
        {
            return UnitResult.Failure(new Error("Settings.SaveFailed", "Unknown error during save."));
        }
    }
}
