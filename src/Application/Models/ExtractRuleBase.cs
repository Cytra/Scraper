using Application.Models.Enums;

namespace Application.Models;

public class ExtractRuleBase
{
    /// <summary>
    /// What item should it be
    /// </summary>
    public ItemType? ItemType { get; set; }

    /// <summary>
    /// Nested Output 
    /// </summary>
    public object? Output { get; set; }

    /// <summary>
    /// If false returns the html
    /// </summary>
    public bool Clean { get; set; } = true;
}