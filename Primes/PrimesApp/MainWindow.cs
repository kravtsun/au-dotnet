using System;
using System.Windows.Forms;

namespace PrimesApp
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            AcceptButton = submitButton;
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            var decimalValue = numberInput.Value;
            var taskView = new TaskView((int) decimalValue)
            {
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            taskPanel.Controls.Add(taskView);
            taskPanel.ScrollControlIntoView(taskView);
        }
    }
}
