namespace Inventory
{
    partial class NewDataForm
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
            this.addButton = new System.Windows.Forms.Button();
            this.addL0 = new System.Windows.Forms.Label();
            this.firstTB = new System.Windows.Forms.TextBox();
            this.addL1 = new System.Windows.Forms.Label();
            this.addL2 = new System.Windows.Forms.Label();
            this.secondTB = new System.Windows.Forms.TextBox();
            this.addL4 = new System.Windows.Forms.Label();
            this.addL3 = new System.Windows.Forms.Label();
            this.thirdTB = new System.Windows.Forms.TextBox();
            this.fourthTB = new System.Windows.Forms.TextBox();
            this.saveModificationButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(135, 115);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "Felvétel";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // addL0
            // 
            this.addL0.AutoSize = true;
            this.addL0.Location = new System.Drawing.Point(10, 10);
            this.addL0.Name = "addL0";
            this.addL0.Size = new System.Drawing.Size(92, 13);
            this.addL0.TabIndex = 1;
            this.addL0.Text = "Személy felvétele:";
            // 
            // firstTB
            // 
            this.firstTB.Location = new System.Drawing.Point(110, 40);
            this.firstTB.Name = "firstTB";
            this.firstTB.Size = new System.Drawing.Size(100, 20);
            this.firstTB.TabIndex = 2;
            this.firstTB.TextChanged += new System.EventHandler(this.personNameTB_TextChanged);
            // 
            // addL1
            // 
            this.addL1.AutoSize = true;
            this.addL1.Location = new System.Drawing.Point(12, 43);
            this.addL1.Name = "addL1";
            this.addL1.Size = new System.Drawing.Size(30, 13);
            this.addL1.TabIndex = 3;
            this.addL1.Text = "Név:";
            // 
            // addL2
            // 
            this.addL2.AutoSize = true;
            this.addL2.Location = new System.Drawing.Point(9, 83);
            this.addL2.Name = "addL2";
            this.addL2.Size = new System.Drawing.Size(46, 13);
            this.addL2.TabIndex = 4;
            this.addL2.Text = "Pozíció:";
            // 
            // secondTB
            // 
            this.secondTB.Location = new System.Drawing.Point(110, 80);
            this.secondTB.Name = "secondTB";
            this.secondTB.Size = new System.Drawing.Size(100, 20);
            this.secondTB.TabIndex = 5;
            this.secondTB.TextChanged += new System.EventHandler(this.personPosiTB_TextChanged);
            // 
            // addL4
            // 
            this.addL4.AutoSize = true;
            this.addL4.Location = new System.Drawing.Point(10, 163);
            this.addL4.Name = "addL4";
            this.addL4.Size = new System.Drawing.Size(40, 13);
            this.addL4.TabIndex = 8;
            this.addL4.Text = "Leírás:";
            this.addL4.Visible = false;
            // 
            // addL3
            // 
            this.addL3.AutoSize = true;
            this.addL3.Location = new System.Drawing.Point(10, 120);
            this.addL3.Name = "addL3";
            this.addL3.Size = new System.Drawing.Size(55, 13);
            this.addL3.TabIndex = 7;
            this.addL3.Text = "Ketegória:";
            this.addL3.Visible = false;
            // 
            // thirdTB
            // 
            this.thirdTB.Location = new System.Drawing.Point(110, 120);
            this.thirdTB.Name = "thirdTB";
            this.thirdTB.Size = new System.Drawing.Size(100, 20);
            this.thirdTB.TabIndex = 6;
            this.thirdTB.Visible = false;
            this.thirdTB.TextChanged += new System.EventHandler(this.thirdTB_TextChanged);
            // 
            // fourthTB
            // 
            this.fourthTB.Location = new System.Drawing.Point(110, 160);
            this.fourthTB.Name = "fourthTB";
            this.fourthTB.Size = new System.Drawing.Size(100, 20);
            this.fourthTB.TabIndex = 9;
            this.fourthTB.Visible = false;
            this.fourthTB.TextChanged += new System.EventHandler(this.fourthTB_TextChanged);
            // 
            // saveModificationButton
            // 
            this.saveModificationButton.Location = new System.Drawing.Point(15, 115);
            this.saveModificationButton.Name = "saveModificationButton";
            this.saveModificationButton.Size = new System.Drawing.Size(75, 23);
            this.saveModificationButton.TabIndex = 10;
            this.saveModificationButton.Text = "Módosítás";
            this.saveModificationButton.UseVisualStyleBackColor = true;
            this.saveModificationButton.Visible = false;
            this.saveModificationButton.Click += new System.EventHandler(this.saveModificationButton_Click);
            // 
            // newData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 153);
            this.Controls.Add(this.saveModificationButton);
            this.Controls.Add(this.fourthTB);
            this.Controls.Add(this.addL4);
            this.Controls.Add(this.addL3);
            this.Controls.Add(this.thirdTB);
            this.Controls.Add(this.secondTB);
            this.Controls.Add(this.addL2);
            this.Controls.Add(this.addL1);
            this.Controls.Add(this.firstTB);
            this.Controls.Add(this.addL0);
            this.Controls.Add(this.addButton);
            this.Name = "newData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adatok felvétele";
            this.Load += new System.EventHandler(this.newData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Label addL0;
        private System.Windows.Forms.TextBox firstTB;
        private System.Windows.Forms.Label addL1;
        private System.Windows.Forms.Label addL2;
        private System.Windows.Forms.TextBox secondTB;
        private System.Windows.Forms.Label addL4;
        private System.Windows.Forms.Label addL3;
        private System.Windows.Forms.TextBox thirdTB;
        private System.Windows.Forms.TextBox fourthTB;
        private System.Windows.Forms.Button saveModificationButton;
    }
}