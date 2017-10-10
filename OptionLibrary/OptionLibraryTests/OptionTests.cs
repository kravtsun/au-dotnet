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
            _twoOption = Option<int>.Some(2);
            _noneOption = IntOption.None();
        }

        [TestMethod]
        public void SomeTest()
        {
            Assert.IsTrue(_twoOption.IsSome);
            Assert.IsFalse(_twoOption.IsNone);
            Assert.AreEqual(2, _twoOption.Value());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueFailsOnEmptyTest()
        {
            Console.WriteLine($"none_value = {_noneOption.Value()}");
        }

        [TestMethod]
        public void NoneTest()
        {
            Assert.IsTrue(_noneOption.IsNone);
            Assert.IsFalse(_noneOption.IsSome);
        }

        [TestMethod]
        public void ValueTest()
        {
            Assert.AreEqual(2, _twoOption.Value());
        }

        [TestMethod]
        public void MapTest()
        {
            Assert.AreEqual(4, _twoOption.Map(_multiplier).Value());
            Assert.IsTrue(_noneOption.Map(_multiplier).IsNone);
        }

        [TestMethod]
        public void FlattenTest()
        {
            var someNoneOption = Option<IntOption>.Some(_noneOption);
            Assert.IsTrue(IntOption.Flatten(someNoneOption).IsNone);

            var noneNoneOption = Option<IntOption>.None();
            Assert.IsTrue(noneNoneOption.IsNone);

            var twoTwoOption = Option<IntOption>.Some(_twoOption);
            Assert.IsTrue(twoTwoOption.IsSome);
            Assert.AreEqual(2, IntOption.Flatten(twoTwoOption).Value());
        }
    }
}