using NUnit.Framework;
using System.Net;
using System.Threading;
using System.Text;


namespace WereWolves.Tests
{
    [TestFixture]
    public class NetworkTests
    {
        Client client1, client2, client3;
        Server server1;
        [TestFixtureSetUp]
        public void TestSetup()
        {
            client1 = new Client(8181);
            client2 = new Client(8383);
            client3 = new Client(8484);
            server1 = new Server(8282);
        }

        [TestFixtureTearDown]
        public void TestTearDown() { client1 = null; client2 = null; }

        [Test]
        public void p2pTest()
        {
            string t = "{\"status\" : \"ok\"}";
            CommandBuilder b = new CommandBuilder();
            b = b.ready();
            IPEndPoint e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8383);
            client1.sendUdp(e, b.build());
            Thread.Sleep(1000);
            string r = client2.receivedString;
            
            e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8181);
            client2.sendUdp(e, t);
            Thread.Sleep(1000);
            string u = client1.receivedString;

            e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8383);
            client1.sendUdp(e, "angkruk");
            Thread.Sleep(1000);
            string v = client2.receivedString;

            e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8484);
            client3.sendUdp(e, "angkruk");
            Thread.Sleep(1000);
            string w = client2.receivedString;

            Assert.That(b.build(), Is.EqualTo(r));
            Assert.That(t, Is.EqualTo(u));
            Assert.That("angkruk", Is.EqualTo(v));
            Assert.That("angkruk", Is.EqualTo(w));
        }

        [Test]
        public void c2sTest()
        {
            string s = "Aku cinta kamu";
            string s1 = "Y";
            //IPEndPoint e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8383);

            client1.setServer((server1.ipep as IPEndPoint).Address.ToString(), 8282);
            client1.sendToServer(s);
            //Client.sentDone.WaitOne();
            Thread.Sleep(2000);

            string t = client1.receivedString;
            client2.setServer((server1.ipep as IPEndPoint).Address.ToString(), 8282);
            client2.sendToServer(s1);
            //Client.sentDone.WaitOne();

            Thread.Sleep(2000);

            
            //string t1 = client1.receivedString;
            string t1 = client2.receivedString;
            //Assert.That(null,r);
            Assert.That(t, Is.EqualTo(s));
            Assert.That(t1, Is.EqualTo(s1));
        }

        [Test]
        public void s2cTest()
        {
            string s = "Aku cinta kamu";
            string lol = (server1.ipep as IPEndPoint).ToString();
            //IPEndPoint e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8383);
            client1.setServer((server1.ipep as IPEndPoint).Address.ToString(), 8282);
            server1.SendToClient(s);
            //client1.Received();
            Thread.Sleep(2000);
            string t = client1.receivedString;
            //string t = server1.yangdilempar;
            //Assert.That(null,r);
            Assert.That(t, Is.EqualTo(s));
            /*string s = "Aku cinta kamu";
            IPEndPoint e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8383);
            client1.sendUdp(e, s);
            Thread.Sleep(1000);
            string r = client2.receivedString;

            Assert.That(s, Is.EqualTo(r));*/
        }
    }
}
