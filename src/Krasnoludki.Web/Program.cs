using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddElectron();

builder.UseElectron(args, static async () =>
{
    var options = new BrowserWindowOptions
    {
        Width = 1500,
        Height = 1000,
        Title = "K.S.Z.K",
        Center = true,
        BackgroundColor = "#fdf5e6",
        WebPreferences = new WebPreferences
        {
            NodeIntegration = true
        }
    };

    var window = await Electron.WindowManager.CreateWindowAsync(options);
    window.SetMenuBarVisibility(false);

    window.OnClosed += () =>
    {
        Electron.App.Quit();
    };
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

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();