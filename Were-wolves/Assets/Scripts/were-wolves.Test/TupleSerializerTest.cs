using NUnit.Framework;
using Newtonsoft.Json;

namespace WereWolves.Tests
{
    [TestFixture]
    public class TupleSerializerTest
    {
        // Tuple parser
        [Test]
        public void parserTest()
        {
            Tuple[] weird = { new Tuple(2, -1), new Tuple(999, 45) };
            var result = JsonConvert.SerializeObject(weird);
            var test = JsonConvert.DeserializeObject<Tuple[]>(result);
            Assert.AreEqual(weird, test);
        }
    }
}
