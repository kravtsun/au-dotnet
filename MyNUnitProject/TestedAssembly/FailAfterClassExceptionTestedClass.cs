using System;
using MyNUnitFramework.Attributes;

namespace TestedAssembly
{
    public class FailAfterClassExceptionTestedClass
    {
        public static bool IsTestRun { get; private set; }

        [AfterClass]
        public static void FailAfterClass()
        {
            throw new Exception("FailAfterClass");
        }

        [Test]
        public void Test()
        {
            IsTestRun = true;
        }
    }
}
