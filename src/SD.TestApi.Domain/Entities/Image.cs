using System;
using System.Collections.Generic;

namespace SD.TestApi.Domain.Entities;

public class Image
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<string> RelatedNames { get; set; } = [];
    public bool IsDefault { get; set; }
}
