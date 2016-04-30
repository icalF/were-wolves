using NUnit.Framework;
using System.Net;
using System.Threading;

namespace WereWolves.Tests
{
    [TestFixture]
    public class NetworkTests
    {
        Client client1, client2;
        Server server1;
        [TestFixtureSetUp]
        public void TestSetup()
        {
            client1 = new Client(8181);
            client2 = new Client(8383);
            server1 = new Server();
        }

        [TestFixtureTearDown]
        public void TestTearDown() { client1 = null; client2 = null; }

        [Test]
        public void p2pTest()
        {
            string s = "Aku sayang kamu :*";
            string t = "Aku juga sayang sama kamu";
            
            IPEndPoint e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8383);
            client1.sendUdp(e, s);
            Thread.Sleep(1000);
            string r = client2.receivedString;
            
            e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8181);
            client2.sendUdp(e, t);
            Thread.Sleep(1000);
            string u = client1.receivedString;

            Assert.That(s, Is.EqualTo(r));
            Assert.That(t, Is.EqualTo(u));
        }

        [Test]
        public void c2sTest()
        {
            string s = "Aku cinta kamu";
            //IPEndPoint e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8383);
            client1.setServer((server1.ipep as IPEndPoint).Address.ToString(), 8282);
            client1.sendToServer(s);
            Thread.Sleep(1000);
            string r = server1.receivedString;

            Thread.Sleep(1000);
            string t = client1.receivedString;
            //Assert.That(null,r);
            Assert.That(t, Is.EqualTo(r));
        }

        [Test]
        public void s2cTest()
        {
            /*string s = "Aku cinta kamu";
            IPEndPoint e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8383);
            client1.sendUdp(e, s);
            Thread.Sleep(1000);
            string r = client2.receivedString;

            Assert.That(s, Is.EqualTo(r));*/
        }
    }
}
