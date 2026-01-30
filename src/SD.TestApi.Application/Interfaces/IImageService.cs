namespace SD.TestApi.Application.Interfaces;

public interface IImageService
{
    Task<string> GetImageUrlAsync(string term, string questionType, CancellationToken cancellationToken);
}
