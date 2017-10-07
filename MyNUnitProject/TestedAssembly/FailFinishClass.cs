using System;
using MyNUnitFramework.Attribute;

namespace TestedAssembly
{
    [Test]
    public class FailFinishClass
    {
        public static bool IsTestRun { get; private set; }

        [After]
        public void FailFinish()
        {
            throw new Exception("FailSetUp");
        }

        [Test]
        public void Test()
        {
            IsTestRun = true;
        }
    }
}
