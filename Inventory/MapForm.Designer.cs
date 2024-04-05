namespace Inventory
{
    partial class MapForm
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
            this.MapUploadB = new System.Windows.Forms.Button();
            this.MapUploadTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MapUploadB
            // 
            this.MapUploadB.Location = new System.Drawing.Point(189, 31);
            this.MapUploadB.Name = "MapUploadB";
            this.MapUploadB.Size = new System.Drawing.Size(75, 23);
            this.MapUploadB.TabIndex = 0;
            this.MapUploadB.Text = "Felvétel";
            this.MapUploadB.UseVisualStyleBackColor = true;
            this.MapUploadB.Click += new System.EventHandler(this.MapUpload_Click);
            // 
            // MapUploadTB
            // 
            this.MapUploadTB.Location = new System.Drawing.Point(15, 34);
            this.MapUploadTB.Name = "MapUploadTB";
            this.MapUploadTB.Size = new System.Drawing.Size(139, 20);
            this.MapUploadTB.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Adja meg az új térkép nevét:";
            // 
            // MapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 73);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MapUploadTB);
            this.Controls.Add(this.MapUploadB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MapForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Új térkép felvétele";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button MapUploadB;
        private System.Windows.Forms.TextBox MapUploadTB;
        private System.Windows.Forms.Label label1;
    }
}