using System;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using SistemaAvaliacao.Repositories;
using SistemaAvaliacao.Services;

namespace SistemaAvaliacao.Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Registro dos repositórios
            services.AddScoped<ICargoRepository, CargoRepository>();

            // Registro dos serviços de negócio
            services.AddScoped<CargoService>();
            services.AddScoped<ExportService>();

            // Adicione outros serviços conforme necessário
        }
    }
}
