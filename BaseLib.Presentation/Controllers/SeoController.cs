using System.Xml.Linq;
using BaseLib.Presentation.Services.Site;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;
public sealed class SeoController(ISiteContentService site, IConfiguration configuration, IPublicJobApiService jobs) : Controller
{
    [HttpGet("/robots.txt")]
    public IActionResult Robots() => Content($"User-agent: *\nAllow: /\nDisallow: /Admin\nDisallow: /arama\nSitemap: {BaseUrl}/sitemap.xml\n", "text/plain");
    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> Sitemap(CancellationToken ct)
    {
        var listings = await jobs.GetPublishedAsync("all", null, ct);
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var paths = new List<string> { "/", "/ilanlar", "/iletisim", "/bilgi/hakkimizda" };
        var data = await site.GetAsync(ct);
        foreach (var card in data.ScoreCards)
        {
            paths.Add($"/taban-puanlari/{card.Key}");
            paths.AddRange(data.Departments.Select(x => $"/taban-puanlari/{card.Key}/bolum/{x.Id}"));
        }
        paths.AddRange(listings.Select(x => $"/ilanlar/{x.Id}"));
        return Content(new XDocument(new XElement(ns + "urlset", paths.Select(x => new XElement(ns + "url", new XElement(ns + "loc", BaseUrl + x))))).ToString(), "application/xml");
    }
    private string BaseUrl => configuration["Site:BaseUrl"]!.TrimEnd('/');
}
