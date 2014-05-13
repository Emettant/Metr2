namespace Votes_Interface
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
            this.voteBox = new System.Windows.Forms.NumericUpDown();
            this.is_voted = new System.Windows.Forms.CheckBox();
            this.applyBut = new System.Windows.Forms.Button();
            this.clearBut = new System.Windows.Forms.Button();
            this.solutionBox = new System.Windows.Forms.TextBox();
            this.browseSolutionBut = new System.Windows.Forms.Button();
            this.comboProjects = new System.Windows.Forms.ComboBox();
            this.loadProjectBut = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.voteBox)).BeginInit();
            this.SuspendLayout();
            // 
            // symbolView
            // 
            this.symbolView.Location = new System.Drawing.Point(33, 55);
            this.symbolView.Name = "symbolView";
            this.symbolView.Size = new System.Drawing.Size(364, 383);
            this.symbolView.TabIndex = 0;
            this.symbolView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.symbolView_AfterSelect);
            // 
            // VotesBrowse
            // 
            this.VotesBrowse.Location = new System.Drawing.Point(663, 27);
            this.VotesBrowse.Name = "VotesBrowse";
            this.VotesBrowse.Size = new System.Drawing.Size(93, 50);
            this.VotesBrowse.TabIndex = 1;
            this.VotesBrowse.Text = "Browse and Load";
            this.VotesBrowse.UseVisualStyleBackColor = true;
            this.VotesBrowse.Click += new System.EventHandler(this.VotesBrowse_Click);
            // 
            // votesFileBox
            // 
            this.votesFileBox.Location = new System.Drawing.Point(33, 27);
            this.votesFileBox.Name = "votesFileBox";
            this.votesFileBox.ReadOnly = true;
            this.votesFileBox.Size = new System.Drawing.Size(624, 22);
            this.votesFileBox.TabIndex = 2;
            this.votesFileBox.TextChanged += new System.EventHandler(this.votesFileBox_TextChanged);
            // 
            // SaveVotes
            // 
            this.SaveVotes.Location = new System.Drawing.Point(663, 83);
            this.SaveVotes.Name = "SaveVotes";
            this.SaveVotes.Size = new System.Drawing.Size(93, 50);
            this.SaveVotes.TabIndex = 3;
            this.SaveVotes.Text = "Save";
            this.SaveVotes.UseVisualStyleBackColor = true;
            this.SaveVotes.Click += new System.EventHandler(this.SaveVotes_Click);
            // 
            // voteBox
            // 
            this.voteBox.Location = new System.Drawing.Point(532, 135);
            this.voteBox.Name = "voteBox";
            this.voteBox.Size = new System.Drawing.Size(87, 22);
            this.voteBox.TabIndex = 5;
            this.voteBox.ValueChanged += new System.EventHandler(this.voteBox_ValueChanged);
            this.voteBox.Enter += new System.EventHandler(this.voteBox_Enter);
            // 
            // is_voted
            // 
            this.is_voted.AutoSize = true;
            this.is_voted.Enabled = false;
            this.is_voted.Location = new System.Drawing.Point(429, 136);
            this.is_voted.Name = "is_voted";
            this.is_voted.Size = new System.Drawing.Size(79, 21);
            this.is_voted.TabIndex = 6;
            this.is_voted.Text = "is voted";
            this.is_voted.UseVisualStyleBackColor = true;
            this.is_voted.CheckedChanged += new System.EventHandler(this.is_voted_CheckedChanged);
            // 
            // applyBut
            // 
            this.applyBut.Location = new System.Drawing.Point(532, 175);
            this.applyBut.Name = "applyBut";
            this.applyBut.Size = new System.Drawing.Size(87, 30);
            this.applyBut.TabIndex = 7;
            this.applyBut.Text = "Apply";
            this.applyBut.UseVisualStyleBackColor = true;
            this.applyBut.Click += new System.EventHandler(this.applyBut_Click);
            // 
            // clearBut
            // 
            this.clearBut.Location = new System.Drawing.Point(429, 175);
            this.clearBut.Name = "clearBut";
            this.clearBut.Size = new System.Drawing.Size(87, 30);
            this.clearBut.TabIndex = 8;
            this.clearBut.Text = "Clear";
            this.clearBut.UseVisualStyleBackColor = true;
            this.clearBut.Click += new System.EventHandler(this.clearBut_Click);
            // 
            // solutionBox
            // 
            this.solutionBox.Location = new System.Drawing.Point(33, 456);
            this.solutionBox.Name = "solutionBox";
            this.solutionBox.ReadOnly = true;
            this.solutionBox.Size = new System.Drawing.Size(609, 22);
            this.solutionBox.TabIndex = 9;
            this.solutionBox.TextChanged += new System.EventHandler(this.solutionBox_TextChanged);
            // 
            // browseSolutionBut
            // 
            this.browseSolutionBut.Location = new System.Drawing.Point(657, 432);
            this.browseSolutionBut.Name = "browseSolutionBut";
            this.browseSolutionBut.Size = new System.Drawing.Size(93, 46);
            this.browseSolutionBut.TabIndex = 10;
            this.browseSolutionBut.Text = "Browse and Load";
            this.browseSolutionBut.UseVisualStyleBackColor = true;
            this.browseSolutionBut.Click += new System.EventHandler(this.browseSolutionBut_Click);
            // 
            // comboProjects
            // 
            this.comboProjects.ForeColor = System.Drawing.Color.Black;
            this.comboProjects.FormattingEnabled = true;
            this.comboProjects.Location = new System.Drawing.Point(413, 337);
            this.comboProjects.Name = "comboProjects";
            this.comboProjects.Size = new System.Drawing.Size(337, 24);
            this.comboProjects.TabIndex = 11;
            // 
            // loadProjectBut
            // 
            this.loadProjectBut.Location = new System.Drawing.Point(625, 367);
            this.loadProjectBut.Name = "loadProjectBut";
            this.loadProjectBut.Size = new System.Drawing.Size(125, 23);
            this.loadProjectBut.TabIndex = 12;
            this.loadProjectBut.Text = "Load Project";
            this.loadProjectBut.UseVisualStyleBackColor = true;
            this.loadProjectBut.Click += new System.EventHandler(this.loadProjectBut_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 498);
            this.Controls.Add(this.loadProjectBut);
            this.Controls.Add(this.comboProjects);
            this.Controls.Add(this.browseSolutionBut);
            this.Controls.Add(this.solutionBox);
            this.Controls.Add(this.clearBut);
            this.Controls.Add(this.applyBut);
            this.Controls.Add(this.is_voted);
            this.Controls.Add(this.voteBox);
            this.Controls.Add(this.SaveVotes);
            this.Controls.Add(this.votesFileBox);
            this.Controls.Add(this.VotesBrowse);
            this.Controls.Add(this.symbolView);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.voteBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView symbolView;
        private System.Windows.Forms.Button VotesBrowse;
        private System.Windows.Forms.TextBox votesFileBox;
        private System.Windows.Forms.Button SaveVotes;
        private System.Windows.Forms.NumericUpDown voteBox;
        private System.Windows.Forms.CheckBox is_voted;
        private System.Windows.Forms.Button applyBut;
        private System.Windows.Forms.Button clearBut;
        private System.Windows.Forms.TextBox solutionBox;
        private System.Windows.Forms.Button browseSolutionBut;
        private System.Windows.Forms.ComboBox comboProjects;
        private System.Windows.Forms.Button loadProjectBut;
    }
}

