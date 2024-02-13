using Application.Models;

namespace Application.Interfaces;

public interface ISelectorService<T> where T : ExtractRuleBase
{
    string? GetImplicitInputSelector(T selector);

    string GetImplicitOutputSelector(T selector);
}