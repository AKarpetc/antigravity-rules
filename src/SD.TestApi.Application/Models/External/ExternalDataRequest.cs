namespace SD.TestApi.Application.Models.External;

public class ExternalDataRequest
{
    public int StoreId { get; set; }
    public List<OptionLimit> OptionLimits { get; set; } = [];
}
