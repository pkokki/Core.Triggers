using Core.Triggers.Domain.Events;
using Core.Triggers.Domain.Exceptions;
using Core.Triggers.Domain.Model;
using System;
using System.Linq;
using Xunit;

namespace Core.Triggers.Domain.Tests
{
    public class CancelTriggerTests
    {
        [Fact]
        public void CancelTrigger_NewTrigger_Success()
        {
            // Given (events in the past)
            var cancelInfo = new TriggerCancellation("U1", DateTime.UtcNow);
            var trigger = new Trigger("X1", DateTime.UtcNow.AddSeconds(5));

            // When (an aggregate method is called)
            trigger.Cancel(cancelInfo);

            // Expect (the following Events or an exception)
            Assert.Null(trigger.FiredOn);
            Assert.Equal(cancelInfo, trigger.Cancellation);

            var ev = trigger.DomainEvents.OfType<TriggerCancelledDomainEvent>().SingleOrDefault();
            Assert.Equal(trigger, ev.Trigger);
            Assert.Equal(cancelInfo, ev.Cancellation);
        }

        [Fact]
        public void CancelTrigger_FiredTrigger_Throws()
        {
            // Given (events in the past)
            var cancelInfo = new TriggerCancellation("U1", DateTime.UtcNow);
            var trigger = new Trigger("X1", DateTime.UtcNow.AddSeconds(5));
            trigger.Fire(DateTime.UtcNow);

            // When
            void action() => trigger.Cancel(cancelInfo);

            // Expect
            Assert.Throws<AlreadyFiredTriggerException>(action);
        }

        [Fact]
        public void CancelTrigger_CancelledTrigger_Ignored()
        {
            // Given (events in the past)
            var cancelInfo1 = new TriggerCancellation("U1", DateTime.UtcNow);
            var cancelInfo2 = new TriggerCancellation("U1", DateTime.UtcNow.AddSeconds(1));
            var trigger = new Trigger("X1", DateTime.UtcNow.AddSeconds(5));
            trigger.Cancel(cancelInfo1);

            // When
            trigger.Cancel(cancelInfo2);

            // Expect
            Assert.Equal(cancelInfo1, trigger.Cancellation);
            var ev = trigger.DomainEvents.OfType<TriggerCancelledDomainEvent>().SingleOrDefault();
            Assert.Equal(trigger, ev.Trigger);
            Assert.Equal(cancelInfo1, ev.Cancellation);
        }
    }
}
