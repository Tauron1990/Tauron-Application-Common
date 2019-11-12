using System.Threading.Tasks;
using Tauron.Application.CQRS.Client;
using Tauron.Application.CQRS.Client.Domain;
using Tauron.Application.CQRS.Client.Querys;

namespace EventDeliveryTest.Test
{
    [CQRSHandler]
    public class TestReadModel : IReadModel<TestQueryData, TestData>
    {
        private readonly ISession _session;

        public TestReadModel(ISession session) 
            => _session = session;

        public async Task<TestData> Query(TestQueryData query)
        {
            var aggregate = await _session.GetAggregate<TestAggregate>(TestAggregate.IdField);

            return new TestData(aggregate.LastValue);
        }
    }
}