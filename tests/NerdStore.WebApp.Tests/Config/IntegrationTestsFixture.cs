using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using NerdStore.WebApp.MVC;
using System.Text.RegularExpressions;

namespace NerdStore.WebApp.Tests.Config
{
    [CollectionDefinition(nameof(IntegrationApiTestsCollection))]
    public class IntegrationApiTestsCollection : ICollectionFixture<IntegrationTestsFixture<Program>> { }

    [CollectionDefinition(nameof(IntegrationWebTestsCollection))]
    public class IntegrationWebTestsCollection : ICollectionFixture<IntegrationTestsFixture<Program>> { }

    public class IntegrationTestsFixture<TProgram> : IDisposable where TProgram : class
    {
        public string AntiForgeryFieldName = "__RequestVerificationToken";

        public HttpClient Client;
        public LojaAppFactory<TProgram> Factory;

        public string UserEmail;
        public string UserPassword;

        public IntegrationTestsFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true,
                BaseAddress = new Uri("http://localhost"),
                HandleCookies = true,
                MaxAutomaticRedirections = 7
            };

            Factory = new LojaAppFactory<TProgram>();
            Client = Factory.CreateClient(clientOptions);
        }

        public void GerarEmailESenha()
        {
            var faker = new Faker("pt_BR");

            UserEmail = faker.Internet.Email();
            UserPassword = faker.Internet.Password(8,false, "", "@1Ab_");
        }

        public async Task RealizarLogin()
        {
            var initialresponse = await Client.GetAsync("/Identity/Account/Login");
            var antiForgeryToken = ObterAntiForgeryToken(await initialresponse.Content.ReadAsStringAsync());

            var formData = new Dictionary<string, string>()
            {
                { AntiForgeryFieldName, antiForgeryToken},
                { "Input.Email", "thiago.pereira.370@hotmail.com" },
                { "Input.Password", "Senha@123" },
            };

            var httpMessage = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Login")
            {
                Content = new FormUrlEncodedContent(formData),
            };
           
           await Client.SendAsync(httpMessage);
        }

        public string ObterAntiForgeryToken(string htmlBody)
        {
            var requestVerificationTokenMatch =
                Regex.Match(htmlBody, $@"\<input name=""{AntiForgeryFieldName}"" type=""hidden"" value=""([^""]+)"" \/\>");

            if (requestVerificationTokenMatch.Success)
            {
                return requestVerificationTokenMatch.Groups[1].Captures[0].Value;
            }

            throw new ArgumentException($"Anti forgery token '{AntiForgeryFieldName}' não encontrado no HTML", nameof(htmlBody));
        }

        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
        }
    }
}
