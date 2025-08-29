using Habitus.Application.Services;
using Habitus.Domain.Interfaces;
using Habitus.Domain.Interfaces.Services;
using Habitus.Domain.Interfaces.Repositories;
using Habitus.Infrastructure.Data;
using Habitus.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Habitus.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<HabitusDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
                ));

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<ITaskRepository, TaskRepository>();
            // services.AddScoped<ICategoryRepository, CategoryRepository>();
            // services.AddScoped<IHabitRepository, HabitRepository>();
            // services.AddScoped<IHabitLogRepository, HabitLogRepository>();
            // services.AddScoped<ICalendarEventRepository, CalendarEventRepository>();
            // services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            // services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IUserService, UserServiceNew>();
            // services.AddScoped<IAuthService, AuthService>();
            // services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
