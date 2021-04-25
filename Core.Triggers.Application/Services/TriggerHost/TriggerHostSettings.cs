using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Application.Services.TriggerHost
{
    public class TriggerHostSettings
    {
        public static readonly TriggerHostSettings DEFAULT = new TriggerHostSettings()
        {
            CheckEverySeconds = 10
        };

        public int CheckEverySeconds { get; set; }
    }
}
