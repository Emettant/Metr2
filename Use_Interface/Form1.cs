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
        public UseForm()
        {
            openFileGroup = new MetrInterface.OpenFileGroup(this, 20, @"C:\", "Browse");
            openFileGroup2 = new MetrInterface.OpenFileGroup(this, 70, @"C:\", "Browse");
            InitializeComponent();
           
        }

      
    }
}
