using System;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using System.Net.NetworkInformation;
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
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static ManualResetEvent accDone = new ManualResetEvent(false);


        public IPEndPoint ipep;
        Socket tcpServer;

        List<ClientData> clients;
        CommandBuilder builder;
       
        Socket ikkeh;
        public string yangdilempar = "";
        byte[] data;
        int stage;
        public string receivedString = "";
        short kpuId;
        

        public Server(short port)
        {
            listenPort = port;
            allDone = new ManualResetEvent(false);
            builder = new CommandBuilder(); 

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
            int i = 0;
            while (true)
            {
                allDone.Reset();
                accDone.Reset();
                tcpServer.BeginAccept(
                    new AsyncCallback(receiveTcp),
                    tcpServer);
                accDone.WaitOne();
                allDone.WaitOne();
                i++;
            }
        }
        ~Server()
        {
            tcpServer.Close();
        }

        public void receiveTcp(IAsyncResult ar)
        {
            
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            ikkeh = handler;
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

            yangdilempar = handler.RemoteEndPoint.ToString();
            if (bytesRead > 0) {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                receivedString = content;
                // Echo the data back to the client.
                Send(handler, content);

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

        public void SendToClient(String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            accDone.WaitOne();
            //yangdilempar = ikkeh.LocalEndPoint.ToString();
            
            ikkeh.SendTo(byteData, ikkeh.LocalEndPoint);
            //Send(ikkeh, data);
            
        }
                
        public void Send(Socket handler, string data)
        {
            //yangdilempar = data;
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
