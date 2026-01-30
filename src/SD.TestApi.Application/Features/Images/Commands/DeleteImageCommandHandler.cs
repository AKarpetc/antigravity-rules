using CSharpFunctionalExtensions;
using MediatR;
using SD.TestApi.Application.Features.Images.Notifications;
using SD.TestApi.Application.Interfaces;

namespace SD.TestApi.Application.Features.Images.Commands;

internal class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand, Result>
{
    private readonly IImageRepository _repository;
    private readonly IPublisher _publisher;

    public DeleteImageCommandHandler(IImageRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Result> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (image == null)
            return Result.Failure("Image.NotFound");

        await _repository.DeleteAsync(image, cancellationToken);

        await _publisher.Publish(new ImagesChangedNotification(), cancellationToken);

        return Result.Success();
    }
}
