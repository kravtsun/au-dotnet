using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;
using TestedAssembly;

namespace MyNUnitTest
{
    [TestClass()]
    public class MethodTesterTests
    {
        private MethodTester _methodTester;
        private Assembly _testedAssembly;
        private TypeInfo _testedClass1Info;
        private TestedClass1 _invoker;

        [TestInitialize]
        public void SetUp()
        {
            string testedAssemblyPath = Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .First(path => path.Contains("TestedAssembly"));
            _testedAssembly = Assembly.LoadFrom(testedAssemblyPath);
            _testedClass1Info = _testedAssembly.GetType("TestedAssembly.TestedClass1").GetTypeInfo();
            _invoker = (TestedClass1) _testedClass1Info.GetConstructor(Type.EmptyTypes)?.Invoke(null);
            Assert.IsNotNull(_invoker);
            var startAction = TypeTester.StartActionForType(_testedClass1Info.AsType());
            var finishAction = TypeTester.FinishActionForType(_testedClass1Info.AsType());
            _methodTester = new MethodTester(startAction, finishAction) {Invoker = _invoker};
        }

        [TestCleanup]
        public void TearDown()
        {
            _invoker.CleanTest();
        }

        [TestMethod()]
        public void SimpleTestMethod_IsTestedSuccessfullyWithTestContracts()
        {
            TestStateCorrectCleanup();
            MethodInfo simpleMethod = _testedClass1Info.GetDeclaredMethod("SimpleTest");
            string testResult = _methodTester.TestMethod(simpleMethod);
            Assert.IsNull(testResult);
            TestStartAndFinishAreCalled();
        }

        [TestMethod()]
        public void ExceptionThrowingTest_IsTestedWithExceptionMessageReturned()
        {
            TestStateCorrectCleanup();
            MethodInfo exceptionThrowingMethod = _testedClass1Info.GetDeclaredMethod("ExceptionFailTest");
            string testResult = _methodTester.TestMethod(exceptionThrowingMethod);
            Assert.IsTrue(testResult.Contains("ExceptionFailTest"));
            TestStartOnlyCalled();
        }

        [TestMethod()]
        public void ExceptionExpectingTest_IsTestedWithSuccess()
        {
            TestStateCorrectCleanup();
            MethodInfo exceptionThrowingMethod = _testedClass1Info.GetDeclaredMethod("ExceptionTest");
            string testResult = _methodTester.TestMethod(exceptionThrowingMethod);
            Assert.IsNull(testResult);
            TestStartOnlyCalled();
        }

        [TestMethod()]
        public void NullReferenceExceptionExpectingTest_IsTestedWithSuccess()
        {
            TestStateCorrectCleanup();
            MethodInfo exceptionThrowingMethod = _testedClass1Info.GetDeclaredMethod("NullReferenceExceptionTest");
            string testResult = _methodTester.TestMethod(exceptionThrowingMethod);
            Assert.IsNull(testResult);
            TestStartOnlyCalled();
        }

        private void TestStateCorrectCleanup()
        {
            Assert.IsFalse(_invoker.IsFirstSetUpRun);
            Assert.IsFalse(_invoker.IsSecondSetUpRun);
            Assert.IsFalse(_invoker.IsFirstTearDownRun);
        }

        private void TestStartAndFinishAreCalled()
        {
            Assert.IsTrue(_invoker.IsFirstSetUpRun);
            Assert.IsTrue(_invoker.IsSecondSetUpRun);
            Assert.IsTrue(_invoker.IsFirstTearDownRun);
        }

        private void TestStartOnlyCalled()
        {
            Assert.IsTrue(_invoker.IsFirstSetUpRun);
            Assert.IsTrue(_invoker.IsSecondSetUpRun);
            Assert.IsFalse(_invoker.IsFirstTearDownRun);
        }
    }
}