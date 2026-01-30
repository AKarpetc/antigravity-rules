using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD.TestApi.Infrastructure.Persistence;

[Table("Settings")]
public class SettingsEntity
{
    [Key]
    public int Id { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string JsonContent { get; set; }
}
