using System.ServiceModel;
using ProtoBuf.Grpc;
using SD.TestApi.Grpc.Contracts.Models;

namespace SD.TestApi.Grpc.Contracts;

[ServiceContract]
public interface IImageManagementService
{
    [OperationContract]
    Task<ImageResponse> AddImageAsync(AddImageRequest request, CallContext context = default);
    
    [OperationContract]
    Task<ImageResponse> UpdateImageAsync(UpdateImageRequest request, CallContext context = default);
    
    [OperationContract]
    Task<DeleteImageResponse> DeleteImageAsync(DeleteImageRequest request, CallContext context = default);
}
