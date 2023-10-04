using Azure.Core;
using Features.Tests;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;

namespace NerdStore.WebApp.Tests
{
    [TestCaseOrderer("Features.Tests.PriorityOrderer", "Features.Tests")]
    [Collection(nameof(IntegrationWebTestsCollection))]
    public class UsuarioTests
    {
        private readonly IntegrationTestsFixture<Program> _fixture;

        public UsuarioTests(IntegrationTestsFixture<Program> integrationTestsFixture)
        {
            _fixture = integrationTestsFixture;
        }

        [Fact(DisplayName = "Realizar cadastro com sucesso"), TestPriority(1)]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async Task Usuario_RealizarCadastro_DeveExecutarComSucesso()
        {
            // Assert
            var initialresponse = await _fixture.Client.GetAsync("/Identity/Account/Register");
            initialresponse.EnsureSuccessStatusCode();

            var antiForgeryToken = _fixture.ObterAntiForgeryToken(await initialresponse.Content.ReadAsStringAsync());
            _fixture.GerarEmailESenha();


            var formData = new Dictionary<string, string>()
            {
                { _fixture.AntiForgeryFieldName, antiForgeryToken},
                { "Input.Email", _fixture.UserEmail },
                { "Input.Password", _fixture.UserPassword },
                { "Input.ConfirmPassword", _fixture.UserPassword },
            };

            var httpMessage = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Register")
            {
                Content = new FormUrlEncodedContent(formData),
            };

            // Act
            var postResquest = await _fixture.Client.SendAsync(httpMessage);

            //Assert
            var response = await postResquest.Content.ReadAsStringAsync();

            postResquest.EnsureSuccessStatusCode();
            Assert.Contains(expectedSubstring: $"Hello {_fixture.UserEmail}!", response);
        }

        [Fact(DisplayName = "Realizar cadastro senha fraca"), TestPriority(3)]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async Task Usuario_RealizarCadastroComSenhaFraca_DeveRetornarMensagemDeErro()
        {
            // Assert
            var initialresponse = await _fixture.Client.GetAsync("/Identity/Account/Register");
            initialresponse.EnsureSuccessStatusCode();

            var antiForgeryToken = _fixture.ObterAntiForgeryToken(await initialresponse.Content.ReadAsStringAsync());
            _fixture.GerarEmailESenha();

            var senhaFraca = "123456";

            var formData = new Dictionary<string, string>()
            {
                { _fixture.AntiForgeryFieldName, antiForgeryToken},
                { "Input.Email", _fixture.UserEmail },
                { "Input.Password", senhaFraca },
                { "Input.ConfirmPassword", senhaFraca },
            };

            var httpMessage = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Register")
            {
                Content = new FormUrlEncodedContent(formData),
            };

            // Act
            var postResquest = await _fixture.Client.SendAsync(httpMessage);

            //Assert
            var response = await postResquest.Content.ReadAsStringAsync();

            postResquest.EnsureSuccessStatusCode();
            Assert.Contains("Passwords must have at least one non alphanumeric character.", response);
            Assert.Contains("Passwords must have at least one lowercase (&#x27;a&#x27;-&#x27;z&#x27;).", response);
            Assert.Contains("Passwords must have at least one uppercase (&#x27;A&#x27;-&#x27;Z&#x27;).", response);
        }

        [Fact(DisplayName = "Realizar login com sucesso"), TestPriority(2)]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async Task Usuario_RealizarLoginComSucesso_DeveExecutarComSucesso()
        {
            // Assert
            var initialresponse = await _fixture.Client.GetAsync("/Identity/Account/Login");
            initialresponse.EnsureSuccessStatusCode();

            var antiForgeryToken = _fixture.ObterAntiForgeryToken(await initialresponse.Content.ReadAsStringAsync());
            
            var formData = new Dictionary<string, string>()
            {
                { _fixture.AntiForgeryFieldName, antiForgeryToken},
                { "Input.Email", _fixture.UserEmail },
                { "Input.Password", _fixture.UserPassword },
            };

            var httpMessage = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Login")
            {
                Content = new FormUrlEncodedContent(formData),
            };

            // Act
            var postResquest = await _fixture.Client.SendAsync(httpMessage);

            //Assert
            var response = await postResquest.Content.ReadAsStringAsync();

            postResquest.EnsureSuccessStatusCode();
            Assert.Contains(expectedSubstring: $"Hello {_fixture.UserEmail}!", response);
        }
    }
}
