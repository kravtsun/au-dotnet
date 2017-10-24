using System;
using MyNUnitFramework.Attributes;

namespace TestedAssembly
{
    public class TestedClass2
    {
        public static bool IsBeforeRun { get; private set; }
        public static bool IsAfterRun { get; private set; }
        public static bool IsBeforeNonStaticRun { get; private set; }
        public static bool IsAfterNonStaticRun { get; private set; }
        public static double MustBePi { get; private set; } = 2.67;

        [BeforeClass]
        public static void BeforeMethod()
        {
            IsBeforeRun = true;
        }

        [Before]
        public void BeforeNonStaticMethod()
        {
            IsBeforeNonStaticRun = true;
        }

        [AfterClass]
        public static void AfterMethod()
        {
            IsAfterRun = true;
        }

        [After]
        public void AfterNonStaticMethod()
        {
            IsAfterNonStaticRun = true;
        }

        [Test]
        public void SimpleTest()
        {
            MustBePi = 3.14;
        }

        [Test]
        public void SimpleFailTest()
        {
            throw new Exception("SimpleFailTest");
        }
    }
}