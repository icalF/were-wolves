using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WereWolves
{
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class Server
    {
        short listenPort;        
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static ManualResetEvent accDone = new ManualResetEvent(false);

        public IPEndPoint ipep;
        Socket tcpServer;
        List<Socket> clientHandlers;

        List<ClientData> clients;
        List<ClientData> wolves;
        CommandBuilder builder;
       
        byte[] data;
        int day = 0;
        bool isDay = true;

        public Dictionary<string, string> receivedReqs;

        public int kpuId;
        public string winner = "";
        public List<bool> isClientReady;

        static int Main(string[] args)
        {
            try {
                Server server = new Server();
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
            }
            
            return 0;
        }

        public Server(short port = 8282)
        {
            listenPort = port;
            allDone = new ManualResetEvent(false);
            builder = new CommandBuilder();
            isClientReady = new List<bool>();

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            ipep = new IPEndPoint(ipAddress, listenPort);
            
            builder = new CommandBuilder();
            tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                tcpServer.Bind(ipep);
                tcpServer.Listen(10);
                Thread loopThread = new Thread(loop);
                loopThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void loop()
        {
            while (true)
            {
                allDone.Reset();
                accDone.Reset();
                tcpServer.BeginAccept(
                    new AsyncCallback(AcceptClient),
                    tcpServer);
                accDone.WaitOne();
                allDone.WaitOne();
            }
        }

        ~Server()
        {
            tcpServer.Close();
        }

        private void addClient()
        {
            bool werewolf = new Random().NextDouble() < 0.3;
            // TODO : read data from socket
            isClientReady.Add(false);
            var client = new ClientData(null, null, 0, werewolf);
            clients.Add(client);
            if (werewolf) wolves.Add(client);
        }

        private void startGame()
        {
            // assert all clients ready
            foreach (var client in clients)
            {
                bool werewolf = client.isWerewolf();
                List<ClientData> friends = null;
                if (werewolf) {
                    friends = new List<ClientData>();
                    foreach (var wolf in wolves)
                    {
                        if (!wolf.Equals(client))
                            friends.Add(wolf);
                    }
                }
                builder.start(false, null, werewolf, friends);
            }

            day = 0;
            isDay = true;
        }

        private void startVote()
        {
            foreach (var client in clients)
            {
                builder.startVote(isDay);
            }

            // TODO : wait result and save; revote if result failed
        }

        private void changePhase()
        {
            foreach (var client in clients)
            {
                builder.change(isDay, "", day);
            }

        }

        private void gameOver()
        {
            foreach (var client in clients)
            {
                winner = "werewolf";
                builder.over(winner, "");
            }
        }

        private void leaveGame() { }

        public void AcceptClient(IAsyncResult ar)
        {            
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            accDone.Set();

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                ReadCallback, state);
        }

        public void ReadCallback(IAsyncResult ar)
        { 
            String content = String.Empty;
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.workSocket;
            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);
            
            if (bytesRead > 0) {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();

                while (receivedReqs != null) { }            // wait until empty

                receivedReqs = JsonConvert.DeserializeObject< Dictionary<string, string> > (content);

                if (content.IndexOf("<EOF>") > -1) {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content );

                } else {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
                }
            }
        }

        public void SendToClients(String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            accDone.WaitOne();
        }
                
        public void SendToClient(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
                // Signal the main thread to continue.
                allDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /*public void ReceiveUdp(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            Socket handler = tcpServer.Accept();
            receivedReqs = null;
            while (true)
            {
                data = new byte[1024];
                int bytesRec = handler.Receive(data);
                receivedReqs += Encoding.ASCII.GetString(data, 0, bytesRec);
                if (receivedReqs.IndexOf("<EOF>") > -1)
                {
                    break;
                }
            }
            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }*/
    }
}
