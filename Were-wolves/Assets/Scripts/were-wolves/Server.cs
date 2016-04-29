using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
namespace were_wolves
{
    class Server
    {
        IPEndPoint ipep;
        UdpClient udpsock;
        byte[] data;
        int stage;
        public Server(){
            data = new byte[1024];
            ipep = new IPEndPoint(IPAddress.Any, 9050);
            udpsock = new UdpClient(ipep);
        }

        public void receive()
        {
           

            Console.WriteLine("Waiting for a client...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            data = udpsock.Receive(ref sender);

            Console.WriteLine("Message received from {0}:", sender.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            udpsock.Send(data, data.Length, sender);

            while (true)
            {
                data = udpsock.Receive(ref sender);

                Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));
                udpsock.Send(data, data.Length, sender);
            }
        }
    }
}
