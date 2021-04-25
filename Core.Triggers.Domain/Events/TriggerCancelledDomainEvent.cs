using Core.Triggers.Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Domain.Events
{
    public class TriggerCancelledDomainEvent : INotification
    {
        public TriggerCancelledDomainEvent(Trigger trigger, TriggerCancellation cancellation)
        {
            Trigger = trigger;
            Cancellation = cancellation;
        }

        public Trigger Trigger { get; }
        public TriggerCancellation Cancellation { get; }
    }
}
