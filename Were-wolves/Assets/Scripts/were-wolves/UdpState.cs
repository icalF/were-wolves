using System.Net;
using System.Net.Sockets;

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
}
