using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Data;
using SistemaAvaliacao.Moderno.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMudServices();

builder.Services.AddScoped<IAssociadosService, AssociadosService>();
builder.Services.AddScoped<ICargosService, CargosService>();
builder.Services.AddScoped<IPerfisService, PerfisService>();
builder.Services.AddScoped<IPromocoesService, PromocoesService>();
builder.Services.AddScoped<IFotosAssociadosService, FotosAssociadosService>();
builder.Services.AddScoped<IVerticaisService, VerticaisService>();
builder.Services.AddScoped<IEmpresasService, EmpresasService>();
builder.Services.AddScoped<IExcelService, ExcelService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();