using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace WebShop.Api
{
    public static class DiExtensions
    {
        public static void AddMediatorExt(this IServiceCollection services)
        {
            services.AddMediatR(typeof(Program));

            services.Scan(scan =>
            {
                scan.FromAssembliesOf(typeof(Program))
                    .AddClasses(classes => classes
                        .AssignableToAny(
                            typeof(IValidator<>), typeof(INotificationHandler<>), typeof(IPipelineBehavior<,>)))
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .WithTransientLifetime();
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), 
                typeof(RequestPreProcessorBehavior<,>));
            
            services.AddTransient(typeof(IPipelineBehavior<,>), 
                typeof(RequestPostProcessorBehavior<,>));
        }
    }
}