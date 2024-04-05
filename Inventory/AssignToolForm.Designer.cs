namespace Inventory
{
    partial class AssignToolForm
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
            this.addButton = new System.Windows.Forms.Button();
            this.comboBoxPeople = new System.Windows.Forms.ComboBox();
            this.dataGridViewTools = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.kilépésToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fájlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kilépésToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.visszavételToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTools)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(532, 40);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "Kiadás";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_click);
            // 
            // comboBoxPeople
            // 
            this.comboBoxPeople.FormattingEnabled = true;
            this.comboBoxPeople.Location = new System.Drawing.Point(12, 42);
            this.comboBoxPeople.Name = "comboBoxPeople";
            this.comboBoxPeople.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPeople.TabIndex = 1;
            // 
            // dataGridViewTools
            // 
            this.dataGridViewTools.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTools.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTools.Location = new System.Drawing.Point(12, 69);
            this.dataGridViewTools.Name = "dataGridViewTools";
            this.dataGridViewTools.ReadOnly = true;
            this.dataGridViewTools.Size = new System.Drawing.Size(595, 271);
            this.dataGridViewTools.TabIndex = 2;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kilépésToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(112, 26);
            // 
            // kilépésToolStripMenuItem
            // 
            this.kilépésToolStripMenuItem.Name = "kilépésToolStripMenuItem";
            this.kilépésToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.kilépésToolStripMenuItem.Text = "Kilépés";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fájlToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(619, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fájlToolStripMenuItem
            // 
            this.fájlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.visszavételToolStripMenuItem,
            this.kilépésToolStripMenuItem1});
            this.fájlToolStripMenuItem.Name = "fájlToolStripMenuItem";
            this.fájlToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fájlToolStripMenuItem.Text = "Fájl";
            // 
            // kilépésToolStripMenuItem1
            // 
            this.kilépésToolStripMenuItem1.Name = "kilépésToolStripMenuItem1";
            this.kilépésToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.kilépésToolStripMenuItem1.Text = "Kilépés";
            this.kilépésToolStripMenuItem1.Click += new System.EventHandler(this.kilépésToolStripMenuItem1_Click);
            // 
            // visszavételToolStripMenuItem
            // 
            this.visszavételToolStripMenuItem.Name = "visszavételToolStripMenuItem";
            this.visszavételToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.visszavételToolStripMenuItem.Text = "Visszavétel";
            this.visszavételToolStripMenuItem.Click += new System.EventHandler(this.visszavételToolStripMenuItem_Click);
            // 
            // AssignToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 352);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dataGridViewTools);
            this.Controls.Add(this.comboBoxPeople);
            this.Controls.Add(this.addButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "AssignToolForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Eszköz kiadás";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTools)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ComboBox comboBoxPeople;
        private System.Windows.Forms.DataGridView dataGridViewTools;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem kilépésToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fájlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kilépésToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem visszavételToolStripMenuItem;
    }
}