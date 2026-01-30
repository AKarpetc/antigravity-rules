namespace SD.TestApi.Application.Models.External;

public class SelectableQuestionOption
{
    public List<OptionItem> Options { get; set; } = [];
    public string QuestionType { get; set; }
}
