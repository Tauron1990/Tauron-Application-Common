using Tauron.Application.CQRS.Client.Querys;

namespace EventDeliveryTest.Test
{
    public class TestData : IQueryResult
    {
        public string  Parameter { get; set; }

        public TestData()
        {
            
        }

        public TestData(string parameter) => Parameter = parameter;
    }
}