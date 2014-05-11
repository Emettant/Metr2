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

namespace Votes_Interface
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            form_votes = new Dictionary<Tuple<string, string, string>, int>();
            current_solution = @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln";
            current_project = @"Metr2";

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

        private Dictionary<Tuple<string, string, string>, Int32> form_votes;
        private String current_solution;
        private String current_project;

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
    }
}
