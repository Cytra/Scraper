using Application.Models.Enums;

namespace Application.Models;

public class Selector
{
    public ElementType? Element { get; set; }

    public string? Class { get; set; }

    public string? Id { get; set; }
}