using Newtonsoft.Json;
using NUnit.Framework;
using Rx.Net.Plus;

namespace Rx_Tests
{
    [TestFixture]
    public class RxVarJsonFlatTests
    {
        [Test]
        public void SerializeRxVarFlatJson()
        {
            RxVarFlatJsonExtensions.RegisterToJsonGlobalSettings();

            var origObject = new User();

            var serializedObject = JsonConvert.SerializeObject(origObject, Formatting.Indented);
            var deserializedObject = JsonConvert.DeserializeObject<User>(serializedObject);

            TestContext.WriteLine($"Json Serializing Output: \n{serializedObject}");

            Assert.AreEqual (origObject, deserializedObject);
        }
    }
}
