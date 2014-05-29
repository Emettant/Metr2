using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Learn_Interface;
using Use_Interface;

namespace Votes_Interface
{
    public partial class MainMDIForm : Form
    {
        public MainMDIForm()
        {
            InitializeComponent();
        }

        private void votesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var votesForms = new Votes_Form();
            votesForms.Show();
            votesForms.MdiParent = this;
        }

        private void lernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lernForm = new Learnform();
            lernForm.Show();
            lernForm.MdiParent = this;
        }

        private void useToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var useForm = new UseForm();
            useForm.Show();
            useForm.MdiParent = this;
        }
    }
}
