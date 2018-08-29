using System;
using System.Reactive.Linq;
using NUnit.Framework;
using Rx.Net.Plus;

namespace Rx_Tests
{
    [TestFixture]
    public class RxPropertyTests
    {
        [Test]
        public void BasicConstruction()
        {
            var rxProperty = false.ToRxProperty();
            Assert.IsFalse(rxProperty);
        }

        [Test]
        public void SetValueFromBasicType()
        {
            var rxProperty = false.ToRxProperty();
            rxProperty.Value = true;
            Assert.IsTrue(rxProperty);
        }

        [Test]
        public void TestToRxExtension()
        {
            var rxProperty = 10.ToRxProperty();
            rxProperty.Value = 20;
            Assert.AreEqual(rxProperty, 20);
        }

        
        [Test]
        public void TestToRxExtensionFromRxVar()
        {
            var rxVar = 10.ToRxVar();
            var rxVar2 = rxVar.ToRxPropertyAndSubscribe();
            Assert.AreEqual(rxVar, rxVar2);

            rxVar.Value = 30;
            Assert.AreEqual(rxVar, rxVar2);
        }

        [Test]
        public void ComparerxPropertyToValue()
        {
            var rxProperty1 = false.ToRxProperty();

            Assert.IsTrue(rxProperty1.CompareTo(false) == 0);
        }
        
        [Test]
        public void CompareTwoRxProperties()
        {
            var rxProperty1 = false.ToRxProperty();
            var rxProperty2 = false.ToRxProperty();

            Assert.IsTrue(rxProperty1.CompareTo(rxProperty2) == 0);
        }


        [Test]
        public void EqualsOperator()
        {
            var rxProperty1 = false.ToRxProperty();
            var rxProperty2 = false.ToRxProperty();

            Assert.IsTrue(rxProperty1 == rxProperty2);
        }

        [Test]
        public void NonEqualsOperator()
        {
            var rxProperty1 = false.ToRxProperty();
            var rxProperty2 = true.ToRxProperty();

            Assert.IsTrue(rxProperty1 != rxProperty2);
        }

        [Test]
        public void EqualsOperatorWithNull1()
        {
            var rxProperty1 = false.ToRxProperty();
            var rxProperty2 = true.ToRxProperty();
            rxProperty2 = null;

            Assert.IsFalse(rxProperty1 == rxProperty2);
        }

        [Test]
        public void EqualsOperatorWithNull2()
        {
            var rxProperty1 = false.ToRxProperty();
            var rxProperty2 = true.ToRxProperty();
            rxProperty1 = null;

            Assert.IsFalse(rxProperty1 == rxProperty2);
        }

        [Test]
        public void EqualsOperatorWithNull3()
        {
            var rxProperty1 = false.ToRxProperty();
            rxProperty1 = null;

            Assert.IsFalse(rxProperty1 == true);
        }

        [Test]
        public void NonEqualsOperatorWithValue()
        {
            var rxProperty1 = false.ToRxProperty();

            Assert.IsTrue(rxProperty1 != true);
        }

        [Test]
        public void EqualsOperatorWithValue()
        {
            var rxProperty1 = false.ToRxProperty();

            Assert.IsTrue(rxProperty1 == false);
        }

        [Test]
        public void ConvertToByte()
        {
            var rxProperty1 = true.ToRxProperty();
            var b = Convert.ToByte(rxProperty1);
            Assert.IsTrue(b == 1);
        }


        [Test]
        public void ConvertToChar()
        {
            var rxProperty1 = "C".ToRxProperty();
            var c = Convert.ToChar(rxProperty1);
            Assert.IsTrue(c == 'C');
        }

        [Test]
        public void ConvertToDateTime()
        {
            var dt = new DateTime(2000, 1, 1);
            var rxProperty1 = "1/1/2000".ToRxProperty();
            var dt2 = Convert.ToDateTime(rxProperty1);
            Assert.IsTrue(dt == dt2);
        }


        [Test]
        public void SetValueFromrxProperty()
        {
            var rxProperty = false.ToRxProperty();
            var rxProperty2 = true.ToRxProperty();
            
            rxProperty.Value = rxProperty2;
            Assert.IsTrue(rxProperty);
        }

        [Test]
        public void WithSuscribers()
        {
            var rxProperty = false.ToRxProperty();
            bool recipient = true;
            rxProperty.Subscribe (value => recipient = value);

            Assert.IsFalse(recipient);
        }

        [Test]
        public void WithSuscribersAndUpdateOfValue()
        {
            var rxProperty = false.ToRxProperty();

            bool recipient = true;
            
            rxProperty
                .Do(val => TestContext.WriteLine($"Value is: {val}"))
                .Subscribe(value => recipient = value);

            Assert.IsFalse(recipient);

            rxProperty.Value = true;

            Assert.IsTrue(recipient);
        }


        [Test]
        public void WithrxPropertyClassicReactiveSyntax()
        {
            WithSuscribersAndUpdateOfValue();
        }


        [Test]
        public void WithRxSuscriberListenToSyntax()
        {
            var rxProperty = false.ToRxProperty();
            var rxPropertyRecipient = true.ToRxProperty();

            rxPropertyRecipient.ListenTo(rxProperty.Do(val => TestContext.WriteLine($"Value is: {val}")));

            Assert.IsFalse(rxPropertyRecipient);

            rxProperty.Value = true;

            Assert.IsTrue(rxPropertyRecipient);
        }

        [Test]
        public void DisposableInterface()
        {
            var rxProperty = false.ToRxProperty();

            int counter = 0;
    
            rxProperty
                .Subscribe(_ =>
                {
                    ++counter;            // Count the total changes of rxProperty
                    TestContext.WriteLine($"Outside counter: {counter}");
                });

            var rxPropertyRecipient = true.ToRxProperty();

            int counterRecipient = 0;

            rxPropertyRecipient.ListenTo(rxProperty.Do(val =>
            {
                ++counterRecipient;
                TestContext.WriteLine($"Invocation# {counterRecipient}, Value is: {val}");
            }));

            using (rxPropertyRecipient)
            {
                Assert.IsFalse(rxPropertyRecipient);

                rxProperty.Value = true;
                Assert.IsTrue(rxPropertyRecipient);
            }

            Assert.True(rxPropertyRecipient.IsDisposed);

            rxProperty.Value ^= true;

            Assert.True (counterRecipient == 2);       // Since  rxPropertyRecipient is disposed
            Assert.True (counter == 3);
        }

        [Test]
        public void CheckDistinctNotifications()
        {
            var rxProperty = false.ToRxProperty();

            int counter = 0;

            rxProperty
                .Subscribe(_ =>
                {
                    ++counter;            // Count the total changes of rxProperty
                    TestContext.WriteLine($"Counter: {counter}");
                });

            TestContext.WriteLine($"Counter is reset to count alteration");
            counter = 0;

            rxProperty.Value = false;
            rxProperty.Value = false;
            rxProperty.Value = false;
            rxProperty.Value = false;
            
            rxProperty.Value = true;
            rxProperty.Value = true;
            rxProperty.Value = true;

            rxProperty.Value = false;

            Assert.True (counter == 2);
        }

        [Test]
        public void CheckConcurrency()
        {
            var rxProperty = false.ToRxProperty();

            int counter = 0;

            rxProperty
                .Subscribe(_ =>
                {
                    ++counter;            // Count the total changes of rxProperty
                    TestContext.WriteLine($"Counter: {counter}");
                });

            TestContext.WriteLine($"Counter is reset to count alteration");
            counter = 0;

            rxProperty.Value = false;
            rxProperty.Value = false;
            rxProperty.Value = false;
            rxProperty.Value = false;
            
            rxProperty.Value = true;
            rxProperty.Value = true;
            rxProperty.Value = true;

            rxProperty.Value = false;

            Assert.True (counter == 2);
        }
    }
}
