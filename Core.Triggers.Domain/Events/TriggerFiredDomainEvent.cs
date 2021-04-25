using Core.Triggers.Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Domain.Events
{
    public class TriggerFiredDomainEvent : INotification
    {
        public TriggerFiredDomainEvent(Trigger trigger, DateTime firedOn)
        {
            Trigger = trigger;
            FiredOn = firedOn;
        }

        public Trigger Trigger { get; }
        public DateTime FiredOn { get; }
    }
}
