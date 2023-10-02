using MediatR;
using NerdStore.Core.Messages;
using NerdStore.Vendas.Application.Pedidos.Commands;
using NerdStore.Vendas.Application.Pedidos.Events;
using NerdStore.Vendas.Domain;

namespace NerdStore.Vendas.Application.Pedidos.Handlers
{
    public class PedidoCommandHandler : CommandHandler, IRequestHandler<AdicionarItemCommand, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoCommandHandler(IMediator mediator, IPedidoRepository pedidoRepository) : base(mediator)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<bool> Handle(AdicionarItemCommand message, CancellationToken cancellationToken)
        {
            var ehValido = await ValidarSeCommandEhValido(message);

            if(!ehValido) return false;

            var pedido = await ObterOuCriarPedido(message.ClienteId);
            var pedidoItem = CriarPedidoItem(message);

            AdicionarOuAtualizarItemNoPedido(pedido, pedidoItem);
            await PublicarEventoPedidoItemAdicionado(message, pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
        }

        private async Task<Pedido> ObterOuCriarPedido(Guid clientId)
        {
            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(clientId);

            if (pedido is null)
            {
                pedido = Pedido.PedidoFactory.NovoPedidoRascunho(clientId);
                _pedidoRepository.Adicionar(pedido);
            }

            return pedido;
        }

        private PedidoItem CriarPedidoItem(AdicionarItemCommand message)
        {
            return new PedidoItem(
                produtoId: message.ProdutoId,
                produtoNome: message.Nome,
                quantidade: message.Quantidade,
                valorUnitario: message.ValorUnitario
            );
        }

        private void AdicionarOuAtualizarItemNoPedido(Pedido pedido, PedidoItem pedidoItem)
        {
            var itemExistente = pedido.PedidoItemExistente(pedidoItem.ProdutoId);

            pedido.AdicionarItem(pedidoItem);

            if (itemExistente)
            {
                _pedidoRepository.AtualizarItem(pedidoItem);
            }
            else
            {
                _pedidoRepository.AdicionarItem(pedidoItem);
            }

            _pedidoRepository.Atualizar(pedido);
        }

        private async Task PublicarEventoPedidoItemAdicionado(AdicionarItemCommand message, Pedido pedido)
        {

            var evento = new PedidoItemAdicionadoEvent(
                message.ClienteId,
                pedido.Id,
                message.ProdutoId,
                message.Nome,
                message.Quantidade,
                message.ValorUnitario
            );

            await _mediator.Publish(evento);
        }

    }
}
