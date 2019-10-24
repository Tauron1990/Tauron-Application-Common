using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tauron.Application.CQRS.Common.Converter
{
    public class ObjectInfoConverter : JsonConverter<ObjectInfo>
    {
        public override ObjectInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string type;
            if(!reader.Read()) throw new InvalidOperationException();
            type = reader.GetString();

            if (!reader.Read()) throw new InvalidOperationException();
            return new ObjectInfo
                   {
                       Element = JsonSerializer.Deserialize(reader.ValueSpan, Type.GetType(type), options)
                   };
        }

        public override void Write(Utf8JsonWriter writer, ObjectInfo value, JsonSerializerOptions options)
        {
            writer.WriteString(JsonEncodedText.Encode("Type", options.Encoder), value.Element.GetType().AssemblyQualifiedName);

        }
    }
}