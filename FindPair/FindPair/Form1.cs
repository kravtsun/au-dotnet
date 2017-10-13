using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindPair
{
    public partial class Form1 : Form
    {
        private readonly Random _randomNumberGenerator = new Random();
        private List<GameButton> activeButtons = new List<GameButton>();

        public Form1()
        {
            InitializeComponent();
            this.ActiveControl = this._startButton;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            _baseFormLayout.GetControlFromPosition(0, 0);
            Debug.Assert(sender == _startButton);

            int n = int.Parse(_nParameterTextBox.Text);
            var currentGameLayout = _baseFormLayout.GetControlFromPosition(0, 0);
            if (currentGameLayout != null)
            {
                _baseFormLayout.Controls.Remove(currentGameLayout);
            }
            _baseFormLayout.Controls.Add(GetGameTableLayout(n), 0, 0);
        }

        private TableLayoutPanel GetGameTableLayout(int n)
        {
            Debug.Assert(n > 0, $"N={n} must be positive");
            var gameLayout = new TableLayoutPanel
            {
                RowCount = n,
                ColumnCount = n,
                Dock = DockStyle.Fill
            };

            int maxValue = n * n / 2;
            //int maxValue = 10;
            for (int i = 0; i < n; ++i)
            {
                gameLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
                gameLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 1));
                for (int j = 0; j < n; ++j)
                {
                    int value = _randomNumberGenerator.Next(maxValue);
                    var gameButton = GetGameButton(value);
                    gameLayout.Controls.Add(gameButton, i, j);
                }
            }
            return gameLayout;
        }

        private Button GetGameButton(int num)
        {
            var button = new GameButton
            {
                Text = num.ToString(),
                Dock = DockStyle.Fill,
                Num = num,
            };
            button.SetVisible(false);
            button.Click += (sender, args) =>
            {
                button.Visible = true;
                newButtonClicked(button);
            };
            button.Font = this._startButton.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            return button;
        }

        private void _nParameterTextBox_Validating(object sender, CancelEventArgs e)
        {
            Debug.Assert(sender == this._nParameterTextBox);
            var nParameterString = this._nParameterTextBox.Text;
            int nParameter;
            if (!int.TryParse(nParameterString, out nParameter) || nParameter <= 1)
            {
                e.Cancel = true;
                return;
            }
            if (nParameter % 2 != 0)
            {
                this._nParameterTextBox.Text = (nParameter - 1).ToString();
            }
            e.Cancel = false;
        }

        private void newButtonClicked(GameButton button)
        {
            Debug.Assert(activeButtons.Count <= 1);
            button.SetVisible(true);

            if (activeButtons.Count == 0)
            {
                activeButtons.Add(button);
            }
            else if (activeButtons.Count == 1)
            {
                if (activeButtons[0] == button)
                {
                    return;
                }
                if (activeButtons[0].Num != button.Num)
                {
                    button.Update();
                    System.Threading.Thread.Sleep(500);
                    activeButtons[0].SetVisible(false);
                    button.SetVisible(false);
                }
                else
                {
                    activeButtons[0].Enabled = false;
                    button.Enabled = false;
                }
                activeButtons.Clear();
            }
        }

        private class GameButton : Button
        {
            public int Num { get; set; }

            public void SetVisible(bool fl)
            {
                this.Text = fl ? Num.ToString() : "?";
            }
        }
    }
}
