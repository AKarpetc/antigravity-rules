using CSharpFunctionalExtensions;
using MediatR;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Application.Models.External;
using SD.TestApi.Domain.Common;
using SD.TestApi.Domain.Models;

namespace SD.TestApi.Application.Features.CartAssistant.Queries.GetCartAssistantData;

internal class GetCartAssistantDataQueryHandler : IRequestHandler<GetCartAssistantDataQuery, Result<CartAssistantModel, Error>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICartAssistantExternalService _externalService;
    private readonly IImageService _imageService;

    public GetCartAssistantDataQueryHandler(
        ISettingsRepository settingsRepository,
        ICartAssistantExternalService externalService,
        IImageService imageService)
    {
        _settingsRepository = settingsRepository;
        _externalService = externalService;
        _imageService = imageService;
    }

    public async Task<Result<CartAssistantModel, Error>> Handle(GetCartAssistantDataQuery request, CancellationToken cancellationToken)
    {
         // 1. Get Settings
         var settingsResult = await _settingsRepository.GetSettingsAsync(cancellationToken);
         if (settingsResult.IsFailure)
         {
             return Result.Failure<CartAssistantModel, Error>(settingsResult.Error);
         }

         var settings = settingsResult.Value;

         // 2. Prepare External Request
         List<OptionLimit> limits = [];
         foreach (var q in settings.Questions)
         {
             if (q.MaxCount > 0 && 
                 (q.Type == "Category" || q.Type == "Effect" || q.Type == "Flavor" || q.Type == "Strength"))
             {
                 limits.Add(new OptionLimit
                 {
                     QuestionType = q.Type,
                     MaxCount = q.MaxCount
                 });
             }
         }

         var externalReq = new ExternalDataRequest
         {
             StoreId = request.StoreId,
             OptionLimits = limits
         };

         // 3. Call External Service
         var externalDataResult = await _externalService.GetOptionSetListAsync(externalReq, cancellationToken);
         if (externalDataResult.IsFailure)
         {
             return Result.Failure<CartAssistantModel, Error>(externalDataResult.Error);
         }
         var externalData = externalDataResult.Value;

         // 4. Merge
         var result = new CartAssistantModel
         {
             Total = settings.ProductCount,
             ResultTitle = settings.ResultTitle?.Default,
             StartOver = settings.StartOver?.Default,
             Tooltip = settings.Tooltip?.Default,
             NotFoundMessage = settings.NotFoundMessage?.Default,
             IconLabel = settings.IconLabel?.Default,
             Questions = []
         };

         foreach (var qSetting in settings.Questions)
         {
             var qModel = new QuestionModel
             {
                 Step = qSetting.Step,
                 Type = qSetting.Type,
                 AnswerLimit = qSetting.AnswerLimit,
                 ElementType = qSetting.ElementType,
                 Title = qSetting.Title?.Default,
                 Subtitle = qSetting.Subtitle?.Default,
                 IsOptional = qSetting.IsOptional,
                 Buttons = new ButtonsModel
                 {
                     Back = new ButtonStateModel { Title = qSetting.Buttons?.Back?.Title?.Default, Visible = qSetting.Buttons?.Back?.Visible ?? false },
                     Next = new ButtonStateModel { Title = qSetting.Buttons?.Next?.Title?.Default, Visible = qSetting.Buttons?.Next?.Visible ?? false },
                     Skip = new ButtonStateModel { Title = qSetting.Buttons?.Skip?.Title?.Default, Visible = qSetting.Buttons?.Skip?.Visible ?? false }
                 },
                 MaxCount = qSetting.MaxCount,
                 MinCount = qSetting.MinCount,
                 MinText = new MinMaxTextModel { Default = qSetting.MinText?.Default },
                 MaxText = new MinMaxTextModel { Default = qSetting.MaxText?.Default },
                 Answers = []
             };

             // Range / Strength Logic
             if (qSetting.ElementType == "Range" || qSetting.Type == "Strength")
             {
                 for (int i = 0; i < qSetting.MaxCount; i++)
                 {
                     var title = "";
                     if (i == 0) title = qSetting.MinText?.Default ?? "";
                     else if (i == qSetting.MaxCount - 1) title = qSetting.MaxText?.Default ?? "";

                     qModel.Answers.Add(new AnswerModel
                     {
                         Id = (i + 1) * 20, 
                         Order = i,
                         Title = title,
                         Value = i.ToString(),
                         Image = ""
                     });
                 }
             }
             else
             {
                 // Find Options from External Data
                 var extOptionsContainer = externalData.OptionSetList?.FirstOrDefault()?.SelectableQuestionOptions
                     ?.FirstOrDefault(x => x.QuestionType == qSetting.Type);

                 if (extOptionsContainer != null)
                 {
                     foreach (var opt in extOptionsContainer.Options.OrderBy(x => x.Order))
                     {
                         // Get Title: Type: "Basic" preferred.
                         var titleItem = opt.Titles.FirstOrDefault(t => t.Type == "Basic") ?? opt.Titles.FirstOrDefault();
                         var title = titleItem?.Title ?? "";

                         // Get Image
                         var imageUrl = await _imageService.GetImageUrlAsync(title, qSetting.Type, cancellationToken);
                         
                         qModel.Answers.Add(new AnswerModel
                         {
                             Id = opt.Id,
                             Order = opt.Order,
                             Title = title,
                             Value = "",
                             Image = imageUrl
                         });
                     }
                 }
             }
             
             result.Questions.Add(qModel);
         }

         result.Total = result.Questions.Count;

         return Result.Success<CartAssistantModel, Error>(result);
    }
}
