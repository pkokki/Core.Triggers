using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Triggers.Application.Services.Queries
{
    public interface ITriggerQueries
    {
        Task<IEnumerable<string>> GetScheduledTriggersBeforeAsync(DateTime before, int maxCount = 100, CancellationToken ct = default);
    }
}
