using System;
using Newtonsoft.Json;

namespace WereWolves
{
    [JsonConverter(typeof(TupleSerializer))]
    public class Tuple : object
    {
        public int First { get; private set; }
        public int Second { get; private set; }
        public Tuple(int first, int second) { First = first; Second = second; }
        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Tuple p = obj as Tuple;
            if (p == null)
            {
                return false;
            }

            return Equals(p);
        }
        public bool Equals(Tuple p)
        {
            if (p == null)
            {
                return false;
            }

            return (First == p.First) && (Second == p.Second);
        }
    }

    public class TupleSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var tuple = value as Tuple;
            writer.WriteStartArray();
            writer.WriteValue(tuple.First);
            writer.WriteValue(tuple.Second);
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                int[] arr = serializer.Deserialize<int[]>(reader);
                return new Tuple(arr[0], arr[1]);
            } else
            {
                throw new JsonSerializationException();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            //return typeof(Tuple).IsAssignableFrom(objectType);
            return true;
            //throw new NotImplementedException();
        }
    }
}
