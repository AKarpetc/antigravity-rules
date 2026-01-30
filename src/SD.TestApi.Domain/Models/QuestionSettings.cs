namespace SD.TestApi.Domain.Models;

public class QuestionSettings
{
    public List<AnswerOrder> AnswerOrder { get; set; } = [];
    public int Step { get; set; }
    public string Type { get; set; }
    public int AnswerLimit { get; set; }
    public LocalizedString Title { get; set; }
    public LocalizedString Subtitle { get; set; }
    public ButtonsSettings Buttons { get; set; }
    public int MaxCount { get; set; }
    public int MinCount { get; set; }
    public LocalizedString MinText { get; set; }
    public LocalizedString MaxText { get; set; }
    public string ElementType { get; set; }
    public bool IsOptional { get; set; }
}
