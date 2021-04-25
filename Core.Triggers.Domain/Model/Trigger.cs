using Core.Triggers.Domain.Events;
using Core.Triggers.Domain.Exceptions;
using Core.Triggers.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Domain.Model
{
    public class Trigger : Entity, IAggregateRoot
    {
        public string TriggerUid { get; }
        public string CorrelationUid { get; private set; }
        public DateTime FiresOn { get; private set; }
        public DateTime? FiredOn { get; private set; }
        public TriggerCancellation Cancellation { get; private set; }
        public string PublishEventUid { get; private set; }

        protected Trigger()
        {
        }

        public Trigger(string correlationUid, DateTime firesOn)
        {
            TriggerUid = Guid.NewGuid().ToString();
            CorrelationUid = correlationUid;
            FiresOn = firesOn;

            AddDomainEvent(new TriggerScheduledDomainEvent(this, correlationUid, FiresOn));
        }

        public void Fire(DateTime firedOn)
        {
            if (FiredOn.HasValue)
                return;
            if (Cancellation != null)
                throw new AlreadyCancelledTriggerException(this, Cancellation);

            FiredOn = firedOn;

            AddDomainEvent(new TriggerFiredDomainEvent(this, FiredOn.Value));
        }

        public void Cancel(TriggerCancellation info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            if (FiredOn.HasValue)
                throw new AlreadyFiredTriggerException(this, FiredOn);
            if (Cancellation != null)
                return;

            Cancellation = info;

            AddDomainEvent(new TriggerCancelledDomainEvent(this, Cancellation));
        }

        public void MarkTriggerPublished(string publishEventUid)
        {
            PublishEventUid = publishEventUid;

            AddDomainEvent(new TriggerPublishedDomainEvent(this, publishEventUid));
        }
    }
}
