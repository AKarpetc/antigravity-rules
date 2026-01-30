using ProtoBuf;

namespace SD.TestApi.Grpc.Contracts.Models;

[ProtoContract]
public class AddImageRequest
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]
    public string Url { get; set; }
    
    [ProtoMember(3)]
    public string QuestionType { get; set; }
    
    [ProtoMember(4)]
    public List<string> RelatedNames { get; set; } = new();
    
    [ProtoMember(5)]
    public bool IsDefault { get; set; }
}
