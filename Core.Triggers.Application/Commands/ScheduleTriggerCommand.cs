using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Application.Commands
{
    public class ScheduleTriggerCommand : IRequest<string>
    {
        public string CorrelationUid { get; }
        public DateTime? FireOn { get; }
        public TimeSpan? FireAfter { get; }

        private ScheduleTriggerCommand(string correlationUid)
        {
            if (string.IsNullOrEmpty(correlationUid))
                throw new ArgumentException(nameof(correlationUid));

            CorrelationUid = correlationUid;
        }
        public ScheduleTriggerCommand(string correlationUid, DateTime fireOn)
            : this(correlationUid)
        {
            FireOn = fireOn;
        }
        public ScheduleTriggerCommand(string correlationUid, TimeSpan fireAfter)
            : this(correlationUid)
        {
            FireAfter = fireAfter;
        }
    }
}
