using System.Globalization;
using BaseLib.Presentation.Extensions;
using BaseLib.Presentation.Filters;
using BaseLib.Presentation.Services.Api;
using BaseLib.Presentation.Services.Authentication;
using BaseLib.Presentation.Services.Content;
using BaseLib.Presentation.Services.Site;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    options.Filters.Add<ApiExceptionFilter>();
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddOptions<ApiOptions>().BindConfiguration("Api").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddHttpClient("BaselibApi", (services, client) =>
{
    var options = services.GetRequiredService<IOptions<ApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
builder.Services.AddScoped<ApiTransport>();
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddManagementApi();
builder.Services.AddSingleton<IServerSessionStore, ServerSessionStore>();
builder.Services.AddScoped<IAccountSession, AccountSession>();
builder.Services.AddScoped<SessionCookieEvents>();
builder.Services.AddSingleton<ISiteContentService, MockSiteContentService>();
builder.Services.AddSingleton<IContentService, MockContentService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "BaseLib.Presentation.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    options.LoginPath = "/Admin/Account/Login";
    options.AccessDeniedPath = "/Admin/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = false;
    options.EventsType = typeof(SessionCookieEvents);
});
builder.Services.AddAuthorization();
var app = builder.Build();
var culture = CultureInfo.GetCultureInfo("tr-TR");
app.UseRequestLocalization(new RequestLocalizationOptions { DefaultRequestCulture = new(culture), SupportedCultures = [culture], SupportedUICultures = [culture] });
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/hata"); app.UseHsts(); app.UseHttpsRedirection(); }
app.UseStatusCodePagesWithReExecute("/hata/{0}");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute("areas", "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
public partial class Program;
