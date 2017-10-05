using System;
using System.Diagnostics;
using MyNUnitFramework.Attribute;

namespace TestedAssembly
{
    public class TestedClass1
    {
        // TODO convert properties into fields as it generates "Auto-property accessor never used" warning.

        public int TestMethodResult { get; } = 42;

        private bool IsFirstSetUpRun { get; set; }

        private bool IsSecondSetUpRun { get; set; }

        private bool IsFirstTearDownRun { get; set; }

        private bool IsIgnoredRun { get; set; }

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

        // Successful tests.

        [Test(Expected = typeof(Exception))]
        public void ExpectedExceptionTest()
        {
            throw new Exception("ExpectedExceptionTest");
        }

        [Test(Expected = typeof(AccessViolationException))]
        public void AccessViolationExceptionTest()
        {
            throw new AccessViolationException("AccessViolationExceptionTest");
        }

        [Test(Expected = typeof(NullReferenceException))]
        public void NullReferenceExceptionTest()
        {
            // ReSharper disable once PossibleNullReferenceException
            throw null;
        }

        // Failing tests.
        [Test]
        public void ExceptionFailTest()
        {
            throw new Exception("ExceptionFailTest");
        }

        [Test]
        public void AccessViolationExceptionFailTest()
        {
            throw new AccessViolationException("AccessViolationExceptionFailTest");
        }

        [Test]
        public void FastFailTest()
        {
            Environment.FailFast("FastFailTest");
        }

        [Test]
        public void AssertFailTest()
        {
            Debug.Assert(false);
        }

        // Ignored tests.
        [Test]
        public void IgnoreTest()
        {
            IsIgnoredRun = false;
        }

        public void CleanTest()
        {
            IsFirstSetUpRun = false;
            IsSecondSetUpRun = false;
            IsFirstTearDownRun = false;
            IsIgnoredRun = true;
        }
    }
}
