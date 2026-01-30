using ProtoBuf;

namespace SD.TestApi.Grpc.Contracts.Models;

[ProtoContract]
public class DeleteImageResponse
{
    [ProtoMember(1)]
    public bool Success { get; set; }
    
    [ProtoMember(2)]
    public string Error { get; set; }
}
