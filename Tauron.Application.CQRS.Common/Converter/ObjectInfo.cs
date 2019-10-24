using System.Text.Json.Serialization;

namespace Tauron.Application.CQRS.Common.Converter
{
    [JsonConverter(typeof(ObjectInfoConverter))]
    public sealed class ObjectInfo
    {
        public object Element { get; set; }
    }
}