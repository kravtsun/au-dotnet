using System;
using System.Reflection;

namespace MyNUnit
{
    public class TestingMethodResult
    {
        private enum Verdict
        {
            Success,
            Skip,
            Fail
        }

        private enum Stage
        {
            BeforeClass,
            Before,
            Running,
            After,
            AfterClass,
            None
        }

        internal const string TimeSplitter = "### time: ";
        public TimeSpan Elapsed { set; private get; } = TimeSpan.Zero;
        private readonly Verdict _verdict;
        private readonly string _message;

        public static TestingMethodResult Success(MethodInfo method)
        {
            return new TestingMethodResult(Verdict.Success, method);
        }

        public static TestingMethodResult Skip(MethodInfo method)
        {
            return new TestingMethodResult(Verdict.Skip, method);
        }

        public static TestingMethodResult BeforeClassFailure(MethodInfo method, string message)
        {
            return new TestingMethodResult(Verdict.Fail, method, Stage.BeforeClass, message);
        }

        public static TestingMethodResult BeforeClassFailure(MethodInfo method, Exception exception)
        {
            return BeforeClassFailure(method, ExceptionMessage(exception));
        }

        public static TestingMethodResult BeforeFailure(MethodInfo method, string message)
        {
            return new TestingMethodResult(Verdict.Fail, method, Stage.Before, message);
        }

        public static TestingMethodResult BeforeFailure(MethodInfo method, Exception exception)
        {
            return BeforeFailure(method, ExceptionMessage(exception));
        }

        public static TestingMethodResult AfterClassFailure(MethodInfo method, string message)
        {
            return new TestingMethodResult(Verdict.Fail, method, Stage.AfterClass, message);
        }

        public static TestingMethodResult AfterClassFailure(MethodInfo method, Exception exception)
        {
            return AfterClassFailure(method, ExceptionMessage(exception));
        }
        
        public static TestingMethodResult AfterFailure(MethodInfo method, Exception exception)
        {
            return new TestingMethodResult(Verdict.Fail, method, Stage.After, ExceptionMessage(exception));
        }

        public static TestingMethodResult MethodExceptionFailure(MethodInfo method, Exception exception)
        {
            return new TestingMethodResult(Verdict.Fail, method, Stage.Running, ExceptionMessage(exception));
        }

        public static TestingMethodResult MethodExpectedExceptionFailure(MethodInfo method, Exception exception,
            Type expectedException)
        {
            var errorMessage = $"expected exception of type {expectedException} but got exception: {exception}";
            return new TestingMethodResult(Verdict.Fail, method, Stage.Running, errorMessage);
        }

        public string Message
        {
            get
            {
                var message = VerdictMessage(_verdict) + _message;
                if (Elapsed != TimeSpan.Zero)
                {
                    message += TimeSplitter + Elapsed;
                }
                return message;
            }
        }

        public bool IsSuccess()
        {
            return _verdict == Verdict.Success;
        }

        private TestingMethodResult(Verdict verdict, MethodInfo method, Stage stage = Stage.None, string message = "")
        {
            _verdict = verdict;
            _message = method == null ? "null" : $"{method.ReflectedType?.Name}.{method.Name}";
            switch (stage)
            {
                case Stage.BeforeClass:
                    _message += "[BeforeClass]";
                    break;
                case Stage.Before:
                    _message += "[Before]";
                    break;
                case Stage.Running:
                    _message += "[Running]";
                    break;
                case Stage.After:
                    _message += "[After]";
                    break;
                case Stage.AfterClass:
                    _message += "[AfterClass]";
                    break;
                case Stage.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
            }
            if (message.Length > 0)
            {
                _message += ": " + message;
            }
        }

        private static string VerdictMessage(Verdict verdict)
        {
            switch (verdict)
            {
                case Verdict.Success:
                    return "SUCCESS: ";
                case Verdict.Skip:
                    return "SKIPPED: ";
                case Verdict.Fail:
                    return "FAILED: ";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string ExceptionMessage(Exception exception)
        {
            return $"throws {exception.GetType().Name} with message: {exception.Message}";
        }
    }
}
