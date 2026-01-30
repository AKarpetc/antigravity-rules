using CSharpFunctionalExtensions;
using MediatR;
using SD.TestApi.Application.Features.Images.Notifications;
using SD.TestApi.Domain.Entities;
using SD.TestApi.Application.Interfaces;

namespace SD.TestApi.Application.Features.Images.Commands;

internal class CreateImageCommandHandler : IRequestHandler<CreateImageCommand, Result<Guid>>
{
    private readonly IImageRepository _repository;
    private readonly IPublisher _publisher;

    public CreateImageCommandHandler(IImageRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Result<Guid>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
    {
        var image = new Image
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Url = request.Url,
            QuestionType = request.QuestionType,
            RelatedNames = request.RelatedNames,
            IsDefault = request.IsDefault
        };

        await _repository.AddAsync(image, cancellationToken);
        await _publisher.Publish(new ImagesChangedNotification(), cancellationToken);

        return Result.Success(image.Id);
    }
}
