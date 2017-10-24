using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;
using MyNUnitFramework.Attributes;

namespace MyNUnitTest
{
    [TestClass]
    public class TypeTesterTests
    {
        [TestInitialize]
        public void TestClassInitialize()
        {
            TestClass.Field = 0;
        }

        [TestMethod]
        public void StartActionForTypeTest()
        {
            var startAction = TypeTester.StartActionForType(typeof(TestClass));
            Assert.AreEqual(0, TestClass.Field);
            startAction(null);
            Assert.AreEqual(3, TestClass.Field);
            startAction(null);
            Assert.AreEqual(6, TestClass.Field);
        }

        [TestMethod]
        public void FinishActionForTypeTest()
        {
            var finishAction = TypeTester.FinishActionForType(typeof(TestClass));
            Assert.AreEqual(0, TestClass.Field);
            finishAction(null);
            Assert.AreEqual(-3, TestClass.Field);
            finishAction(null);
            Assert.AreEqual(-6, TestClass.Field);
        }

        public class TestClass
        {
            public static int Field { get; set; }

            [Before]
            public static void FirstSetUp()
            {
                Field += 1;
            }

            [Before]
            public static void SecondSetUp()
            {
                Field += 2;
            }

            [After]
            public static void FirstTearDown()
            {
                Field -= 1;
            }

            [After]
            public static void SecondTearDown()
            {
                Field -= 2;
            }
        }
    }
}