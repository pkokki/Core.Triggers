using Core.Triggers.Domain.Events;
using Core.Triggers.Domain.Exceptions;
using Core.Triggers.Domain.Model;
using System;
using System.Linq;
using Xunit;

namespace Core.Triggers.Domain.Tests
{
    public class FireTriggerTests
    {
        [Fact]
        public void FireTrigger_NewTrigger_Success()
        {
            // Given (events in the past)
            var firedOn = DateTime.UtcNow;
            var trigger = new Trigger("X1", DateTime.UtcNow.AddSeconds(5));

            // When (an aggregate method is called)
            trigger.Fire(firedOn);

            // Expect (the following Events or an exception)
            Assert.Equal(firedOn, trigger.FiredOn);
            Assert.Null(trigger.Cancellation);

            var ev = trigger.DomainEvents.OfType<TriggerFiredDomainEvent>().SingleOrDefault();
            Assert.Equal(trigger, ev.Trigger);
            Assert.Equal(firedOn, ev.FiredOn);
        }

        [Fact]
        public void FireTrigger_FiredTrigger_Ignored()
        {
            // Given (events in the past)
            var firedOn1 = DateTime.UtcNow;
            var firedOn2 = firedOn1.AddSeconds(1);
            var trigger = new Trigger("X1", firedOn1.AddSeconds(5));
            trigger.Fire(firedOn1);

            // When
            trigger.Fire(firedOn2);

            // Expect
            Assert.Equal(firedOn1, trigger.FiredOn);
            Assert.Null(trigger.Cancellation);
            var ev = trigger.DomainEvents.OfType<TriggerFiredDomainEvent>().SingleOrDefault();
            Assert.Equal(trigger, ev.Trigger);
            Assert.Equal(firedOn1, ev.FiredOn);
        }

        [Fact]
        public void FireTrigger_CancelledTrigger_Throws()
        {
            // Given (events in the past)
            var trigger = new Trigger("X1", DateTime.UtcNow.AddSeconds(5));
            trigger.Cancel(new TriggerCancellation("U1", DateTime.UtcNow));

            // When
            void action() => trigger.Fire(DateTime.UtcNow);

            // Expect
            Assert.Throws<AlreadyCancelledTriggerException>(action);
        }
    }
}
