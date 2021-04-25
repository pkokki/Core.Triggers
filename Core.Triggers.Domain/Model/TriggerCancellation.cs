using Core.Triggers.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Domain.Model
{
    public class TriggerCancellation : ValueObject
    {
        public string RequestedByUid { get; }
        public DateTime CancelledOn { get; }

        protected TriggerCancellation() { }

        public TriggerCancellation(string requestedByUid, DateTime cancelledOn)
        {
            RequestedByUid = requestedByUid ?? throw new ArgumentNullException(nameof(requestedByUid));
            CancelledOn = cancelledOn;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return RequestedByUid;
            yield return CancelledOn;
        }
    }
}
