using EventDeliveryTest.Test;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSTestApp
{
    class Program
    {


        static void Main(string[] args)
        {
            var str = System.Text.Json.JsonSerializer.Serialize(new TestCommand {Message = "Test"});
        }
    }
}
