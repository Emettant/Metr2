﻿using System;
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
            clearModelPart();
            InitCombobox();

        }

        static string pointsFileEnding = " - points.xml";
        static string modelFileEnding = " - model.xml";

        static string status = "Status: ";
        static string yesFileStatus = status + "!!! There is a file already with such name !!! ";
        static string noFileStatus = status + "File does not exist.";
        static string inProgressStatus = status + "In progress...";
        static string isDoneStatus = status + "Done :) ";
        static string ToBuildModelAdvise = " You can build a model";

        void InitCombobox()
        {

            //foreach (var el in Enum.GetValues(typeof(MetrLearn.Train.ModelMethod)))
            //{
            //    methodComboBox.Items.Add(el.ToString());
            //}
            foreach (var el in MetrLearn.ModelParent.ApproachList.Select(x => x.ToString()))
                methodComboBox.Items.Add(el);

            methodComboBox.SelectedIndex = 1;
        }

        private void fillMetricsAndPointsBoxTogether(string name) {
            metricsBox.Text = name;
            trainPointsBox.Text = metricsBox.Text.Replace(".xml", pointsFileEnding);
            refreshPointsFileExistence();
        }

        void trainPointsBoxToModelBox() {
            var t = trainPointsBox.Text.LastIndexOf(pointsFileEnding);
            if (trainPointsBox.Text.LastIndexOf(pointsFileEnding) != -1) {
                modelFileBox.Text = trainPointsBox.Text.Replace(pointsFileEnding, modelFileEnding);
            }
            else modelFileBox.Text = trainPointsBox.Text.Replace(".xml",modelFileEnding);

            refreshModelFileExistence();

        }

        void clearModelPart() {
            browseModel.Enabled = buildModel.Enabled = false;
            modelFileBox.Clear();
            modelFileStatus.Text = "";
        }

        void refreshModelFileExistence() {
            modelFileStatus.Text = (File.Exists(modelFileBox.Text)) ? yesFileStatus : noFileStatus;

        }

        void refreshPointsFileExistence() {
            
            pointsFileStatus.Text = (buildModel.Enabled = File.Exists(trainPointsBox.Text)) ?
                yesFileStatus + ToBuildModelAdvise : noFileStatus;
            browseModel.Enabled = buildModel.Enabled;
            if (browseModel.Enabled)
            {
                trainPointsBoxToModelBox();
            }
            else clearModelPart();
        }

        private void MetricsBrowse_Click(object sender, EventArgs e)
        {
            fillMetricsAndPointsBoxTogether(openDialogWork(metricsBox.Text, "xml"));
        }

        private void browseTrainPoints_Click(object sender, EventArgs e)
        {
            trainPointsBox.Text = openDialogWork(trainPointsBox.Text, "xml");
            refreshPointsFileExistence();
        }

        private void genTrainPoints_Click(object sender, EventArgs e)
        {
            //TODO: Move this part to votes_interface
            pointsFileStatus.Text = inProgressStatus;
            clearModelPart();
            MetrLearn.Train.toTrainPoints(metricsBox.Text, trainPointsBox.Text);
            pointsFileStatus.Text = isDoneStatus + ToBuildModelAdvise;
            refreshPointsFileExistence();
        }

        private void buildModel_Click(object sender, EventArgs e)
        {
            modelFileStatus.Text = inProgressStatus;
            MetrLearn.Train.toTrainedModel(trainPointsBox.Text, modelFileBox.Text, MetrLearn.ModelParent.ApproachList[methodComboBox.SelectedIndex]);
            modelFileStatus.Text = isDoneStatus;
        }

        private void browseModel_Click(object sender, EventArgs e)
        {
            modelFileBox.Text = openDialogWork(modelFileBox.Text,"xml");
            refreshModelFileExistence();
        }
    }
}
