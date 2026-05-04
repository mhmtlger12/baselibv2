using Baselib.Presentation.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5298";

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Admin") &&
        !context.Request.Cookies.ContainsKey("AccessToken"))
    {
        context.Response.Redirect("/Login");
        return;
    }

    await next();
});
app.UseAuthorization();

app.Map("/api/{**path}", async (HttpContext context, IHttpClientFactory httpClientFactory, string path) =>
{
    var client = httpClientFactory.CreateClient("ApiClient");
    var targetPath = $"/api/{path}{context.Request.QueryString}";
    using var request = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetPath);

    if (context.Request.ContentLength > 0 || context.Request.Headers.ContainsKey("Content-Type"))
    {
        request.Content = new StreamContent(context.Request.Body);
        if (!string.IsNullOrWhiteSpace(context.Request.ContentType))
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(context.Request.ContentType);
    }

    var token = context.Request.Cookies["AccessToken"];
    if (!string.IsNullOrWhiteSpace(token))
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

    using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
    context.Response.StatusCode = (int)response.StatusCode;

    foreach (var header in response.Headers)
        context.Response.Headers[header.Key] = header.Value.ToArray();

    foreach (var header in response.Content.Headers)
        context.Response.Headers[header.Key] = header.Value.ToArray();

    context.Response.Headers.Remove("transfer-encoding");
    await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
});

app.MapRazorPages();

app.Run();
