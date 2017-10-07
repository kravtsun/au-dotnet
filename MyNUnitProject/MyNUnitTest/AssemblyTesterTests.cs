using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;
using MyNUnitFramework.Attribute;

namespace MyNUnitTest
{
    [TestClass()]
    public class AssemblyTesterTests
    {
        [TestInitialize]
        public void TestClassInitialize()
        {
            TestClass.Field = 0;
        }

        [TestMethod()]
        public void StartActionForTypeTest()
        {
            var startAction = AssemblyTester.StartActionForType(typeof(TestClass));
            Assert.AreEqual(0, TestClass.Field);
            startAction(null);
            Assert.AreEqual(3, TestClass.Field);
            startAction(null);
            Assert.AreEqual(6, TestClass.Field);
        }

        [TestMethod()]
        public void FinishActionForTypeTest()
        {
            var finishAction = AssemblyTester.FinishActionForType(typeof(TestClass));
            Assert.AreEqual(0, TestClass.Field);
            finishAction(null);
            Assert.AreEqual(-3, TestClass.Field);
            finishAction(null);
            Assert.AreEqual(-6, TestClass.Field);
        }

        public class TestClass
        {
            public static int Field { get; set; }

            [BeforeClass]
            public static void FirstSetUp()
            {
                Field += 1;
            }

            [BeforeClass]
            public static void SecondSetUp()
            {
                Field += 2;
            }

            [AfterClass]
            public static void FirstTearDown()
            {
                Field -= 1;
            }

            [AfterClass]
            public static void SecondTearDown()
            {
                Field -= 2;
            }
        }
    }
}