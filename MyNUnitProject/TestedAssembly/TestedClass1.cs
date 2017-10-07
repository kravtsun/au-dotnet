using System;
using System.Diagnostics;
using MyNUnitFramework.Attribute;

namespace TestedAssembly
{
    public class TestedClass1
    {
        public int TestMethodResult { get; } = 42;

        public bool IsFirstSetUpRun { get; set; }

        public bool IsSecondSetUpRun { get; set; }

        public bool IsFirstTearDownRun { get; set; }

        public static bool IsIgnoredRun { get; set; }

        public TestedClass1()
        {
            CleanTest();
        }

        [Test]
        public int SimpleTest()
        {
            return TestMethodResult;
        }

        [BeforeClass]
        public void FirstSetUp()
        {
            IsFirstSetUpRun = true;
        }

        [BeforeClass]
        public void SecondSetUp()
        {
            IsSecondSetUpRun = true;
        }

        [AfterClass]
        public void FirstTearDown()
        {
            IsFirstTearDownRun = true;
        }

        [Test(Expected = typeof(Exception))]
        public void ExceptionTest()
        {
            throw new Exception("ExceptionTest");
        }

        [Test(Expected = typeof(NullReferenceException))]
        public void NullReferenceExceptionTest()
        {
            // ReSharper disable once PossibleNullReferenceException
            throw null;
        }

        [Test]
        public void ExceptionFailTest()
        {
            throw new Exception("ExceptionFailTest");
        }

        [Test]
        public void AssertFailTest()
        {
            Debug.Assert(false, "FAIL");
        }

        [Test(IgnoreWithCause = "Just ignored")]
        public void IgnoreTest()
        {
            IsIgnoredRun = false;
        }

        public void CleanTest()
        {
            IsFirstSetUpRun = false;
            IsSecondSetUpRun = false;
            IsFirstTearDownRun = false;
            IsIgnoredRun = false;
        }
    }
}
