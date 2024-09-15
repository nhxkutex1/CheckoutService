using Microsoft.Extensions.DependencyInjection;
using SolidToken.SpecFlow.DependencyInjection;
using CheckoutService_TestTask.Models;

namespace CheckoutService_TestTask.Support
{
    [Binding]
    public static class TestDependencies
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();

            services.AddScoped<Order>();

            return services;
        }
    }
}
