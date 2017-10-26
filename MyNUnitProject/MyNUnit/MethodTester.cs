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

        internal TestingMethodResult TestMethod(MethodInfo method)
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
                return TestingMethodResult.BeforeFailure(method, baseException);
            }

            try
            {
                method.Invoke(Invoker, null);
            }
            catch (TargetInvocationException exception)
            {
                var catchedException = exception.GetBaseException();
                var catchedExceptionType = catchedException.GetType();

                if (expectedExceptionType == null)
                {
                    return TestingMethodResult.MethodExceptionFailure(method, catchedException);
                }
                if (expectedExceptionType.IsAssignableFrom(catchedExceptionType))
                {
                    return TestingMethodResult.Success(method);
                }
                return TestingMethodResult.MethodExpectedExceptionFailure(method, catchedException, expectedExceptionType);
            }

            try
            {
                TearDown(Invoker);
            }
            catch (TargetInvocationException invocationException)
            {
                var baseException = invocationException.GetBaseException();
                return TestingMethodResult.AfterFailure(method, baseException);
            }

            return TestingMethodResult.Success(method);
        }

        //internal static string MethodFailMessage(string phase, Exception exception, Type expectedException)
        //{
        //    return expectedException == null
        //        ? $"failed while {phase} with message: {exception.Message}"
        //        : $"failed while {phase} with exception: {exception} when expected exception: {expectedException}";
        //}
    }
}
