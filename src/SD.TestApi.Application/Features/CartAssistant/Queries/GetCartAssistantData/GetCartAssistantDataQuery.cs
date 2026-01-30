using CSharpFunctionalExtensions;
using MediatR;
using SD.TestApi.Domain.Common;
using SD.TestApi.Domain.Models;

namespace SD.TestApi.Application.Features.CartAssistant.Queries.GetCartAssistantData;

public record GetCartAssistantDataQuery(int StoreId) : IRequest<Result<CartAssistantModel, Error>>;
