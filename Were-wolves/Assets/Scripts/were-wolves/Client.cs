using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WereWolves
{
    class Client
    {
        TcpClient tcpClient;
        UdpClient udpClient;
        CommandBuilder builder;
        ClientData[] clients;
        short kpuId;

        public Client() {}

        public void setServer(string host, short port)
        {
            tcpClient = new TcpClient(host, port);
            udpClient = new UdpClient();
            builder = new CommandBuilder();
        }

        public void sendToServer(string command)
        {
            Byte[] data = Encoding.ASCII.GetBytes(command);

            NetworkStream stream = tcpClient.GetStream();
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Sent: {0}", command);


            data = new Byte[256];
            String responseData = String.Empty;

            int bytes = stream.Read(data, 0, data.Length);
            responseData = Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);

            stream.Close();
            tcpClient.Close();
        }

        public void sendToPeer(int id, string command)
        {
            ClientData peer = clients[id];
            udpClient.Connect(peer.getAddress(), peer.getPort());

            byte[] sendBytes = Encoding.ASCII.GetBytes(command);

            udpClient.Send(sendBytes, sendBytes.Length);

            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
            string returnData = Encoding.ASCII.GetString(receiveBytes);

            udpClient.Close();
        }

        public void join(string username) { sendToServer(builder.join(username).build()); }

        public void leave() { sendToServer(builder.leave().build()); }

        public void ready() { sendToServer(builder.ready().build()); }

        public ClientData[] list()
        {
            sendToServer(builder.listClient().build());
            return null;
        }

        public void updateClientsList() { clients = list(); }

        public void voteKill(bool werewolf, int id)
        {
            string command = werewolf ?
                builder.killWere(id).build() :
                builder.killCiv(id).build();

            sendToPeer(kpuId, command);
        }
    }
}
