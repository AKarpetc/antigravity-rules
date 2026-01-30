using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Domain.Entities;

namespace SD.TestApi.Infrastructure.Persistence;

internal class ImageRepository : IImageRepository
{
    private readonly SettingsDbContext _context;

    public ImageRepository(SettingsDbContext context)
    {
        _context = context;
    }

    public async Task<Image?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Images.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Image>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (!await _context.Images.AnyAsync(cancellationToken))
        {
            await SeedAsync(cancellationToken);
        }
        return await _context.Images.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Image image, CancellationToken cancellationToken)
    {
        _context.Images.Add(image);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Image image, CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Image image, CancellationToken cancellationToken)
    {
        _context.Images.Remove(image);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        var current = Directory.GetCurrentDirectory();
        string[] candidates = 
        {
            Path.Combine(current, "docs", "images.json"),
            Path.Combine(current, "..", "..", "docs", "images.json"),
            Path.Combine(current, "..", "..", "..", "docs", "images.json"),
            "/Users/artyomkarpets/RiderProjects/TestApiSolution/docs/images.json"
        };

        string? content = null;
        foreach (var path in candidates)
        {
            if (File.Exists(path))
            {
                content = await File.ReadAllTextAsync(path, cancellationToken);
                break;
            }
        }

        if (string.IsNullOrEmpty(content)) return;

        try 
        {
            var data = JsonSerializer.Deserialize<ImageFileModel>(content);
            if (data?.ImageList?.ImageList == null) return;

            var images = new List<Image>();
            foreach (var item in data.ImageList.ImageList)
            {
                images.Add(new Image
                {
                    Id = Guid.NewGuid(),
                    Name = item.Name,
                    Url = item.Url,
                    QuestionType = item.QuestionType,
                    RelatedNames = item.RelatedNames ?? new List<string>(),
                    IsDefault = item.IsDefault
                });
            }

            if (!string.IsNullOrEmpty(data.ImageList.DefaultImageUrl))
            {
                // Add Global Default
                images.Add(new Image
                {
                    Id = Guid.NewGuid(),
                    Name = "GlobalDefault",
                    Url = data.ImageList.DefaultImageUrl,
                    QuestionType = "Global",
                    IsDefault = true,
                    RelatedNames = new List<string>()
                });
            }

            _context.Images.AddRange(images);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Ignore seeding errors
        }
    }

    // Private models for seeding
    private class ImageFileModel { public ImageListContainer ImageList { get; set; } }
    private class ImageListContainer { public List<ImageItem> ImageList { get; set; } public string DefaultImageUrl { get; set; } }
    private class ImageItem { public List<string> RelatedNames { get; set; } public string QuestionType { get; set; } public string Url { get; set; } public string Name { get; set; } public bool IsDefault { get; set; } }
}
