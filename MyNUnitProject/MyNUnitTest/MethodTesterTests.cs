using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;
using TestedAssembly;

namespace MyNUnitTest
{
    [TestClass]
    public class MethodTesterTests
    {
        private MethodTester _methodTester;
        private TestedClass1 _invoker;
        private readonly TypeInfo _testedClass1Info;
        private readonly Action<object> _finishAction;
        private readonly Action<object> _startAction;
        private readonly ConstructorInfo _constructor;

        public MethodTesterTests()
        {
            string testedAssemblyPath = Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .First(path => path.Contains("TestedAssembly"));
            var testedAssembly = Assembly.LoadFrom(testedAssemblyPath);
            _testedClass1Info = testedAssembly.GetType("TestedAssembly.TestedClass1").GetTypeInfo();
            
            _startAction = TypeTester.StartActionForType(_testedClass1Info.AsType());
            _finishAction = TypeTester.FinishActionForType(_testedClass1Info.AsType());
            _constructor = _testedClass1Info.GetConstructor(Type.EmptyTypes);
        }

        [TestInitialize]
        public void SetUp()
        {
            _invoker = (TestedClass1) _constructor.Invoke(null);
            Assert.IsNotNull(_invoker);
            _methodTester = new MethodTester(_startAction, _finishAction) {Invoker = _invoker};
        }

        [TestMethod]
        public void SimpleTestMethod_IsTestedSuccessfullyWithTestContracts()
        {
            TestStateCorrectCleanup();
            var simpleMethod = _testedClass1Info.GetDeclaredMethod("SimpleTest");
            var testResult = _methodTester.TestMethod(simpleMethod);
            Assert.IsTrue(testResult.IsSuccess());
            TestStartAndFinishAreCalled();
        }

        [TestMethod]
        public void ExceptionThrowingTest_IsTestedWithExceptionMessageReturned()
        {
            TestStateCorrectCleanup();
            var exceptionThrowingMethod = _testedClass1Info.GetDeclaredMethod("ExceptionFailTest");
            var testResult = _methodTester.TestMethod(exceptionThrowingMethod);
            Assert.IsTrue(testResult.Message.Contains("ExceptionFailTest"));
            TestStartOnlyCalled();
        }

        [TestMethod]
        public void ExceptionExpectingTest_IsTestedWithSuccess()
        {
            TestStateCorrectCleanup();
            var exceptionThrowingMethod = _testedClass1Info.GetDeclaredMethod("ExceptionTest");
            var testResult = _methodTester.TestMethod(exceptionThrowingMethod);
            Assert.IsTrue(testResult.IsSuccess());
            TestStartOnlyCalled();
        }

        [TestMethod]
        public void NullReferenceExceptionExpectingTest_IsTestedWithSuccess()
        {
            TestStateCorrectCleanup();
            var exceptionThrowingMethod = _testedClass1Info.GetDeclaredMethod("NullReferenceExceptionTest");
            var testResult = _methodTester.TestMethod(exceptionThrowingMethod);
            Assert.IsTrue(testResult.IsSuccess());
            TestStartOnlyCalled();
        }

        [TestMethod]
        public void TestingMethodFailsIfSetUpThrowsException()
        {
            const string exceptionMessage = "SetUpException";
            Action<object> newStartAction = obj =>
            {
                _startAction(obj);
                throw new TargetInvocationException(new Exception(exceptionMessage));
            };
            _methodTester.SetUp = newStartAction;
            var simpleMethod = _testedClass1Info.GetDeclaredMethod("SimpleTest");

            var testResult = _methodTester.TestMethod(simpleMethod);
            Assert.IsTrue(testResult.Message.Contains(exceptionMessage));
            TestStartOnlyCalled();
        }

        [TestMethod]
        public void TestingMethodFailsIfTearDownThrowsException()
        {
            const string exceptionMessage = "TearDownException";
            Action<object> newFinishAction = obj =>
            {
                throw new TargetInvocationException(new Exception(exceptionMessage));
            };
            _methodTester.TearDown = newFinishAction;
            var simpleMethod = _testedClass1Info.GetDeclaredMethod("SimpleTest");

            var testResult = _methodTester.TestMethod(simpleMethod);
            Assert.IsTrue(testResult.Message.Contains(exceptionMessage));
            TestStartOnlyCalled();
        }

        [TestMethod]
        [ExpectedException(typeof(TargetException))]
        public void TestingNonStaticMethodFailsIfNoObject()
        {
            _invoker = null;
            _methodTester.Invoker = null;
            var simpleMethod = _testedClass1Info.GetDeclaredMethod("SimpleTest");
            var testResult = _methodTester.TestMethod(simpleMethod);
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