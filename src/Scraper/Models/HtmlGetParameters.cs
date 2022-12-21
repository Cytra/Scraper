using System.ComponentModel.DataAnnotations;

namespace Scraper.Models;

public class HtmlGetParameters
{
    [Required] public required string Url { get; set; }
}