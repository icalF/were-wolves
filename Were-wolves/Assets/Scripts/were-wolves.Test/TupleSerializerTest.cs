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

            // Assert
            Assert.AreEqual(weird[0], test[0]);
            Assert.AreEqual(weird[1], test[1]);
        }
    }
}
