using CSharpFunctionalExtensions;
using MediatR;
using SD.TestApi.Domain.Common;

namespace SD.TestApi.Application.Features.Settings.Commands.SaveSettings;

public record SaveSettingsCommand(string SettingsJson) : IRequest<UnitResult<Error>>;
