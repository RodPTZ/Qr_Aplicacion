using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Inheritance;
using SistemaDeBoleteria.Repositories;
using SistemaDeBoleteria.Services;
using SistemaDeBoleteria.Core.Validations;
using Microsoft.AspNetCore.Identity;
using SistemaDeBoleteria.Core.Enums;
using System.Security.Claims;
namespace SistemaDeBoleteria.API.Extensions
{
    public static class ServiceExtensions
    {
        // public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
        // {
        //     services.AddHttpContextAccessor();

        //     string GetConnectionString(IServiceProvider sp)
        //     {
        //         var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;

        //         var rolString = httpContext?.User.FindFirstValue(ClaimTypes.Role);
        //         if (string.IsNullOrEmpty(rolString))
        //         {
        //             return configuration.GetConnectionString("DefaultConnection")!;
        //         }

        //         var connectionString = configuration.GetConnectionString(rolString);

        
        //         return string.IsNullOrEmpty(connectionString)
        //             ? configuration.GetConnectionString("DefaultConnection")!
        //             : connectionString;
        //     }

        //     var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        //     services.AddScoped<ILoginRepository>(sp => new LoginRepository(connectionString));
        //     services.AddScoped<ILocalRepository>(sp => new LocalRepository(GetConnectionString(sp)));
        //     services.AddScoped<ISectorRepository>(sp => new SectorRepository(GetConnectionString(sp)));
        //     services.AddScoped<IEventoRepository>(sp => new EventoRepository(GetConnectionString(sp)));
        //     services.AddScoped<IFuncionRepository>(sp => new FuncionRepository(GetConnectionString(sp)));
        //     services.AddScoped<ITarifaRepository>(sp => new TarifaRepository(GetConnectionString(sp)));
        //     services.AddScoped<IClienteRepository>(sp => new ClienteRepository(GetConnectionString(sp)));
        //     services.AddScoped<IOrdenRepository>(sp => new OrdenRepository(GetConnectionString(sp)));
        //     services.AddScoped<IEntradaRepository>(sp => new EntradaRepository(GetConnectionString(sp)));
        //     services.AddScoped<ICodigoQRRepository>(sp => new CodigoQRRepository(GetConnectionString(sp)));
        //     // services.AddScoped<ILoginRepository>(sp => new LoginRepository(GetConnectionString(sp)));
        //     services.AddScoped<ITokenRepository>(sp => new TokenRepository(GetConnectionString(sp)));
        // }
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<ILocalRepository, LocalRepository>();
            services.AddScoped<ISectorRepository, SectorRepository>();
            services.AddScoped<IEventoRepository, EventoRepository>();
            services.AddScoped<IFuncionRepository, FuncionRepository>();
            services.AddScoped<ITarifaRepository, TarifaRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IOrdenRepository, OrdenRepository>();
            services.AddScoped<IEntradaRepository, EntradaRepository>();
            services.AddScoped<ICodigoQRRepository, CodigoQRRepository>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<ITokenRepository,TokenRepository>();
        }
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<ICodigoQRService, CodigoQRService>();
            services.AddScoped<IEventoService, EventoService>();
            services.AddScoped<IEntradaService, EntradaService>();
            services.AddScoped<IFuncionService, FuncionService>();
            services.AddScoped<ILocalService, LocalService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IOrdenService, OrdenService>();
            services.AddScoped<ISectorService, SectorService>();
            services.AddScoped<ITarifaService, TarifaService>();
        }
        public static void AddValidations(this IServiceCollection services)
        {
            services.AddTransient<IValidator<CrearActualizarLocalDTO>, LocalValidator>();
            services.AddTransient<IValidator<CrearActualizarEventoDTO>, EventoValidator>();

            services.AddTransient<IValidator<CrearActualizarSectorDTO>, SectorValidator>();

            services.AddTransient<IValidator<CrearActualizarEventoDTO>, EventoValidator>();

            services.AddTransient<IValidator<CrearFuncionDTO>, FuncionValidator>();
            services.AddTransient<IValidator<ActualizarFuncionDTO>, ActualizarFuncionValidator>();

            services.AddTransient<IValidator<CrearTarifaDTO>, TarifaValidator>();
            services.AddTransient<IValidator<ActualizarTarifaDTO>, ActualizarTarifaDTOValidator>();

            services.AddTransient<IValidator<CrearClienteDTO>, ClienteValidator>();
            services.AddTransient<IValidator<ActualizarClienteDTO>, ActualizarClienteDTOValidator>();

            services.AddTransient<IValidator<CrearOrdenDTO>, OrdenValidator>();

            services.AddTransient<IValidator<LoginRequest>, LoginValidator>();
            services.AddTransient<IValidator<RegisterRequest>, RegisterValidator>();
        }
    }
}