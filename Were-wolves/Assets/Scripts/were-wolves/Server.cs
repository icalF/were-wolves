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
        short listenPort = 8282;        // default

        public IPEndPoint ipep;
        UdpClient udpServer;
        Socket tcpServer;

        List<ClientData> clients;
        List<ClientData> wolves;
        CommandBuilder builder;

        static ManualResetEvent allDone;
        byte[] data;
        int day = 0;
        bool isDay = true;
        public string receivedString = "";
        int kpuId;
        string winner = "";
        List<bool> isClientReady;

        public Server(short port)
        {
            listenPort = port;
            allDone = new ManualResetEvent(false);
            builder = new CommandBuilder();
            isClientReady = new List<bool>();

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            ipep = new IPEndPoint(ipAddress, listenPort);
            udpServer = new UdpClient(ipep);
            tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                tcpServer.Bind(ipep);
                tcpServer.Listen(10);
                allDone.Reset();

                Console.WriteLine("Waiting for a connection...");
                tcpServer.BeginAccept(receiveTcp, tcpServer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        ~Server()
        {
            tcpServer.Close();
            udpServer.Close();
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
        private void 
        private void leaveGame() { }

        public void receiveTcp(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                ReadCallback, state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            string content = string.Empty;
        
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                receivedString = content;
                Send(handler, content);
                if (content.IndexOf("<EOF>") > -1) {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content );
                    // Echo the data back to the client.
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
                }
            }
        }
                
        public void Send(Socket handler, string data)
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

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
