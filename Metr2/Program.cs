//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;

//using Microsoft.CodeAnalysis.CSharp.Symbols;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.CodeAnalysis.Text;

//namespace Metr2
//{   
//    class Program
//    {
//        static void Main(string[] args)
//        {

//            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"using System;

//                namespace HelloWorld
//                {
//                    class Program
//                    {
//                        static void Main(string[] args)
//                        {
//                            Console.WriteLine(""Hello, World!"");
//                        }
//                    }
//                }");

//            //var work = new Workspace()
//            //  Microsoft.CodeAnalysis.Composition.FeaturePack
//            var bld = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create()

//        Compilation compilation = CSharpCompilation.Create("HelloWorld")
//                               .AddReferences(
//                new MetadataFileReference(
//                typeof(object).Assembly.Location))
//                              .AddSyntaxTrees(tree);

//            INamespaceSymbol globalNamespace = compilation.GlobalNamespace;


//            foreach (INamespaceOrTypeSymbol member in globalNamespace.GetMembers())
//            {

//                if (member.Name == "HelloWorld") {
//                    foreach (ITypeSymbol mem in ((INamespaceSymbol)member).GetMembers())
//                    {
//                        Console.WriteLine(mem.ToString());
//                        foreach (ISymbol mem2 in mem.GetMembers())
//                        {
//                            Console.WriteLine(mem2.ToString());
//                        }
//                    }
//                    break;
//                }
//            }
//        }
//    }
//}
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Diagnostics;

using System.Collections.Generic;



namespace MetrMain
{
    class Program
    {
        static void Main(string[] args)
        {
            MetrLearn.KNNModel.TestRun();
            //MetrLearn.LeastSquaresModel_MinMax.getModel(@"C:\temp2\Real_Metr2\Hierarchical - points.xml").SerializeTo(@"C:\temp2\Real_Metr2\Hierarchical - model2.xml");

            //var lst = MetrXML.GoodSerializer.loadFromFileTrainPointsList(@"C:\temp2\Real_Metr2\Hierarchical - points.xml");
            //MetrLearn.ModelParent model;
            //MetrXML.GoodSerializer.loadFromFile(@"C:\temp2\Real_Metr2\Hierarchical - model2.xml", out model);
            //var mod = (MetrLearn.LeastSquaresModel_MinMax)model;
            //var ttt = mod.Apply(lst.Points[1]);
            //var t = 1;
            
            //MetrTest.MetricCalculatorTest.Run();

            //string solutionPath = @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln";

            //var compilation = Metr.RoslynAPI.GetCompilation(solutionPath, "Metr2");
            ////var what = compilation.GetTypeByMetadataName("MetrExamples.C");
            //var loc = compilation.GetTypeByMetadataName("MetrExamples.ClassLOC");
            //var t = Metr.MetricCalculator.CS(compilation, what);//10
            //var t2 = Metr.MetricCalculator.NOO(compilation, what);//1
            //var t3 = Metr.MetricCalculator.NOA(compilation, what);//3
            //var t4 = Metr.MetricCalculator.SI100(compilation, what);//27
            //var t5 = Metr.MetricCalculator.OS(compilation, compilation.GetTypeByMetadataName("MetrExamples.ClassLOC"));
            //var t6 = Metr.MetricCalculator.OC(compilation, compilation.GetTypeByMetadataName("MetrExamples.CyclomaticClass"));//12
            //var t7 = Metr.MetricCalculator.NP100(compilation, compilation.GetTypeByMetadataName("MetrExamples.NumParam"));
            //var tt = 2;
            //var m1 = c.GetMembers().Where(x => x.Name == "anotherMethod").First();
            //var m1pro = m1.GetType().GetProperties();
            //var t3 = Metr.MetricCalculator.NOA(compilation, compilation.GetTypeByMetadataName("MetrExamples.PartialClass"));

            //var m2 = c.GetMembers().Where(x => x.Name == "MethodToBeOverwritten").First();
            //var m2pro = m2.GetType().GetProperties();
            //for (var i = 0; i < m1pro.Length; ++i) {
            //    if (m1pro[i].GetValue(m1) != m2pro[i].GetValue(m2)) Console.WriteLine(m1pro[i].Name+" : "+m1pro[i].GetValue(m1).ToString() +" : "+ m2pro[i].GetValue(m2).ToString() + "\n\n");
            //}
            //foreach (var el in m1pro) Console.WriteLine(el.Item1, " ", el.Item2);
            //foreach (var el in m2pro) Console.WriteLine(el.Item1, " ", el.Item2);
            //var l1 = Metr.RoslynAPI.getMethods(c);
            //var l2 = Metr.RoslynAPI.getMethods(c.BaseType);
            //var ttt = Metr.MetricCalculator.mergeChildParentMethodList(Metr.RoslynAPI.getMethods(c),Metr.RoslynAPI.getMethods(c.BaseType));




            // MetrExpertXML.EstimationListTest.Run();
            //MetrMath.ModelTest.Run();
            // MetrLearn.TrainTest.Run();

            //MetrXML.GoodSerializerTest.Run();


            //string solutionPath = @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln";
            //var workspace = MSBuildWorkspace.Create();
            //var solution = workspace.OpenSolutionAsync(solutionPath).Result;
            //var project = solution.Projects.Where(p => p.Name == "Metr2").First();
            //var compilation = project.GetCompilationAsync().Result;


            //var programClass = compilation.GetTypeByMetadataName("RoslynTest.Program");

            //var barMethod = programClass.GetMembers("Bar").First();
            //var fooMethod = programClass.GetMembers("Foo").First();

            //var barResult = SymbolFinder.FindReferencesAsync(barMethod, solution).Result.ToList();
            //var fooResult = SymbolFinder.FindReferencesAsync(fooMethod, solution).Result.ToList();

            //Debug.Assert(barResult.First().Locations.Count() == 1);
            //Debug.Assert(fooResult.First().Locations.Count() == 0);
        }


    }
}


//namespace MetrGit {
//    using System.IO;
//    public class GitIterator
//    {

//        readonly String nameIteratorBranch = "git_iterator";
//        ProcessStartInfo StartInfo = new ProcessStartInfo();
//        Process proc = new Process();
//        static public StreamWriter streamWriter = (StreamWriter)Console.Out;

//        //static public void Run() {
//        //    var gi = new GitIterator("front");
//        //    do
//        //    {
//        //        ProcessCommitFiles(GetFilesList());
//        //    }
//        //    while (!gi.isErrorToRollBack());
//        //}

//        void InvokeAction(String Arg, out String Cerr)
//        {

//            proc.StartInfo.Arguments = Arg;
//            proc.Start();
//            Cerr = proc.StandardError.ReadToEnd();
//            streamWriter.WriteLine(proc.StandardOutput.ReadToEnd());
//            streamWriter.WriteLine(Cerr);
//            proc.WaitForExit();
//        }
//        void InvokeAction(String Arg)
//        {
//            String c;
//            InvokeAction(Arg, out c);
//        }
//        public bool isErrorToRollBack()
//        {
//            proc.StartInfo = StartInfo;
//            InvokeAction("checkout " + nameIteratorBranch);

//            String result;
//            InvokeAction("reset --hard HEAD~1", out result);
//            return result.IndexOf("fatal") == 0;
//        }
//        public GitIterator(String endBranchName, String workingDirectory)
//        {

//            StartInfo.FileName = "C:\\Program Files (x86)\\Git\\bin\\git.exe";
//            StartInfo.WorkingDirectory = workingDirectory;
//            StartInfo.UseShellExecute = false;
//            StartInfo.RedirectStandardOutput = true;
//            StartInfo.RedirectStandardError = true;

//            proc.StartInfo = StartInfo;
//            InvokeAction("checkout " + endBranchName);
//            InvokeAction("branch -D " + nameIteratorBranch);
//            InvokeAction("branch " + nameIteratorBranch);
//            InvokeAction("checkout " + nameIteratorBranch);
//        }

//    }
//}
