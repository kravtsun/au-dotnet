﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MyNUnitFramework.Attributes;   

using MethodResultCallback = System.Action<System.Reflection.MethodInfo, string>;

namespace MyNUnit
{
    internal class TypeTester
    {
        internal const string TimeSplitter = "### time: ";
        private readonly MethodResultCallback _successAction;
        private readonly MethodResultCallback _failAction;
        private readonly MethodResultCallback _skipAction;

        private class MyTraceListener : TraceListener
        {
            private readonly Action<string> _messageAction;

            public MyTraceListener(Action<string> messageAction)
            {
                _messageAction = messageAction;
            }

            public override void Fail(string msg) => _messageAction(msg);

            public override void Write(string message) => _messageAction(message);

            public override void WriteLine(string message) => _messageAction(message);
        }

        [Serializable]
        private class DebugAssertException : Exception
        {
            public DebugAssertException(string message)
                : base(message)
            { }

            protected DebugAssertException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            { }
        }

        public TypeTester(MethodResultCallback successAction, MethodResultCallback failAction, MethodResultCallback skipAction)
        {
            _successAction = successAction;
            _failAction = failAction;
            _skipAction = skipAction;
        }

        public void TestType(Type type)
        {
            var debugAssertListener = new MyTraceListener(msg =>
            {
                throw new DebugAssertException($"Debug.Assert: {msg}");
            });
            Debug.Listeners.Clear();
            Debug.Listeners.Add(debugAssertListener);

            var testMethods = GetMethodsWithAttribute(type, typeof(TestAttribute));
            var startAction = StartActionForType(type);
            var finishAction = FinishActionForType(type);
            var methodTester = new MethodTester(startAction, finishAction);
            var beforeClassErrorMessage = RunClassTestMethods(type, false);
            if (beforeClassErrorMessage != null)
            {
                _failAction(null, beforeClassErrorMessage);
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
                            _failAction(null,
                                $"type {type} has to have default constructor as there are non-static test methods.");
                            continue;
                        }
                        invoker = ctr.Invoke(null);
                    }
                    catch (Exception exception)
                    {
                        _failAction(null,
                            $"failed to initialize an object of type {type} with default constructor resulting in {exception}");
                        continue;
                    }
                }

                var skipMessage = CheckIfSkippableTest(testMethod);
                if (skipMessage != null)
                {
                    _skipAction(testMethod, skipMessage);
                    continue;
                }

                methodTester.Invoker = invoker;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var errorMessage = methodTester.TestMethod(testMethod);
                var elapsed = stopwatch.Elapsed;
                var elapsedFormatted = $"{TimeSplitter}{elapsed.ToString()}";
                if (errorMessage == null)
                {
                    _successAction(testMethod, $"{elapsedFormatted}");
                }
                else
                {
                    _failAction(testMethod, $"{errorMessage}{elapsedFormatted}");
                }
            }

            var afterClassErrorMessage = RunClassTestMethods(type, true);
            if (afterClassErrorMessage != null)
            {
                _failAction(null, afterClassErrorMessage);
                return;
            }

            Debug.Listeners.Clear();
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

        private static string RunClassTestMethods(Type type, bool isAfterClass)
        {
            var classAttribute = isAfterClass ? typeof(AfterClassAttribute) : typeof(BeforeClassAttribute);
            foreach (var method in GetMethodsWithAttribute(type, classAttribute))
            {
                if (!method.IsStatic)
                {
                    var methodType = isAfterClass ? "After" : "Before";
                    return $"{methodType}Class method {method} is non-static";
                }
                try
                {
                    method.Invoke(null, null);
                }
                catch (TargetInvocationException invocationException)
                {
                    var baseException = invocationException.GetBaseException();
                    return MethodTester.MethodFailMessage("BeforeClass", baseException, null);
                }
                catch (Exception exception)
                {
                    return MethodTester.TestingFailMessage("BeforeClass", exception);
                }
            }
            return null;
        }
    }
}
