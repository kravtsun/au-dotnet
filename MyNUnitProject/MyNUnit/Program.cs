using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;

namespace MyNUnit
{
    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Func<MethodInfo, string> PrettyMethodName = method =>
        {
            return method == null ? "null" : $"{method.ReflectedType?.Name}.{method.Name}";
        };

        private static readonly Action<MethodInfo, string> SuccessAction = (method, message) =>
        {
            var methodName = PrettyMethodName(method);
            var clientMessage = $"SUCCESS: {methodName}" + message;
            Logger.Info(clientMessage);
        };

        private static readonly Action<MethodInfo, string> FailAction = (method, message) =>
        {
            var methodName = PrettyMethodName(method);
            var clientMessage = $"FAILED: {methodName} with message: {message}";
            Logger.Info(clientMessage);
        };

        private static readonly Action<MethodInfo, string> SkipAction = (method, message) =>
        {
            var methodName = PrettyMethodName(method);
            Logger.Info($"SKIPPED: {methodName}");
        };

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                var executablePath = Assembly.GetEntryAssembly().Location;
                Console.WriteLine($"USAGE: {executablePath} <assemblies directory>");
                return;
            }
            var assemblyDir = args[0];

            var assemblyPaths = GetAssemblyLikePaths(assemblyDir);
            
            foreach (var assemblyPath in assemblyPaths)
            {
                Logger.Debug($"assembly path: {assemblyPath}");

                var assembly = LoadAssembly(assemblyPath);

                if (assembly == null)
                {
                    continue;
                }

                var typeTester = new TypeTester(SuccessAction, FailAction, SkipAction);

                foreach (var type in assembly.GetExportedTypes())
                {
                    typeTester.TestType(type);
                }
            }
        }

        private static Assembly LoadAssembly(string assemblyPath)
        {
            var currentDomain = AppDomain.CurrentDomain;

            AssemblyName assemblyName = null;
            try
            {
                assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            }
            catch (BadImageFormatException)
            {
                Logger.Info($"Bad assembly: {assemblyPath}");
            }
            catch (Exception)
            {
                Logger.Error($"Error while loading assembly: {assemblyPath}");
            }

            if (assemblyName == null)
            {
                return null;
            }

            Logger.Info($"Testing assembly: {assemblyName.Name}");

            Assembly assembly = null;
            try
            {
                assembly = currentDomain.Load(assemblyName);
            }
            catch (Exception exception)
            {
                Logger.Error($"Error while loading assembly: {exception.Message}");
            }

            return assembly;
        }

        private static IEnumerable<string> GetAssemblyLikePaths(string dir)
        {
            var exeFiles = Directory.EnumerateFiles(dir, "*.exe");
            var dllFiles = Directory.EnumerateFiles(dir, "*.dll");
            return exeFiles.Concat(dllFiles);
        }
    }
}
