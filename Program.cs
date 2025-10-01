using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Data;
using Services;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAssociadosService, AssociadosService>();
builder.Services.AddScoped<ICargosService, CargosService>();
builder.Services.AddScoped<IPerfisService, PerfisService>();
builder.Services.AddScoped<IVerticalService, VerticalService>();
builder.Services.AddScoped<IFotosAssociadosService, FotosAssociadosService>();
builder.Services.AddScoped<IExportFileService, ExportFileService>();
builder.Services.AddScoped<IImportFileService, ImportFileService>();

builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorComponents<App.Startup>("/");

app.Run();