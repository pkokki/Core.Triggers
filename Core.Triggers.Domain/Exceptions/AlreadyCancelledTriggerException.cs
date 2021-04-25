using Core.Triggers.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Domain.Exceptions
{
    public class AlreadyCancelledTriggerException : Exception
    {
        public AlreadyCancelledTriggerException(Trigger trigger, TriggerCancellation cancellation)
        {
        }
    }
}
