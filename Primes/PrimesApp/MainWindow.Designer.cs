namespace PrimesApp
{
    partial class MainWindow
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
            this.submitButton = new System.Windows.Forms.Button();
            this.numberInput = new System.Windows.Forms.NumericUpDown();
            this.taskPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.allLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.numberInput)).BeginInit();
            this.allLayoutPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // submitButton
            // 
            this.submitButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.submitButton.Location = new System.Drawing.Point(692, 10);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 23);
            this.submitButton.TabIndex = 1;
            this.submitButton.Text = "Submit";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // numberInput
            // 
            this.numberInput.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numberInput.Location = new System.Drawing.Point(183, 12);
            this.numberInput.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numberInput.Name = "numberInput";
            this.numberInput.Size = new System.Drawing.Size(120, 20);
            this.numberInput.TabIndex = 0;
            this.numberInput.ThousandsSeparator = true;
            this.numberInput.Value = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            // 
            // taskPanel
            // 
            this.taskPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.taskPanel.AutoScroll = true;
            this.taskPanel.Location = new System.Drawing.Point(3, 3);
            this.taskPanel.MinimumSize = new System.Drawing.Size(700, 300);
            this.taskPanel.Name = "taskPanel";
            this.taskPanel.Size = new System.Drawing.Size(973, 533);
            this.taskPanel.TabIndex = 2;
            // 
            // allLayoutPanel
            // 
            this.allLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.allLayoutPanel.ColumnCount = 1;
            this.allLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.allLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.allLayoutPanel.Controls.Add(this.taskPanel, 0, 0);
            this.allLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.allLayoutPanel.Name = "allLayoutPanel";
            this.allLayoutPanel.RowCount = 2;
            this.allLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.allLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.allLayoutPanel.Size = new System.Drawing.Size(979, 589);
            this.allLayoutPanel.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.numberInput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.submitButton, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 542);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(973, 44);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 680);
            this.Controls.Add(this.allLayoutPanel);
            this.Name = "MainWindow";
            this.Text = "Простые числа";
            ((System.ComponentModel.ISupportInitialize)(this.numberInput)).EndInit();
            this.allLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.NumericUpDown numberInput;
        private System.Windows.Forms.FlowLayoutPanel taskPanel;
        private System.Windows.Forms.TableLayoutPanel allLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}

