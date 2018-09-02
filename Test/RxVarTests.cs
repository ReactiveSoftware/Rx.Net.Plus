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
        public void TestToRxExtension()
        {
            var rxVar = 10.ToRxVar();
            rxVar.Value = 20;
            Assert.AreEqual(rxVar, 20);
        }

        [Test]
        public void TestToRxExtensionFromRxVar()
        {
            var rxVar = 10.ToRxVar();
            var rxVar2 = rxVar.ToRxVarAndSubscribe();
            Assert.AreEqual(rxVar, rxVar2);

            rxVar.Value = 30;
            Assert.AreEqual(rxVar, rxVar2);
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
        public void WithSuscribers()
        {
            var rxVar = false.ToRxVar();
            bool recipient = true;
            rxVar.Subscribe (value => recipient = value);

            Assert.IsFalse(recipient);
        }

        [Test]
        public void WithSuscribersAndUpdateOfValue()
        {
            var rxVar = false.ToRxVar();

            bool recipient = true;
            
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
            WithSuscribersAndUpdateOfValue();
        }


        [Test]
        public void WithRxSuscriberListenToSyntax()
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
                    ++counter;            // Count the total changes of rxVar
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

            Assert.True (counterRecipient == 2);       // Since  rxVarRecipient is disposed
            Assert.True (counter == 3);
        }

        [Test]
        public void CheckDistinctNotifications()
        {
            var rxVar = false.ToRxVar();

            int counter = 0;

            rxVar
                .Subscribe(_ =>
                {
                    ++counter;            // Count the total changes of rxVar
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

            Assert.True (counter == 2);

            rxVar.IsDistinctMode = false;
            rxVar.Value = false;
            rxVar.Value = false;
            rxVar.Value = false;

            Assert.True (counter == 5);
        }


        [Test]
        public void CheckConcurrency()
        {
            var rxVar = false.ToRxVar();

            int counter = 0;

            rxVar
                .Subscribe(_ =>
                {
                    ++counter;            // Count the total changes of rxVar
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

            Assert.True (counter == 2);
        }

        [Test]
        public void CheckExtensions()
        {
            var rxVar = false.ToRxVar();

            bool occured = false;

            rxVar.When(true).Notify(_ => occured = true);
            Assert.False(occured);
            
            rxVar.When(false).Notify(_ => occured = true);
            Assert.True(occured);

            rxVar.If(false).Notify(_ => occured = false);
            Assert.False(occured);

            rxVar.IfNot(true).Notify(_ => occured = true);
            Assert.True(occured);
        }

        
        [Serializable]
        public class User : IEquatable<User>
        {
            public RxVar<string> Name = "John".ToRxVar();
            public RxVar<bool> IsMale = true.ToRxVar();
            public RxVar<double> Age = 16.5.ToRxVar();
            public User() { Name.IsDistinctMode = false; }

            public bool Equals(User other)
            {
                if (other != null)
                {
                    return Name.Equals(other.Name)
                           &&
                           IsMale.Equals(other.IsMale)
                           &&
                           Age.Equals(other.Age);
                }
                else
                {
                    return false;
                }
            }
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

            file = File.Open(path, FileMode.Open);
            var deserializedObject = xmlSerializer.Deserialize(file) as User;
            
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
            var serializedObject = JsonConvert.SerializeObject(origObject);
            var deserializedObject = JsonConvert.DeserializeObject<User>(serializedObject);

            Assert.AreEqual (origObject, deserializedObject);
        }

        [Test]
        public void TestInstantationFromObject()
        {
            var rxvar = new RxVar<bool>("true");
            Assert.True(rxvar);
        }
        
    }
}
