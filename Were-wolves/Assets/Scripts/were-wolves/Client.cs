namespace WereWolves
{
    public class Client
    {
        private bool is_alive;
        private string address;
        private short port;
        private string username;

        // constructor
        public Client(string username, string address, short port)
        {
            this.username = username;
            this.address = address;
            this.port = port;
            is_alive = true;
        }

        // setter
        public void killed() { is_alive = false; }
    }
}