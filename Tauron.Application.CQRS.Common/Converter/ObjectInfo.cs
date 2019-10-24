using System.Text.Json.Serialization;

namespace Tauron.Application.CQRS.Common.Converter
{
    public sealed class ObjectInfo
    {
        [JsonConverter(typeof(ObjectInfoConverter))]
        public object Element { get; set; }
    }
}