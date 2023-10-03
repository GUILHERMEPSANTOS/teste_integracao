using Microsoft.AspNetCore.Mvc.Testing;
using NerdStore.WebApp.MVC;

namespace NerdStore.WebApp.Tests.Config
{
    [CollectionDefinition(nameof(IntegrationApiTestsCollection))]
    public class IntegrationApiTestsCollection : ICollectionFixture<IntegrationTestsFixture<Program>> { }

    [CollectionDefinition(nameof(IntegrationWebTestsCollection))]
    public class IntegrationWebTestsCollection : ICollectionFixture<IntegrationTestsFixture<Program>> { }

    public class IntegrationTestsFixture<TProgram> : IDisposable where TProgram : class
    {
        public HttpClient Client;
        public LojaAppFactory<TProgram> Factory;

        public IntegrationTestsFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions { };

            Factory = new LojaAppFactory<TProgram>();
            Client = Factory.CreateClient(clientOptions);
        }

        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
        }
    }
}
