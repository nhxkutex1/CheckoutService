using Microsoft.Extensions.DependencyInjection;
using SolidToken.SpecFlow.DependencyInjection;

public class TestDependencies
{
    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddHttpClient("CheckoutApiClient", client =>
        {
            client.BaseAddress = new Uri("http://localhost:7777/");
        });

        return serviceCollection;
    }
}