namespace SD.TestApi.Domain.Models;

public class QuestionModel
{
    public List<AnswerModel> Answers { get; set; } = [];
    public int Step { get; set; }
    public string Type { get; set; }
    public int AnswerLimit { get; set; }
    public string ElementType { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public ButtonsModel Buttons { get; set; }
    public bool IsOptional { get; set; }
    public int MaxCount { get; set; }
    public int MinCount { get; set; }
    public MinMaxTextModel MinText { get; set; }
    public MinMaxTextModel MaxText { get; set; }
}
