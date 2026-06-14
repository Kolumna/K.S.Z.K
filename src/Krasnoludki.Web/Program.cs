using System.Text.Json.Serialization;
using Krasnoludki.Web.Services;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit     = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout      = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly  = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<ScenarioFileService>(provider =>
{
    var env  = provider.GetRequiredService<IWebHostEnvironment>();
    var path = Path.Combine(env.ContentRootPath, "Data", "Scenarios");
    return new ScenarioFileService(path);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();