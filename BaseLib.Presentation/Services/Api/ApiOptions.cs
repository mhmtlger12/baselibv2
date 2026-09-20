using System.ComponentModel.DataAnnotations;
namespace BaseLib.Presentation.Services.Api;
public sealed class ApiOptions
{
    [Required, Url] public string BaseUrl { get; set; } = "http://localhost:5298/";
    [Range(1, 120)] public int TimeoutSeconds { get; set; } = 30;
}
