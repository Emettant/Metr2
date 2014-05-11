﻿namespace Votes_Interface
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
            this.symbolView = new System.Windows.Forms.TreeView();
            this.VotesBrowse = new System.Windows.Forms.Button();
            this.votesFileBox = new System.Windows.Forms.TextBox();
            this.SaveVotes = new System.Windows.Forms.Button();
            this.LoadVotes = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // symbolView
            // 
            this.symbolView.Location = new System.Drawing.Point(33, 27);
            this.symbolView.Name = "symbolView";
            this.symbolView.Size = new System.Drawing.Size(206, 324);
            this.symbolView.TabIndex = 0;
            this.symbolView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.symbolView_AfterSelect);
            // 
            // VotesBrowse
            // 
            this.VotesBrowse.Location = new System.Drawing.Point(675, 69);
            this.VotesBrowse.Name = "VotesBrowse";
            this.VotesBrowse.Size = new System.Drawing.Size(75, 23);
            this.VotesBrowse.TabIndex = 1;
            this.VotesBrowse.Text = "Browse";
            this.VotesBrowse.UseVisualStyleBackColor = true;
            this.VotesBrowse.Click += new System.EventHandler(this.VotesBrowse_Click);
            // 
            // votesFileBox
            // 
            this.votesFileBox.Location = new System.Drawing.Point(245, 27);
            this.votesFileBox.Name = "votesFileBox";
            this.votesFileBox.ReadOnly = true;
            this.votesFileBox.Size = new System.Drawing.Size(505, 22);
            this.votesFileBox.TabIndex = 2;
            // 
            // SaveVotes
            // 
            this.SaveVotes.Location = new System.Drawing.Point(453, 69);
            this.SaveVotes.Name = "SaveVotes";
            this.SaveVotes.Size = new System.Drawing.Size(75, 50);
            this.SaveVotes.TabIndex = 3;
            this.SaveVotes.Text = "Save";
            this.SaveVotes.UseVisualStyleBackColor = true;
            this.SaveVotes.Click += new System.EventHandler(this.SaveVotes_Click);
            // 
            // LoadVotes
            // 
            this.LoadVotes.Location = new System.Drawing.Point(564, 69);
            this.LoadVotes.Name = "LoadVotes";
            this.LoadVotes.Size = new System.Drawing.Size(75, 50);
            this.LoadVotes.TabIndex = 4;
            this.LoadVotes.Text = "Load";
            this.LoadVotes.UseVisualStyleBackColor = true;
            this.LoadVotes.Click += new System.EventHandler(this.LoadVotes_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 450);
            this.Controls.Add(this.LoadVotes);
            this.Controls.Add(this.SaveVotes);
            this.Controls.Add(this.votesFileBox);
            this.Controls.Add(this.VotesBrowse);
            this.Controls.Add(this.symbolView);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView symbolView;
        private System.Windows.Forms.Button VotesBrowse;
        private System.Windows.Forms.TextBox votesFileBox;
        private System.Windows.Forms.Button SaveVotes;
        private System.Windows.Forms.Button LoadVotes;
    }
}

