namespace Use_Interface
{
    partial class UseForm
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
            this.toLearnButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // toLearnButton
            // 
            this.toLearnButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.toLearnButton.Location = new System.Drawing.Point(12, 166);
            this.toLearnButton.Name = "toLearnButton";
            this.toLearnButton.Size = new System.Drawing.Size(99, 41);
            this.toLearnButton.TabIndex = 14;
            this.toLearnButton.Text = "<== go Learn";
            this.toLearnButton.UseVisualStyleBackColor = true;
            // 
            // UseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 712);
            this.Controls.Add(this.toLearnButton);
            this.Name = "UseForm";
            this.Text = "Use";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button toLearnButton;
    }
}

