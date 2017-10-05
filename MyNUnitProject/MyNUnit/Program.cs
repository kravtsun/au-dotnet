using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using MyNUnitFramework.Attribute;
using NLog;

namespace MyNUnit
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            //foreach (var arg in args)
            //{
            //    System.Console.WriteLine(arg);
            //}
            //System.Console.WriteLine(Directory.GetCurrentDirectory());

            var assemblyDir = args[0];

            foreach (var assemblyPath in GetAssemblyLikePaths(assemblyDir))
            {
                Logger.Debug("assembly path: {0}", assemblyPath);
                var currentDomain = AppDomain.CreateDomain("CurrentDomain");

                AssemblyName assemblyName = null;
                try
                {
                    assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                }
                catch (BadImageFormatException)
                {
                    Logger.Info("Not an assembly: {0}", assemblyPath);
                }
                catch (Exception)
                {
                    Logger.Error("Error while loading assembly: {0}", assemblyPath);
                }

                if (assemblyName == null)
                {
                    continue;
                }

                // TODO: check if loading recursively all needed assemblies.
                var assembly = currentDomain.Load(assemblyName);

                foreach (var type in assembly.GetExportedTypes())
                {
                    var startMethods = GetMethodsWithAttribute(type, typeof(BeforeClassAttribute));
                    var finishMethods = GetMethodsWithAttribute(type, typeof(AfterClassAttribute));
                    var testMethods = GetMethodsWithAttribute(type, typeof(TestAttribute));

                    if (testMethods.Count == 0)
                    {
                        continue;
                    }

                    foreach (var testMethod in testMethods)
                    {
                        // TODO skip abstract methods (or other, not appropriate to run in current context).

                        var invoker = GetInvokerForType(type);
                        if (invoker == null && !testMethod.IsStatic)
                        {
                            Logger.Info($"non-static test method {testMethod}, skipping...");
                            continue;
                        }
                        RunTestMethod(invoker, testMethod, startMethods, finishMethods);
                    }
                }
            }
        }

        private static IEnumerable<string> GetAssemblyLikePaths(string dir)
        {
            var exeFiles = Directory.EnumerateFiles(dir, "*.exe");
            var dllFiles = Directory.EnumerateFiles(dir, "*.dll");
            return exeFiles.Concat(dllFiles);
        }

        private static object GetInvokerForType(Type type)
        {
            var testMethods = GetMethodsWithAttribute(type, typeof(TestAttribute));

            if (testMethods.Any(m => !m.IsStatic))
            {
                try
                {
                    var ctr = type.GetConstructor(null);
                    if (ctr == null)
                    {
                        Logger.Error(
                            $"Type {type} has to have default constructor as there are non-static test methods.");
                        return null;
                    }
                    return ctr.Invoke(null);
                }
                catch (Exception exception)
                {
                    Logger.Error($"Failed to initialize an object of type {type} with defaul constructor resulting in {exception}");
                }
            }
            return null;
        }

        // TODO class for getting methods with given attribute.
        private static ICollection<MethodInfo> GetMethodsWithAttribute(Type type, Type attributeType)
        {
            return type
                .GetMethods()
                .Where(methodInfo => methodInfo.GetCustomAttribute(attributeType) != null)
                .ToList();
        }

        private static void RunTestMethod(object invoker, MethodInfo testMethod, IEnumerable<MethodInfo> startMethods, IEnumerable<MethodInfo> finishMethods)
        {
            var testAttribute = testMethod.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;

            Debug.Assert(testAttribute != null, nameof(testAttribute) + " != null");
            if (testAttribute.IgnoreWithCause != null)
            {
                Logger.Info(
                    $"test method {testMethod} is ignored with cause: {testAttribute.IgnoreWithCause}, skipping...");
                return;
            }

            var expectedException = testAttribute.Expected;

            // TODO how to catch exception from another domain? 
            // shall I do marshalling?
            try
            {
                foreach (var startMethod in startMethods)
                {
                    startMethod.Invoke(invoker, null);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"StartUp for method: {testMethod} failed with exception: {e}");
                return;
            }

            Exception catchedException = null;
            try
            {
                testMethod.Invoke(invoker, null);
            }
            catch (Exception e)
            {
                catchedException = e;
            }

            if (catchedException != null && expectedException != catchedException.GetType())
            {
                Logger.Info(
                    $"testMethod: {testMethod} failed with exception: {catchedException} when expected exception: {expectedException}");
            }

            try
            {
                foreach (var finishMethod in finishMethods)
                {
                    finishMethod.Invoke(invoker, null);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"TearDown for method: {testMethod} failed with exception: {e}");
            }
        }
    }
}
