namespace WereWolves
{
    public class ClientData
    {
        private bool is_alive;
        private string address;
        private short port;
        private string username;

        // constructor
        public ClientData(string username, string address, short port)
        {
            this.username = username;
            this.address = address;
            this.port = port;
            is_alive = true;
        }

        // getter
        public string getAddress() { return address; }
        public short getPort() { return port; }

        // setter
        public void killed() { is_alive = false; }
    }
}