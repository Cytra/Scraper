using Application.Models.Enums;

namespace Application.Models;

public class ExplicitExtractRule : ExtractRuleBase
{
    required public Selector Selector { get; set; }
}