using System.ComponentModel.DataAnnotations;

namespace Tauron.Application.CQRS.Dispatcher.EventStore.Data
{
    public class ApiKey
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}