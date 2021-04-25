using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Triggers.Application.SeedWork
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Uid = Guid.NewGuid().ToString();
            OccurredOn = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(string id, DateTime occurredOn)
        {
            Uid = id;
            OccurredOn = occurredOn;
        }

        [JsonProperty]
        public string Uid { get; private set; }

        [JsonProperty]
        public DateTime OccurredOn { get; private set; }
    }
}
