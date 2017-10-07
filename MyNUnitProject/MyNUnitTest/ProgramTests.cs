using System;
using System.Net.NetworkInformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace MyNUnitTest
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void MainTest()
        {
            string[] args = {AppDomain.CurrentDomain.BaseDirectory};

            var clientConsoleTarget = LogManager.Configuration.FindTargetByName<ConsoleTarget>("client_console");

            var newConfig = new LoggingConfiguration();
            var memoryTarget = new MemoryTarget();
            newConfig.AddTarget("memory", memoryTarget);
            memoryTarget.Layout = clientConsoleTarget.Layout;

            newConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, memoryTarget));

            LogManager.Configuration = newConfig;
            
            Program.Main(args);
            Assert.IsFalse(TestedAssembly.TestedClass1.IsIgnoredRun);
            Assert.AreEqual(3.14, TestedAssembly.TestedClass2.MustBePi);

            string[] expectedLogs =
            {   "Testing assembly: MyNUnit",
                "Testing assembly: Microsoft.VisualStudio.QualityTools.UnitTestFramework",
                "Testing assembly: MyNUnitFramework",
                "Testing assembly: MyNUnitTest",
                "Testing assembly: NLog",
                "Testing assembly: TestedAssembly",
                "SUCCESS: TestedClass2.SimpleTest",
                "FAILED: TestedClass2.SimpleFailTest with message: failed while running with message: SimpleFailTest",
                "SUCCESS: TestedClass1.SimpleTest",
                "SUCCESS: TestedClass1.ExceptionTest",
                "SUCCESS: TestedClass1.NullReferenceExceptionTest",
                "FAILED: TestedClass1.ExceptionFailTest with message: failed while running with message: ExceptionFailTest",
                "FAILED: TestedClass1.AssertFailTest with message: failed while running with message: Debug.Assert: FAIL",
                "SKIPPED: TestedClass1.IgnoreTest"
            };

            var logs = memoryTarget.Logs;
            Assert.AreEqual(expectedLogs.Length, logs.Count);

            for (int i = 0; i < logs.Count; ++i)
            {
                string rawLogMessage = CutTimeStampSuffix(logs[i]);
                Assert.AreEqual(expectedLogs[i], rawLogMessage);
                System.Console.WriteLine(logs[i]);
            }
        }

        private static string CutTimeStampSuffix(string message)
        {
            string timeSplitter = TypeTester.TimeSplitter;
            int timeSplitterStartIndex = message.Length - timeSplitter.Length - 1;
            while (timeSplitterStartIndex >= 0)
            {
                if (message.Substring(timeSplitterStartIndex, timeSplitter.Length).Equals(timeSplitter))
                {
                    break;
                }
                --timeSplitterStartIndex;
            }

            if (timeSplitterStartIndex == -1)
            {
                timeSplitterStartIndex = message.Length;
            }

            return message.Substring(0, timeSplitterStartIndex);
        }
    }
}