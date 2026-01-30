namespace SD.TestApi.Domain.Models;

public class SettingsModel
{
    public List<QuestionSettings> Questions { get; set; } = [];
    public LocalizedString StartOver { get; set; }
    public LocalizedString Tooltip { get; set; }
    public int ProductCount { get; set; }
    public LocalizedString ResultTitle { get; set; }
    public LocalizedString NotFoundMessage { get; set; }
    public int ProductCatalogMaxCount { get; set; }
    public int AiMatchingMaxPercent { get; set; }
    public int AiMatchingStep { get; set; }
    public LocalizedString IconLabel { get; set; }
}
