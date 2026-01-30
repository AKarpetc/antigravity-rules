using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Application.Models;
using SD.TestApi.Domain.Common;
using SD.TestApi.Domain.Models;

namespace SD.TestApi.Infrastructure.Persistence;

internal class SettingsRepository : ISettingsRepository
{
    private readonly SettingsDbContext _context;

    public SettingsRepository(SettingsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SettingsModel, Error>> GetSettingsAsync(CancellationToken cancellationToken)
    {
        try 
        {
            var entity = await _context.Settings.FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
            {
                // Seed
                var seeded = await SeedFromFilesAsync(cancellationToken);
                if (seeded != null)
                {
                    _context.Settings.Add(new SettingsEntity { Id = 1, JsonContent = JsonSerializer.Serialize(seeded) });
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success<SettingsModel, Error>(seeded);
                }
                return Result.Failure<SettingsModel, Error>(new Error("Settings.NotFound", "Settings not found and seeding failed."));
            }

            var model = JsonSerializer.Deserialize<SettingsModel>(entity.JsonContent);
            return model != null ? Result.Success<SettingsModel, Error>(model) : Result.Failure<SettingsModel, Error>(new Error("Settings.Invalid", "Invalid JSON settings"));
        }
        catch (Exception ex)
        {
             return Result.Failure<SettingsModel, Error>(new Error("Db.Error", ex.Message));
        }
    }

    public async Task<UnitResult<Error>> SaveSettingsAsync(SettingsModel settings, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _context.Settings.FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
            {
                entity = new SettingsEntity { Id = 1 };
                _context.Settings.Add(entity);
            }

            entity.JsonContent = JsonSerializer.Serialize(settings);
            await _context.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            return UnitResult.Failure(new Error("Db.Error", ex.Message));
        }
    }

    private async Task<SettingsModel> SeedFromFilesAsync(CancellationToken cancellationToken)
    {
        // Try to locate docs/settings.json
        var current = Directory.GetCurrentDirectory();
        string[] candidates = 
        {
            Path.Combine(current, "docs", "settings.json"),
            Path.Combine(current, "..", "..", "docs", "settings.json"),
            Path.Combine(current, "..", "..", "..", "docs", "settings.json"),
            "/Users/artyomkarpets/RiderProjects/TestApiSolution/docs/settings.json"
        };

        foreach (var path in candidates)
        {
            if (File.Exists(path))
            {
                var content = await File.ReadAllTextAsync(path, cancellationToken);
                return JsonSerializer.Deserialize<SettingsModel>(content);
            }
        }
        return null;
    }
}
