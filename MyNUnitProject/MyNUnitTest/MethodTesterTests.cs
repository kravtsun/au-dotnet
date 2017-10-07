using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;
using TestedAssembly;

namespace MyNUnitTest
{
    [TestClass()]
    public class MethodTesterTests
    {
        private MethodTester _methodTester;
        private TestedClass1 _invoker;
        private TypeInfo _testedClass1Info;
        private Action<object> _finishAction;
        private Action<object> _startAction;
        private ConstructorInfo _constructor;

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

        [TestMethod]
        public void TestingMethodFailsIfSetUpThrowsException()
        {
            string exceptionMessage = "SetUpException";
            Action<object> newStartAction = (obj) =>
            {
                _startAction(obj);
                throw new Exception(exceptionMessage);
            };
            _methodTester.SetUp = newStartAction;
            MethodInfo simpleMethod = _testedClass1Info.GetDeclaredMethod("SimpleTest");

            var testResult = _methodTester.TestMethod(simpleMethod);
            Assert.IsTrue(testResult.Contains(exceptionMessage));
            TestStartOnlyCalled();
        }

        [TestMethod]
        public void TestingMethodFailsIfTearDownThrowsException()
        {
            string exceptionMessage = "TearDownException";
            Action<object> newFinishAction = (obj) =>
            {
                _startAction(obj);
                throw new Exception(exceptionMessage);
            };
            _methodTester.TearDown = newFinishAction;
            MethodInfo simpleMethod = _testedClass1Info.GetDeclaredMethod("SimpleTest");

            var testResult = _methodTester.TestMethod(simpleMethod);
            Assert.IsTrue(testResult.Contains(exceptionMessage));
            TestStartOnlyCalled();
        }

        [TestMethod]
        public void TestingNonStaticMethodFailsIfNoObject()
        {
            _invoker = null;
            _methodTester.Invoker = null;
            MethodInfo simpleMethod = _testedClass1Info.GetDeclaredMethod("SimpleTest");
            var testResult = _methodTester.TestMethod(simpleMethod);
            Assert.IsTrue(testResult.Contains("Non-static method requires a target."));
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