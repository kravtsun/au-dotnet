using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MyNUnitFramework.Attributes;   

using MethodResultCallback = System.Action<MyNUnit.TestingMethodResult>;

namespace MyNUnit
{
    internal class TypeTester
    {
        private readonly MethodResultCallback _reportAction;
        
        public TypeTester(MethodResultCallback reportAction)
        {
            _reportAction = reportAction;
        }

        public void TestType(Type type)
        {
            var testMethods = GetMethodsWithAttribute(type, typeof(TestAttribute));
            var startAction = StartActionForType(type);
            var finishAction = FinishActionForType(type);
            var methodTester = new MethodTester(startAction, finishAction);
            if (!RunClassTestMethods(type, false))
            {
                return;
            }

            var areAllTestMethodsStatic = testMethods.All(m => m.IsStatic);

            foreach (var testMethod in testMethods)
            {
                object invoker = null;

                if (!areAllTestMethodsStatic)
                {
                    try
                    {
                        var ctr = type.GetConstructor(Type.EmptyTypes);
                        if (ctr == null)
                        {
                            _reportAction(TestingMethodResult.BeforeFailure(testMethod, "no default constructor"));
                            continue;
                        }
                        invoker = ctr.Invoke(null);
                    }
                    catch (TargetInvocationException exception)
                    {
                        _reportAction(TestingMethodResult.BeforeFailure(testMethod, $"failed to initialize an object of type {type} with default constructor resulting in {exception}"));
                        continue;
                    }
                }

                var skipMessage = CheckIfSkippableTest(testMethod);
                if (skipMessage != null)
                {
                    _reportAction(TestingMethodResult.Skip(testMethod));
                    continue;
                }

                methodTester.Invoker = invoker;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var testMethodResult = methodTester.TestMethod(testMethod);
                testMethodResult.Elapsed = stopwatch.Elapsed;
                _reportAction(testMethodResult);
            }

            RunClassTestMethods(type, true);
        }

        internal static Action<object> StartActionForType(Type type)
        {
            var startMethods = GetMethodsWithAttribute(type, typeof(BeforeAttribute));
            return invoker =>
            {
                foreach (var startMethod in startMethods)
                {
                    startMethod.Invoke(invoker, null);
                }
            };
        }

        internal static Action<object> FinishActionForType(Type type)
        {
            var finishMethods = GetMethodsWithAttribute(type, typeof(AfterAttribute));
            return invoker =>
            {
                foreach (var finishMethod in finishMethods)
                {
                    finishMethod.Invoke(invoker, null);
                }
            };
        }

        private static ICollection<MethodInfo> GetMethodsWithAttribute(Type type, Type attributeType)
        {
            return type
                .GetMethods()
                .Where(methodInfo => methodInfo.GetCustomAttribute(attributeType) != null)
                .ToList();
        }

        // Check if testMethod is skippable and return reason message.
        private static string CheckIfSkippableTest(MethodBase testMethod)
        {
            if (testMethod.IsAbstract)
            {
                return "abstract";
            }

            var testAttribute = testMethod.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;
            return testAttribute?.IgnoreWithCause;
        }

        // returns true if methods' run was successful.
        private bool RunClassTestMethods(Type type, bool isAfterClass)
        {
            var classAttribute = isAfterClass ? typeof(AfterClassAttribute) : typeof(BeforeClassAttribute);
            foreach (var method in GetMethodsWithAttribute(type, classAttribute))
            {
                if (!method.IsStatic)
                {
                    const string errorMessage = "non-static";
                    _reportAction(isAfterClass
                        ? TestingMethodResult.AfterClassFailure(method, errorMessage)
                        : TestingMethodResult.BeforeClassFailure(method, errorMessage));
                    return false;
                }
                try
                {
                    method.Invoke(null, null);
                }
                catch (TargetInvocationException invocationException)
                {
                    var baseException = invocationException.GetBaseException();
                    _reportAction(isAfterClass
                        ? TestingMethodResult.AfterClassFailure(method, baseException)
                        : TestingMethodResult.BeforeClassFailure(method, baseException));
                    return false;
                }
            }
            return true;
        }
    }
}
