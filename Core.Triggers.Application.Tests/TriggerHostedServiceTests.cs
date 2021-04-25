using Core.Triggers.Application.SeedWork;
using Core.Triggers.Application.Services.Queries;
using Core.Triggers.Application.Services.TriggerHost;
using Core.Triggers.Domain.Model;
using Core.Triggers.Domain.SeedWork;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Core.Triggers.Application.Tests
{
    public class TriggerHostedServiceTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<ITriggerQueries> triggerQueries;
        private readonly Mock<ITriggerRepository> triggerRepository;
        private readonly Mock<ILogger<TriggerHostedService>> logger;
        private readonly TriggerHostedService triggerHostedService;

        public TriggerHostedServiceTests()
        {
            triggerQueries = new Mock<ITriggerQueries>();
            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(u => u.SaveEntitiesAsync(default)).ReturnsAsync(true);
            triggerRepository = new Mock<ITriggerRepository>();
            triggerRepository.SetupGet(o => o.UnitOfWork).Returns(unitOfWork.Object);
            logger = new Mock<ILogger<TriggerHostedService>>();
            triggerHostedService = new TriggerHostedService(
                TriggerHostSettings.DEFAULT,
                triggerQueries.Object,
                triggerRepository.Object,
                logger.Object
                );
        }

        [Fact]
        public void LoopWithTwoValidTriggers_Success()
        {
            // Arrange
            var trigger1 = new Trigger("C1", new DateTime(2042, 1, 1));
            var trigger2 = new Trigger("C2", new DateTime(2042, 1, 1));
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            triggerQueries
                .Setup(q => q.GetScheduledTriggersBeforeAsync(It.IsAny<DateTime>(), It.IsAny<int>(), cts.Token))
                .ReturnsAsync(new string[] { trigger1.TriggerUid, trigger2.TriggerUid });
            triggerRepository.Setup(r => r.GetAsync(trigger1.TriggerUid, cts.Token)).ReturnsAsync(trigger1);
            triggerRepository.Setup(r => r.GetAsync(trigger2.TriggerUid, cts.Token)).ReturnsAsync(trigger2);
            
            // Act
            _ = triggerHostedService.ExecuteAsync(cts.Token);
            cts.Token.WaitHandle.WaitOne();
            
            // Assert
            Assert.NotNull(trigger1.FiredOn);
            Assert.NotNull(trigger2.FiredOn);
            Assert.Equal(2U, triggerHostedService.TriggersFired);
            Assert.Equal(1U, triggerHostedService.Loops);
        }

        [Fact]
        public void LoopWithNotFoundTrigger_Success()
        {
            // Arrange
            var trigger1 = new Trigger("C1", new DateTime(2042, 1, 1));
            var trigger2 = new Trigger("C2", new DateTime(2042, 1, 1));
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            triggerQueries
                .Setup(q => q.GetScheduledTriggersBeforeAsync(It.IsAny<DateTime>(), It.IsAny<int>(), cts.Token))
                .ReturnsAsync(new string[] { trigger1.TriggerUid, trigger2.TriggerUid });
            triggerRepository.Setup(r => r.GetAsync(trigger1.TriggerUid, cts.Token)).ReturnsAsync(default(Trigger));
            triggerRepository.Setup(r => r.GetAsync(trigger2.TriggerUid, cts.Token)).ReturnsAsync(trigger2);
            
            // Act
            _ = triggerHostedService.ExecuteAsync(cts.Token);
            cts.Token.WaitHandle.WaitOne();

            // Assert
            Assert.Null(trigger1.FiredOn);
            Assert.NotNull(trigger2.FiredOn);
            Assert.Equal(1U, triggerHostedService.TriggersFired);
            Assert.Equal(1U, triggerHostedService.Loops);
        }

        [Fact]
        public void LoopWithCancelledTrigger_Success()
        {
            // Arrange
            var trigger1 = new Trigger("C1", new DateTime(2042, 1, 1));
            trigger1.Cancel(new TriggerCancellation("U1", DateTime.UtcNow));
            var trigger2 = new Trigger("C2", new DateTime(2042, 1, 1));
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            triggerQueries
                .Setup(q => q.GetScheduledTriggersBeforeAsync(It.IsAny<DateTime>(), It.IsAny<int>(), cts.Token))
                .ReturnsAsync(new string[] { trigger1.TriggerUid, trigger2.TriggerUid });
            triggerRepository.Setup(r => r.GetAsync(trigger1.TriggerUid, cts.Token)).ReturnsAsync(trigger1);
            triggerRepository.Setup(r => r.GetAsync(trigger2.TriggerUid, cts.Token)).ReturnsAsync(trigger2);
            
            // Act
            _ = triggerHostedService.ExecuteAsync(cts.Token);
            cts.Token.WaitHandle.WaitOne();

            // Assert
            Assert.Null(trigger1.FiredOn);
            Assert.NotNull(trigger2.FiredOn);
            Assert.Equal(1U, triggerHostedService.TriggersFired);
            Assert.Equal(1U, triggerHostedService.Loops);
        }

        [Fact]
        public void Run_WithDelay_CorrectLoops()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.CancelAfter(3500);

            triggerQueries
                .Setup(q => q.GetScheduledTriggersBeforeAsync(It.IsAny<DateTime>(), It.IsAny<int>(), cts.Token))
                .ReturnsAsync(new string[0]);
            var customHostedService = new TriggerHostedService(
                new TriggerHostSettings()
                {
                    CheckEverySeconds = 1
                },
                triggerQueries.Object,
                triggerRepository.Object,
                logger.Object
                );

            // Act
            _ = customHostedService.ExecuteAsync(cts.Token);
            cts.Token.WaitHandle.WaitOne();

            // Assert
            Assert.Equal(4U, customHostedService.Loops); // The checks are at 0, 1, 2, 3
        }
    }
}
