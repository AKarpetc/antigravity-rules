using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SD.TestApi.Domain.Entities;

namespace SD.TestApi.Infrastructure.Persistence;

public class SettingsDbContext : DbContext
{
    public SettingsDbContext(DbContextOptions<SettingsDbContext> options) : base(options) { }

    public DbSet<SettingsEntity> Settings { get; set; }
    public DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<SettingsEntity>().HasKey(x => x.Id);

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.QuestionType).IsRequired();
            
            // Map RelatedNames to jsonb
            entity.Property(e => e.RelatedNames)
                .HasColumnType("jsonb");
        });
    }
}

