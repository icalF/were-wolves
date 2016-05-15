using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WereWolves
{
    [JsonConverter(typeof(ClientSerializer))]
    public class ClientData : object
    {
        static int globalId;
        private string endPoint;

        public int player_id { get; set; }
        public bool is_alive { get; set; }
        public string address { get; set; }
        public short port { get; set; }
        public string username { get; set; }
        public bool is_werewolf { get; set; } /*= false;*/

        // constructor
        public ClientData(string username, string address, short port, bool is_werewolf = false)
        {
            player_id = globalId;
            globalId++;

            this.username = username;
            this.address = address;
            this.port = port;
            this.is_werewolf = is_werewolf;
            is_alive = true;
        }

        // getter
        public int getId() { return player_id; }
        public string getAddress() { return address; }
        public string getUsername() { return username; }
        public short getPort() { return port; }
        public bool isWerewolf() { return is_werewolf; }
        public bool isAlive() { return is_alive; }
        public string getEndPoint() { return endPoint; }

        // setter
        public void killed() { is_alive = false; }
        public void setRole(bool is_werewolf) { this.is_werewolf = is_werewolf; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ClientData p = obj as ClientData;
            if (p == null)
            {
                return false;
            }

            return Equals(p);
        }

        public bool Equals(ClientData p)
        {
            if (p == null)
            {
                return false;
            }

            return (getUsername() == p.getUsername()) 
                && (getAddress() == p.getAddress())
                && (getPort() == p.getPort())
                && (isAlive() == p.isAlive())
                && (isWerewolf() == p.isWerewolf())
                && (getEndPoint() == p.getEndPoint());
        }

        public void addEndPoint(string v)
        {
            endPoint = v;
        }
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
            if (reader.TokenType == JsonToken.StartObject)
            {
                var dict = serializer.Deserialize<Dictionary<string, string>>(reader);
                var cl = new ClientData(
                    dict["username"],
                    dict["address"],
                    Convert.ToInt16(dict["port"])
                );
                if (dict.ContainsKey("is_werewolf"))
                    cl.setRole(dict["is_werewolf"].Equals("werewolf"));

                return cl;
            }
            throw new JsonSerializationException();
        }

        public override bool CanConvert(Type objectType)
        {
            //return typeof(Tuple).IsAssignableFrom(objectType);
            return true;
            //throw new NotImplementedException();
        }
    }
}