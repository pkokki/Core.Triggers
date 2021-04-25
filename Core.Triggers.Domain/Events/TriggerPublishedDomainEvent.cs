using Core.Triggers.Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Domain.Events
{
    public class TriggerPublishedDomainEvent : INotification
    {
        public TriggerPublishedDomainEvent(Trigger trigger, string triggerEventUid)
        {
            Trigger = trigger;
            TriggerEventUid = triggerEventUid;
        }

        public Trigger Trigger { get; }
        public string TriggerEventUid { get; }
    }
}
