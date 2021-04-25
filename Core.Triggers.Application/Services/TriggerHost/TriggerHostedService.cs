using Core.Triggers.Application.Services.Queries;
using Core.Triggers.Domain.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Triggers.Application.Services.TriggerHost
{
    public class TriggerHostedService : ITriggerHostedService
    {
        private readonly string serviceName;
        private readonly TriggerHostSettings settings;
        private readonly ITriggerQueries triggerQueries;
        private readonly ITriggerRepository triggerRepository;
        private readonly ILogger<TriggerHostedService> logger;

        public TriggerHostedService(
            TriggerHostSettings settings, 
            ITriggerQueries triggerQueries, 
            ITriggerRepository triggerRepository, 
            ILogger<TriggerHostedService> logger)
        {
            serviceName = $"{nameof(TriggerHostedService)}[{Guid.NewGuid()}]";
            this.settings = settings ?? TriggerHostSettings.DEFAULT;
            this.triggerQueries = triggerQueries ?? throw new ArgumentNullException(nameof(triggerQueries));
            this.triggerRepository = triggerRepository ?? throw new ArgumentNullException(nameof(triggerRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public uint Loops { get; private set; }
        public uint TriggersFired { get; private set; }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug("{0} is starting.", serviceName);
            stoppingToken.Register(() => logger.LogDebug("{0} is stopping.", serviceName));

            var stopwatch = new Stopwatch();
            var windowMilliseconds = settings.CheckEverySeconds * 1000;
            while (!stoppingToken.IsCancellationRequested)
            {
                stopwatch.Start();
                logger.LogDebug("{0} is checking for triggers to fire.", serviceName);
                
                await CheckTriggers(stoppingToken);
                ++Loops;

                stopwatch.Stop();
                var elapsed = stopwatch.ElapsedMilliseconds;
                if (elapsed < int.MaxValue)
                {
                    var delay = windowMilliseconds - Convert.ToInt32(stopwatch.ElapsedMilliseconds);
                    if (delay > 100)
                    {
                        logger.LogDebug("{0} is sleeping for {1} ms.", serviceName, delay);
                        await Task.Delay(delay, stoppingToken);
                    }
                }
            }

            logger.LogDebug("{0} stopped.", serviceName);
        }

        private async Task CheckTriggers(CancellationToken cancellationToken)
        {
            var triggerUids = await triggerQueries.GetScheduledTriggersBeforeAsync(DateTime.UtcNow, ct: cancellationToken);
            logger.LogDebug("{0} Found {1} triggers to fire.", serviceName, triggerUids.Count());

            foreach (var triggerUid in triggerUids)
            {
                var trigger = await triggerRepository.GetAsync(triggerUid, cancellationToken);
                if (trigger != null)
                {
                    try
                    {
                        trigger.Fire(DateTime.UtcNow);
                        await triggerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                        ++TriggersFired;
                        logger.LogDebug("{0} Trigger {1} fired.", serviceName, triggerUid);
                    }
                    catch (Exception ex)
                    {
                        logger.LogDebug("{0} Trigger {1} failed to fire: {2}", serviceName, triggerUid, ex);
                    }
                }
            }
        }
    }
}
