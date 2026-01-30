namespace SD.TestApi.Application.Models.External;

public class OptionSet
{
    public List<SelectableQuestionOption> SelectableQuestionOptions { get; set; } = [];
    public List<RangeOption> RangeOptions { get; set; } = [];
    public string SaleType { get; set; }
    public string StockType { get; set; }
}
