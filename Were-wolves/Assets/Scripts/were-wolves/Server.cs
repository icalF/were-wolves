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
        bool isGame = false;

        public int kpuId;
        int killed;
        public string winner;
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

        private bool addClient(ClientData client)
        {
            bool werewolf = new Random().NextDouble() < 0.3;

            foreach (ClientData ecl in clients)
            {
                if (ecl.getUsername().Equals(client.getUsername()))
                    return false;
            }
            
            isClientReady.Add(false);
            clients.Add(client);

            if (werewolf)
            {
                client.setRole(true);
                wolves.Add(client);
            }
            return true;
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

            for (int i = 0; i < isClientReady.Count; ++i) isClientReady[i] = false;
        }

        private void leaveGame(string endPoint) {
            ClientData client = findByRemote(endPoint);
            if (!clients.Remove(client))
                throw new SystemException("Remove list failed");
        }

        private ClientData findByRemote(string endPoint)
        {
            for (int i = 0; i < clientHandlers.Count; ++i)
            {
                Socket ec = clientHandlers[i];
                if (ec.RemoteEndPoint.ToString() == endPoint)
                    return clients[i];                    
            }            
            return null;
        }

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
            string content = string.Empty;

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

                Dictionary<string, string> income = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                // Handle every request
                if (income.ContainsKey("command"))            // save only commands (ignore response)
                {
                    handleReq(handler, income);
                }

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

        private void handleReq(Socket handler, Dictionary<string, string> income)
        {
            switch (income["command"])
            {
                case "join":
                    if (isGame)
                    {
                        SendToClient(handler,
                            builder.joinResp(1,
                                "Please wait,game is currently running")
                                .build());

                        break;
                    }

                    ClientData client = new ClientData(
                        income["username"], 
                        income["udp_address"],
                        short.Parse(income["port"]));

                    if (addClient(client))
                    {
                        SendToClient(handler,
                            builder.joinResp(0,
                                client.getId())      
                                .build());
                    } else
                    {
                        SendToClient(handler,
                            builder.joinResp(1,
                                "User exists")
                                .build());
                    }
                    break;

                case "leave":
                    try
                    {
                        leaveGame(handler.RemoteEndPoint.ToString());

                        SendToClient(handler,
                            builder.response(0)
                                .build());
                    } catch (SystemException e)
                    {
                        string msg = e.Message;
                        Console.Write(msg);

                        SendToClient(handler,
                            builder.response(1, msg)
                                .build());
                    }
                    break;

                case "ready":
                    isClientReady[
                        findByRemote(handler.RemoteEndPoint.ToString())
                            .getId()] 
                        = true;

                    SendToClient(handler,
                        builder.response(0,
                            "Waiting for other player to start")
                            .build());

                    break;

                case "client_address":
                    SendToClient(handler,
                        builder.listClientResp(0,
                            "list of clients retrieved", 
                            clients).build());
                    break;

                case "accepted_proposal":
                    kpuId = int.Parse(income["kpu_id"]);

                    SendToClient(handler,
                        builder.response(0, "").build());
                    break;

                case "vote_result":
                case "vote_result_civilian":
                case "vote_result_werewolf":
                    if (income["kpu_id"].Equals("1"))
                        killed = int.Parse(income["player_killed"]);
                    break;
            }
        }

        public void SendToClients(string data)
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
            /*try
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
            }*/
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
