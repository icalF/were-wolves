using System;
using Newtonsoft.Json;

namespace WereWolves
{
    [JsonConverter(typeof(ClientSerializer))]
    public class ClientData
    {
        static int globalId = 0;

        int id;
        bool is_alive;
        string address;
        short port;
        string username;
        bool is_werewolf;

        // constructor
        public ClientData(string username, string address, short port, bool is_werewolf = false)
        {
            id = globalId;
            globalId++;

            this.username = username;
            this.address = address;
            this.port = port;
            this.is_werewolf = is_werewolf;
            is_alive = true;
        }

        // getter
        public int getId() { return id; }
        public string getAddress() { return address; }
        public string getUsername() { return username; }
        public short getPort() { return port; }
        public bool isWerewolf() { return is_werewolf; }
        public bool isAlive() { return is_alive; }

        // setter
        public void killed() { is_alive = false; }
        public void setRole(bool is_werewolf) { this.is_werewolf = is_werewolf; }
    }

    public class ClientSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var clientData = value as ClientData;

            writer.WriteStartObject();

            writer.WritePropertyName("player_id");
            writer.WriteValue(clientData.getId());

            writer.WritePropertyName("is_alive");
            writer.WriteValue(clientData.isAlive() ? 1 : 0);

            writer.WritePropertyName("address");
            writer.WriteValue(clientData.getAddress());

            writer.WritePropertyName("port");
            writer.WriteValue(clientData.getPort());

            writer.WritePropertyName("username");
            writer.WriteValue(clientData.getUsername());

            if (!clientData.isAlive())
            {
                writer.WritePropertyName("role");
                writer.WriteValue(clientData.isWerewolf() ? "werewolf" : "civilian");
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                int[] arr = serializer.Deserialize<int[]>(reader);
                return new Tuple(arr[0], arr[1]);
            }
            else
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