using PortalPokemon.Clients;
using PortalPokemon.Configuration;
using PortalPokemon.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<PokeApiClient>(client =>
{
    client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
    client.Timeout = TimeSpan.FromSeconds(15);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression =
        DecompressionMethods.GZip | DecompressionMethods.Deflate
});
builder.Services.AddScoped<PokemonService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddHttpClient<ExcelExportService>();
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Smtp"));
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "pokemon-home",
    pattern: "",
    defaults: new { controller = "Pokemon", action = "Paginated" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
