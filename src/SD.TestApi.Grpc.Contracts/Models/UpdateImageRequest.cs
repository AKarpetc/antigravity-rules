using ProtoBuf;

namespace SD.TestApi.Grpc.Contracts.Models;

[ProtoContract]
public class UpdateImageRequest
{
    [ProtoMember(1)]
    public Guid Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; }
    
    [ProtoMember(3)]
    public string Url { get; set; }
    
    [ProtoMember(4)]
    public string QuestionType { get; set; }
    
    [ProtoMember(5)]
    public List<string> RelatedNames { get; set; } = new();
    
    [ProtoMember(6)]
    public bool IsDefault { get; set; }
}
