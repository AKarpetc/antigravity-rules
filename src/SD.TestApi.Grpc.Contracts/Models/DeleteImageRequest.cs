using ProtoBuf;

namespace SD.TestApi.Grpc.Contracts.Models;

[ProtoContract]
public class DeleteImageRequest
{
    [ProtoMember(1)]
    public Guid Id { get; set; }
}
