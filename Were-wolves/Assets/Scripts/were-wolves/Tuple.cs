using System;
using Newtonsoft.Json;

namespace WereWolves
{
    [JsonConverter(typeof(TupleSerializer))]
    public class Tuple
    {
        public int First { get; private set; }
        public int Second { get; private set; }
        internal Tuple(int first, int second)
        {
            First = first;
            Second = second;
        }
    }

    public class TupleSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var tuple = value as Tuple;
            writer.WriteRaw("(");
            serializer.Serialize(writer, tuple.First);
            serializer.Serialize(writer, tuple.Second);
            writer.WriteRaw(")");
            //throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            //return typeof(Tuple).IsAssignableFrom(objectType);
            return true;
            //throw new NotImplementedException();
        }
    }
}
