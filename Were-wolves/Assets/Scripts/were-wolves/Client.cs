using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
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
        public static ManualResetEvent allDone =
        new ManualResetEvent(false);
        public static ManualResetEvent connectDone =
       new ManualResetEvent(false);
        public static ManualResetEvent sentDone =
       new ManualResetEvent(false);
        public static ManualResetEvent recvDone =
       new ManualResetEvent(false);
        public Socket tcpClient;
        UdpClient udpClient;
        CommandBuilder builder;
        ClientData[] clients;
        IPEndPoint e;
        string localIP;
        public string receivedString;
        short kpuId;
        string queue;
        string yangterlempar = "";

        int player_id;
        int day = 0;
        bool isDay = false;
        bool isLeader = false;

        public Client(short port)
        {
            //allDone.Reset();
            listenPort = port;
            e = new IPEndPoint(IPAddress.Any, listenPort);
            udpClient = new UdpClient(e);
            Thread loopThread = new Thread(loop);
            loopThread.Start();
            builder = new CommandBuilder();
        }
        private void loop()
        {
            int i = 0;
            while (i < 2)
            {
                UdpState sta = new UdpState(udpClient, e);
                udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), sta);
                i++;
            }
        }
        private void recvloop()
        {
            int i = 0;
            while (i < 2)
            {
                sentDone.WaitOne();
            }
        }
        private void asynrecvloop()
        {

            int i = 0;
            while (i < 2)
            {
                recvDone.Reset();
                connectDone.WaitOne();
                //while (tcpClient.RemoteEndPoint == null) ;
                //sentDone.WaitOne();
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = tcpClient;
                tcpClient.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                recvDone.WaitOne();
                //allDone.WaitOne();
            }
        }

        ~Client()
        {
            tcpClient.Close();
            udpClient.Close();
        }

        public void setServer(string host, short port)
        {
            // clean resources used by socket up

            //tcpClient.Close();              
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(host), port);
            if (tcpClient!=null&&tcpClient.Connected)
                tcpClient.Close();               

            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpClient.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), tcpClient);
            connectDone.WaitOne();
            localIP = (tcpClient.LocalEndPoint as IPEndPoint).Address.ToString();

            //Thread loopThread = new Thread(recvloop);
            //loopThread.Start();
            Thread loopThread2 = new Thread(asynrecvloop);
            loopThread2.Start();
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                // Signal that the connection has been made.
                connectDone.Set();
                //int bytes = client.EndReceive(ar);
                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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
            sentDone.Reset();
            byte[] data = Encoding.ASCII.GetBytes(command);
            tcpClient.Send(data);
            //Received();
            
            sentDone.Set();
        }
        public void Received()
        {
            byte[] bytes = new byte[1024];
            int bytesRec = tcpClient.Receive(bytes);
            receivedString = Encoding.ASCII.GetString(bytes, 0, bytesRec);
            receivedString = tcpClient.RemoteEndPoint.ToString();
            sentDone.Set();
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
        public int getProposerId()
        {
            return 1;
        }
        public void leave() { sendToServer(builder.leave().build()); }
        private void methodReceivedPaxosHandler(Dictionary<string, string> json, string s, int idSender)
        {
            string command;
            switch (s)
            {
                case "prepare_proposal":
                    command = builder.proposeResp(0, "accepted", kpuId).build();
                    sendToPeer(idSender, command);
                    break;
                case "accept_proposal":
                    command = builder.response(0, "accepted").build();
                    sendToPeer(idSender, command);
                    break;
                case "accepted_proposal":
                    command = builder.response(0, "").build();
                    sendToPeer(idSender, command);
                    break;
                case "vote_werewolf":
                    command = builder.response(0, "").build();
                    sendToPeer(idSender, command);
                    break;
                default:
                    break;

            }
        }
        private void responseReceivedPaxosHandler(Dictionary<string, string> json, string s, int idSender)
        {
            string command;
            switch (s)
            {
                case "prepare_proposal":
                    command = builder.proposeResp(0, "accepted", kpuId).build();
                    sendToPeer(idSender, command);
                    break;
                case "accept_proposal":
                    command = builder.response(0, "accepted").build();
                    sendToPeer(idSender, command);
                    break;
                case "accepted_proposal":
                    command = builder.response(0, "").build();
                    sendToPeer(idSender, command);
                    break;
                case "vote_werewolf":
                    command = builder.response(0, "").build();
                    sendToPeer(idSender, command);
                    break;
                default:
                    break;

            }
        }
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
        public void receiveTcp(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            //allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        public void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            byte[] receiveBytes = u.EndReceive(ar, ref e);
            receivedString = Encoding.ASCII.GetString(receiveBytes);
            Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(receivedString); ;
            //receivedString = json.ToString();
            if (json.ContainsKey("method"))
            {
                //request
                //json.TryGetValue("method", out receivedString);

            }
            else if (json.ContainsKey("status"))
            {
                //response
                //json.TryGetValue("status", out receivedString);
                
            }

        }
        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            // Read data from the client socket.
            //sentDone.WaitOne();
            int bytesRead = handler.EndReceive(ar);

            //receivedString = "mlebu angkruk";
            yangterlempar = "masuk loop";
            if (bytesRead == 0) { recvDone.Set(); }// receivedString = "mlebu tapi kosong";
            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                receivedString = content;
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }


            }
        }
    }
}
