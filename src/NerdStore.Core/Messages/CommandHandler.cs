using MediatR;
using NerdStore.Core.Messages.CommonMessages.Notifications;

namespace NerdStore.Core.Messages
{
    public abstract class CommandHandler
    {
        protected readonly IMediator _mediator;

        protected CommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected async Task<bool> ValidarSeCommandEhValido(Command command)
        {
            var ehValido = command.ValidarSeEhValido();

            if(!ehValido)
            {
                await LancarDomainNotifications(command);
            }

            return ehValido;
        }

        private async Task LancarDomainNotifications(Command command)
        {
            foreach (var erros in command.ValidationResult.Errors)
            {
                await _mediator.Publish(new DomainNotification(command.MessageType, erros.ErrorMessage));
            }
        }
    }
}
