using AngleSharp.Html.Parser;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;

namespace NerdStore.WebApp.Tests
{
    [Collection(nameof(IntegrationWebTestsCollection))]
    public class PedidoTests
    {
        private readonly IntegrationTestsFixture<Program> _fixture;

        public PedidoTests(IntegrationTestsFixture<Program> fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Adicionar item no pedido novo")]
        [Trait("Categoria", "Integração Web Pedido")]
        public async Task AdicionarItem_NovoPedido_DeveAtualizarValorTotal()
        {
            //Assert 
            var produtoId = "fc184e11-014c-4978-aa10-9eb5e1af369b";
            var pedidoDetalhe = await _fixture.Client.GetAsync($"/produto-detalhe/{produtoId}");
            var antiForgeryToken = _fixture.ObterAntiForgeryToken(await pedidoDetalhe.Content.ReadAsStringAsync());
            const int quantidade = 2;

            pedidoDetalhe.EnsureSuccessStatusCode();

            await _fixture.RealizarLogin();

            var formData = new Dictionary<string, string>()
            {
                { "Id", produtoId },
                { "quantidade", quantidade.ToString() },
                { _fixture.AntiForgeryFieldName, antiForgeryToken }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/meu-carrinho")
            {
                Content = new FormUrlEncodedContent(formData),
            };

            //Act
            var adicionarCarrinhoRequest = await _fixture.Client.SendAsync(requestMessage);

            //Assert
            adicionarCarrinhoRequest.EnsureSuccessStatusCode();
            var html = await new HtmlParser()
                 .ParseDocumentAsync(await adicionarCarrinhoRequest.Content.ReadAsStringAsync());

            var parseDocument = html.All;
            var formQuantidade = parseDocument?.FirstOrDefault(c => c.Id == "quantidade")?.GetAttribute("value")?.ApenasNumeros();
            var valorUnitario = parseDocument?.FirstOrDefault(c => c.Id == "valorUnitario")?.TextContent.Split(".")[0]?.ApenasNumeros();
            var valorTotal = parseDocument?.FirstOrDefault(c => c.Id == "valorTotal")?.TextContent.Split(".")[0]?.ApenasNumeros();

            Assert.Equal(valorTotal, valorUnitario * formQuantidade);
        }
    }
}
