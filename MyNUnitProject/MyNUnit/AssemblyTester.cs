using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MyNUnitFramework.Attribute;

using MethodResultCallback = System.Action<System.Reflection.MethodInfo, string>;

namespace MyNUnit
{
    interface IAssemblyTester
    {
        void TestAssembly(Assembly assembly);
    }

    internal class AssemblyTester : IAssemblyTester
    {
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
        private class DebugAssertException : Exception{
            public DebugAssertException(string message)
                : base(message)
            {}

            protected DebugAssertException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context)
                : base(info, context)
            {}
        }

        public AssemblyTester(MethodResultCallback successAction, MethodResultCallback failAction, MethodResultCallback skipAction)
        {
            _successAction = successAction;
            _failAction = failAction;
            _skipAction = skipAction;
        }

        public void TestAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                return;
            }

            var publicTypes = assembly.GetExportedTypes();
            foreach (var type in publicTypes)
            {
                var testMethods = GetMethodsWithAttribute(type, typeof(TestAttribute));
                var startAction = StartActionForType(type);
                var finishAction = FinishActionForType(type);
                MethodTester methodTester = new MethodTester(startAction, finishAction);

                bool areAllTestMethodsStatic = testMethods.All(m => m.IsStatic);

                foreach (var testMethod in testMethods)
                {
                    TraceListener debugAssertListener = new MyTraceListener(msg =>
                    {
                        throw new DebugAssertException($"Debug.Assert failed: {msg}");
                    });
                    Debug.Listeners.Clear();
                    Debug.Listeners.Add(debugAssertListener);

                    object invoker = null;

                    if (!areAllTestMethodsStatic)
                    {
                        try
                        {
                            var ctr = type.GetConstructor(Type.EmptyTypes);
                            if (ctr == null)
                            {
                                _failAction(null,
                                    $"Type {type} has to have default constructor as there are non-static test methods.");
                               continue;
                            }
                            invoker = ctr.Invoke(null);
                        }
                        catch (Exception exception)
                        {
                            _failAction(null,
                                $"Failed to initialize an object of type {type} with default constructor resulting in {exception}");
                            continue;
                        }
                    }

                    string skipMessage = CheckIfSkippableTest(invoker, testMethod);
                    if (skipMessage != null)
                    {
                        _skipAction(testMethod, skipMessage);
                        continue;
                    }

                    methodTester.Invoker = invoker;
                    string errorMessage = methodTester.TestMethod(testMethod);
                    if (errorMessage == null)
                    {
                        _successAction(testMethod, "");
                    }
                    else
                    {
                        _failAction(testMethod, errorMessage);
                    }
                }
                Debug.Listeners.Clear();
            }
        }

        internal static Action<object> StartActionForType(Type type)
        {
            var startMethods = GetMethodsWithAttribute(type, typeof(BeforeClassAttribute));
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
            var finishMethods = GetMethodsWithAttribute(type, typeof(AfterClassAttribute));
            return invoker =>
            {
                foreach (var finishMethod in finishMethods)
                {
                    finishMethod.Invoke(invoker, null);
                }
            };
        }

        // Check if testMethod is skippable and return reason message.
        private string CheckIfSkippableTest(object invoker, MethodBase testMethod)
        {
            if (invoker == null && !testMethod.IsStatic)
            {
                return "non-static";
            }

            if (testMethod.IsAbstract)
            {
                return "abstract";
            }

            var testAttribute = testMethod.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;
            return testAttribute?.IgnoreWithCause;
        }

        private static ICollection<MethodInfo> GetMethodsWithAttribute(Type type, Type attributeType)
        {
            return type
                .GetMethods()
                .Where(methodInfo => methodInfo.GetCustomAttribute(attributeType) != null)
                .ToList();
        }
    }
}
