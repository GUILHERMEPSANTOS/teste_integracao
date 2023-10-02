using FluentValidation;

namespace NerdStore.Vendas.Domain.Validations
{
    public class VoucherValidation : AbstractValidator<Voucher>
    {
        public static string CodigoErroMsg => "Voucher sem código válido.";
        public static string DataValidadeErroMsg => "Este voucher está expirado.";
        public static string AtivoErroMsg => "Este voucher não é mais válido.";
        public static string UtilizadoErroMsg => "Este voucher já foi utilizado.";
        public static string QuantidadeErroMsg => "Este voucher não está mais disponível";
        public static string ValorDescontoErroMsg => "O valor do desconto precisa ser superior a 0";
        public static string PercentualDescontoErroMsg => "O valor da porcentagem de desconto precisa ser superior a 0";

        public VoucherValidation()
        {
            RuleFor(voucher => voucher.Codigo)
                .NotEmpty()
                .WithMessage(CodigoErroMsg);

            RuleFor(voucher => voucher.DataValidade)
                .Must(ValidarSeDataVencimentoSuperiorAtual)
                .WithMessage(DataValidadeErroMsg);

            RuleFor(voucher => voucher.Ativo)
                .Equal(true)
                .WithMessage(AtivoErroMsg);

            RuleFor(voucher => voucher.Quantidade)
                .GreaterThan(0)
                .WithMessage(QuantidadeErroMsg);

            When(f => f.TipoDesconto == TipoDesconto.ValorFixo, () =>
            {
                RuleFor(f => f.ValorDesconto)
                    .NotNull()
                    .WithMessage(ValorDescontoErroMsg)
                    .GreaterThan(0)
                    .WithMessage(ValorDescontoErroMsg);
            });

            When(f => f.TipoDesconto == TipoDesconto.Percentual, () =>
            {
                RuleFor(f => f.PercentualDesconto)
                    .NotNull()
                    .WithMessage(PercentualDescontoErroMsg)
                    .GreaterThan(0)
                    .WithMessage(PercentualDescontoErroMsg);
            });
        }

        private bool ValidarSeDataVencimentoSuperiorAtual(DateTime dataValidade)
        {
            return dataValidade.Date >= DateTime.UtcNow.Date;
        }
    }
}
