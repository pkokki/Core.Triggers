using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Application.Commands
{
    public class CancelTriggerCommand : IRequest<bool>
    {
        public string TriggerUid { get; }
        public string RequestedByUid { get; }

        public CancelTriggerCommand(string triggerUid, string requestedByUid)
        {
            if (string.IsNullOrEmpty(triggerUid))
                throw new ArgumentException(nameof(triggerUid));

            TriggerUid = triggerUid;
            RequestedByUid = requestedByUid;
        }
    }
}
