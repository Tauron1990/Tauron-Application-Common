using System.Threading.Tasks;
using Tauron.Application.CQRS.Client;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Common.Dto;

namespace EventDeliveryTest.Test
{
    [CQRSHandler]
    public class TestCommandHandler : CommandHandlerBase, ICommandHandler<TestCommand>
    {
        async Task<OperationResult> ICommandHandler<TestCommand>.Handle(TestCommand command)
        {
            var aggregate = await Session.GetAggregate<TestAggregate>(TestAggregate.IdField);
            await aggregate.SetLastValue(command.Message);

            return OperationResult.Success;
        }
    }
}