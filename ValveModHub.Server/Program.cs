using System.Text.Json;
using System.Text.Json.Serialization;
using ValveModHub.Server.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

services.AddControllers();
services.AddHttpClient();
services.AddHttpContextAccessor();
services.AddMemoryCache();

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddSingleton<SteamServerBrowserApiService>();
services.AddSingleton<SteamPlayerDetailApiService>();

services
    .AddRazorPages()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals;
        options.JsonSerializerOptions.IncludeFields = true;
    });

#if !DEBUG
builder.WebHost.UseUrls("http://0.0.0.0:8085");
#endif

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    //app.UseHsts();
}

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
});
app.UseAntiforgery();

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();