using CSharpFunctionalExtensions;
using MediatR;
using ProtoBuf.Grpc;
using SD.TestApi.Application.Features.CartAssistant.Queries.GetCartAssistantData;
using SD.TestApi.Application.Features.Settings.Commands.SaveSettings;
using SD.TestApi.Grpc.Contracts;
using DomainModel = SD.TestApi.Domain.Models;

namespace SD.TestApi.Grpc.Services;

internal class TestApiService : ICartAssistantService
{
    private readonly IMediator _mediator;

    public TestApiService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<GetCartAssistantDataResponse> GetCartAssistantDataAsync(GetCartAssistantDataRequest request, CallContext context = default)
    {
        var query = new GetCartAssistantDataQuery(request.StoreId);
        var result = await _mediator.Send(query, context.CancellationToken);

        if (result.IsFailure)
        {
            return new GetCartAssistantDataResponse { Error = result.Error.Message };
        }

        return new GetCartAssistantDataResponse { CartAssistant = MapToContract(result.Value) };
    }

    public async Task<SaveSettingsResponse> SaveSettingsAsync(SaveSettingsRequest request, CallContext context = default)
    {
        var command = new SaveSettingsCommand(request.SettingsJson);
        var result = await _mediator.Send(command, context.CancellationToken);

        return new SaveSettingsResponse { Success = result.IsSuccess };
    }

    private static CartAssistantModel MapToContract(DomainModel.CartAssistantModel model)
    {
        return new CartAssistantModel
        {
            Total = model.Total,
            ResultTitle = model.ResultTitle,
            StartOver = model.StartOver,
            Tooltip = model.Tooltip,
            NotFoundMessage = model.NotFoundMessage,
            IconLabel = model.IconLabel,
            Questions = model.Questions.Select(q => new QuestionModel
            {
                Step = q.Step,
                Type = q.Type,
                AnswerLimit = q.AnswerLimit,
                ElementType = q.ElementType,
                Title = q.Title,
                Subtitle = q.Subtitle,
                IsOptional = q.IsOptional,
                MaxCount = q.MaxCount,
                MinCount = q.MinCount,
                MinText = q.MinText != null ? new MinMaxTextModel { Default = q.MinText.Default, Short = q.MinText.Short } : null,
                MaxText = q.MaxText != null ? new MinMaxTextModel { Default = q.MaxText.Default, Short = q.MaxText.Short } : null,
                Buttons = q.Buttons != null ? new ButtonsModel
                {
                    Back = new ButtonStateModel { Title = q.Buttons.Back?.Title, Visible = q.Buttons.Back?.Visible ?? false },
                    Next = new ButtonStateModel { Title = q.Buttons.Next?.Title, Visible = q.Buttons.Next?.Visible ?? false },
                    Skip = new ButtonStateModel { Title = q.Buttons.Skip?.Title, Visible = q.Buttons.Skip?.Visible ?? false }
                } : null,
                Answers = q.Answers.Select(a => new AnswerModel
                {
                    Id = a.Id,
                    Order = a.Order,
                    Title = a.Title,
                    Image = a.Image,
                    Value = a.Value
                }).ToList()
            }).ToList()
        };
    }
}
