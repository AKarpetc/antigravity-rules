using CSharpFunctionalExtensions;
using FluentAssertions;
using NSubstitute;
using SD.TestApi.Application.Features.Settings.Commands.SaveSettings;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Domain.Common;
using SD.TestApi.Domain.Models;

namespace SD.TestApi.Application.Tests.Features.Settings.Commands;

public class SaveSettingsCommandHandlerTests
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly SaveSettingsCommandHandler _sut;

    public SaveSettingsCommandHandlerTests()
    {
        _settingsRepository = Substitute.For<ISettingsRepository>();
        _sut = new SaveSettingsCommandHandler(_settingsRepository);
    }

    [Fact]
    public async Task Handle_WhenJsonIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var command = new SaveSettingsCommand("invalid json");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Settings.InvalidJson");
    }

    [Fact]
    public async Task Handle_WhenRepositoryFails_ShouldReturnFailure()
    {
        // Arrange
        var command = new SaveSettingsCommand("{}");
        _settingsRepository.SaveSettingsAsync(Arg.Any<SettingsModel>(), Arg.Any<CancellationToken>())
            .Returns(UnitResult.Failure(new Error("Repo.Error", "Error")));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Repo.Error");
    }

    [Fact]
    public async Task Handle_WhenSuccess_ShouldReturnSuccess()
    {
        // Arrange
        var command = new SaveSettingsCommand("{\"ProductCount\": 5}");
        _settingsRepository.SaveSettingsAsync(Arg.Any<SettingsModel>(), Arg.Any<CancellationToken>())
            .Returns(UnitResult.Success<Error>());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _settingsRepository.Received(1).SaveSettingsAsync(Arg.Is<SettingsModel>(s => s.ProductCount == 5), Arg.Any<CancellationToken>());
    }
}
