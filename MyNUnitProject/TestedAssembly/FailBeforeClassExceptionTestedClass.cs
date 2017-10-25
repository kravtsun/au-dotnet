using System;
using MyNUnitFramework.Attributes;

namespace TestedAssembly
{
    public class FailBeforeClassExceptionTestedClass
    {
        public static bool IsTestRun { get; private set; }

        [BeforeClass]
        public static void FailBeforeClass()
        {
            throw new Exception("FailBeforeClass");
        }

        [Test]
        public void Test()
        {
            IsTestRun = true;
        }
    }
}
