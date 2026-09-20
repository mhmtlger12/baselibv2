using System.Xml.Linq;
using BaseLib.Presentation.Services.Site;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;
public sealed class SeoController(ISiteContentService site, IConfiguration configuration) : Controller
{
    [HttpGet("/robots.txt")]
    public IActionResult Robots() => Content($"User-agent: *\nAllow: /\nDisallow: /Admin\nDisallow: /arama\nSitemap: {BaseUrl}/sitemap.xml\n", "text/plain");
    [HttpGet("/sitemap.xml")]
    public IActionResult Sitemap()
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var paths = new List<string> { "/", "/ilanlar", "/iletisim", "/bilgi/hakkimizda" };
        foreach (var card in site.Content.ScoreCards)
        {
            paths.Add($"/taban-puanlari/{card.Key}");
            paths.AddRange(site.Content.Departments.Select(x => $"/taban-puanlari/{card.Key}/bolum/{x.Id}"));
        }
        paths.AddRange(site.Content.JobListings.Select(x => $"/ilanlar/{x.Id}"));
        return Content(new XDocument(new XElement(ns + "urlset", paths.Select(x => new XElement(ns + "url", new XElement(ns + "loc", BaseUrl + x))))).ToString(), "application/xml");
    }
    private string BaseUrl => configuration["Site:BaseUrl"]!.TrimEnd('/');
}
