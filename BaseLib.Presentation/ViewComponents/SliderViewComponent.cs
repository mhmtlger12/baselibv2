using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;

namespace BaseLib.Presentation.ViewComponents;

public sealed class SliderViewComponent(IPublicSliderApiService sliders) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var items = await sliders.GetPublishedAsync(HttpContext.RequestAborted);
            return items.Count == 0 ? Content(string.Empty) : View("~/Views/Shared/_Slider.cshtml", items);
        }
        catch (ApiException)
        {
            return Content(string.Empty);
        }
    }
}
