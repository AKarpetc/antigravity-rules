namespace SD.TestApi.Application.Models.External;

public class OptionItem
{
    public List<TitleItem> Titles { get; set; } = [];
    public int Id { get; set; }
    public int Order { get; set; }
}
