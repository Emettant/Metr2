using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Diagnostics;

using System.Collections.Generic;

namespace MetrInterface
{
    using System.IO;
    using System.Windows.Forms;

    public static class Tools
    {
        public static string openDialogWork(string startPath, string extention)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            try
            {
                var tt = Directory.GetParent(startPath).ToString();
                openFileDialog1.InitialDirectory = tt;
            }
            catch
            { }
            openFileDialog1.Filter = "files (*." + extention + ")|*." + extention;
            //openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //openFileDialog1.FilterIndex = 2;
            //openFileDialog1.RestoreDirectory = true;
            String ret = startPath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        ret = openFileDialog1.FileName;
                        myStream.Close();// Super-imporant
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

            return ret;
        }

    }

    public class ControlsGroup
    {
        protected static int enumeratorOpenFileGroup = 0;
        protected ControlsGroup()
        {
            enumeratorOpenFileGroup++;
        }
    }

    public class OpenFileGroup : ControlsGroup
    {

        Label nameLabel;
        Label statusLabel;
        TextBox fileBox;
        Button browseButton;
        Form parent;
        Action<OpenFileGroup> action;

        string extention;

        static string status = "Status: ";
        static string yesFileStatus = status + "!!! There is a file already with such name !!! ";
        static string noFileStatus = status + "File does not exist.";
        static string inProgressStatus = status + "In progress...";
        static string isDoneStatus = status + "Done :) ";


        public bool FileExist { get; private set; }
        public string FileName { get; private set; }

        void refreshFileExistence()
        {
            statusLabel.Text = (FileExist = File.Exists(fileBox.Text)) ?
                yesFileStatus : noFileStatus;
            FileName = fileBox.Text;

        }
        private void Browse_Click(object sender, EventArgs e)
        {
            fileBox.Text = Tools.openDialogWork(fileBox.Text, extention);
            refreshFileExistence();
            action.Invoke(this);
        }

        public OpenFileGroup(Form parent, int y, string startPath, string buttonText, string nameLabelText, string _extention, Action<OpenFileGroup> _action) : base()
        {
            action = _action;

            browseButton = new Button();
            nameLabel = new Label();
            statusLabel = new Label();
            fileBox = new TextBox();
            extention = _extention;

            browseButton.Location = new System.Drawing.Point(648, y + 23);
            browseButton.Name = "browseButton" + enumeratorOpenFileGroup.ToString();
            browseButton.Size = new System.Drawing.Size(93, 50);
            browseButton.Text = buttonText;
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += new System.EventHandler(Browse_Click);
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Location = new System.Drawing.Point(15, y + 23);
            nameLabel.Name = "nameLabel" + enumeratorOpenFileGroup.ToString();
            nameLabel.Size = new System.Drawing.Size(75, 17);
            nameLabel.Text = nameLabelText;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new System.Drawing.Point(165, y + 23);
            statusLabel.Name = "statusLabel" + enumeratorOpenFileGroup.ToString();
            statusLabel.Size = new System.Drawing.Size(124, 17);
            statusLabel.Text = "Status";
            // 
            // fileBox
            // 
            fileBox.Location = new System.Drawing.Point(18, y + 51);
            fileBox.Name = "fileBox" + enumeratorOpenFileGroup.ToString();
            fileBox.ReadOnly = true;
            fileBox.Size = new System.Drawing.Size(624, 22);
            fileBox.Text = startPath;

            parent.Controls.Add(browseButton);
            parent.Controls.Add(nameLabel);
            parent.Controls.Add(statusLabel);
            parent.Controls.Add(fileBox);

            refreshFileExistence();
            action.Invoke(this);
        }


    }

    public class CompilationViewGroup : ControlsGroup
    {
        TreeView symbolView;
        ComboBox comboProjects;
        Button loadProjectBut;
        DataGridView symbolTableView;
        List<Control> current_controls;

        Solution _solution;
        string current_project;
        string current_solution_file;
        SortedSet<string> current_names;
        List<DataGridSourceItem> dataGridSourceList;
        class DataGridSourceItem
        {
            public string TypeOrNamespaceName { get; set; }
            public int ModelEstimation { get; set; }
            public DataGridSourceItem(string name, int est)
            {
                TypeOrNamespaceName = name;
                ModelEstimation = est;
            }
        }

        string curent_model_file;

        bool _is_blocked = false;

        const int defaultMetricValue = -1;

        private bool BlockedGroup
        {
            get
            {
                return _is_blocked;
            }
            set
            {
                _is_blocked = value;
                refreshBlocked();
            }
        }

        public void refreshBlocked()
        {
            foreach (var control in current_controls) control.Enabled = !_is_blocked;
        }

        void TryInitAll()
        {
            try
            {
                _solution = Metr.RoslynAPI.GetSolution(current_solution_file);
                if (_solution == null) return;
                var projects = _solution.Projects;

                comboProjects.Items.Clear();
                foreach (var el in projects)
                {
                    comboProjects.Items.Add(el.Name.ToString());
                }
                comboProjects.SelectedIndex = 0;
                InitProject();
                BlockedGroup = false;
            }
            catch
            {
                BlockedGroup = true;
            }
        }
        public void InitSolution(string solutionPath)
        {
            current_solution_file = solutionPath;
            TryInitAll();
        }
        public void InitModel(string modelFilePath)
        {
            curent_model_file = modelFilePath;
            TryInitAll();
        }

        private void InitProject()
        {
            String projectToPick = comboProjects.Items[comboProjects.SelectedIndex].ToString();
            var project = _solution.Projects.Where(x => x.Name == projectToPick).FirstOrDefault();
            current_project = project.Name.ToString();

            Compilation compilation = Metr.RoslynAPI.ProjectCompile(project);
            InitListOfUniqueNames(compilation);

            CalculateModel();

            //InitTreeView(Metr.RoslynAPI.ProjectCompile(project));
            InitGridView();
        }

        private void dfs_through_members_build_name(String name, INamespaceOrTypeSymbol symbol)
        {
            if (symbol != null && symbol.GetMembers() != null)
                foreach (var el in symbol.GetMembers())
                {
                    var cur = el as INamespaceOrTypeSymbol;
                    if (cur != null)
                    {
                        var cur_name = name + "." + cur.Name.ToString();
                        current_names.Add(cur_name);
                        dfs_through_members_build_name(cur_name, cur);
                    }
                }
        }
        private void InitListOfUniqueNames(Compilation compilation)
        {

            current_names = new SortedSet<string>();

            foreach (var syntaxTree in compilation.SyntaxTrees)
                foreach (var member in ((CompilationUnitSyntax)syntaxTree.GetRoot()).Members)
                {
                    var ns = member as NamespaceDeclarationSyntax;
                    if (ns != null)
                    {
                        //symbolView.Nodes.Add(ns.Name.ToString());
                        current_names.Add(ns.Name.ToString());
                        //  var treenode = symbolView.Nodes[symbolView.Nodes.Count - 1];

                        dfs_through_members_build_name(ns.Name.ToString(), compilation.GlobalNamespace.GetMembers(ns.Name.ToString()).FirstOrDefault());
                        // dfs_through_members(symbolView.Nodes[symbolView.Nodes.Count - 1].Nodes, compilation.GlobalNamespace.GetMembers(ns.Name.ToString()).FirstOrDefault());
                        //TODO: kill duplicates
                        //MakeUnique(symbolView.Nodes);
                    }
                }
        }

        private void InitGridView()
        {
            symbolTableView.DataSource = dataGridSourceList;//current_names.ToList();
            symbolTableView.Columns[0].Width = 300;

        }
        private void InitTreeView(Compilation compilation)
        {
            //symbolView.Nodes.Clear();

            //symbolView.BeginUpdate();
            //foreach (var syntaxTree in compilation.SyntaxTrees)
            //    foreach (var member in ((CompilationUnitSyntax)syntaxTree.GetRoot()).Members)
            //    {
            //        var ns = member as NamespaceDeclarationSyntax;
            //        if (ns != null)
            //        {
            //            symbolView.Nodes.Add(ns.Name.ToString());
            //            //  var treenode = symbolView.Nodes[symbolView.Nodes.Count - 1];


            //            dfs_through_members(symbolView.Nodes[symbolView.Nodes.Count - 1].Nodes, compilation.GlobalNamespace.GetMembers(ns.Name.ToString()).FirstOrDefault());
            //            //TODO: kill duplicates
            //            //MakeUnique(symbolView.Nodes);
            //        }
            //    }
            //symbolView.EndUpdate();
            //LoadVotesFromFile();
        }

        void CalculateModel()
        {
            dataGridSourceList = current_names.Select(x =>
                new DataGridSourceItem(
                    x,
                MetrLearn.Train.getAnswerForClassByModel(curent_model_file, current_solution_file, current_project, x
                )
            )).ToList();
        }


        private void loadProjectBut_Click(object sender, EventArgs e)
        {
            InitProject();
        }
        private void InitNotInterfaceElements(string solutionPath)
        {
            InitSolution(solutionPath);
        }

        public CompilationViewGroup(Form parent, int y, string solutionPath)
        {

            symbolView = new System.Windows.Forms.TreeView();
            comboProjects = new System.Windows.Forms.ComboBox();
            loadProjectBut = new System.Windows.Forms.Button();
            symbolTableView = new DataGridView();
            // 
            // symbolView
            // 
            this.symbolView.Location = new System.Drawing.Point(33, y + 55);
            this.symbolView.Name = "symbolView";
            this.symbolView.Size = new System.Drawing.Size(364, 183);
            //this.symbolView.TabIndex = 0;
            //symbolView.Visible = false;
            //this.symbolView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.symbolView_AfterSelect);

            // 
            // comboProjects
            // 
            this.comboProjects.ForeColor = System.Drawing.Color.Black;
            this.comboProjects.FormattingEnabled = true;
            this.comboProjects.Location = new System.Drawing.Point(413, y + 270);
            this.comboProjects.Name = "comboProjects";
            this.comboProjects.Size = new System.Drawing.Size(337, 24);


            //this.comboProjects.TabIndex = 11;
            // 
            // loadProjectBut
            // 
            this.loadProjectBut.Location = new System.Drawing.Point(625, y + 300);
            this.loadProjectBut.Name = "loadProjectBut";
            this.loadProjectBut.Size = new System.Drawing.Size(125, 23);
            //this.loadProjectBut.TabIndex = 12;
            this.loadProjectBut.Text = "Load Project";
            this.loadProjectBut.UseVisualStyleBackColor = true;
            this.loadProjectBut.Click += new System.EventHandler(this.loadProjectBut_Click);
            // 
            // symbolTableView
            // 
            ((System.ComponentModel.ISupportInitialize)(this.symbolTableView)).BeginInit();
            parent.SuspendLayout();
            this.symbolTableView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.symbolTableView.Location = new System.Drawing.Point(43, y + 330);
            this.symbolTableView.Name = "symbolTableView";
            this.symbolTableView.RowTemplate.Height = 24;
            this.symbolTableView.Size = new System.Drawing.Size(669, 170);
            //this.symbolTableView.TabIndex = 0;
            ((System.ComponentModel.ISupportInitialize)(this.symbolTableView)).EndInit();
            parent.ResumeLayout(false);

            current_controls = new List<Control>() { symbolView, symbolTableView, loadProjectBut, comboProjects };
            foreach (var control in current_controls) parent.Controls.Add(control);

            InitNotInterfaceElements(solutionPath);
        }

    }
}
