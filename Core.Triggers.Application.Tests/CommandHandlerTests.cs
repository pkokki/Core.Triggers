using Core.Triggers.Application.CommandHandlers;
using Core.Triggers.Application.Commands;
using Core.Triggers.Domain.Events;
using Core.Triggers.Domain.Exceptions;
using Core.Triggers.Domain.Model;
using Core.Triggers.Domain.SeedWork;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Core.Triggers.Application.Tests
{
    public class CommandHandlerTests
    {
        private readonly Mock<ITriggerRepository> repository;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<ILogger<ScheduleTriggerCommandHandler>> logger;

        public CommandHandlerTests()
        {
            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(u => u.SaveEntitiesAsync(default)).ReturnsAsync(true);
            repository = new Mock<ITriggerRepository>();
            repository.SetupGet(o => o.UnitOfWork).Returns(unitOfWork.Object);
            logger = new Mock<ILogger<ScheduleTriggerCommandHandler>>();
        }

        [Fact]
        public async Task ScheduleTrigger_ValidCommands_Success()
        {
            // Arrange
            var command1 = new ScheduleTriggerCommand("CORR_ID", new DateTime());
            var command2 = new ScheduleTriggerCommand("CORR_ID", new TimeSpan());
            var handler = new ScheduleTriggerCommandHandler(repository.Object, logger.Object);

            // Act
            var uid1 = await handler.Handle(command1, default);
            var uid2 = await handler.Handle(command2, default);

            // Assert
            Assert.NotNull(uid1);
            Assert.NotNull(uid2);
            repository.Verify(r => r.Add(It.Is<Trigger>(t => t.TriggerUid == uid1 || t.TriggerUid == uid2)), Times.Exactly(2));
        }

        [Fact]
        public async Task CancelTrigger_ExistingTrigger_Success()
        {
            // Arrange
            var trigger = new Trigger("CORR", new DateTime());
            repository.Setup(r => r.GetAsync("T1", default)).ReturnsAsync(trigger);

            var command = new CancelTriggerCommand("T1", "U1");
            var handler = new CancelTriggerCommandHandler(repository.Object, logger.Object);

            // Act
            var success = await handler.Handle(command, default);

            // Assert
            Assert.True(success);
            Assert.Single(trigger.DomainEvents.OfType<TriggerCancelledDomainEvent>());
        }

        [Fact]
        public async Task CancelTrigger_NotExistingTrigger_ReturnsFalse()
        {
            // Arrange
            Trigger trigger = null;
            repository.Setup(r => r.GetAsync("T1", default)).ReturnsAsync(trigger);

            var command = new CancelTriggerCommand("T1", "U1");
            var handler = new CancelTriggerCommandHandler(repository.Object, logger.Object);

            // Act
            var success = await handler.Handle(command, default);

            // Assert
            Assert.False(success);
            unitOfWork.Verify(r => r.SaveEntitiesAsync(default), Times.Never);
        }

        
    }
}
