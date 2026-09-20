using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.ViewComponents;
public sealed class AdminNavigationViewComponent(ISystemApiService system) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var menus = await system.MyMenusAsync(HttpContext.RequestAborted);
            return View(new NavigationModel(Flatten(menus).Where(x => x.IsActive && x.Url is not null).Select(x => x.Url!.TrimEnd('/')).ToHashSet(StringComparer.OrdinalIgnoreCase), null));
        }
        catch (ApiException ex) { return View(new NavigationModel(new HashSet<string>(), ex.Message)); }
    }
    private static IEnumerable<MenuDto> Flatten(IEnumerable<MenuDto> menus) => menus.SelectMany(x => new[] { x }.Concat(Flatten(x.SubMenus)));
}
