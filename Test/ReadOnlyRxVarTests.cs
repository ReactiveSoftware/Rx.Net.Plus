using System;
using System.Reactive.Linq;
using NUnit.Framework;
using Rx.Net.Plus;

namespace Rx_Tests
{
    [TestFixture]
    public partial class ReadOnlyRxVarTests
    {
        [Test]
        public void BasicConstruction()
        {
            var rxVar = false.ToRxVar().ToReadOnlyRxVar();
            Assert.IsFalse(rxVar);
        }


        [Test]
        public void TestReadonlyRxVar()
        {
            var rxVar = 10.ToRxVar();
            var readOnlyRxVar = rxVar.ToReadOnlyRxVar();
            rxVar.Value = 20;
            Assert.AreEqual(readOnlyRxVar, 20);
        }

        [Test]
        public void CompareRxVarToValue()
        {
            var readOnlyRxVar = false.ToRxVar().ToReadOnlyRxVar();
            Assert.IsTrue(readOnlyRxVar.CompareTo(false) == 0);
        }

        [Test]
        public void CompareTwoRxVars()
        {
            var readOnlyRxVar = false.ToRxVar().ToReadOnlyRxVar();
            var other = false.ToRxVar().ToReadOnlyRxVar();

            Assert.IsTrue(readOnlyRxVar.CompareTo(other) == 0);
        }


        [Test]
        public void EqualsOperator()
        {
            var readOnlyRxVar = false.ToRxVar().ToReadOnlyRxVar();
            var other = false.ToRxVar().ToReadOnlyRxVar();

            Assert.IsTrue(readOnlyRxVar == other);
        }

        [Test]
        public void NonEqualsOperator()
        {
            var readOnlyRxVar = false.ToRxVar().ToReadOnlyRxVar();
            var other = true.ToRxVar().ToReadOnlyRxVar();

            Assert.IsTrue(readOnlyRxVar != other);
        }

        [Test]
        public void EqualsOperatorWithNull1()
        {
            var readOnlyRxVar = false.ToRxVar().ToReadOnlyRxVar();
            var other = false.ToRxVar().ToReadOnlyRxVar();
            other = null;

            Assert.IsFalse(readOnlyRxVar == other);
        }

        [Test]
        public void EqualsOperatorWithNull2()
        {
            var readOnlyRxVar = false.ToRxVar().ToReadOnlyRxVar();
            var other = false.ToRxVar().ToReadOnlyRxVar();
            readOnlyRxVar = null;

            Assert.IsFalse(readOnlyRxVar == other);
        }

        [Test]
        public void EqualsOperatorWithNull3()
        {
            var readOnlyRxVar = false.ToRxVar().ToReadOnlyRxVar();
            readOnlyRxVar = null;

            Assert.IsFalse(readOnlyRxVar == true);
        }

        [Test]
        public void NonEqualsOperatorWithValue()
        {
            var readOnlyRxVar = false.ToRxVar().ToReadOnlyRxVar();

            Assert.IsTrue(readOnlyRxVar != true);
        }

        [Test]
        public void EqualsWithObject()
        {
            var readOnlyRxVar1 = false.ToRxVar().ToReadOnlyRxVar();
            var readOnlyRxVar2 = false.ToRxVar().ToReadOnlyRxVar();

            Assert.True(readOnlyRxVar1.Equals((object) readOnlyRxVar2));
            Assert.True(readOnlyRxVar1.Equals((object) false));
        }

        [Test]
        public void EqualsOperatorWithValue()
        {
            var readOnlyRxVar = false.ToRxVar().ToReadOnlyRxVar();
            Assert.IsTrue(readOnlyRxVar == false);
        }

        [Test]
        public void ConvertToByte()
        {
            var readOnlyRxVar = true.ToRxVar().ToReadOnlyRxVar();
            var b = Convert.ToByte(readOnlyRxVar);
            Assert.IsTrue(b == 1);
        }


        [Test]
        public void ConvertToChar()
        {
            var readOnlyRxVar = "C".ToRxVar().ToReadOnlyRxVar();
            var c = Convert.ToChar(readOnlyRxVar);
            Assert.IsTrue(c == 'C');
        }

        [Test]
        public void ConvertToDateTime()
        {
            var dt = new DateTime(2000, 1, 1);
            var readOnlyRxVar = "1/1/2000".ToRxVar().ToReadOnlyRxVar();
            var dt2 = Convert.ToDateTime(readOnlyRxVar);
            Assert.IsTrue(dt == dt2);
        }


        [Test]
        public void WithSubscribers()
        {
            var readOnlyRxVar = false.ToRxVar().ToReadOnlyRxVar();
            var recipient = true;
            readOnlyRxVar.Subscribe(value => recipient = value);

            Assert.IsFalse(recipient);
        }

        [Test]
        public void WithSubscribersAndUpdateOfValue()
        {
            var rxVar = false.ToRxVar();
            var readOnlyRxVar = rxVar.ToReadOnlyRxVar();

            var recipient = true;

            readOnlyRxVar
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
            var readOnlyRxVar = rxVar.ToReadOnlyRxVar();
            var rxVarRecipient = true.ToRxVar();

            rxVarRecipient.ListenTo(readOnlyRxVar.Do(val => TestContext.WriteLine($"Value is: {val}")));

            Assert.IsFalse(rxVarRecipient);

            rxVar.Value = true;

            Assert.IsTrue(rxVarRecipient);
        }

      [Test]
        public void TestInstantiationFromObject()
        {
            var readOnlyRxVar = new RxVar<bool>("true").ToReadOnlyRxVar();
            Assert.True(readOnlyRxVar);
        }
    }
}
