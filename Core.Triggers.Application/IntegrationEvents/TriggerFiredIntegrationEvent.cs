using Core.Triggers.Application.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Application.IntegrationEvents
{
    public class TriggerFiredIntegrationEvent : IntegrationEvent
    {
        public TriggerFiredIntegrationEvent(string triggerUid, string correlationUid, DateTime? firedOn)
        {
            TriggerUid = triggerUid;
            CorrelationUid = correlationUid;
            FiredOn = firedOn;
        }

        public string TriggerUid { get; }
        public string CorrelationUid { get; }
        public DateTime? FiredOn { get; }
    }
}
