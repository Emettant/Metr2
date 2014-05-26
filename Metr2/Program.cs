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
            //MetrTest.MetricCalculatorTest.Run();

            // MetrExpertXML.EstimationListTest.Run();
            //MetrMath.ModelTest.Run();
            // MetrLearn.TrainTest.Run();

            MetrXML.GoodSerializerTest.Run();
            

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
