using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WereWolves
{
    public class UdpState
    {
        public UdpClient u;
        public IPEndPoint e;
        public UdpState() { }
        public UdpState(UdpClient u, IPEndPoint e)
        {
            this.u = u;
            this.e = e;
        }
    }

    public class Client
    {
        short listenPort = 8282;

        Socket tcpClient;
        UdpClient udpClient;
        string localIP;
        public string receivedString;

        CommandBuilder builder;
        ClientData[] clients;
        int kpuId;
        int player_id;
        int day = 0;
        bool isDay = false;
        bool isLeader = false;

        public Client(short port)
        {
            listenPort = port;
            IPEndPoint e = new IPEndPoint(IPAddress.Any, listenPort);
            udpClient = new UdpClient(e);
            UdpState sta = new UdpState(udpClient, e);
            udpClient.BeginReceive(ReceiveCallback, sta);

            builder = new CommandBuilder();
        }

        ~Client()
        {
            tcpClient.Close();
            udpClient.Close();
        }

        public void setServer(string host, short port)
        {
            // clean resources used by socket up
            if (tcpClient.Connected)
                tcpClient.Close();               

            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpClient.Connect(host, port);
            localIP = (tcpClient.LocalEndPoint as IPEndPoint).Address.ToString();
        }

        private void startGame() { changePhase(true); }

        private void changePhase(bool isDay)
        {
            if (isDay)
            {
                day++;
                updateClientsList();
            }
            this.isDay = isDay;
        }

        private void voteKpu()
        {
            if (isLeader)
            {
                proposePrepare();
                proposeAccept();
            }
        }

        private void updateKPU()
        {
            // TODO : update KPU from server request
        }

        private void proposeAccept()
        {
            throw new NotImplementedException();
        }

        private void proposePrepare()
        {
            throw new NotImplementedException();
        }

        public void sendToServer(string command)
        {
            byte[] data = Encoding.ASCII.GetBytes(command);
            tcpClient.Send(data);
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

        public void join(string username) {
            sendToServer(builder.join(username, localIP, listenPort).build());
        }

        public void leave() { sendToServer(builder.leave().build()); }

        public void ready() { sendToServer(builder.ready().build()); }

        public ClientData[] list()
        {
            sendToServer(builder.listClient().build());
            // TODO : return from bucket
            return null;
        }

        public void updateClientsList()
        {
            clients = list();
            for (int i = clients.Length - 1, counter = 2; i >= 0 && counter > 0; i--)
            {
                if (clients[i].isAlive()) counter--;
                if (i == player_id) isLeader = true;
            }
        }

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
