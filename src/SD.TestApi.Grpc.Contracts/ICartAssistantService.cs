using System.ServiceModel;
using System.Runtime.Serialization;
using ProtoBuf;
using ProtoBuf.Grpc;

namespace SD.TestApi.Grpc.Contracts;

[ServiceContract]
public interface ICartAssistantService
{
    [OperationContract]
    Task<GetCartAssistantDataResponse> GetCartAssistantDataAsync(GetCartAssistantDataRequest request, CallContext context = default);
    
    [OperationContract]
    Task<SaveSettingsResponse> SaveSettingsAsync(SaveSettingsRequest request, CallContext context = default);
}

[ProtoContract]
public class GetCartAssistantDataRequest
{
    [ProtoMember(1)]
    public int StoreId { get; set; }
    [ProtoMember(2)]
    public string SaleType { get; set; }
    [ProtoMember(3)]
    public string StockType { get; set; }
    [ProtoMember(4)]
    public string CensorshipType { get; set; }
    [ProtoMember(5)]
    public bool IsShortTitles { get; set; }
}

[ProtoContract]
public class GetCartAssistantDataResponse
{
    [ProtoMember(1)]
    public CartAssistantModel CartAssistant { get; set; }
    [ProtoMember(2)]
    public string Error { get; set; }
}

[ProtoContract]
public class CartAssistantModel
{
    [ProtoMember(1)]
    public List<QuestionModel> Questions { get; set; }
    [ProtoMember(2)]
    public int Total { get; set; }
    [ProtoMember(3)]
    public string ResultTitle { get; set; }
    [ProtoMember(4)]
    public string StartOver { get; set; }
    [ProtoMember(5)]
    public string Tooltip { get; set; }
    [ProtoMember(6)]
    public string NotFoundMessage { get; set; }
    [ProtoMember(7)]
    public string IconLabel { get; set; }
}

[ProtoContract]
public class QuestionModel
{
    [ProtoMember(1)]
    public List<AnswerModel> Answers { get; set; }
    [ProtoMember(2)]
    public int Step { get; set; }
    [ProtoMember(3)]
    public string Type { get; set; }
    [ProtoMember(4)]
    public int AnswerLimit { get; set; }
    [ProtoMember(5)]
    public string ElementType { get; set; }
    [ProtoMember(6)]
    public string Title { get; set; }
    [ProtoMember(7)]
    public string Subtitle { get; set; }
    [ProtoMember(8)]
    public ButtonsModel Buttons { get; set; }
    [ProtoMember(9)]
    public bool IsOptional { get; set; }
    // Add Range fields as they appear in response.json
    [ProtoMember(10)]
    public int MaxCount { get; set; }
    [ProtoMember(11)]
    public int MinCount { get; set; }
    [ProtoMember(12)]
    public MinMaxTextModel MinText { get; set; }
    [ProtoMember(13)]
    public MinMaxTextModel MaxText { get; set; }
}

[ProtoContract]
public class MinMaxTextModel
{
     [ProtoMember(1)]
     public string Default { get; set; }
     [ProtoMember(2)]
     public string Short { get; set; }
}


[ProtoContract]
public class AnswerModel
{
    [ProtoMember(1)]
    public int Id { get; set; }
    [ProtoMember(2)]
    public int Order { get; set; }
    [ProtoMember(3)]
    public string Title { get; set; }
    [ProtoMember(4)]
    public string Image { get; set; }
    [ProtoMember(5)]
    public string Value { get; set; }
}

[ProtoContract]
public class ButtonsModel
{
    [ProtoMember(1)]
    public ButtonStateModel Back { get; set; }
    [ProtoMember(2)]
    public ButtonStateModel Next { get; set; }
    [ProtoMember(3)]
    public ButtonStateModel Skip { get; set; }
}

[ProtoContract]
public class ButtonStateModel
{
    [ProtoMember(1)]
    public string Title { get; set; }
    [ProtoMember(2)]
    public bool Visible { get; set; }
}

[ProtoContract]
public class SaveSettingsRequest
{
    [ProtoMember(1)] 
    public string SettingsJson { get; set; } // Simplified for now
}

[ProtoContract]
public class SaveSettingsResponse
{
    [ProtoMember(1)]
    public bool Success { get; set; }
}
