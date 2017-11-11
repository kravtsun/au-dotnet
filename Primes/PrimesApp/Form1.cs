using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrimesApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //numberInput.Enter += (sender, args) =>
            //{
            //    submitButton.PerformClick();
            //};
            //taskPanel.Controls.Clear();
            AcceptButton = submitButton;
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            var decimalValue = this.numberInput.Value;
            var taskView = new TaskView((int) decimalValue);
            taskPanel.Controls.Add(taskView);
            taskPanel.ScrollControlIntoView(taskView);
        }
    }
}
