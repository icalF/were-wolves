using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WereWolves
    {// State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
    class Server
    {
        short listenPort = 8282;
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public IPEndPoint ipep;
        UdpClient udpServer;

        ClientData[] clients;
        CommandBuilder builder;

        Socket tcpServer;
        byte[] data;
        int stage;
        public string receivedString ="";
        short kpuId;
        public Server()
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            ipep = new IPEndPoint(ipAddress, listenPort);
            udpServer = new UdpClient(ipep);
            builder = new CommandBuilder(); 
            tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                tcpServer.Bind(ipep);
                tcpServer.Listen(10);
                allDone.Reset();

                Console.WriteLine("Waiting for a connection...");
                tcpServer.BeginAccept(
                    new AsyncCallback(receiveTcp),
                    tcpServer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // TcpState sta = new TcpState(tcpServer, ipep);
            //tcpServer.BeginReceive(new AsyncCallback(receiveTcp), sta);

        }
        ~Server()
        {
            //tcpClient.Dispose();
            tcpServer.Close();

            //udpClient.Dispose();
            udpServer.Close();
        }
        public void setServer()
        {
            // clean resources used by socket up
            
        }
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
                new AsyncCallback(ReadCallback), state);
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
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer,0,bytesRead));

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
                } else {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }


        }
    }
        public void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
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

    public void receiveUdp(IAsyncResult ar)
    {
        UdpClient u = ((UdpState)(ar.AsyncState)).u;
        IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

        Socket handler = tcpServer.Accept();
        receivedString = null;
        while (true)
        {
            data = new byte[1024];
            int bytesRec = handler.Receive(data);
            receivedString += Encoding.ASCII.GetString(data, 0, bytesRec);
            if (receivedString.IndexOf("<EOF>") > -1)
            {
                break;
            }
        }
        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();
    }
    }
}
