using Core.Triggers.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Triggers.Domain.Model
{
    public interface ITriggerRepository : IRepository<Trigger>
    {
        Trigger Add(Trigger trigger);
        Task<Trigger> GetAsync(string triggerUid, CancellationToken ct = default);
    }
}
