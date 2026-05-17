using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Application.Tasks;

namespace TaskTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register all handlers from the assembly
        var assembly = typeof(CreateTaskCommand).Assembly;

        // Register validators
        services.Scan(scan => scan
            .FromAssemblyOf<CreateTaskCommandValidator>()
            .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Register handlers
        services.Scan(scan => scan
            .FromAssemblyOf<CreateTaskCommandHandler>()
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblyOf<DeleteTaskCommandHandler>()
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
