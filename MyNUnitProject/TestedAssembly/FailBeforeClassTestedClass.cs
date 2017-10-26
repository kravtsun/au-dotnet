using MyNUnitFramework.Attributes;

namespace TestedAssembly
{
    public class FailBeforeClassNonStaticTestedClass
    {
        public static bool IsBeforeClassRun { get; private set; }
        public static bool IsTestRun { get; private set; }

        [BeforeClass]
        public void BeforeClass()
        {
            IsBeforeClassRun = true;
        }
        
        [Test]
        public void Test()
        {
            IsTestRun = true;
        }
    }
}
