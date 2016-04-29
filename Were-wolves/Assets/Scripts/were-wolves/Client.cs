using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WereWolves
{
    public class Client
    {
        short listenPort = 8282;

        Socket tcpClient;
        UdpClient udpClient;
        CommandBuilder builder;
        ClientData[] clients;
        string localIP;
        public string receivedString;
        short kpuId;

        public Client(short port)
        {
            listenPort = port;
            IPEndPoint e = new IPEndPoint(IPAddress.Any, listenPort);
            udpClient = new UdpClient(e);
            UdpState sta = new UdpState(udpClient, e);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), sta);

            builder = new CommandBuilder();
        }

        ~Client()
        {
            //tcpClient.Dispose();
            tcpClient.Close();

            //udpClient.Dispose();
            udpClient.Close();
        }

        public void setServer(string host, short port)
        {
            // clean resources used by socket up
            //tcpClient.Dispose();
            tcpClient.Close();              

            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            tcpClient.Connect(host, port);
            localIP = (tcpClient.LocalEndPoint as IPEndPoint).Address.ToString();
        }

        public void sendToServer(string command)
        {
            byte[] data = Encoding.ASCII.GetBytes(command);

            tcpClient.Send(data);
            Console.WriteLine("Sent: {0}", command);

            data = new byte[256];
            string responseData = string.Empty;
        }

        public void sendToPeer(int id, string command)
        {
            ClientData peer = clients[id];
            IPAddress ipAddress = Dns.GetHostEntry(peer.getAddress()).AddressList[0];

            sendUdp(new IPEndPoint(ipAddress, peer.getPort()), command);
        }

        public void sendUdp(IPEndPoint remote, string command)
        {
            udpClient.Connect(remote);
            byte[] sendBytes = Encoding.ASCII.GetBytes(command);
            udpClient.Send(sendBytes, sendBytes.Length);
        }

        public void join(string username, string address, short port) { sendToServer(builder.join(username, address, port).build()); }

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

        public void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            byte[] receiveBytes = u.EndReceive(ar, ref e);
            receivedString = Encoding.ASCII.GetString(receiveBytes);
            Console.WriteLine(receivedString);
        }
    }
}
