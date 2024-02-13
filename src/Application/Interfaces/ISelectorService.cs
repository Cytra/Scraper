namespace Application.Interfaces;

public interface ISelectorService
{
    string? GetImplicitInputSelector(string? selector);

    string GetImplicitOutputSelector(string selector);
}