using MediatR;
using CSharpFunctionalExtensions;

namespace SD.TestApi.Application.Features.Images.Commands;

public record DeleteImageCommand(Guid Id) : IRequest<Result>;
