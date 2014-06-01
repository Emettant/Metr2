namespace Votes_Interface
{
    partial class MainMDIForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.votesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lernToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.votesToolStripMenuItem,
            this.lernToolStripMenuItem,
            this.useToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1184, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // votesToolStripMenuItem
            // 
            this.votesToolStripMenuItem.Name = "votesToolStripMenuItem";
            this.votesToolStripMenuItem.Size = new System.Drawing.Size(58, 24);
            this.votesToolStripMenuItem.Text = "Votes";
            this.votesToolStripMenuItem.Click += new System.EventHandler(this.votesToolStripMenuItem_Click);
            // 
            // lernToolStripMenuItem
            // 
            this.lernToolStripMenuItem.Name = "lernToolStripMenuItem";
            this.lernToolStripMenuItem.Size = new System.Drawing.Size(49, 24);
            this.lernToolStripMenuItem.Text = "Lern";
            this.lernToolStripMenuItem.Click += new System.EventHandler(this.lernToolStripMenuItem_Click);
            // 
            // useToolStripMenuItem
            // 
            this.useToolStripMenuItem.Name = "useToolStripMenuItem";
            this.useToolStripMenuItem.Size = new System.Drawing.Size(45, 24);
            this.useToolStripMenuItem.Text = "Use";
            this.useToolStripMenuItem.Click += new System.EventHandler(this.useToolStripMenuItem_Click);
            // 
            // MainMDIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 765);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainMDIForm";
            this.Text = "MainMDIForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem votesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lernToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useToolStripMenuItem;
    }
}