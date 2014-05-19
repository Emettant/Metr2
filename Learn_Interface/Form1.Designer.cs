namespace Learn_Interface
{
    partial class Learn_form
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
            this.metricsBox = new System.Windows.Forms.TextBox();
            this.MetricsBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.trainPointsBox = new System.Windows.Forms.TextBox();
            this.browseTrainPoints = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.genTrainPoints = new System.Windows.Forms.Button();
            this.pointsFileStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // metricsBox
            // 
            this.metricsBox.Location = new System.Drawing.Point(18, 51);
            this.metricsBox.Name = "metricsBox";
            this.metricsBox.ReadOnly = true;
            this.metricsBox.Size = new System.Drawing.Size(624, 22);
            this.metricsBox.TabIndex = 4;
            // 
            // MetricsBrowse
            // 
            this.MetricsBrowse.Location = new System.Drawing.Point(648, 23);
            this.MetricsBrowse.Name = "MetricsBrowse";
            this.MetricsBrowse.Size = new System.Drawing.Size(93, 50);
            this.MetricsBrowse.TabIndex = 3;
            this.MetricsBrowse.Text = "Browse";
            this.MetricsBrowse.UseVisualStyleBackColor = true;
            this.MetricsBrowse.Click += new System.EventHandler(this.MetricsBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Metrics file";
            // 
            // trainPointsBox
            // 
            this.trainPointsBox.Location = new System.Drawing.Point(18, 184);
            this.trainPointsBox.Name = "trainPointsBox";
            this.trainPointsBox.ReadOnly = true;
            this.trainPointsBox.Size = new System.Drawing.Size(624, 22);
            this.trainPointsBox.TabIndex = 7;
            this.trainPointsBox.TextChanged += new System.EventHandler(this.trainPointsBox_TextChanged);
            // 
            // browseTrainPoints
            // 
            this.browseTrainPoints.Location = new System.Drawing.Point(648, 156);
            this.browseTrainPoints.Name = "browseTrainPoints";
            this.browseTrainPoints.Size = new System.Drawing.Size(93, 50);
            this.browseTrainPoints.TabIndex = 6;
            this.browseTrainPoints.Text = "Browse";
            this.browseTrainPoints.UseVisualStyleBackColor = true;
            this.browseTrainPoints.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Train points file";
            // 
            // genTrainPoints
            // 
            this.genTrainPoints.Location = new System.Drawing.Point(292, 99);
            this.genTrainPoints.Name = "genTrainPoints";
            this.genTrainPoints.Size = new System.Drawing.Size(162, 51);
            this.genTrainPoints.TabIndex = 9;
            this.genTrainPoints.Text = "Calculate Metrics / Generate points file";
            this.genTrainPoints.UseVisualStyleBackColor = true;
            this.genTrainPoints.Click += new System.EventHandler(this.genTrainPoints_Click);
            // 
            // pointsFileStatus
            // 
            this.pointsFileStatus.AutoSize = true;
            this.pointsFileStatus.Location = new System.Drawing.Point(480, 116);
            this.pointsFileStatus.Name = "pointsFileStatus";
            this.pointsFileStatus.Size = new System.Drawing.Size(0, 17);
            this.pointsFileStatus.TabIndex = 10;
            // 
            // Learn_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 457);
            this.Controls.Add(this.pointsFileStatus);
            this.Controls.Add(this.genTrainPoints);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trainPointsBox);
            this.Controls.Add(this.browseTrainPoints);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.metricsBox);
            this.Controls.Add(this.MetricsBrowse);
            this.Name = "Learn_form";
            this.Text = "Learn";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox metricsBox;
        private System.Windows.Forms.Button MetricsBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox trainPointsBox;
        private System.Windows.Forms.Button browseTrainPoints;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button genTrainPoints;
        private System.Windows.Forms.Label pointsFileStatus;
    }
}

