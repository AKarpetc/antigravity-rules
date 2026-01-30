using CSharpFunctionalExtensions;
using FluentAssertions;
using NSubstitute;
using SD.TestApi.Application.Features.CartAssistant.Queries.GetCartAssistantData;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Application.Models.External;
using SD.TestApi.Domain.Common;
using SD.TestApi.Domain.Models;

namespace SD.TestApi.Application.Tests.Features.CartAssistant.Queries;

public class GetCartAssistantDataQueryHandlerTests
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICartAssistantExternalService _externalService;
    private readonly IImageService _imageService;
    private readonly GetCartAssistantDataQueryHandler _sut;

    public GetCartAssistantDataQueryHandlerTests()
    {
        _settingsRepository = Substitute.For<ISettingsRepository>();
        _externalService = Substitute.For<ICartAssistantExternalService>();
        _imageService = Substitute.For<IImageService>();
        _sut = new GetCartAssistantDataQueryHandler(_settingsRepository, _externalService, _imageService);
    }

    [Fact]
    public async Task Handle_WhenSettingsMissing_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetCartAssistantDataQuery(1);
        _settingsRepository.GetSettingsAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Failure<SettingsModel, Error>(new Error("Settings.NotFound", "Not Found")));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Settings.NotFound");
    }

    [Fact]
    public async Task Handle_WhenExternalServiceFails_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetCartAssistantDataQuery(1);
        var settings = new SettingsModel { Questions = [] };
        
        _settingsRepository.GetSettingsAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success<SettingsModel, Error>(settings));

        _externalService.GetOptionSetListAsync(Arg.Any<ExternalDataRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<ExternalDataResponse, Error>(new Error("External.Failed", "Failed")));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("External.Failed");
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ShouldReturnData()
    {
        // Arrange
        var query = new GetCartAssistantDataQuery(1);
        var settings = new SettingsModel
        {
            Questions = 
            [
                new QuestionSettings { Type = "Category", MaxCount = 5, ElementType = "MultipleChoice" }
            ],
            ResultTitle = new LocalizedString { Default = "Result" }
        };
        
        var externalData = new ExternalDataResponse
        {
             OptionSetList =
             [
                 new OptionSet
                 {
                     SelectableQuestionOptions =
                     [
                         new SelectableQuestionOption
                         {
                             QuestionType = "Category",
                             Options = 
                             [
                                 new OptionItem { Id = 1, Order = 1, Titles = [ new TitleItem { Type = "Basic", Title = "Edibles" } ] }
                             ]
                         }
                     ]
                 }
             ]
        };

        _settingsRepository.GetSettingsAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success<SettingsModel, Error>(settings));

        _externalService.GetOptionSetListAsync(Arg.Any<ExternalDataRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<ExternalDataResponse, Error>(externalData));
            
        _imageService.GetImageUrlAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("http://image.url");

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ResultTitle.Should().Be("Result");
        result.Value.Questions.Should().HaveCount(1);
        result.Value.Questions[0].Answers.Should().HaveCount(1);
        result.Value.Questions[0].Answers[0].Title.Should().Be("Edibles");
    }
}
