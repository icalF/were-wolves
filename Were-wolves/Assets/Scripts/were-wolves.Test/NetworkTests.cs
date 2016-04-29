using NUnit.Framework;
using System.Net;
using System.Text;
using System.Threading;

namespace WereWolves.Tests
{
    [TestFixture]
    public class NetworkTests
    {
        Client client1, client2;

        [TestFixtureSetUp]
        public void TestSetup()
        {
            client1 = new Client(8181);
            client2 = new Client(8383);
        }

        [TestFixtureTearDown]
        public void TestTearDown() { client1 = null; client2 = null; }

        [Test]
        public void dataSentTest()
        {
            string s = "Aku cinta kamu";
            IPEndPoint e = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8383);
            client1.sendUdp(e, s);
            Thread.Sleep(1000);
            string r = client2.receivedString;

            Assert.That(s, Is.EqualTo(r));
        }
    }
}
