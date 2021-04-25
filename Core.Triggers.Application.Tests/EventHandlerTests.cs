using Core.Triggers.Application.DomainEventHandlers;
using Core.Triggers.Application.IntegrationEvents;
using Core.Triggers.Application.SeedWork;
using Core.Triggers.Domain.Events;
using Core.Triggers.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Core.Triggers.Application.Tests
{
    public class EventHandlerTests
    {
        [Fact]
        public async Task TriggerFired_ValidEvent_Success()
        {
            // Arrange
            var eventBus = new Mock<IEventBus>();
            var handler = new TriggerFiredDomainEventHandler(eventBus.Object);

            var trigger = new Trigger("CORR", new DateTime());
            var ev = new TriggerFiredDomainEvent(trigger, DateTime.UtcNow);

            // Act
            await handler.Handle(ev, default);

            // Assert
            Assert.NotNull(trigger.PublishEventUid);

            eventBus.Verify(b => b.Publish(It.Is<TriggerFiredIntegrationEvent>(e =>
                e.TriggerUid == trigger.TriggerUid
                && e.CorrelationUid == trigger.CorrelationUid
                && e.FiredOn == trigger.FiredOn)), Times.Once);
        }
    }
}
