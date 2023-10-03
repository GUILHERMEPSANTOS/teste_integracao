using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;

namespace NerdStore.WebApp.Tests
{
    [Collection(nameof(IntegrationWebTestsCollection))]
    public class UsuarioTests
    {
        private readonly IntegrationTestsFixture<Program> _integrationTestsFixture;

        public UsuarioTests(IntegrationTestsFixture<Program> integrationTestsFixture)
        {
            _integrationTestsFixture = integrationTestsFixture;            
        }
    }
}
