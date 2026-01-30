using CSharpFunctionalExtensions;
using FluentAssertions;
using SD.TestApi.Domain.Common;

namespace SD.TestApi.Domain.Tests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldReturnSuccessResult()
    {
        // Act
        var result = UnitResult.Success<Error>();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Failure_ShouldReturnFailureResult()
    {
        // Arrange
        var error = new Error("Code", "Message");

        // Act
        var result = UnitResult.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Create_WhenValueIsNotNull_ShouldReturnSuccess()
    {
        // Act
        var result = Result.Success<string, Error>("value");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("value");
    }

    [Fact]
    public void FailureT_ShouldReturnFailure()
    {
        // Arrange
        var error = new Error("Code", "Message");

        // Act
        var result = Result.Failure<string, Error>(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }
    
    [Fact]
    public void Value_WhenFailure_ShouldThrowException()
    {
         // Arrange
         var error = new Error("Code", "Message");
         var result = Result.Failure<string, Error>(error);
         
         // Act
         Action act = () => { var x = result.Value; };

         // Assert
         act.Should().Throw<ResultFailureException<Error>>();
    }
}
