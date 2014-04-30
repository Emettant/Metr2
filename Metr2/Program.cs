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

namespace MetrHelper
{
    class A
    {

    }

    class B : A
    {
        protected String _field1;

        protected B() { }
        private B(Int32 a)
        {
            _field1 = a.ToString();
        }

        protected B(String a)
        {
            _field1 = a.ToString();
        }

        public B(Int64 a)
        {
            _field1 = a.ToString();
        }

        private void PrivateMethod1_B() { }
        protected void ProtectedMethod1_B() { }
        public void PublicMethod1_B() { }
        public void PublicMethod2_B() { }

    }

    class B2 : A {

    }

    class B3 : A
    {

    }

    class B2Children1 : B2 {

    }

    class B2Children2 : B2
    {

    }

    class C : B
    {
        class InternalDefinedClass1 {

        }


        private C(Int32 a)
        {
            _field1 = a.ToString();
        }

        protected C(String a) {
            _field1 = a.ToString();
        }

        public C(Int64 a)
        {
            _field1 = a.ToString();
        }


        private void PrivateMethod1_C() { }
        protected void ProtectedMethod1_C() { }
        public void PublicMethod1_C() { }
        public void PublicMethod2_C() { }



    }
}
namespace MetrTest { 
    //  [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    class MetricCalculatorTest
    {

        static Compilation _compilation;
        public static void Run()
        {
                string solutionPath = @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln";
                var workspace = MSBuildWorkspace.Create();
                var solution = workspace.OpenSolutionAsync(solutionPath).Result;
                var project = solution.Projects.Where(p => p.Name == "Metr2").First();
                _compilation = project.GetCompilationAsync().Result;


                var classType = Type.GetType("MetrTest.MetricCalculatorTest");
                var methodInfos = classType.GetMethods(//System.Reflection.BindingFlags
                    ).Where(x => x.Name.IndexOf("Test") != -1);
                //System.Reflection.CustomAttributeData
                object classInstance = Activator.CreateInstance(classType, null);
                foreach (var meth in methodInfos)
                {
                    meth.Invoke(classInstance, new object[] { meth.Name });
                }
           
        }

        //  [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        static public void TestDIT0(String nameOfThisMethod)
        {
            try
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(3, 
                Metr.MetricCalculator.DIT(_compilation, "MetrHelper.C"), "Blah");
                Console.WriteLine(nameOfThisMethod + " - OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static public void TestWMC0(String nameOfThisMethod)
        {
            try
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(7, 
                Metr.MetricCalculator.WMC(_compilation, "MetrHelper.C"), "Blah");
                Console.WriteLine(nameOfThisMethod + " - OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static public void TestNOC0(String nameOfThisMethod)
        {
            try
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(3,
                Metr.MetricCalculator.NOC(_compilation, "MetrHelper.A"), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(2,
                Metr.MetricCalculator.NOC(_compilation, "MetrHelper.B2"), "Blah");

                Console.WriteLine(nameOfThisMethod + " - OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }

}


namespace Metr
{
    class MetricCalculator
    {


        static private Dictionary<Compilation, Dictionary<ITypeSymbol, List<ITypeSymbol>>> _cacheTypesHierarchyField;
        static private Dictionary<Compilation, Dictionary<ITypeSymbol, List<ITypeSymbol>>> _cacheTypesHierarchy {
            get {
                if (_cacheTypesHierarchyField == null) _cacheTypesHierarchyField = new Dictionary<Compilation, Dictionary<ITypeSymbol, List<ITypeSymbol>>>();
                return _cacheTypesHierarchyField;
            }
        }
        private MetricCalculator() { }


        static private void dfs(Compilation compilation, INamespaceOrTypeSymbol v)
        {
            foreach (var cur in v.GetMembers())
            {
                var to = cur as INamespaceOrTypeSymbol;
                if (to != null)
                {
                    var toAsType = to as ITypeSymbol;
                    if (toAsType != null)
                    {
                        if (toAsType.BaseType != null)
                        {
                            if (!_cacheTypesHierarchy[compilation].ContainsKey(toAsType.BaseType))
                                _cacheTypesHierarchy[compilation].Add(toAsType.BaseType, new List<ITypeSymbol>());

                            _cacheTypesHierarchy[compilation][toAsType.BaseType].Add(toAsType);
                        }
                    }
                    dfs(compilation, to);
                }
            }
        }

        static private void BuildTypesHierarchyForCompilation(Compilation compilation)
        {
            _cacheTypesHierarchy.Add(compilation, new Dictionary<ITypeSymbol, List<ITypeSymbol>>());
            dfs(compilation, compilation.GlobalNamespace);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="className"></param>
        /// <returns>Count of methods (including all constructors) of given class. Without members of parent class</returns>
        static public Int32 WMC(Compilation compilation, String className)
        {
            /// TODO Complexity of Methods Analysis 
            ITypeSymbol cur = compilation.GetTypeByMetadataName(className);
                
            var t = cur.GetMembers()
            .Where(x => x is IMethodSymbol);
            var res = t.Count();
            return res;
        }


        static public Int32 DIT(Compilation compilation, String className)
        {
            int k = -1;
            ITypeSymbol cur = compilation.GetTypeByMetadataName(className);
            while (cur != null)
            {
                cur = cur.BaseType;
                k++;
            }
            return k;
        }

        static public Int32 NOC(Compilation compilation, String className)
        {
            ITypeSymbol cur = compilation.GetTypeByMetadataName(className);
            if (!_cacheTypesHierarchy.ContainsKey(compilation)) BuildTypesHierarchyForCompilation(compilation);

            return _cacheTypesHierarchy[compilation][cur].Count;
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            MetrTest.MetricCalculatorTest.Run();

            

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

        public bool Foo()
        {
            return "Bar" == Bar();
        }

        public string Bar()
        {
            return "Bar";
        }
    }

   
}

