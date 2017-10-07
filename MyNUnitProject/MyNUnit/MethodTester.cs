using MyNUnitFramework.Attribute;
using System;
using System.Reflection;

namespace MyNUnit
{
    internal class MethodTester
    {
        private Action<object> setUp;
        private Action<object> tearDown;

        internal MethodTester(Action<object> setUp, Action<object> tearDown)
        {
            SetUp = setUp;
            TearDown = tearDown;
        }

        internal object Invoker { get; set; }

        internal Action<object> TearDown
        {
            get
            {
                return tearDown;
            }

            set
            {
                tearDown = value;
            }
        }

        internal Action<object> SetUp
        {
            get
            {
                return setUp;
            }

            set
            {
                setUp = value;
            }
        }

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
                TearDown(Invoker);
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
