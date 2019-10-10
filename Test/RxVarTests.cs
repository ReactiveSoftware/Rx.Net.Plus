using System;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using Rx.Net.Plus;

namespace Rx_Tests
{
    [TestFixture]
    public class RxVarTests
    {
        [Test]
        public void BasicConstruction()
        {
            var rxVar = false.ToRxVar();

            Assert.IsFalse(rxVar);
        }

        [Test]
        public void SetValueFromBasicType()
        {
            var rxVar = false.ToRxVar();
            rxVar.Value = true;
            Assert.IsTrue(rxVar);
        }

        [Test]
        public void TestReadonlyRxVar()
        {
            var rxVar = 10.ToRxVar();
            var roRxVar = (IReadOnlyRxVar<int>) rxVar;
            rxVar.Value = 20;
            Assert.AreEqual(roRxVar, 20);
        }

        [Test]
        public void CompareRxVarToValue()
        {
            var rxVar1 = false.ToRxVar();

            Assert.IsTrue(rxVar1.CompareTo(false) == 0);
        }

        [Test]
        public void CompareTwoRxVars()
        {
            var rxVar1 = false.ToRxVar();
            var rxVar2 = false.ToRxVar();

            Assert.IsTrue(rxVar1.CompareTo(rxVar2) == 0);
        }


        [Test]
        public void EqualsOperator()
        {
            var rxVar1 = false.ToRxVar();
            var rxVar2 = false.ToRxVar();

            Assert.IsTrue(rxVar1 == rxVar2);
        }

        [Test]
        public void NonEqualsOperator()
        {
            var rxVar1 = false.ToRxVar();
            var rxVar2 = true.ToRxVar();

            Assert.IsTrue(rxVar1 != rxVar2);
        }

        [Test]
        public void EqualsOperatorWithNull1()
        {
            var rxVar1 = false.ToRxVar();
            var rxVar2 = true.ToRxVar();
            rxVar2 = null;

            Assert.IsFalse(rxVar1 == rxVar2);
        }

        [Test]
        public void EqualsOperatorWithNull2()
        {
            var rxVar1 = false.ToRxVar();
            var rxVar2 = true.ToRxVar();
            rxVar1 = null;

            Assert.IsFalse(rxVar1 == rxVar2);
        }

        [Test]
        public void EqualsOperatorWithNull3()
        {
            var rxVar1 = false.ToRxVar();
            rxVar1 = null;

            Assert.IsFalse(rxVar1 == true);
        }

        [Test]
        public void NonEqualsOperatorWithValue()
        {
            var rxVar1 = false.ToRxVar();

            Assert.IsTrue(rxVar1 != true);
        }

        [Test]
        public void EqualsWithObject()
        {
            var rxVar1 = false.ToRxVar();
            var rxVar2 = false.ToRxVar();

            Assert.True(rxVar1.Equals((object) rxVar2));
            Assert.True(rxVar1.Equals((object) false));
        }

        [Test]
        public void EqualsOperatorWithValue()
        {
            var rxVar1 = false.ToRxVar();

            Assert.IsTrue(rxVar1 == false);
        }

        [Test]
        public void ConvertToByte()
        {
            var rxVar1 = true.ToRxVar();
            var b = Convert.ToByte(rxVar1);
            Assert.IsTrue(b == 1);
        }


        [Test]
        public void ConvertToChar()
        {
            var rxVar1 = "C".ToRxVar();
            var c = Convert.ToChar(rxVar1);
            Assert.IsTrue(c == 'C');
        }

        [Test]
        public void ConvertToDateTime()
        {
            var dt = new DateTime(2000, 1, 1);
            var rxVar1 = "1/1/2000".ToRxVar();
            var dt2 = Convert.ToDateTime(rxVar1);
            Assert.IsTrue(dt == dt2);
        }


        [Test]
        public void SetValueFromRxVar()
        {
            var rxVar = false.ToRxVar();
            var rxVar2 = true.ToRxVar();

            rxVar.Value = rxVar2;
            Assert.IsTrue(rxVar);
        }

        [Test]
        public void WithSubscribers()
        {
            var rxVar = false.ToRxVar();
            var recipient = true;
            rxVar.Subscribe(value => recipient = value);

            Assert.IsFalse(recipient);
        }

        [Test]
        public void WithSubscribersAndUpdateOfValue()
        {
            var rxVar = false.ToRxVar();

            var recipient = true;

            rxVar
                .Do(val => TestContext.WriteLine($"Value is: {val}"))
                .Subscribe(value => recipient = value);

            Assert.IsFalse(recipient);

            rxVar.Value = true;

            Assert.IsTrue(recipient);
        }


        [Test]
        public void WithRxVarClassicReactiveSyntax()
        {
            WithSubscribersAndUpdateOfValue();
        }


        [Test]
        public void WithRxSubscriberListenToSyntax()
        {
            var rxVar = false.ToRxVar();
            var rxVarRecipient = true.ToRxVar();

            rxVarRecipient.ListenTo(rxVar.Do(val => TestContext.WriteLine($"Value is: {val}")));

            Assert.IsFalse(rxVarRecipient);

            rxVar.Value = true;

            Assert.IsTrue(rxVarRecipient);
        }

        [Test]
        public void DisposableInterface()
        {
            var rxVar = false.ToRxVar();

            int counter = 0;

            rxVar
                .Subscribe(_ =>
                {
                    ++counter; // Count the total changes of rxVar
                    TestContext.WriteLine($"Outside counter: {counter}");
                });

            var rxVarRecipient = true.ToRxVar();

            int counterRecipient = 0;

            rxVarRecipient.ListenTo(rxVar.Do(val =>
            {
                ++counterRecipient;
                TestContext.WriteLine($"Invocation# {counterRecipient}, Value is: {val}");
            }));

            using (rxVarRecipient)
            {
                Assert.IsFalse(rxVarRecipient);

                rxVar.Value = true;
                Assert.IsTrue(rxVarRecipient);
            }

            Assert.True(rxVarRecipient.IsDisposed);

            rxVar.Value ^= true;

            Assert.True(counterRecipient == 2); // Since  rxVarRecipient is disposed
            Assert.True(counter == 3);
        }

        [Test]
        public void CheckDistinctNotifications()
        {
            var rxVar = false.ToRxVar();

            int counter = 0;

            rxVar
                .Subscribe(_ =>
                {
                    ++counter; // Count the total changes of rxVar
                    TestContext.WriteLine($"Counter: {counter}");
                });

            TestContext.WriteLine($"Counter is reset to count alteration");
            counter = 0;

            rxVar.Value = false;
            rxVar.Value = false;
            rxVar.Value = false;
            rxVar.Value = false;

            rxVar.Value = true;
            rxVar.Value = true;
            rxVar.Value = true;

            rxVar.Value = false;

            Assert.True(counter == 2);

            rxVar.IsDistinctMode = false;
            rxVar.Value = false;
            rxVar.Value = false;
            rxVar.Value = false;

            Assert.True(counter == 5);
        }


        [Test]
        public void CheckConcurrency()
        {
            var rxVar = false.ToRxVar();

            int counter = 0;

            rxVar
                .Subscribe(_ =>
                {
                    ++counter; // Count the total changes of rxVar
                    TestContext.WriteLine($"Counter: {counter}");
                });

            TestContext.WriteLine($"Counter is reset to count alteration");
            counter = 0;

            rxVar.Value = false;
            rxVar.Value = false;
            rxVar.Value = false;
            rxVar.Value = false;

            rxVar.Value = true;
            rxVar.Value = true;
            rxVar.Value = true;

            rxVar.Value = false;

            Assert.True(counter == 2);
        }



        [Test]
        public void SerializeRxVarXml()
        {
            var xmlSerializer = new XmlSerializer(typeof(User));

            var path = @"c:\temp\RxVarTests.xml";
            var file = File.Create(path);

            var origObject = new User();
            xmlSerializer.Serialize(file, origObject);

            file.Close();

            TestContext.WriteLine($"Xml Serializing Output: \n{File.ReadAllText(path)}");

            file = File.Open(path, FileMode.Open);
            var deserializedObject = xmlSerializer.Deserialize(file) as User;

            bool areEqual = origObject.Equals(deserializedObject);
            Assert.AreEqual(origObject, deserializedObject);
            
            file.Close();
        }

        [Test]
        public void SerializeRxVarBinary()
        {
            IFormatter formatter = new BinaryFormatter();

            Stream stream;
            using (stream = new FileStream(@"c:\temp\rxvar.bin",
                FileMode.Create,
                FileAccess.ReadWrite, FileShare.None))
            {
                var origObject = new User();
                formatter.Serialize(stream, origObject);

                stream.Position = 0;
                var deserializedObject = formatter.Deserialize(stream) as User;
                
                Assert.AreEqual(origObject, deserializedObject);
            }
        }
 
        [Test]
        public void SerializeRxVarJson()
        {
            var origObject = new User();

            var serializedObject = JsonConvert.SerializeObject(origObject, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            });
            
            TestContext.WriteLine($"Json Serializing Output: \n{serializedObject}");

            var deserializedObject = JsonConvert.DeserializeObject<User>(serializedObject);

            Assert.AreEqual (origObject, deserializedObject);
        }

        [Test]
        public void TestInstantiationFromObject()
        {
            var rxvar = new RxVar<bool>("true");
            Assert.True(rxvar);
        }
    }
}
