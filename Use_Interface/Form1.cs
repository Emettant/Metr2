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
        public UseForm()
        {
            InitializeComponent();
            modelFileBox.Text = @"C:\temp2\Metr2 - .xml";
        }

        private void browseModel_Click(object sender, EventArgs e)
        {
            modelFileBox.Text = MetrInterface.Tools.openDialogWork(modelFileBox.Text,"xml");

        }
    }
}
