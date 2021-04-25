using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Triggers.Application.Services.TriggerHost
{
    public interface ITriggerHostedService
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
