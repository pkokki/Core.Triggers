using Core.Triggers.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Domain.Exceptions
{
    public class CannotCancelFiredTriggerException : Exception
    {
        public CannotCancelFiredTriggerException(Trigger trigger, string cancelelledByUid)
        {
        }
    }
}
