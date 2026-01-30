namespace SD.TestApi.Domain.Models;

public class CartAssistantModel
{
    public List<QuestionModel> Questions { get; set; } = [];
    public int Total { get; set; }
    public string ResultTitle { get; set; }
    public string StartOver { get; set; }
    public string Tooltip { get; set; }
    public string NotFoundMessage { get; set; }
    public string IconLabel { get; set; }
}
