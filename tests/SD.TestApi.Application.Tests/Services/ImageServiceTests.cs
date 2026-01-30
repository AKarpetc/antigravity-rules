using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Application.Services;
using SD.TestApi.Domain.Entities;
using Xunit;

namespace SD.TestApi.Application.Tests.Services;

public class ImageServiceTests
{
    private readonly IImageRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly ImageService _sut;

    public ImageServiceTests()
    {
        _repository = Substitute.For<IImageRepository>();
        _cache = Substitute.For<IDistributedCache>();
        _sut = new ImageService(_repository, _cache);
    }

    [Fact]
    public async Task GetImageUrlAsync_ShouldReturnExactMatch_WhenScoreIsHigh()
    {
        // Arrange
        var images = new List<Image>
        {
            new() { Name = "Apple", Url = "http://apple.com", QuestionType = "Fruit" },
            new() { Name = "Banana", Url = "http://banana.com", QuestionType = "Fruit" }
        };
        MockCache(images);

        // Act
        var result = await _sut.GetImageUrlAsync("Apple", "Fruit", CancellationToken.None);

        // Assert
        result.Should().Be("http://apple.com");
    }

    [Fact]
    public async Task GetImageUrlAsync_ShouldReturnFuzzyMatch_WhenScoreIsAboveThreshold()
    {
        // Arrange
        var images = new List<Image>
        {
            new() { Name = "Strawberry", Url = "http://strawberry.com", QuestionType = "Fruit" },
            new() { Name = "Blueberry", Url = "http://blueberry.com", QuestionType = "Fruit" }
        };
        MockCache(images);

        // Act - "Strawbery" vs "Strawberry" should be > 80
        var result = await _sut.GetImageUrlAsync("Strawbery", "Fruit", CancellationToken.None);

        // Assert
        result.Should().Be("http://strawberry.com");
    }

    [Fact]
    public async Task GetImageUrlAsync_ShouldReturnDefault_WhenScoreIsLow()
    {
        // Arrange
        var images = new List<Image>
        {
            new() { Name = "Strawberry", Url = "http://strawberry.com", QuestionType = "Fruit" },
            new() { Name = "DefaultFruit", Url = "http://default.com", QuestionType = "Fruit", IsDefault = true }
        };
        MockCache(images);

        // Act - "Xyz" vs "Strawberry" should be low
        var result = await _sut.GetImageUrlAsync("Xyz", "Fruit", CancellationToken.None);

        // Assert
        result.Should().Be("http://default.com");
    }

    [Fact]
    public async Task GetImageUrlAsync_ShouldReturnRelatedNameMatch_WhenNameScoreIsLow()
    {
        // Arrange
        var images = new List<Image>
        {
            new() 
            { 
                Name = "Grapes", 
                Url = "http://grapes.com", 
                QuestionType = "Fruit", 
                RelatedNames = ["VineFruit", "WineBase"] 
            }
        };
        MockCache(images);

        // Act - "WineBase" vs "Grapes" is low, but vs RelatedNames is high
        var result = await _sut.GetImageUrlAsync("WineBase", "Fruit", CancellationToken.None);

        // Assert
        result.Should().Be("http://grapes.com");
    }

    private void MockCache(List<Image> images)
    {
        var json = JsonSerializer.Serialize(images);
        var bytes = Encoding.UTF8.GetBytes(json);
        _cache.GetAsync("Images.All", Arg.Any<CancellationToken>())
            .Returns(bytes); // GetAsync returns byte[]? No, IDistributedCache extension GetStringAsync uses GetAsync(byte[]) but mock setup for GetAsync works. 
            // Actually GetStringAsync calls GetAsync.
    }
}
