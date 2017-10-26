using System;
using MyNUnitFramework.Attributes;

namespace TestedAssembly
{
    public class FailFinishClass
    {
        public static bool IsTestRun { get; private set; }

        [After]
        public void FailFinish()
        {
            throw new Exception("FailTearDown");
        }

        [Test]
        public void Test()
        {
            IsTestRun = true;
        }
    }
}
