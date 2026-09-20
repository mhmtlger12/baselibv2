using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;

namespace BaseLib.Presentation.ViewComponents;

public sealed class SliderViewComponent(IPublicSliderApiService sliders, ILogger<SliderViewComponent> logger) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var items = await sliders.GetPublishedAsync(HttpContext.RequestAborted);
            return items.Count == 0 ? Content(string.Empty) : View("~/Views/Shared/_Slider.cshtml", items);
        }
        catch (ApiException exception)
        {
            logger.LogWarning(exception, "Yayınlanan slaytlar alınamadı.");
            return Content(string.Empty);
        }
    }
}
