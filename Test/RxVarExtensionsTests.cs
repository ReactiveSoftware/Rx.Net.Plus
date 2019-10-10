using System;
using NUnit.Framework;
using Rx.Net.Plus;

namespace Rx_Tests
{
    public class RxVarExtensionsTests
    {
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
        public void CheckExtensions()
        {
            var rxVar = false.ToRxVar();

            bool flag = false;

            rxVar.When(true).Notify(_ => flag = true);
            Assert.False(flag);

            rxVar.When(false).Notify(_ => flag = true);
            Assert.True(flag);

            rxVar.If(false).Notify(_ => flag = false);
            Assert.False(flag);

            rxVar.IfNot(true).Notify(_ => flag = true);
            Assert.True(flag);

            var rxVar2 = "Hello".ToRxVar();

            flag = false;
            rxVar2
                .IgnoreNull()
                .Subscribe(_ => flag = true);

            Assert.True(flag);
            flag = false;
            rxVar2.Value = null;

            Assert.False(flag);
        }

        [Test]
        public void CheckExtensionsEqual()
        {
            var flag = false;

            var numeric = 10.ToRxVar();

            numeric.IfEqualTo(10).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfEqualTo(11).Notify(_ => flag = false);
            Assert.True(flag);
        }

        [Test]
        public void CheckExtensionsNotEqual()
        {
            var flag = false;

            var numeric = 10.ToRxVar();

            numeric.IfNotEqualTo(9).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfNotEqualTo(10).Notify(_ => flag = false);
            Assert.True(flag);
        }

        [Test]
        public void CheckExtensionsLessOrEqual()
        {
            var flag = false;
            var numeric = 10.ToRxVar();

            numeric.IfLessThan(11).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfLessThan(10).Notify(_ => flag = false);
            Assert.True(flag);

            numeric.IfLessThanOrEqualTo(10).Notify(_ => flag = false);
            Assert.False(flag);

            numeric.IfLessThanOrEqualTo(11).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfLessThanOrEqualTo(9).Notify(_ => flag = false);
            Assert.True(flag);
        }

        [Test]
        public void CheckExtensionsGreaterOrEqual()
        {
            var flag = false;

            var numeric = 10.ToRxVar();

            numeric.IfGreaterThan(9).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfGreaterThan(10).Notify(_ => flag = false);
            Assert.True(flag);

            numeric.IfGreaterThanOrEqualTo(10).Notify(_ => flag = false);
            Assert.False(flag);

            numeric.IfGreaterThanOrEqualTo(9).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfGreaterThan(11).Notify(_ => flag = false);
            Assert.True(flag);
        }

        [Test]
        public void CheckExtensionsInRange()
        {
            bool flag = false;

            var numeric = 10.ToRxVar();

            numeric.IfInRange(9, 11).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfInRange(8, 9).Notify(_ => flag = false);
            Assert.True(flag);

            numeric.IfInRange(9, 10).Notify(_ => flag = false);
            Assert.False(flag);
        }

        [Test]
        public void CheckExtensionsInStrictRange()
        {
            var flag = false;

            var numeric = 10.ToRxVar();

            numeric.IfInStrictRange(9, 11).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfInStrictRange(8, 9).Notify(_ => flag = false);
            Assert.True(flag);

            numeric.IfInStrictRange(9, 10).Notify(_ => flag = false);
            Assert.True(flag);

            numeric.IfInStrictRange(10, 11).Notify(_ => flag = false);
            Assert.True(flag);
        }

        [Test]
        public void CheckExtensionsOutOfRange()
        {
            var flag = false;

            var numeric = 10.ToRxVar();

            numeric.IfOutOfRange(11, 15).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfOutOfRange(10, 15).Notify(_ => flag = false);
            Assert.False(flag);

            numeric.IfOutOfRange(9, 10).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfOutOfRange(5, 9).Notify(_ => flag = false);
            Assert.False(flag);
        }

        [Test]
        public void CheckExtensionsOutOfStrictRange()
        {
            var flag = false;

            var numeric = 10.ToRxVar();

            numeric.IfOutOfStrictRange(11, 15).Notify(_ => flag = true);
            Assert.True(flag);

            numeric.IfOutOfStrictRange(10, 15).Notify(_ => flag = false);
            Assert.True(flag);

            numeric.IfOutOfStrictRange(9, 10).Notify(_ => flag = false);
            Assert.True(flag);

            numeric.IfOutOfStrictRange(5, 9).Notify(_ => flag = false);
            Assert.False(flag);
        }

        [Test]
        public void CheckExtensionsClip()
        {
            var numericIn = 10.ToRxVar();
            var numericOut = numericIn.Clip(11, 15).ToRxVarAndSubscribe();

            Assert.AreEqual(numericOut, 11);

            numericIn.Value = 12;
            Assert.AreEqual(numericOut, 12);

            numericIn.Value = 15;
            Assert.AreEqual(numericOut, 15);

            numericIn.Value = 16;
            Assert.AreEqual(numericOut, 15);
        }
    }
}