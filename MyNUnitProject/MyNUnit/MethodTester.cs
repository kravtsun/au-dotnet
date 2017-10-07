using MyNUnitFramework.Attribute;
using System;
using System.Reflection;

namespace MyNUnit
{
    internal class MethodTester
    {
        private readonly Action<object> _setUp;
        private readonly Action<object> _tearDown;

        internal MethodTester(Action<object> setUp, Action<object> tearDown)
        {
            _setUp = setUp;
            _tearDown = tearDown;
        }

        internal object Invoker { get; set; }

        // returns error message or null in case of success.
        internal string TestMethod(MethodInfo method)
        {
            var testAttribute = method.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;
            var expectedExceptionType = testAttribute?.Expected;
            try
            {
                _setUp(Invoker);
            }
            catch (TargetInvocationException invocationException)
            {
                Exception baseException = invocationException.GetBaseException();
                return MethodFailMessage("SetUp", baseException, null); 
            }
            catch (Exception exception)
            {
                return TestingFailMessage("SetUp", exception);
            }

            try
            {
                method.Invoke(Invoker, null);
            }
            catch (TargetInvocationException exception)
            {
                Exception catchedException = exception.GetBaseException();
                Type catchedExceptionType = catchedException.GetType();
                if (expectedExceptionType != null && expectedExceptionType.IsAssignableFrom(catchedExceptionType))
                {
                    return null;
                }

                return MethodFailMessage("running", catchedException, expectedExceptionType);
            }
            catch (Exception exception)
            {
                return TestingFailMessage("running", exception);
            }

            try
            {
                _tearDown(Invoker);
            }
            catch (TargetInvocationException invocationException)
            {
                Exception baseException = invocationException.GetBaseException();
                return MethodFailMessage("TearDown", baseException, null);
            }
            catch (Exception exception)
            {
                return TestingFailMessage("TearDown", exception);
            }

            return null;
        }

        private static string MethodFailMessage(string phase, Exception exception, Type expectedException) => expectedException == null ?
            $"failed while {phase} with message: {exception.Message}" :
            $"failed while {phase} with exception: {exception} when expected exception: {expectedException}";

        private static string TestingFailMessage(string phase, Exception exception) => $"Testing failed in {phase} with error: {exception.Message}";
    }
}
