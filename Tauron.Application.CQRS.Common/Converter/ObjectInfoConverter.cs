using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tauron.Application.CQRS.Common.Converter
{
    public class ObjectInfoConverter : JsonConverter<ObjectInfo>
    {
        private const string IsNullMarker = "isNull";
        //private const string InfoObjectObject = "objectInfo";
        private const string TypeInfo = "typeInfo";
        private const string Objectlocation = "elementObject";

        public override ObjectInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            TryRead(ref reader, JsonTokenType.PropertyName);
            CheckProperty(IsNullMarker, reader.GetString());
            reader.Read();

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                    TryRead(ref reader, JsonTokenType.EndObject);
                    return new ObjectInfo();
                case JsonTokenType.False:
                    TryRead(ref reader, JsonTokenType.PropertyName);
                    CheckProperty(TypeInfo, reader.GetString());

                    TryRead(ref reader, JsonTokenType.String);
                    Type type = Type.GetType(reader.GetString(), true);
                    
                    TryRead(ref reader, JsonTokenType.PropertyName);
                    CheckProperty(Objectlocation, reader.GetString());

                    TryRead(ref reader, JsonTokenType.String);
                    var content = JsonSerializer.Deserialize(reader.GetString(), type, options);

                    //TryRead(ref reader, JsonTokenType.EndObject);
                    TryRead(ref reader, JsonTokenType.EndObject);

                    return new ObjectInfo { Element = content };
                default:
                    throw new JsonException("Wrong Json Format");
            }
        }

        private void CheckProperty(string expected, string ist)
        {
            if(expected == ist) return;

            throw new JsonException($"Worng ProperyName: {ist}. Expected: {expected}");
        }

        private void TryRead(ref Utf8JsonReader reader, JsonTokenType expectedTokenType)
        {
            if (!reader.Read()) throw new JsonException("Json Read Failed");

            if(reader.TokenType != expectedTokenType)
                throw new JsonException("Wrong Json Format");

            if (!Enum.IsDefined(typeof(JsonTokenType), expectedTokenType)) 
                throw new InvalidEnumArgumentException(nameof(expectedTokenType), (int) expectedTokenType, typeof(JsonTokenType));
        }

        public override void Write(Utf8JsonWriter writer, ObjectInfo value, JsonSerializerOptions options)
        {
            //JsonEncodedText.Encode(InfoObjectObject, options.Encoder)
            writer.WriteStartObject();

            if (value.Element == null)
                writer.WriteBoolean(JsonEncodedText.Encode(IsNullMarker), true);
            else
            {
                writer.WriteBoolean(JsonEncodedText.Encode(IsNullMarker), false);
                writer.WriteString(JsonEncodedText.Encode(TypeInfo, options.Encoder), value.Element.GetType().AssemblyQualifiedName);
                //writer.WriteStartObject(JsonEncodedText.Encode(Objectlocation, options.Encoder));
                writer.WriteString(JsonEncodedText.Encode(Objectlocation, options.Encoder) , JsonSerializer.Serialize(value.Element, options));
                //writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }
}