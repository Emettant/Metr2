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


namespace MetrExamples
{
    class PropertyClass {

        int d_field1;
        public int d_pro1 {
            get {
                B.PublicMethod2_B();//*** for CBO is the same that lower// that's why counted 1 per d_pro1
                return d_field1;
            }
            set {
                C.PublicMethod1_C();
                C.PublicMethod2_B();//*** for CBO counted as method B.PublicMethod2_B(), which was present earlier // that's why counted 1 per d_pro1
                d_field1 = value;
            }
        }
    }

    //CBO == 2, PublicMethod1_B() called in different places
    class DiffPlaceSameExternalMethodCall {
        public void op1() {
            B.PublicMethod1_B();
        }
        public void op2()
        {
            B.PublicMethod1_B();
        }
    }

    //CBO == 0, no count for self calling
    class SingleClass {
        public void s_op1() {
            s_op2();
        }
        public void s_op2()
        {

        }
    }

    class E
    {
        //CBO == 2, because B.PublicMethod1_B() and C.PublicMethod1_B() are the same
        public void e_op1()
        {
            B.PublicMethod1_B();
            C.PublicMethod1_B();

            C.PublicMethod1_C();
        }
    }

    class A
    {

    }

    class B : A
    {
        static public void PublicMethod1_B() { }
        static public void PublicMethod2_B() { }

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
        static public void PublicMethod1_C() { }
        public void PublicMethod2_C() { }



    }
}
namespace MetrTest { 
    //  [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    class MetricCalculatorTest
    {

        static Compilation _compilation;
        static Solution _solution;
        static String ExampleNamespaceName = "MetrExamples";
        
        public static void Run()
        {
                string solutionPath = @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln";
                var workspace = MSBuildWorkspace.Create();
                _solution = workspace.OpenSolutionAsync(solutionPath).Result;
                var project = _solution.Projects.Where(p => p.Name == "Metr2").First();
                _compilation = project.GetCompilationAsync().Result;
            // var temp = _compilation.GetCompilationNamespace(_compilation.GlobalNamespace);

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
                Metr.MetricCalculator.DIT(_compilation, ExampleNamespaceName + ".C"), "Blah");
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
                Metr.MetricCalculator.WMC(_compilation, ExampleNamespaceName + ".C"), "Blah");
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
                Metr.MetricCalculator.NOC(_solution, _compilation, ExampleNamespaceName + ".A"), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(2,
                Metr.MetricCalculator.NOC(_solution, _compilation, ExampleNamespaceName + ".B2"), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                   Assert.AreEqual(0,
                   Metr.MetricCalculator.NOC(_solution, _compilation, ExampleNamespaceName + ".D"), "Blah");

                Console.WriteLine(nameOfThisMethod + " - OK");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static public void TestCBO_0(String nameOfThisMethod)
        {
            try
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(2,
                Metr.MetricCalculator.CBO(_solution, _compilation, ExampleNamespaceName + ".E"), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(0,
                Metr.MetricCalculator.CBO(_solution, _compilation, ExampleNamespaceName + ".SingleClass"), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(0,
                Metr.MetricCalculator.CBO(_solution, _compilation, ExampleNamespaceName + ".DiffPlaceSameExternalMethodCall"), "Blah");

                

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(5,
                Metr.MetricCalculator.CBO(_solution, _compilation, ExampleNamespaceName + ".D"), "Blah");
               
                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(0,
                Metr.MetricCalculator.CBO(_solution, _compilation, ExampleNamespaceName + ".A"), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(0,
                Metr.MetricCalculator.CBO(_solution, _compilation, ExampleNamespaceName + ".B"), "Blah");

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
    class MetricCalculatorCore  {
        static private Dictionary<Compilation, Dictionary<ITypeSymbol, List<ITypeSymbol>>> _cacheTypesHierarchyField;
        static protected Dictionary<Compilation, Dictionary<ITypeSymbol, List<ITypeSymbol>>> _cacheTypesHierarchy
        {
            get
            {
                if (_cacheTypesHierarchyField == null) _cacheTypesHierarchyField = new Dictionary<Compilation, Dictionary<ITypeSymbol, List<ITypeSymbol>>>();
                return _cacheTypesHierarchyField;
            }
        }
        /// <summary>
        /// d.op() { b.meth(); } // calling method
        /// b.op() { } // called method
        ///calling Method -> called Method
        /// it could be Method or Property 
        /// 
        /// </summary>
        static private Dictionary<Compilation, Dictionary<ISymbol, HashSet<ISymbol>>> _cacheMethodsCouplingField;
        static protected Dictionary<Compilation, Dictionary<ISymbol, HashSet<ISymbol>>> _cacheMethodsCoupling
        {
            get
            {
                if (_cacheMethodsCouplingField == null) _cacheMethodsCouplingField = new Dictionary<Compilation, Dictionary<ISymbol, HashSet<ISymbol>>>();
                return _cacheMethodsCouplingField;
            }
        }

        static protected void BuildCompilationCacheMap(Solution solution, Compilation compilation)
        {
            _cacheTypesHierarchy.Add(compilation, new Dictionary<ITypeSymbol, List<ITypeSymbol>>());
            _cacheMethodsCoupling.Add(compilation, new Dictionary<ISymbol, HashSet<ISymbol>>());

            //for every SyntaxTree was analysed in current compilation
            //for every "global-declared" namespace member was present in this compilation
            foreach (var syntaxTree in compilation.SyntaxTrees)
                foreach (var member in ((CompilationUnitSyntax)syntaxTree.GetRoot()).Members)
                {
                    var ns = member as NamespaceDeclarationSyntax;
                    dfs(solution, compilation, compilation.GlobalNamespace.GetMembers(ns.Name.ToString()).FirstOrDefault());
                }
        }

        static private void dfs(Solution solution, Compilation compilation, INamespaceOrTypeSymbol v)
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
                    dfs(solution, compilation, to);
                }

                var asMeth = cur as IMethodSymbol;
                var asPro = cur as IPropertySymbol;
                var MethodList = new List<IMethodSymbol> { asMeth };
                //we do not need analyse property, because its methods already is members to see
                //if (asPro != null) MethodList = new List<IMethodSymbol> { asPro.GetMethod, asPro.SetMethod};

                foreach (var meth in MethodList) {
                    if (meth != null)
                    {
                        var too = SymbolFinder.FindCallersAsync(meth, solution);
                        too.Wait();
                        var CallingMethods = new List<IMethodSymbol>();
                        foreach (var el in too.Result) {
                            var method = el.CallingSymbol as IMethodSymbol;
                            if (method != null) CallingMethods.Add(method);
                            var property = el.CallingSymbol as IPropertySymbol;
                            if (property != null)
                            {
                                CallingMethods.Add(property.GetMethod); CallingMethods.Add(property.SetMethod);
                            }
                        }
                   
                        foreach (var el in CallingMethods)
                        {
                            if (el.ContainingType != meth.ContainingType)
                            {
                                if (!_cacheMethodsCoupling[compilation].ContainsKey(el))
                                    _cacheMethodsCoupling[compilation].Add(el, new HashSet<ISymbol>());

                                _cacheMethodsCoupling[compilation][el].Add(meth);
                            }
                        }

                    }
                }
            }
        }

      
    }

    class MetricCalculator : MetricCalculatorCore
    {
       private MetricCalculator() { }

        /// <summary>
        /// Counts of methods (including all constructors) of given class. Without members of parent class
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        static public Int32 RS(Compilation compilation, String className)
        {
            ITypeSymbol cur = compilation.GetTypeByMetadataName(className);

            var t = cur.GetMembers()
            .Where(x => x is IMethodSymbol);
            var res = t.Count();
            return res;
        }

        static public Int32 WMC(Compilation compilation, String className)
        {
            /// TODO Complexity of Methods Analysis, not C==1 
            return RS(compilation, className);
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

        static public Int32 NOC(Solution solution, Compilation compilation, String className)
        {
            ITypeSymbol cur = compilation.GetTypeByMetadataName(className);
            if (!_cacheTypesHierarchy.ContainsKey(compilation))
                BuildCompilationCacheMap(solution, compilation);

            return (_cacheTypesHierarchy[compilation].ContainsKey(cur)) ? _cacheTypesHierarchy[compilation][cur].Count : 0;
        }

        static public Int32 CBO(Solution solution, Compilation compilation, String className)
        {
            ITypeSymbol cur = compilation.GetTypeByMetadataName(className);
            if (!_cacheMethodsCoupling.ContainsKey(compilation))
                BuildCompilationCacheMap(solution, compilation);

            Int32 ret = 0;
            foreach (ISymbol mem in cur.GetMembers()) {
                ISymbol meth = mem as IMethodSymbol;
                // There are property.get ans property.set concidered as IMethodSymbol-s, 
                //if (meth == null) meth = mem as IPropertySymbol;
                if (meth == null) continue;

                if (_cacheMethodsCoupling[compilation].ContainsKey(meth))
                {
                    var temp = _cacheMethodsCoupling[compilation][meth];
                    ret += _cacheMethodsCoupling[compilation][meth].Count();
                }
                
            }
            return ret;
        }

        static public Int32 RFC(Solution solution, Compilation compilation, String className) {
            return 0;//CBO(Solution solution, Compilation compilation, String className) 
            }


    }


  
   
}

namespace MetrMain {
    class Program
    {
        static void Main(string[] args)
        {
            //MetrTest.MetricCalculatorTest.Run();
            // MetrExpertXML.EstimationListTest.Run();
            MetrMath.ModelTest.Run();



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

        //public bool Foo()
        //{
        //    return "Bar" == Bar();
        //}

        //public string Bar()
        //{
        //    return "Bar";
        //}
    }
}

namespace MetrExpertXML {
    using System.Xml.Serialization;
    using System.IO;

    [XmlType("EstimationOfElement")]
    public class EstimationOfElement{
        [XmlAttribute("Solution")]
        public string Solution { get; set; }

        [XmlAttribute("Project")]
        public string Project { get; set; }

        [XmlAttribute("FullName")]
        public string FullName { get; set; }

        [XmlAttribute("Estimation")]
        public int Estimation { get; set; }

        public EstimationOfElement() { }
        public EstimationOfElement(string solution, string project, string name, int est) {
            this.Solution = solution;
            this.Project = project;
            this.FullName = name;
            this.Estimation = est;
        }
    }

    [XmlRoot("EstimationList")]
    [XmlInclude(typeof(EstimationOfElement))] // include type class EstimationOfElement
    public class EstimationList
    {
        [XmlArray("EstimationArray")]
        [XmlArrayItem("EstimationObject")]
        public List<EstimationOfElement> Estimations = new List<EstimationOfElement>();

        [XmlElement("ListName")]
        public string ListName { get; set; }

        public EstimationList() { }

        public EstimationList(string name)
        {
            this.ListName = name;
        }

        public void AddEstimation(EstimationOfElement estimation)
        {
            Estimations.Add(estimation);
        }
    }

    public class EstimationListTest {
        static public void Run() {

            var estimList = new EstimationList();
            estimList.ListName = "TestEstims";

            estimList.AddEstimation(new EstimationOfElement("Metr2","Metr2", "MetrExamples.D",33));
            estimList.AddEstimation(new EstimationOfElement("Metr2", "Metr2", "MetrExamples.B", 11));
            estimList.AddEstimation(new EstimationOfElement("Metr2", "Metr2", "MetrExamples.A", 12));

            // Serialize 
            Type[] estimationTypes = { typeof(EstimationOfElement) };
            XmlSerializer serializer = new XmlSerializer(typeof(EstimationList), estimationTypes);
            FileStream fs = new FileStream("EstimationList.xml", FileMode.Create);
            serializer.Serialize(fs, estimList);
            fs.Close();
            estimList = null;

            // Deserialize 
            fs = new FileStream("EstimationList.xml", FileMode.Open);
            estimList = (EstimationList)serializer.Deserialize(fs);
            serializer.Serialize(Console.Out, estimList);
        }
    }

}

namespace MetrMath {
    using Wolfram.NETLink;
    using Adapter;

    static class Adapter {
        static IKernelLink kl = null;
        public static string Calc(string input) {
            if (kl == null) {
                kl = MathLinkFactory.CreateKernelLink();//"-linkmode launch -linkname 'c:\\program files\\wolfram research\\mathematica\\9.0\\mathkernel'");
                kl.WaitAndDiscardAnswer();
            }
            return kl.EvaluateToOutputForm(input, input.Length);
        }
        public static string ListToString<T>(List<T> list){
            var sb = new System.Text.StringBuilder("{");
            var separator = ", ";
            foreach (var el in list) {
                sb.Append(el.ToString());
                sb.Append(separator);
            }
            sb.Remove(sb.Length - separator.Length, separator.Length);
            sb.Append("}");
            return sb.ToString();
        }
    }

    class Model {
        static public void Build() {

            
        }
    }

    class ModelTest {
        
        static public void Run() {
            //Model.Build();
            Console.WriteLine(ListToString<Int32>(new List<Int32> { 2, 3, 4 }));
            Console.WriteLine(Calc(ListToString<Int32>(new List<Int32> { 2, 3, 4 })+"+"+ ListToString<Int32>(new List<Int32> { 2, 3, 4 })));
        }
    }


}
