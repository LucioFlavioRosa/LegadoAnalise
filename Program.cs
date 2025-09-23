using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<ICompetenciasService, CompetenciasService>();
builder.Services.AddScoped<ICargosService, CargosService>();
builder.Services.AddScoped<IEixosService, EixosService>();
builder.Services.AddScoped<ISubCompetenciasService, SubCompetenciasService>();
builder.Services.AddScoped<IDimensoesService, DimensoesService>();
builder.Services.AddScoped<IAvaliacoesService, AvaliacoesService>();
builder.Services.AddScoped<IExportFileService, ExportFileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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