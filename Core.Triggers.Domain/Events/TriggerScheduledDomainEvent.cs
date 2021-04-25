using Core.Triggers.Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Domain.Events
{
    public class TriggerScheduledDomainEvent : INotification
    {
        public TriggerScheduledDomainEvent(Trigger trigger, string correlationUid, DateTime firesOn)
        {
            Trigger = trigger;
            CorrelationUid = correlationUid;
            FiresOn = firesOn;
        }

        public Trigger Trigger { get; }
        public string CorrelationUid { get; }
        public DateTime FiresOn { get; }
    }
}
