using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MetrInterface.Tools;

namespace Learn_Interface
{
    public partial class Learn_form : Form
    {
        public Learn_form()
        {
            InitializeComponent();
            fillMetricsAndPointsBoxTogether(@"C:\temp2\Metr2.xml");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            trainPointsBox.Text = openDialogWork(trainPointsBox.Text, "xml");
        }
        private void fillMetricsAndPointsBoxTogether(string name) {
            metricsBox.Text = name;
            trainPointsBox.Text = metricsBox.Text.Replace(".xml", " - points.xml");
        }

        private void MetricsBrowse_Click(object sender, EventArgs e)
        {
            fillMetricsAndPointsBoxTogether(openDialogWork(metricsBox.Text, "xml"));
        }

        private void genTrainPoints_Click(object sender, EventArgs e)
        {
            pointsFileStatus.Text = "In progress...";
            MetrLearn.Train.toTrainPoints(metricsBox.Text, trainPointsBox.Text);
            pointsFileStatus.Text = "Done!";
        }

        private void trainPointsBox_TextChanged(object sender, EventArgs e)
        {
            pointsFileStatus.Text = (!File.Exists(trainPointsBox.Text)) ? "Points file not exist." : "!!! There is a file already with such name !!!";
        }
    }
}
