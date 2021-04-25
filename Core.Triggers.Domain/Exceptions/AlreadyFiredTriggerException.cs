using Core.Triggers.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Domain.Exceptions
{
    public class AlreadyFiredTriggerException : Exception
    {
        public AlreadyFiredTriggerException(Trigger trigger, DateTime? firedOn)
        {
        }
    }
}
