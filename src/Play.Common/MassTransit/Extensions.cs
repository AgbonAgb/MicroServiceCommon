using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitRabbitmq(this IServiceCollection services)
        {

            //Add the RabitQ services
            services.AddMassTransit(configure =>
            {
                //configure clients(inventory that will subscribe to Catalog publish)
                configure.AddConsumers(Assembly.GetEntryAssembly());

                //specify type of transport
                configure.UsingRabbitMq((context, configuration) =>
                {
                    var _configuration = context.GetService<IConfiguration>();//IConfiguration will expose settings of the app that will
                    //use this method from appseeting.json
                    var _serviceSettings = _configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

                    var rabbitMQSettings = _configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    configuration.Host(rabbitMQSettings.Host);
                    //format how the string names will be
                    configuration.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(_serviceSettings.ServiceName, false));
                    configuration.UseMessageRetry(retryConfig =>
                    {
                        retryConfig.Interval(4, System.TimeSpan.FromSeconds(10));//retry the consume Q items five times incase of failure
                        //retry after 10 seconds

                    });
                });

            });

            return services;
        }

    }
}