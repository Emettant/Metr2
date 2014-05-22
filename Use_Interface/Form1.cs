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
        MetrInterface.CompilationViewGroup compilationViewGroup;
        string solutionFile { get { return openFileGroup2.FileName; } }
        string modelFile { get { return openFileGroup.FileName; } }
        public UseForm()
        {
            compilationViewGroup = new MetrInterface.CompilationViewGroup(this, 90, "");
            openFileGroup = new MetrInterface.OpenFileGroup(this, 0, @"C:\temp2\Another-Metric - model.xml", "Browse and Load", "Model file", "xml", 
                new Action<MetrInterface.OpenFileGroup>((MetrInterface.OpenFileGroup op) => {
                compilationViewGroup.InitModel(op.FileName);
            }));
            openFileGroup2 = new MetrInterface.OpenFileGroup(this, 60, @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln", "Browse and Load", "Solution file", "sln",
                new Action<MetrInterface.OpenFileGroup>((MetrInterface.OpenFileGroup op) => {
                    compilationViewGroup.InitSolution(op.FileName);
            }));
            
            InitializeComponent();
            
        }

      
    }
}
