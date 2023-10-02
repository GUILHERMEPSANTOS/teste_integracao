using NerdStore.Core.Messages;
using NerdStore.Vendas.Application.Pedidos.Validations;

namespace NerdStore.Vendas.Application.Pedidos.Commands
{
    public class AdicionarItemCommand : Command
    {
        public Guid ProdutoId { get; set; }
        public Guid ClienteId { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public AdicionarItemCommand(Guid produtoId, Guid clienteId, string nome, int quantidade, decimal valorUnitario)
        {
            ProdutoId = produtoId;
            ClienteId = clienteId;
            Nome = nome;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }

        public override bool ValidarSeEhValido()
        {
            ValidationResult = new AdicionarItemCommandValidation().Validate(this);

            return ValidationResult.IsValid;
        }
    }
}
