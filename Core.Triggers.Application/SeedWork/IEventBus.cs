using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Triggers.Application.SeedWork
{
    public interface IEventBus
    {
        Task Publish(IntegrationEvent integrationEvent);
    }
}
