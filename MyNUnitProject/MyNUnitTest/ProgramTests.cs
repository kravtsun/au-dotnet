using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;
using NLog;
using NLog.Config;
using NLog.Targets;
using TestedAssembly;

namespace MyNUnitTest
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
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
            Assert.IsFalse(TestedClass1.IsIgnoredRun);
            Assert.AreEqual(3.14, TestedClass2.MustBePi);
            Assert.IsTrue(TestedClass2.IsAfterRun);
            Assert.IsTrue(TestedClass2.IsAfterNonStaticRun);
            Assert.IsTrue(TestedClass2.IsBeforeRun);
            Assert.IsTrue(TestedClass2.IsBeforeNonStaticRun);
            Assert.IsFalse(FailStartClass.IsTestRun);
            Assert.IsTrue(FailFinishClass.IsTestRun);
            Assert.IsFalse(FailBeforeClassNonStaticTestedClass.IsBeforeClassRun);
            Assert.IsFalse(FailBeforeClassNonStaticTestedClass.IsTestRun);
            Assert.IsFalse(FailBeforeClassExceptionTestedClass.IsTestRun);
            Assert.IsTrue(FailAfterClassExceptionTestedClass.IsTestRun);

            string[] expectedLogs =
            {   "Testing assembly: MyNUnit",
                "Testing assembly: Microsoft.VisualStudio.QualityTools.UnitTestFramework",
                "Testing assembly: MyNUnitFramework",
                "Testing assembly: MyNUnitTest",
                "Testing assembly: NLog",
                "Testing assembly: TestedAssembly",
                "SUCCESS: FailAfterClassExceptionTestedClass.Test",

                "FAILED: FailAfterClassExceptionTestedClass.FailAfterClass[AfterClass]: throws Exception with message: FailAfterClass",
                "FAILED: FailBeforeClassExceptionTestedClass.FailBeforeClass[BeforeClass]: throws Exception with message: FailBeforeClass",

                "FAILED: FailBeforeClassNonStaticTestedClass.BeforeClass[BeforeClass]: non-static",
                "FAILED: FailFinishClass.Test[After]: throws Exception with message: FailTearDown",
                "FAILED: FailStartClass.Test[Before]: throws Exception with message: FailSetUp",
                "SUCCESS: TestedClass2.SimpleTest",
                "FAILED: TestedClass2.SimpleFailTest[Running]: throws Exception with message: SimpleFailTest",
                "SUCCESS: TestedClass1.SimpleTest",
                "SUCCESS: TestedClass1.ExceptionTest",
                "SUCCESS: TestedClass1.NullReferenceExceptionTest",
                "FAILED: TestedClass1.ExceptionFailTest[Running]: throws Exception with message: ExceptionFailTest",
                "SKIPPED: TestedClass1.IgnoreTest"
            };
            
            var logs = memoryTarget.Logs;

            Assert.AreEqual(expectedLogs.Length, logs.Count);
            for (int i = 0; i < logs.Count; ++i)
            {
                string rawLogMessage = CutTimeStampSuffix(logs[i]);
                Assert.AreEqual(expectedLogs[i], rawLogMessage);
            }
        }

        private static string CutTimeStampSuffix(string message)
        {
            const string timeSplitter = TestingMethodResult.TimeSplitter;
            var timeSplitterStartIndex = message.Length - timeSplitter.Length - 1;
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