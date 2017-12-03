using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrimesApp.Properties;

namespace PrimesApp
{
    public sealed partial class TaskView : UserControl
    {
        private static readonly TaskScheduler GuiTaskScheduler = 
            TaskScheduler.FromCurrentSynchronizationContext();

        private readonly int _x;
        private CancellationTokenSource _cancelSource;

        public TaskView(int x)
        {
            InitializeComponent();
            nameLabel.Text = x.ToString();
            _x = x;
            RunTaskInGui(LifeInGui);
        }

        // returns cancellation callback.
        private Task<EventHandler> InitializeTask()
        {
            return Task.Run(() =>
            {
                State = TaskStatus.Created;
                _cancelSource = new CancellationTokenSource();
                EventHandler cancelButtonCallback = delegate
                {
                    State = TaskStatus.Canceled;
                    _cancelSource.Cancel();
                };
                return cancelButtonCallback;
            });
        }

        private Task<int> CalculationTask()
        {
            return Task.Run(() =>
            {
                var progressBarUpdateStep = _x / 100 + 1;
                Action<int> iterAction = i =>
                {
                    _cancelSource.Token.ThrowIfCancellationRequested();
                    if (i % progressBarUpdateStep == 0)
                    {
                        progressBar.Invoke(new UpdateFormElementCallback(() =>
                        {
                            progressBar.Value = i;
                        }));
                    }
                };
                State = TaskStatus.Running;
                var result = GetNumberOfPrimes(_x, iterAction);
                State = TaskStatus.RanToCompletion;
                return result;
            });
        }

        private async void LifeInGui()
        {
            var initializeTask = InitializeTask();

            progressBar.Maximum = _x;
            progressBar.Visible = true;
            progressBar.Value = 0;

            var cancellationCallback = await initializeTask;
            cancelButton.Click += cancellationCallback;

            await Task.Run(() =>
            {
                State = TaskStatus.WaitingToRun;
            });

            try
            {
                var result = await CalculationTask();
                var resultLabel = new Label
                {
                    Text = string.Format(Resources.result_string_prefix, result),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top
                             | AnchorStyles.Bottom
                             | AnchorStyles.Left
                             | AnchorStyles.Right,
                    AutoSize = true
                };
                layoutPanel.Controls.Add(resultLabel);
            }
            catch (OperationCanceledException e)
            {
                if (e.CancellationToken != _cancelSource.Token)
                {
                    throw;
                }
                await Task.Run(() =>
                {
                    State = TaskStatus.Canceled;
                });
            }
            finally
            {
                // TaskView's State should be set appropriately beforehand.
                progressBar.Visible = false;
                cancelButton.Visible = false;
                cancelButton.Click -= cancellationCallback;
            }
        }

        private TaskStatus State
        {
            set
            {
                string valueString;
                switch (value)
                {
                    case TaskStatus.Created:
                        valueString = "Starting...";
                        break;
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.WaitingToRun:
                    case TaskStatus.WaitingForChildrenToComplete:
                        valueString = "Waiting...";
                        break;
                    case TaskStatus.Running:
                        valueString = "Running";
                        break;
                    case TaskStatus.RanToCompletion:
                        valueString = "Finished";
                        break;
                    case TaskStatus.Canceled:
                        valueString = "Cancelled";
                        break;
                    case TaskStatus.Faulted:
                        valueString = "Failed";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), 
                            value, $@"Wrong TaskStatus: {value}");
                }
                stateLabel.Invoke(new UpdateFormElementCallback(() =>
                {
                    stateLabel.Text = valueString;
                }));
            }
        }

        private static void RunTaskInGui(Action action)
        {
            Task.Factory.StartNew(action, CancellationToken.None, 
                TaskCreationOptions.PreferFairness, GuiTaskScheduler);
        }

        private static int GetNumberOfPrimes(int x, Action<int> yieldAction)
        {
            if (x <= 1)
            {
                return 0;
            }

            var isPrime = Enumerable.Repeat(true, x + 1).ToArray();
            isPrime[0] = isPrime[1] = false;

            // starting from zero in order to allow yieldAction 
            // interrupt calculation as soon as possible.
            for (var i = 0; i <= x; ++i)
            {
                if (isPrime[i])
                {
                    for (var j = 2 * i; j <= x; j += i)
                    {
                        isPrime[j] = false;
                    }
                }
                yieldAction(i);
            }
            return isPrime.Count(fl => fl);
        }

        private delegate void UpdateFormElementCallback();
    }
}