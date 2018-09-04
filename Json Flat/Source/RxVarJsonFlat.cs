using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Rx.Net.Plus
{
    public class RxVarJsonFlatConverter : JsonConverter
    {
        static Type GetRxVarValueType(Type objectType)
        {
            return objectType
                .BaseTypesAndSelf()
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(RxVar<>))
                .Select(t => t.GetGenericArguments()[0])
                .FirstOrDefault();
        }

        public override bool CanConvert(Type objectType)
        {
            bool isRxVar = (GetRxVarValueType(objectType) != null);
            return isRxVar;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // You need to decide whether a null JSON token results in a null ValueObject<T> or 
            // an allocated ValueObject<T> with a null Value.
            if (reader.SkipComments().TokenType == JsonToken.Null)
            {
                return null;
            }

            var valueType = GetRxVarValueType(objectType);
            var value = serializer.Deserialize(reader, valueType);

            // Create a RxVar (or derived) with initial value
            return Activator.CreateInstance(objectType, value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((IAsObject)value).AsObject);
        }
    }

    public static class RxVarFlatJsonExtensions
    {
        public static JsonReader SkipComments(this JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment && reader.Read())
            {
            }

            return reader;
        }

        public static void RegisterToJsonGlobalSettings()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = { new RxVarJsonFlatConverter() },
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
        }
    }

    public static class TypeExtensions
    {
        public static IEnumerable<Type> BaseTypesAndSelf(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}
