namespace Application.Ports;

public interface IHtmlService
{
    public Task<string> GetData(string url, int? waitTime = null);
}