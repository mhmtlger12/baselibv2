using BaseLib.Presentation.Services.Site;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;
public sealed class ScoresController(ISiteContentService site) : Controller
{
    [HttpGet("/taban-puanlari/{category}")]
    public IActionResult Index(string category, string? level, string? q)
    {
        var page = site.Departments(category, level, q);
        return page is null ? NotFound() : View(page);
    }
    [HttpGet("/taban-puanlari/{category}/bolum/{id:int}")]
    public IActionResult Detail(string category, int id, string? period, string? institution, string? city, int page = 1)
    {
        var model = site.Detail(category, id, period, institution, city, page);
        return model is null ? NotFound() : View(model);
    }
}
