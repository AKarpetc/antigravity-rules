using CSharpFunctionalExtensions;
using MediatR;
using SD.TestApi.Application.Features.Images.Notifications;
using SD.TestApi.Application.Interfaces;

namespace SD.TestApi.Application.Features.Images.Commands;

internal class UpdateImageCommandHandler : IRequestHandler<UpdateImageCommand, Result>
{
    private readonly IImageRepository _repository;
    private readonly IPublisher _publisher;

    public UpdateImageCommandHandler(IImageRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Result> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (image == null)
            return Result.Failure("Image.NotFound");

        image.Name = request.Name;
        image.Url = request.Url;
        image.QuestionType = request.QuestionType;
        image.RelatedNames = request.RelatedNames;
        image.IsDefault = request.IsDefault;

        await _repository.UpdateAsync(image, cancellationToken);

        await _publisher.Publish(new ImagesChangedNotification(), cancellationToken);

        return Result.Success();
    }
}
