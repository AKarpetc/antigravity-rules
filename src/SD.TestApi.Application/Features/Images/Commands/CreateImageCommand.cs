using MediatR;
using CSharpFunctionalExtensions;

namespace SD.TestApi.Application.Features.Images.Commands;

public record CreateImageCommand(
    string Name,
    string Url,
    string QuestionType,
    List<string> RelatedNames,
    bool IsDefault
) : IRequest<Result<Guid>>;
