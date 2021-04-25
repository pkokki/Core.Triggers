using Core.Triggers.Domain.Model;
using System;
using System.Linq;
using Xunit;
using Core.Triggers.Domain.Events;

namespace Core.Triggers.Domain.Tests
{
    public class CreateTriggerTests
    {
        [Fact]
        public void CreateTrigger_WithFiresOn_Success()
        {
            // Given (events in the past)
            var correlationKey = "CORR1";
            var firesOn = DateTime.UtcNow.AddSeconds(5);

            // When (an aggregate method is called)
            var trigger = new Trigger(correlationKey, firesOn);

            // Expect (the following Events or an exception)
            Assert.NotNull(trigger.TriggerUid);
            Assert.Equal(correlationKey, trigger.CorrelationUid);
            Assert.Equal(firesOn, trigger.FiresOn);
            Assert.Null(trigger.FiredOn);
            Assert.Null(trigger.Cancellation);
            Assert.Null(trigger.PublishEventUid);

            var ev = trigger.DomainEvents.OfType<TriggerScheduledDomainEvent>().SingleOrDefault();
            Assert.Equal(trigger, ev.Trigger);
            Assert.Equal(trigger.CorrelationUid, ev.CorrelationUid);
            Assert.Equal(trigger.FiresOn, ev.FiresOn);
        }
        
    }
}
