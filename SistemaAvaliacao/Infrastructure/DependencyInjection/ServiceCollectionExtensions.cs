using Microsoft.Extensions.DependencyInjection;
using SistemaAvaliacao.Repositories;
using SistemaAvaliacao.Services;

namespace SistemaAvaliacao.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Extensões para registrar serviços e repositórios no container de injeção de dependências.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra todos os serviços e repositórios necessários para o módulo de Cargos.
        /// </summary>
        /// <param name="services">A coleção de serviços do .NET Core.</param>
        /// <returns>A própria coleção de serviços para encadeamento.</returns>
        public static IServiceCollection AddCargoModule(this IServiceCollection services)
        {
            // Repositórios
            services.AddScoped<ICargoRepository, CargoRepository>();

            // Serviços de negócio
            services.AddScoped<CargoService>();

            // Serviços de exportação
            services.AddScoped<IExportService, ExcelExportService>();

            // Adicione outros serviços relacionados a cargos aqui, se necessário

            return services;
        }
    }
}
