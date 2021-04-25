using Core.Triggers.Application.Commands;
using Core.Triggers.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Triggers.Application.CommandHandlers
{
    public class ScheduleTriggerCommandHandler : IRequestHandler<ScheduleTriggerCommand, string>
    {
        private readonly ITriggerRepository triggerRepository;
        private readonly ILogger<ScheduleTriggerCommandHandler> logger;

        public ScheduleTriggerCommandHandler(ITriggerRepository triggerRepository, ILogger<ScheduleTriggerCommandHandler> logger)
        {
            this.triggerRepository = triggerRepository ?? throw new ArgumentNullException(nameof(triggerRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> Handle(ScheduleTriggerCommand command, CancellationToken cancellationToken)
        {
            var firedOn = command.FireOn ?? DateTime.UtcNow.AddTicks(command.FireAfter.Value.Ticks);

            var trigger = new Trigger(command.CorrelationUid, firedOn);

            logger.LogInformation("----- Scheduling Trigger: {@Trigger}", trigger);

            triggerRepository.Add(trigger);

            await triggerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return trigger.TriggerUid;
        }
    }
}
