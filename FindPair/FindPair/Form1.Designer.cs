namespace FindPair
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._baseFormLayout = new System.Windows.Forms.TableLayoutPanel();
            this._textBoxAndSubmitLayout = new System.Windows.Forms.TableLayoutPanel();
            this._nParameterTextBox = new System.Windows.Forms.TextBox();
            this._startButton = new System.Windows.Forms.Button();
            this._baseFormLayout.SuspendLayout();
            this._textBoxAndSubmitLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // _baseFormLayout
            // 
            this._baseFormLayout.ColumnCount = 1;
            this._baseFormLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._baseFormLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._baseFormLayout.Controls.Add(this._textBoxAndSubmitLayout, 0, 1);
            this._baseFormLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._baseFormLayout.Location = new System.Drawing.Point(0, 0);
            this._baseFormLayout.Name = "_baseFormLayout";
            this._baseFormLayout.RowCount = 2;
            this._baseFormLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.96552F));
            this._baseFormLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.03448F));
            this._baseFormLayout.Size = new System.Drawing.Size(816, 660);
            this._baseFormLayout.TabIndex = 0;
            this._baseFormLayout.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // _textBoxAndSubmitLayout
            // 
            this._textBoxAndSubmitLayout.ColumnCount = 2;
            this._textBoxAndSubmitLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._textBoxAndSubmitLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._textBoxAndSubmitLayout.Controls.Add(this._nParameterTextBox, 0, 0);
            this._textBoxAndSubmitLayout.Controls.Add(this._startButton, 1, 0);
            this._textBoxAndSubmitLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._textBoxAndSubmitLayout.Location = new System.Drawing.Point(3, 458);
            this._textBoxAndSubmitLayout.Name = "_textBoxAndSubmitLayout";
            this._textBoxAndSubmitLayout.RowCount = 1;
            this._textBoxAndSubmitLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._textBoxAndSubmitLayout.Size = new System.Drawing.Size(810, 199);
            this._textBoxAndSubmitLayout.TabIndex = 1;
            // 
            // _nParameterTextBox
            // 
            this._nParameterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._nParameterTextBox.Location = new System.Drawing.Point(3, 3);
            this._nParameterTextBox.Name = "_nParameterTextBox";
            this._nParameterTextBox.Size = new System.Drawing.Size(399, 20);
            this._nParameterTextBox.TabIndex = 0;
            this._nParameterTextBox.Text = "10";
            this._nParameterTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._nParameterTextBox.Validating += new System.ComponentModel.CancelEventHandler(this._nParameterTextBox_Validating);
            // 
            // _startButton
            // 
            this._startButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._startButton.Location = new System.Drawing.Point(408, 3);
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(75, 23);
            this._startButton.TabIndex = 0;
            this._startButton.Text = "Start";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this._startButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 660);
            this.Controls.Add(this._baseFormLayout);
            this.Name = "Form1";
            this.Text = "Form1";
            this._baseFormLayout.ResumeLayout(false);
            this._textBoxAndSubmitLayout.ResumeLayout(false);
            this._textBoxAndSubmitLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _baseFormLayout;
        private System.Windows.Forms.TextBox _nParameterTextBox;
        private System.Windows.Forms.TableLayoutPanel _textBoxAndSubmitLayout;
        private System.Windows.Forms.Button _startButton;
    }
}

