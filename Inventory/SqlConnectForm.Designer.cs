namespace Inventory
{
    partial class SqlConnectForm
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
            this.components = new System.ComponentModel.Container();
            this.connectTB = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.labelConnectionResult = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // connectTB
            // 
            this.connectTB.Location = new System.Drawing.Point(12, 9);
            this.connectTB.Name = "connectTB";
            this.connectTB.Size = new System.Drawing.Size(184, 20);
            this.connectTB.TabIndex = 1;
            this.connectTB.Text = "Kérem adja meg a szerver nevét";
            this.connectTB.MouseClick += new System.Windows.Forms.MouseEventHandler(this.connectTB_MouseClick);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(214, 9);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Csatlakozás";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // labelConnectionResult
            // 
            this.labelConnectionResult.AutoSize = true;
            this.labelConnectionResult.Location = new System.Drawing.Point(12, 48);
            this.labelConnectionResult.Name = "labelConnectionResult";
            this.labelConnectionResult.Size = new System.Drawing.Size(0, 13);
            this.labelConnectionResult.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SqlConnectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 80);
            this.Controls.Add(this.labelConnectionResult);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.connectTB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SqlConnectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MSSQL csatlakozás";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox connectTB;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label labelConnectionResult;
        private System.Windows.Forms.Timer timer1;
    }
}