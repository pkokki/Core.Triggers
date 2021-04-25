using Core.Triggers.Application.IntegrationEvents;
using Core.Triggers.Application.SeedWork;
using Core.Triggers.Domain.Events;
using Core.Triggers.Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Triggers.Application.DomainEventHandlers
{
    public class TriggerFiredDomainEventHandler : INotificationHandler<TriggerFiredDomainEvent>
    {
        private readonly IEventBus eventBus;

        public TriggerFiredDomainEventHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public async Task Handle(TriggerFiredDomainEvent ev, CancellationToken cancellationToken)
        {
            var trigger = ev.Trigger;
            await PublishIntegrationEvent(trigger);
        }

        private async Task PublishIntegrationEvent(Trigger trigger)
        {
            var integrationEvent = new TriggerFiredIntegrationEvent(trigger.TriggerUid, trigger.CorrelationUid, trigger.FiredOn);
            trigger.MarkTriggerPublished(integrationEvent.Uid);
            await eventBus.Publish(integrationEvent);
        }
    }
}
