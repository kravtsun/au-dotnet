using MyNUnitFramework.Attributes;
using System;
using System.Reflection;

namespace MyNUnit
{
    internal class MethodTester
    {
        internal MethodTester(Action<object> setUp, Action<object> tearDown)
        {
            SetUp = setUp;
            TearDown = tearDown;
        }

        internal object Invoker { private get; set; }

        internal Action<object> TearDown { private get; set; }

        internal Action<object> SetUp { private get; set; }

        // returns error message or null in case of success.
        internal string TestMethod(MethodInfo method)
        {
            var testAttribute = method.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;
            var expectedExceptionType = testAttribute?.Expected;
            try
            {
                SetUp(Invoker);
            }
            catch (TargetInvocationException invocationException)
            {
                var baseException = invocationException.GetBaseException();
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
                var catchedException = exception.GetBaseException();
                var catchedExceptionType = catchedException.GetType();
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
                TearDown(Invoker);
            }
            catch (TargetInvocationException invocationException)
            {
                var baseException = invocationException.GetBaseException();
                return MethodFailMessage("TearDown", baseException, null);
            }
            catch (Exception exception)
            {
                return TestingFailMessage("TearDown", exception);
            }

            return null;
        }

        internal static string MethodFailMessage(string phase, Exception exception, Type expectedException)
        {
            return expectedException == null
                ? $"failed while {phase} with message: {exception.Message}"
                : $"failed while {phase} with exception: {exception} when expected exception: {expectedException}";
        }

        internal static string TestingFailMessage(string phase, Exception exception)
        {
            return $"Testing failed in {phase} with error: {exception.Message}";
        }
    }
}
