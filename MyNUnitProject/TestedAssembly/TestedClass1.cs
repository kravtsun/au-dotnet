using System;
using MyNUnitFramework.Attributes;

namespace TestedAssembly
{
    public class TestedClass1
    {
        private int TestMethodResult { get; } = 42;

        public bool IsFirstSetUpRun { get; private set; }

        public bool IsSecondSetUpRun { get; private set; }

        public bool IsFirstTearDownRun { get; private set; }

        public static bool IsIgnoredRun { get; private set; }

        public TestedClass1()
        {
            CleanTest();
        }

        [Test]
        public int SimpleTest()
        {
            return TestMethodResult;
        }

        [Before]
        public void FirstSetUp()
        {
            IsFirstSetUpRun = true;
        }

        [Before]
        public void SecondSetUp()
        {
            IsSecondSetUpRun = true;
        }

        [After]
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
