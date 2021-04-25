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
    public class CancelTriggerCommandHandler : IRequestHandler<CancelTriggerCommand, bool>
    {
        private readonly ITriggerRepository triggerRepository;
        private readonly ILogger<ScheduleTriggerCommandHandler> logger;

        public CancelTriggerCommandHandler(ITriggerRepository triggerRepository, ILogger<ScheduleTriggerCommandHandler> logger)
        {
            this.triggerRepository = triggerRepository ?? throw new ArgumentNullException(nameof(triggerRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CancelTriggerCommand command, CancellationToken cancellationToken)
        {
            var triggerToCancel = await triggerRepository.GetAsync(command.TriggerUid);
            if (triggerToCancel == null)
                return false;

            logger.LogInformation("----- Canceling Trigger: {@Trigger}", triggerToCancel);
            var info = new TriggerCancellation(command.RequestedByUid, DateTime.UtcNow);
            triggerToCancel.Cancel(info);

            return await triggerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
