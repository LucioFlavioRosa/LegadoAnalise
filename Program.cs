using Peers.Moderno.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<IEvolucaoAssociadoService, EvolucaoAssociadoService>();
builder.Services.AddScoped<IAssociadosService, AssociadosService>();
builder.Services.AddScoped<IPremissaService, PremissaService>();
builder.Services.AddScoped<IEvolucaoAssociadoDataService, EvolucaoAssociadoDataService>();
builder.Services.AddScoped<IWebStorageService, WebStorageService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();