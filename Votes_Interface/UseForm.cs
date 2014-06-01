using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Use_Interface
{

    public partial class UseForm : Form
    {

        MetrInterface.OpenFileGroup openFileGroup, openFileGroup2;
        MetrInterface.OpenSaveFileGroup openSaveFileGroup0, openSaveFileGroup1, openSaveFileGroup2;
        MetrInterface.CompilationViewGroup compilationViewGroup;

        string solutionFile { get { return openFileGroup2.FileName; } }
        string modelFile { get { return openFileGroup.FileName; } }
        public UseForm()
        {
            compilationViewGroup = new MetrInterface.CompilationViewGroup(this, 950, 250, 90, "");

            openFileGroup = new MetrInterface.OpenFileGroup(this, 0, @"C:\temp2\Real_Metr2\Hierarchical - model - LS.xml",
                "Load", "Model file", "xml",
                new Action<MetrInterface.OpenFileGroup>((MetrInterface.OpenFileGroup op) =>
            {
                compilationViewGroup.InitModel(op.FileName);
            }));
            openFileGroup2 = new MetrInterface.OpenFileGroup(this, 60, @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln", "Load", "Solution file", "sln",
                new Action<MetrInterface.OpenFileGroup>((MetrInterface.OpenFileGroup op) =>
            {
                compilationViewGroup.InitSolution(op.FileName);
            }));



            //saveOpenFileGroup...
            openSaveFileGroup0 = 
            new MetrInterface.OpenSaveFileGroup(this, 480, @"C:\temp2\ForSavingMetr.xml", "Load", "Saved1 file", "xml",
                new Action<MetrInterface.OpenFileGroup>((MetrInterface.OpenFileGroup op) =>
            {
                compilationViewGroup.InitSavedModel(op.FileName, 0);
            }),
            "Save",
            new Action<MetrInterface.OpenSaveFileGroup>((MetrInterface.OpenSaveFileGroup op) =>
            {
                compilationViewGroup.saveGeneratedMetricToFile(op.FileName);
            })
            );


            openSaveFileGroup1 =
            new MetrInterface.OpenSaveFileGroup(this, 550, @"C:\temp2\ForSavingMetr.xml", "Load", "Saved2 file", "xml",
                new Action<MetrInterface.OpenFileGroup>((MetrInterface.OpenFileGroup op) =>
            {
                compilationViewGroup.InitSavedModel(op.FileName, 1);
            }),
            "Save",
            new Action<MetrInterface.OpenSaveFileGroup>((MetrInterface.OpenSaveFileGroup op) =>
            {
                compilationViewGroup.saveGeneratedMetricToFile(op.FileName);
            })
            );



            openSaveFileGroup2 =
            new MetrInterface.OpenSaveFileGroup(this, 620, @"C:\temp2\ForSavingMetr.xml", "Load", "Saved3 file", "xml",
              new Action<MetrInterface.OpenFileGroup>((MetrInterface.OpenFileGroup op) =>
            {
                compilationViewGroup.InitSavedModel(op.FileName, 2);
            }),
          "Save",
          new Action<MetrInterface.OpenSaveFileGroup>((MetrInterface.OpenSaveFileGroup op) =>
            {
                compilationViewGroup.saveGeneratedMetricToFile(op.FileName);
            })
          );
            InitializeComponent();

        }


    }
}
