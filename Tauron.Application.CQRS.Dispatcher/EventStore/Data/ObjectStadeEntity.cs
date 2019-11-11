using System.ComponentModel.DataAnnotations;

namespace Tauron.Application.CQRS.Dispatcher.EventStore.Data
{
    public class ObjectStadeEntity
    {
        [Key]
        public string? Identifer { get; set; }

        //public string OriginType { get; set; }

        public string? Data { get; set; }
    }
}