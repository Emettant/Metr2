﻿using System;
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
using MetrExpertXML;
using System.Xml.Serialization;

namespace Votes_Interface
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            current_solution = @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln";
            votesFileBox.Text = @"C:\temp2\Metr2.xml";
            current_project = @"Metr2";
            Type[] estimationTypes = { typeof(EstimationOfElement) };
            serializer = new XmlSerializer(typeof(EstimationList), estimationTypes);

            InitTreeView();
        }


        private void dfs_through_members(TreeNodeCollection treenode, Compilation compiltaion, INamespaceOrTypeSymbol symbol) {
            foreach (var el in symbol.GetMembers()) {
                var cur = el as INamespaceOrTypeSymbol;
                if (cur != null) {
                    treenode.Add(cur.Name.ToString());
                    dfs_through_members(treenode[treenode.Count - 1].Nodes, compiltaion, cur);
                }
            }
        }

        private EstimationList form_votes;
        private String current_solution;
        private String current_project;
        XmlSerializer serializer;

        private void InitTreeView(){
            Solution _solution;
            Compilation compilation;
            Metr.RoslynAPI.ProjectCompile(current_solution, current_project, out _solution, out compilation);

            symbolView.BeginUpdate();
            foreach (var syntaxTree in compilation.SyntaxTrees)
                foreach (var member in ((CompilationUnitSyntax)syntaxTree.GetRoot()).Members)
                {
                    var ns = member as NamespaceDeclarationSyntax;
                    if (ns != null)
                    {
                        symbolView.Nodes.Add(ns.Name.ToString());
                        //  var treenode = symbolView.Nodes[symbolView.Nodes.Count - 1];
                        dfs_through_members(symbolView.Nodes[symbolView.Nodes.Count-1].Nodes, compilation, compilation.GlobalNamespace.GetMembers(ns.Name.ToString()).FirstOrDefault());
                    }
                }
            symbolView.EndUpdate();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        
        }

        private void symbolView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
        }

        private void VotesBrowse_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            var tt = Directory.GetParent(votesFileBox.Text).ToString();
            openFileDialog1.InitialDirectory = tt;
            //openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //openFileDialog1.FilterIndex = 2;
            //openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        votesFileBox.Text = openFileDialog1.FileName;
                        using (myStream)
                        {
                          
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void SaveVotes_Click(object sender, EventArgs e)
        {
            form_votes = new EstimationList("TestVoteList");
            form_votes.AddEstimation(new EstimationOfElement(current_solution, current_project, "MetrExamples.E", 33));
            
            FileStream fs = new FileStream(votesFileBox.Text, FileMode.Create);
            serializer.Serialize(fs, form_votes);
            fs.Close();
            form_votes = null;
        }

        private void LoadVotes_Click(object sender, EventArgs e)
        {
            var fs = new FileStream(votesFileBox.Text, FileMode.Open);
            try
            {
                form_votes = (EstimationList)serializer.Deserialize(fs);

            }
            finally
            {
                fs.Close();
            }
        }
    }
}
