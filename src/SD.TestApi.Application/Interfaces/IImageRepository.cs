using SD.TestApi.Domain.Entities;

namespace SD.TestApi.Application.Interfaces;

public interface IImageRepository
{
    Task<Image?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Image>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Image image, CancellationToken cancellationToken);
    Task UpdateAsync(Image image, CancellationToken cancellationToken);
    Task DeleteAsync(Image image, CancellationToken cancellationToken);
}
