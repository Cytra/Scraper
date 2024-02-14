namespace Application.Models;

public class Selector
{
    public string? Element { get; set; }

    public string? Class { get; set; }

    public string? Id { get; set; }

    public void Deconstruct(out string? elementType, out string? classString, out string? id)
    {
        elementType = Element;
        classString = Class;
        id = Id;
    }
}
