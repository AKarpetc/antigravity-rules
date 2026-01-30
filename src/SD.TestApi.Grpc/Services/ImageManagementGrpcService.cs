using MediatR;
using ProtoBuf.Grpc;
using SD.TestApi.Application.Features.Images.Commands;
using SD.TestApi.Grpc.Contracts;
using SD.TestApi.Grpc.Contracts.Models;

namespace SD.TestApi.Grpc.Services;

internal class ImageManagementGrpcService : IImageManagementService
{
    private readonly IMediator _mediator;

    public ImageManagementGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ImageResponse> AddImageAsync(AddImageRequest request, CallContext context = default)
    {
        var command = new CreateImageCommand(
            request.Name,
            request.Url,
            request.QuestionType,
            request.RelatedNames,
            request.IsDefault
        );

        var result = await _mediator.Send(command, context.CancellationToken);

        if (result.IsFailure)
        {
            return new ImageResponse { Success = false, Error = result.Error };
        }

        return new ImageResponse
        {
            Id = result.Value,
            Success = true,
            Name = request.Name,
            Url = request.Url,
            QuestionType = request.QuestionType,
            RelatedNames = request.RelatedNames,
            IsDefault = request.IsDefault
        };
    }

    public async Task<ImageResponse> UpdateImageAsync(UpdateImageRequest request, CallContext context = default)
    {
        var command = new UpdateImageCommand(
            request.Id,
            request.Name,
            request.Url,
            request.QuestionType,
            request.RelatedNames,
            request.IsDefault
        );

        var result = await _mediator.Send(command, context.CancellationToken);

        return new ImageResponse
        {
            Id = request.Id,
            Success = result.IsSuccess,
            Error = result.IsFailure ? result.Error : null,
            Name = request.Name,
            Url = request.Url,
            QuestionType = request.QuestionType,
            RelatedNames = request.RelatedNames,
            IsDefault = request.IsDefault
        };
    }

    public async Task<DeleteImageResponse> DeleteImageAsync(DeleteImageRequest request, CallContext context = default)
    {
        var command = new DeleteImageCommand(request.Id);
        var result = await _mediator.Send(command, context.CancellationToken);

        return new DeleteImageResponse
        {
            Success = result.IsSuccess,
            Error = result.IsFailure ? result.Error : null
        };
    }
}
