using System.Text.Json;
using CSharpFunctionalExtensions;
using FuzzySharp;
using Microsoft.Extensions.Caching.Distributed;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Domain.Entities;

namespace SD.TestApi.Application.Services;

internal class ImageService : IImageService
{
    private readonly IImageRepository _repository;
    private readonly IDistributedCache _cache;
    private const string CacheKey = "Images.All";
    
    // Configurable threshold
    private const int MatchThreshold = 80;

    public ImageService(IImageRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    private async Task<List<Image>> GetImagesAsync(CancellationToken cancellationToken)
    {
        var cachedData = await _cache.GetStringAsync(CacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            try 
            {
                var images = JsonSerializer.Deserialize<List<Image>>(cachedData);
                if (images != null) return images;
            }
            catch
            {
                // Ignore deserialization error, reload from DB
            }
        }

        var dbImages = await _repository.GetAllAsync(cancellationToken);
        
        await _cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(dbImages), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        }, cancellationToken);

        return dbImages;
    }

    public async Task<string> GetImageUrlAsync(string term, string questionType, CancellationToken cancellationToken)
    {
        var allImages = await GetImagesAsync(cancellationToken);
        
        // Filter by QuestionType
        var candidates = allImages
            .Where(x => string.Equals(x.QuestionType, questionType, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (candidates.Count == 0) return string.Empty;

        // Find exact match first (Optimization)
        var exactMatch = candidates.FirstOrDefault(x => string.Equals(x.Name, term, StringComparison.OrdinalIgnoreCase));
        if (exactMatch != null) return exactMatch.Url;

        // Fuzzy Match
        string? bestUrl = null;
        int bestScore = 0;

        foreach (var item in candidates)
        {
            // Check Name
            var scoreName = string.IsNullOrEmpty(item.Name) ? 0 : Fuzz.Ratio(term, item.Name);
            
            // Check RelatedNames
            int scoreRelated = 0;
            if (item.RelatedNames != null && item.RelatedNames.Count != 0)
            {
                var bestRelated = Process.ExtractOne(term, item.RelatedNames);
                if (bestRelated != null)
                {
                    scoreRelated = bestRelated.Score;
                }
            }

            var maxScore = Math.Max(scoreName, scoreRelated);
            if (maxScore > bestScore)
            {
                bestScore = maxScore;
                bestUrl = item.Url;
            }
        }

        if (bestScore >= MatchThreshold)
        {
            return bestUrl ?? string.Empty;
        }

        // Fallback to Default
        var defaultImage = candidates.FirstOrDefault(x => x.IsDefault);
        return defaultImage?.Url ?? string.Empty;
    }
}
