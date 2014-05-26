using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using MetrXML;
using System.Xml.Serialization;
using MetrInterface.Tools;

namespace Votes_Interface
{
    public partial class Votes_Form : Form
    {
        public Votes_Form()
        {
            
            InitializeComponent();
            
            solutionBox.Text = "";
            if ((solutionBox.Text = openDialogWork("", "sln")) == "") Application.Exit();


            //current_solution = @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln";

            votesFileBox.Text = @"C:\temp\Metr.xml";
            //current_project = @"Metr2";
            Type[] estimationTypes = { typeof(EstimationOfElement) };
            serializer = new XmlSerializer(typeof(EstimationList), estimationTypes);
            InitSolution();
        }
        private void saveToFile() {

            form_votes.Estimations.Sort((EstimationOfElement a, EstimationOfElement b) =>
            {

                if (String.Compare(a.Solution, b.Solution) == 1) return 1;
                else if (a.Solution == b.Solution && String.Compare(a.Project, b.Project) == 1) return 1;
                else if (a.Solution == b.Solution && a.Project == b.Project && String.Compare(a.FullName, b.FullName) == 1) return 1;
                else if (a.Solution == b.Solution && a.Project == b.Project && a.FullName == b.FullName) return 0;
                else return -1;

            });

            //FileStream fs = new FileStream(votesFileBox.Text, FileMode.Create);
            //serializer.Serialize(fs, form_votes);
            //fs.Close();
            form_votes.SerializeTo(votesFileBox.Text);
        }
        private void LoadVotesFromFile()
        {
            
            
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(votesFileBox.Text.ToString(), FileMode.Open);
                    form_votes = (EstimationList)serializer.Deserialize(fs);
                    fs.Close();
                }
                catch
                {
                    var path = votesFileBox.Text.ToString();
                    try
                    {
                        if (fs != null) fs.Close();
                        Directory.CreateDirectory(Directory.GetParent(path).ToString());
                        File.Delete(path);
                    }
                    catch
                    {
                    }
                    File.Create(path).Close();

                    form_votes = new EstimationList();
                    saveToFile();
                }
               
            }
            if (symbolView.Nodes.Count > 0) RefreshSelected(symbolView.SelectedNode);
        }

        

        private void dfs_through_members(TreeNodeCollection treenode, INamespaceOrTypeSymbol symbol) {
            if (symbol != null && symbol.GetMembers() != null)
            foreach (var el in symbol.GetMembers()) {
                var cur = el as INamespaceOrTypeSymbol;
                if (cur != null) {
                    treenode.Add(cur.Name.ToString());
                    dfs_through_members(treenode[treenode.Count - 1].Nodes, cur);
                }
            }
        }

        private EstimationList form_votes;
        private String current_solution;
        private String current_project;
        XmlSerializer serializer;
        Solution _solution;

        


        private void InitSolution()
        {
            _solution = Metr.RoslynAPI.GetSolution(current_solution);
            var projects = _solution.Projects;

            comboProjects.Items.Clear();
            foreach (var el in projects) {
                comboProjects.Items.Add(el.Name.ToString());
            }
            comboProjects.SelectedIndex = 0;
            InitProject();
        }

        private void InitProject() {
            String projectToPick = comboProjects.Items[comboProjects.SelectedIndex].ToString();
            var project = _solution.Projects.Where(x => x.Name == projectToPick).FirstOrDefault();
            current_project = project.Name.ToString();
            InitTreeView(Metr.RoslynAPI.ProjectCompile(project));
        }

        private void InitTreeView(Compilation compilation){
            symbolView.Nodes.Clear();

            symbolView.BeginUpdate();
            foreach (var syntaxTree in compilation.SyntaxTrees)
                foreach (var member in ((CompilationUnitSyntax)syntaxTree.GetRoot()).Members)
                {
                    var ns = member as NamespaceDeclarationSyntax;
                    if (ns != null)
                    {
                        symbolView.Nodes.Add(ns.Name.ToString());
                        //  var treenode = symbolView.Nodes[symbolView.Nodes.Count - 1];

                        
                        dfs_through_members(symbolView.Nodes[symbolView.Nodes.Count-1].Nodes, compilation.GlobalNamespace.GetMembers(ns.Name.ToString()).FirstOrDefault());
                        //TODO: kill duplicates
                        //MakeUnique(symbolView.Nodes);
                    }
                }
            symbolView.EndUpdate();
            LoadVotesFromFile();
        }

        private void RefreshSelected(TreeNode boo)
        {
            if (boo == null) return;
            var name = boo.FullPath.Replace('\\', '.');
            foreach (var el in form_votes.Estimations)
            {
                if (current_solution == el.Solution && current_project == el.Project && name == el.FullName)
                {
                    voteBox.Value = el.Estimation;
                    is_voted.Checked = true;
                    return;
                }
            }
            voteBox.Value = 0;
            is_voted.Checked = false;
        }


        /// <summary>
        /// After Apply button pushed (vote Action is done)
        /// </summary>

        private void voteAction()
        {
            var name = symbolView.SelectedNode.FullPath.Replace('\\', '.');
            foreach (var el in form_votes.Estimations)
            {
                if (current_solution == el.Solution && current_project == el.Project && name == el.FullName)
                {
                    el.Estimation = (int)voteBox.Value;
                    return;
                }
            }
            is_voted.Checked = true;
            form_votes.AddEstimation(new EstimationOfElement(current_solution, current_project, name, (int)voteBox.Value));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        
        }
    
        private void symbolView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                RefreshSelected(e.Node);
            }
            catch
            { }
        }

 

        private void VotesBrowse_Click(object sender, EventArgs e)
        {
           votesFileBox.Text = openDialogWork(votesFileBox.Text,"xml");
           LoadVotesFromFile();
        }

        private void SaveVotes_Click(object sender, EventArgs e)
        {
            saveToFile();
        }

        private void LoadVotes_Click(object sender, EventArgs e)
        {
            LoadVotesFromFile();
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }


        private void voteBox_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void is_voted_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void voteBox_Enter(object sender, EventArgs e)
        {
            
        }

        private void applyBut_Click(object sender, EventArgs e)
        {
            is_voted.Checked = true;
            voteAction();
        }

        private void clearBut_Click(object sender, EventArgs e)
        {
            is_voted.Checked = false;
            voteBox.Value = 0;
            var name = symbolView.SelectedNode.FullPath.Replace('\\', '.');
            foreach (var el in form_votes.Estimations)
            {
                if (current_solution == el.Solution && current_project == el.Project && name == el.FullName)
                {
                    form_votes.Estimations.Remove(el);
                    return;
                }
            }
        }

       
        private void browseSolutionBut_Click(object sender, EventArgs e)
        {
            solutionBox.Text = openDialogWork(solutionBox.Text,"sln");
            InitSolution();
        }

        private void solutionBox_TextChanged(object sender, EventArgs e)
        {
            current_solution = solutionBox.Text;
        }

        private void loadProjectBut_Click(object sender, EventArgs e)
        {
            InitProject();
        }

        private void votesFileBox_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
