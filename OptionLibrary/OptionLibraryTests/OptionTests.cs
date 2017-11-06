using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptionLibrary;

using IntOption = OptionLibrary.Option<int>;
namespace OptionLibraryTests
{
    [TestClass]
    public class OptionTests
    {
        private Func<int, int> _multiplier;
        private IntOption _twoOption;
        private IntOption _noneOption;

        [TestInitialize]
        public void SetUp()
        {
            _multiplier = x => x * 2;
            _twoOption = IntOption.Some(2);
            _noneOption = IntOption.None();
        }

        [TestMethod]
        public void IsSomeReturnsTrueForSomeTest()
        {
            Assert.IsTrue(_twoOption.IsSome);
        }

        [TestMethod]
        public void IsNoneReturnsFalseForSomeTest()
        {
            Assert.IsFalse(_twoOption.IsNone);
        }

        [TestMethod]
        public void SomeValueIsExtractibleTest()
        {
            Assert.AreEqual(2, _twoOption.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(OptionException))]
        public void ValueFailsOnEmptyTest()
        {
            Console.WriteLine($"none_value = {_noneOption.Value}");
        }

        [TestMethod]
        public void IsNoneReturnsTrueForNoneTest()
        {
            Assert.IsTrue(_noneOption.IsNone);
        }

        [TestMethod]
        public void IsSomeReturnsFalseForNoneTest()
        {
            Assert.IsFalse(_noneOption.IsSome);
        }

        [TestMethod]
        public void ValueTest()
        {
            Assert.AreEqual(2, _twoOption.Value);
        }

        [TestMethod]
        public void MapTest()
        {
            Assert.AreEqual(4, _twoOption.Map(_multiplier).Value);
            Assert.IsTrue(_noneOption.Map(_multiplier).IsNone);
        }

        [TestMethod]
        public void SomeSomeFlattenedIntoSomeTest()
        {
            var twoTwoOption = Option<IntOption>.Some(_twoOption);
            Assert.IsTrue(twoTwoOption.IsSome);
            Assert.AreEqual(2, IntOption.Flatten(twoTwoOption).Value);
        }

        [TestMethod]
        public void SomeNoneFlattenedIntoNoneTest()
        {
            var someNoneOption = Option<IntOption>.Some(_noneOption);
            Assert.IsTrue(IntOption.Flatten(someNoneOption).IsNone);
        }

        [TestMethod]
        public void NoneNoneFlattenedIntoNoneTest()
        {
            var noneNoneOption = Option<IntOption>.None();
            Assert.IsTrue(noneNoneOption.IsNone);
        }

        [TestMethod]
        public void NoneReferenceEqualsNoneTest()
        {
            var firstNone = IntOption.None();
            var secondNone = IntOption.None();
            Assert.IsTrue(firstNone == secondNone);
        }

        [TestMethod]
        public void SomeEqualsSomeTest()
        {
            Assert.AreEqual(_twoOption.Map(x => x * 2), IntOption.Some(4));
        }

        [TestMethod]
        public void EqualsReflexivityHoldsForNoneTest()
        {
            Assert.AreEqual(_noneOption, _noneOption);
        }

        [TestMethod]
        public void EqualsReflexivityHoldsForSomeTest()
        {
            Assert.AreEqual(_twoOption, _twoOption);
        }

        [TestMethod]
        public void EqualsCommutativityHoldsTest()
        {
            var a = IntOption.None();
            var b = IntOption.None();
            Assert.AreEqual(a, b);
            Assert.AreEqual(b, a);
            var three = IntOption.Some(3);
            Assert.AreNotEqual(_twoOption, three);
            Assert.AreNotEqual(three, _twoOption);
        }

        [TestMethod]
        public void SomeOperatorEqualsSomeTest()
        {
            var mappedFour = _twoOption.Map(x => x * 2);
            var simpleFour = IntOption.Some(4);
            Assert.IsTrue(mappedFour == simpleFour);
            Assert.IsFalse(mappedFour != simpleFour);
        }

        [TestMethod]
        public void SomeNotEqualsNullTest()
        {
            Assert.AreNotEqual(_twoOption, null);
            Assert.IsFalse(_twoOption == null);
            Assert.IsTrue(_twoOption != null);
        }

        [TestMethod]
        public void NullNotEqualsSomeTest()
        {
            Assert.AreNotEqual(null, _twoOption);
            Assert.AreNotEqual(_twoOption, null);
        }

        [TestMethod]
        public void NullOperatorNotEqualsSomeTest()
        {
            Assert.IsTrue(null != _twoOption);
        }

        [TestMethod]
        public void NullOperatorEqualsSomeTest()
        {
            Assert.IsFalse(null == _twoOption);
        }

        [TestMethod]
        public void DifferentTypesNotEqualTest()
        {
            var intOne = IntOption.Some(1);
            Assert.AreNotEqual(Option<bool>.Some(true), intOne);
            Assert.AreNotEqual(intOne, Option<short>.Some(1));
        }

        [TestMethod]
        public void EqualsPersistsOnCastTest()
        {
            var intOne = IntOption.Some(1);
            object objectOne = intOne;
            Assert.AreEqual(objectOne, intOne);
            Assert.IsTrue((IntOption)objectOne == intOne);
            Assert.IsFalse((IntOption)objectOne != intOne);
        }

        [TestMethod]
        public void NoneNotEqualSomeTest()
        {
            var one = IntOption.Some(1);
            Assert.AreNotEqual(one, _noneOption);
            Assert.AreNotEqual(_noneOption, one);
        }

        [TestMethod]
        public void HachCodeEqualsTest()
        {
            Assert.AreEqual(_twoOption.GetHashCode(), IntOption.Some(2).GetHashCode());
        }

        [TestMethod]
        public void HashCodeNotEqualsTest()
        {
            // very unlikely to fail on correct GetHashCode implementation...
            Assert.AreNotEqual(_twoOption, IntOption.Some(3));
        }
    }
}