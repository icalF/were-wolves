using Newtonsoft.Json;
using NUnit.Framework;

namespace WereWolves.Tests
{
    [TestFixture]
    public class ClientTest
    {
        [Test]
        public void parserTest()
        {
            ClientData c = new ClientData("panjadi", "123.44.0.10", 1234);
            var result = JsonConvert.SerializeObject(c);
            var test = JsonConvert.DeserializeObject<ClientData>(result);
            Assert.That(test, Is.EqualTo(c));
        }
    }
}
