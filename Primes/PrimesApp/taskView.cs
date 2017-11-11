using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrimesApp
{
    public sealed partial class TaskView : UserControl
    {
        private readonly TaskScheduler _guiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private enum TaskViewState
        {
            None,
            Waiting,
            Running,
            Finished,
            Cancelled
        }

        private TaskViewState _state;
        private TaskViewState State
        {
            get { return _state; }
            set
            {
                taskInGUI(() => stateLabel.Text = $"{value}");
                _state = value;
            }
        }

        private void taskInGUI(Action action)
        {
            //action();
            new Task(action, TaskCreationOptions.PreferFairness).Start(_guiTaskScheduler);
        }

        public TaskView(int x)
        {
            InitializeComponent();
            nameLabel.Text = x.ToString();
            State = TaskViewState.None;

            var cancelSource = new CancellationTokenSource();
            cancelButton.Click += (sender, args) =>
            {
                State = TaskViewState.Cancelled;
                cancelSource.Cancel();
            };

            Action<int> iterAction = i =>
            {
                cancelSource.Token.ThrowIfCancellationRequested();
                taskInGUI(() => progressBar.Value = i);
            };

            var preCalculationTask = new Task(() =>
            {
                progressBar.Maximum = x;
                progressBar.Visible = true;
                progressBar.Value = 0;
                State = TaskViewState.Waiting;
            });

            var calculationTask = preCalculationTask.ContinueWith(t =>
            {
                State = TaskViewState.Running;
                var result = GetNumberOfPrimes(x, iterAction);
                State = TaskViewState.Finished;
                return result;
            }, cancelSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

            calculationTask.ContinueWith(t =>
            {
                progressBar.Visible = false;
            }, _guiTaskScheduler);

            calculationTask.ContinueWith(t =>
            {
                progressBar.Visible = false;
                cancelButton.Visible = false;
                layoutPanel.Controls.Remove(progressBar);
                layoutPanel.Controls.Remove(cancelButton);

                var resultLabel = new Label {
                    Text = $"Result: {t.Result}",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                             | AnchorStyles.Left
                             | AnchorStyles.Right,
                    AutoSize = true};
                layoutPanel.Controls.Add(resultLabel);
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, _guiTaskScheduler);

            preCalculationTask.Start(_guiTaskScheduler);
        }

        private static int GetNumberOfPrimes(int x, Action<int> postIterAction)
        {
            if (x <= 1)
            {
                return 0;
            }

            var isPrime = Enumerable.Repeat(true, x + 1).ToArray();
            isPrime[0] = isPrime[1] = false;

            // TODO: check than forms accepts only int numbers.
            for (int i = 2; i <= x; ++i)
            {
                if (isPrime[i])
                {
                    for (int j = 2 * i; j <= x; j += i)
                    {
                        isPrime[j] = false;
                    }
                }
                postIterAction(i);
            }
            return isPrime.Count(fl => fl);
        }
    }
}
