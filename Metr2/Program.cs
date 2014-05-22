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

    class ParentChildExternalMethodCalled
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
            Metr.RoslynAPI.ProjectCompile(@"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln", "Metr2", out _solution, out _compilation);
            //    string solutionPath = @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln";
            //    var workspace = MSBuildWorkspace.Create();
            //    _solution = workspace.OpenSolutionAsync(solutionPath).Result;
            //    var project = _solution.Projects.Where(p => p.Name == "Metr2").First();
            //    _compilation = project.GetCompilationAsync().Result;
            //// var temp = _compilation.GetCompilationNamespace(_compilation.GlobalNamespace);

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
                   Metr.MetricCalculator.NOC(_solution, _compilation, ExampleNamespaceName + ".PropertyClass"), "Blah");

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
                Metr.MetricCalculator.CBO(_solution, _compilation, ExampleNamespaceName + ".ParentChildExternalMethodCalled"), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(0,
                Metr.MetricCalculator.CBO(_solution, _compilation, ExampleNamespaceName + ".SingleClass"), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(2,
                Metr.MetricCalculator.CBO(_solution, _compilation, ExampleNamespaceName + ".DiffPlaceSameExternalMethodCall"), "Blah");

                

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(4,
                Metr.MetricCalculator.CBO(_solution, _compilation, ExampleNamespaceName + ".PropertyClass"), "Blah");
               
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
            if (v == null) return;
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
                                if (property.GetMethod != null) CallingMethods.Add(property.GetMethod);
                                if (property.SetMethod != null) CallingMethods.Add(property.SetMethod);
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
        static public Int32 RS(Compilation compilation, ITypeSymbol cur)
        {
            var t = cur.GetMembers()
            .Where(x => x is IMethodSymbol);
            var res = t.Count();
            return res;
        }

        static public Int32 RS(Compilation compilation, String className)
        {
            ITypeSymbol cur = compilation.GetTypeByMetadataName(className);
            return RS(compilation, cur);
        }

        static public Int32 WMC(Compilation compilation, String className)
        {
            /// TODO Complexity of Methods Analysis, not C==1 
            return RS(compilation, className);
        }

        static public Int32 DIT(Compilation compilation, ITypeSymbol cur)
        {
            int k = -1;
            while (cur != null)
            {
                cur = cur.BaseType;
                k++;
            }
            return k;
        }
        static public Int32 DIT(Compilation compilation, String className)
        {
            ITypeSymbol cur = compilation.GetTypeByMetadataName(className);
            return DIT(compilation, cur);
        }

        static public Int32 NOC(Solution solution, Compilation compilation, ITypeSymbol cur)
        {
            if (!_cacheTypesHierarchy.ContainsKey(compilation))
                BuildCompilationCacheMap(solution, compilation);

            return (_cacheTypesHierarchy[compilation].ContainsKey(cur)) ? _cacheTypesHierarchy[compilation][cur].Count : 0;
        }
        static public Int32 NOC(Solution solution, Compilation compilation, String className)
        {
            ITypeSymbol cur = compilation.GetTypeByMetadataName(className);
            return NOC(solution, compilation, cur);
        }

        static public Int32 CBO(Solution solution, Compilation compilation, ITypeSymbol cur)
        {
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
        static public Int32 CBO(Solution solution, Compilation compilation, String className)
        {
            ITypeSymbol cur = compilation.GetTypeByMetadataName(className);
            return NOC(solution, compilation, cur);
        }

        static public Int32 RFC(Solution solution, Compilation compilation, String className) {
            return 0;//CBO(Solution solution, Compilation compilation, String className) 
            }


    }

    public class RoslynAPI {

        
        static public Solution GetSolution(String solutionPath) {
            Solution solution = null;
            try { solution = MSBuildWorkspace.Create().OpenSolutionAsync(solutionPath).Result; }
            catch { }
            return solution;
        }

        static Dictionary<Tuple<string, string>, Compilation> _compilation_cache;
        static Dictionary<Tuple<string, string>, Compilation> compilation_cache
        {
            get
            {
                if (_compilation_cache == null)
                    _compilation_cache = new Dictionary<Tuple<string, string>, Compilation>();
                return _compilation_cache;
            }
        }

        static public Compilation GetCompilation(String solutionPath, String projectToPick) {
            return GetCompilation(GetSolution(solutionPath), projectToPick);
        }

        static private Compilation GetCompilation(Solution _solution, String projectToPick)
        {
            var keySolutionProject = new Tuple<string, string>(_solution.FilePath, projectToPick);
            Compilation compilation;
            if (compilation_cache.Keys.Contains(keySolutionProject))
                compilation = compilation_cache[keySolutionProject];
            else
            {
                compilation = _solution.Projects.ToList().Where(p => p.Name.ToString() == projectToPick).First().GetCompilationAsync().Result;
                compilation_cache[keySolutionProject] = compilation;
            }
            return compilation;
        }

        //static public IEnumerable<Project> GetProjects(String solutionPath) {
        //    return GetSolution(solutionPath).Projects;
        //}

        static public Compilation ProjectCompile(Project project)
        {
            //string solutionPath = @"C:\Users\Vitaliy\Documents\Visual Studio 2013\Projects\Metr2\Metr2.sln";
            //var workspace = MSBuildWorkspace.Create();
            //_solution = workspace.OpenSolutionAsync(solutionPath).Result;
            //var projects = _solution.Projects;
            //var project = projects.Where(p => p.Name.ToString() == projectToPick).First();
            return project.GetCompilationAsync().Result;
            // var temp = _compilation.GetCompilationNamespace(_compilation.GlobalNamespace);
        }

        static public void ProjectCompile(String solutionPath, String projectToPick, out Solution _solution, out Compilation _compilation)
        {
            _solution = GetSolution(solutionPath);
            _compilation = GetCompilation(_solution, projectToPick);
        }


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

        public EstimationList() { ListName = "EstimationList"; }

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
    using System.Text;
    using System.Xml.Serialization;
    using System.IO;
    using MetrExpertXML;
    using Adapter;

    static class Adapter {
        public static class Mathematica
        {
            static IKernelLink kl = null;
            static public IKernelLink Result { get { return kl; } }
            /// <summary>
            /// !!!Make sure you wrote N[...] for numerical!!!
            /// Call math kernel to evaluate string.
            /// </summary>
            /// <param name="input"></param>
            public static void Calc(string input)
            {
                Console.WriteLine("To Math kernel: \n" + input);
                if (kl == null)
                {
                    kl = MathLinkFactory.CreateKernelLink();//"-linkmode launch -linkname 'c:\\program files\\wolfram research\\mathematica\\9.0\\mathkernel'");
                    kl.WaitAndDiscardAnswer();
                }
                Console.WriteLine(input);
                kl.Evaluate(input);
                kl.WaitForAnswer();
            }

        }
        public static string ListToString<T>(IEnumerable<T> list){
            var sb = new StringBuilder("{");
            var separator = ", ";
            foreach (var el in list) {
                sb.Append(el.ToString());
                sb.Append(separator);
            }
            sb.Remove(sb.Length - separator.Length, separator.Length);
            sb.Append("}");
            return sb.ToString();
        }
        public static string ListToString(List<double> list) {
            return ListToString(list.Select(x => DoubleToString_ComaToDot_ForMathematica(x)));
        }
        //TODO: for every float type ComaToDot ... Environment settings
        public static string DoubleToString_ComaToDot_ForMathematica(double d) {
            return d.ToString().Replace(',','.');
        }
    }

    class Model {
        

        static List<double> _model = null;
        static public List<double> model { get { return _model; }}
        static public void Build<T>(List<List<T>> coefs, List<T> results) {

            var coefsString = new List<String>();
            foreach (var el in coefs) coefsString.Add(ListToString<T>(el));

            Mathematica.Calc("N[LeastSquares[" +
            ListToString<String>(coefsString) + "," +
            ListToString<T>(results) +
            "]]");
            var something = Mathematica.Result.GetDoubleArray();    
            _model = something.ToList();
            foreach (var el in _model) Console.WriteLine(el.ToString() + " ");
        }
        static public void Build(List<List<Int32>> coefs, List<Int32> results) {
            Build<Int32>(coefs, results);
        }

        static public double Apply(List<int> coefs, List<double> model) {
            var coefsString = ListToString(coefs);
            var modelString = ListToString(model);
            
            Mathematica.Calc(coefsString + "." + modelString);

            var res = Mathematica.Result.GetDouble(); 
            Console.WriteLine(res);
            return res;
        }

        //static public void Build(string fileMetricName) {
        //    Type[] estimationTypes = { typeof(EstimationOfElement) };
        //    XmlSerializer serializer = new XmlSerializer(typeof(EstimationList), estimationTypes);

        //    var fs = new FileStream(fileMetricName, FileMode.Open);
        //    try
        //    {
        //        var votesList = (EstimationList)serializer.Deserialize(fs);
        //        Solution solution = null;
        //        Compilation compilation = null;
        //        string solutionPath = null;
        //        string projectToPick = null;
        //        var coefs = new List<List<Int32>>();
        //        var ress = new List<Int32>();
        //        foreach (var v in votesList.Estimations) {
        //            if (v.Solution != solutionPath || v.Project != projectToPick)
        //                Metr.RoslynAPI.ProjectCompile((solutionPath = v.Solution), (projectToPick = v.Project), out solution, out compilation);

        //            coefs.Add(new List<Int32>()
        //            {
        //                Metr.MetricCalculator.RS(compilation,v.FullName),
        //                Metr.MetricCalculator.DIT(compilation,v.FullName),
        //                Metr.MetricCalculator.NOC(solution,compilation,v.FullName),
        //                Metr.MetricCalculator.CBO(solution, compilation,v.FullName)
        //            });
        //            ress.Add(v.Estimation);

        //        }
        //        Build(coefs, ress);
        //    }
        //    finally
        //    {
        //        fs.Close();
        //    }


        //}
    }

    class ModelTest {
        
        static public void Run() {
            //Model.Build(new List<List<Int32>> { new List<Int32> { 1, 2, 3 }, new List<Int32> { 2, 3, 4 }, new List<Int32> { 3, 4, 5 } }, new List<Int32> { 1, 1, 1 });
            
            //Model.Build(@"C:\temp2\Metr2.xml");

            //Mathematica.Calc(ListToString<Int32>(new List<Int32> { 2, 3, 4 }) + "+" + ListToString<Int32>(new List<Int32> { 2, 3, 4 }));
            //Console.WriteLine(Mathematica.Result.GetDoubleArray()[0]);
            //Mathematica.Calc("3+4");
            //Console.WriteLine(Mathematica.Result.GetDouble());
        }
    }


}

namespace MetrLearn
{
    using System.Xml.Serialization;
    using System.IO;
    using MetrExpertXML;
    using System.Reflection;

    [XmlType("TrainPoint")]
    public class TrainPoint
    {
        [XmlAttribute("RS")]
        public int RS { get; set; }

        [XmlAttribute("DIT")]
        public int DIT { get; set; }

        [XmlAttribute("NOC")]
        public int NOC { get; set; }

        [XmlAttribute("CBO")]
        public int CBO { get; set; }

        [XmlAttribute("Answer")]
        public int Answer { get; set; }

        [XmlAttribute("ClassName")]
        public string className { get; set; }

        public TrainPoint() { }
        public TrainPoint(List<int> list, string _className)
        {
            RS = list[0];
            DIT = list[1];
            NOC = list[2];
            CBO = list[3];

            Answer = list.Last();
            className = _className;
        }

        List<int> _cached_list;
        List<int> cached_list { get {
                if (_cached_list == null)
                {
                    var classType = this.GetType();
                    var propertiesInfos = classType.GetProperties().Where(x => x.Name != "className");
                    _cached_list = propertiesInfos.Select(x => (Int32)x.GetValue(this)).ToList();
                }
                return _cached_list;
            }
        }
        public List<int> ToList() {
            return cached_list;
        }

        public List<int> getRequest() {
            return cached_list.GetRange(0, cached_list.Count - 1);
        }

        public int getAnswer() {
            return cached_list.Last();
        }


        const int defaultEstimation = -1;
        public static TrainPoint getClassRequestAnswerPoint(Solution solution, Compilation compilation, string className, int estimation = defaultEstimation)
        {
            var curClass = compilation.GetTypeByMetadataName(className);
            if (curClass != null)
                return new TrainPoint(new List<Int32>()
                    {
                        Metr.MetricCalculator.RS(compilation,curClass),
                        Metr.MetricCalculator.DIT(compilation,curClass),
                        Metr.MetricCalculator.NOC(solution,compilation,curClass),
                        Metr.MetricCalculator.CBO(solution, compilation,curClass),
                        estimation
                    }, className);
            else return null;
        }

    }


    [XmlRoot("TrainPointsList")]
    [XmlInclude(typeof(TrainPoint))]
    public class TrainPointsList
    {
        [XmlArray("TrainPointsList")]
        [XmlArrayItem("TrainPointsListItem")]
        public List<TrainPoint> Points = new List<TrainPoint>();
        
       
        public TrainPointsList() { }

     

        public void AddPoint(TrainPoint point)
        {
            Points.Add(point);
        }
    }


    [XmlType("TrainedModelElement")]
    public class TrainedModelElement {
        [XmlAttribute("element")]
        public double Val { get; set; }

        public TrainedModelElement() { Val = -1; }
        public TrainedModelElement(double _val) { Val = _val; }
    }

    [XmlRoot("TrainedModel")]
    [XmlInclude(typeof(TrainedModelElement))]
    public class TrainedModel
    {
        [XmlElement("MethodName")]
        public string methodName { get; set; }

        [XmlArray("TrainedModelList")]
        [XmlArrayItem("TrainedModelListItem")]
        public List<TrainedModelElement> Items = new List<TrainedModelElement>();

        public TrainedModel() { }
        public TrainedModel(List<double> _list, string _methodName) {
            Items = _list.Select(x => new TrainedModelElement(x)).ToList();
            methodName = _methodName;
        }

        public List<double> ToList() {
            return Items.Select(x => x.Val).ToList();
        }

    }




    public class Train
    {
        
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFileName">Voted Metrics by experts</param>
        /// <param name="targetFileName">Points for train (already calculated metrics as coefficients)</param>
        static public void toTrainPoints(string sourceFileName, string targetFileName)
        {
            Type[] estimationTypes = { typeof(EstimationOfElement) };
            XmlSerializer serializer = new XmlSerializer(typeof(EstimationList), estimationTypes);
            var fs = new FileStream(sourceFileName, FileMode.Open);
            try
            {
                var votesList = (EstimationList)serializer.Deserialize(fs);
                Solution solution = null;
                Compilation compilation = null;
                string solutionPath = null;
                string projectToPick = null;
                //var coefs = new List<List<Int32>>();
                var coefs = new TrainPointsList();
                var ress = new List<Int32>();
                foreach (var v in votesList.Estimations)
                {
                    if (v.Solution != solutionPath || v.Project != projectToPick)
                        Metr.RoslynAPI.ProjectCompile((solutionPath = v.Solution), (projectToPick = v.Project), out solution, out compilation);

                    var curClass = compilation.GetTypeByMetadataName(v.FullName);
                    if (curClass != null)
                        coefs.AddPoint(


                    //    new TrainPoint(new List<Int32>()
                    //{
                    //    Metr.MetricCalculator.RS(compilation,curClass),
                    //    Metr.MetricCalculator.DIT(compilation,curClass),
                    //    Metr.MetricCalculator.NOC(solution,compilation,curClass),
                    //    Metr.MetricCalculator.CBO(solution, compilation,curClass),
                    //    v.Estimation
                    //},
                    //v.FullName)

                    TrainPoint.getClassRequestAnswerPoint(solution, compilation, v.FullName, v.Estimation)

                    );
                    //ress.Add(v.Estimation);
                    

                }
                Type[] estimationTypes2 = { typeof(TrainPoint) };
                XmlSerializer serializer2 = new XmlSerializer(typeof(TrainPointsList), estimationTypes2);
                FileStream fs2 = new FileStream(targetFileName, FileMode.Create);
                serializer2.Serialize(fs2, coefs);
                fs.Close();
                //Build(coefs, ress);
                //save to new file
            }
            finally
            {
                fs.Close();
            }
        }

        static public void toTrainedModel(string sourceFileName, string targetFileName, ModelToTrainMethod method) {

            Type[] Types1 = { typeof(TrainPoint) };
            XmlSerializer serializer1 = new XmlSerializer(typeof(TrainPointsList), Types1);
            TrainPointsList trainPoints = null;
            using (var fs = new FileStream(sourceFileName, FileMode.Open)) {
                //try { // TODO: write file not exist, not deserialize
                    trainPoints = (TrainPointsList)serializer1.Deserialize(fs);
            }
          

            var model = getTrainedModel(trainPoints, method);

            Type[] Types2 = { typeof(TrainedModelElement) };
            XmlSerializer serializer2 = new XmlSerializer(typeof(TrainedModel), Types2);
            using (var fs = new FileStream(targetFileName, FileMode.Create)) {
                try
                {
                    serializer2.Serialize(fs, model);
                }
                catch
                { }
            }
            model = null;

            

        }

        static Dictionary<string, TrainedModel> _cached_model;
        static Dictionary<string, TrainedModel> cached_model { get {
                if (_cached_model == null) {
                    _cached_model = new Dictionary<string, TrainedModel>();
                }
                return _cached_model;
            }
        }
        static public int getAnswerForClassByModel(string sourceFile, string solutionPath, string projectToPick, string className)
        {
            TrainedModel model = null;
            if (cached_model.Keys.Contains(sourceFile))
                model = cached_model[sourceFile];
            else
            {

                Type[] Types2 = { typeof(TrainedModelElement) };
                XmlSerializer serializer2 = new XmlSerializer(typeof(TrainedModel), Types2);
                using (var fs = new FileStream(sourceFile, FileMode.Open))
                {
                    try
                    {
                        model = (TrainedModel)serializer2.Deserialize(fs);
                    }
                    catch
                    { }
                }

                cached_model[sourceFile] = model;
            }

            Compilation compilation = Metr.RoslynAPI.GetCompilation(solutionPath, projectToPick);
            var point = TrainPoint.getClassRequestAnswerPoint(Metr.RoslynAPI.GetSolution(solutionPath), compilation, className);
            if (point == null) return -1;// if (className is namespace name)
            else return (int)Math.Round(MetrMath.Model.Apply(point.getRequest(), model.ToList()));
        }

        public enum  ModelToTrainMethod{
            LeastSquaresMethod = 0,
            KNN_Method
        };

        static public TrainedModel getTrainedModel(TrainPointsList trainPoints, ModelToTrainMethod method)
        {
            if (method == ModelToTrainMethod.LeastSquaresMethod)
            {
                var ReAn = trainPoints.Points.Select(x => x.ToList());
                var Request = ReAn.Select(x => x.GetRange(0, x.Count - 1)).ToList();
                var Answer = ReAn.Select(x => x.Last()).ToList();

                MetrMath.Model.Build(Request, Answer);
                return new TrainedModel(MetrMath.Model.model, method.ToString());
                
            }
            else return null;
        }




        static public void RunMathFunction() {
            MetrMath.Adapter.Mathematica.Calc(
                @"

givenDataUnNorm = {{1, 2, 3, 4, 5}, {11, 33, 22, 44, 55}, {6, 7, 8, 9, 0}, {66, 77, 88,
    99, 0}, {2, 4, 6, 8, 0}};
NormalizeMaxMin [Data_] := 
 Module[{NormalizationCoefs = {}, MinCoefs = {}},
  Do[AppendTo[NormalizationCoefs, 
    1/(Max[Data[[All, kkk]]] - Min[Data[[All, kkk]]])];
   AppendTo[MinCoefs, Min[Data[[All, kkk]]]];
   , {kkk, Length[Data]}];
  (# - MinCoefs)*NormalizationCoefs & /@ Data
  ];
N[NormalizeMaxMin[givenDataUnNorm],20][[2]]"
                );
            var t = MetrMath.Adapter.Mathematica.Result.GetDoubleArray();
        }
    }

    class TrainTest {
        static public void Run() {
            // Train.toTrainPoints(@"C:\temp2\Another-Metric.xml", "");

            //Train.RunMathFunction();
            Train.toTrainedModel("C:\\temp2\\Another-Metric - points.xml", "C:\\temp2\\test.xml",Train.ModelToTrainMethod.LeastSquaresMethod);
            //var ttt = new TrainPoint(new List<int> { 1, 2, 3, 4, 5 });
            //var myList = ttt.ToList();

        }
    }
}

namespace MetrMain
{
    class Program
    {


        static void Main(string[] args)
        {
            //MetrTest.MetricCalculatorTest.Run();
            // MetrExpertXML.EstimationListTest.Run();
            //MetrMath.ModelTest.Run();
            MetrLearn.TrainTest.Run();


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

namespace MetrInterface {
    using System.IO;
    using System.Windows.Forms;
    using System.Text;
    public static class Tools {
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

    public class ControlsGroup {
        protected static int enumeratorOpenFileGroup = 0;
        protected ControlsGroup()
        {
            enumeratorOpenFileGroup++;
        }
    }

    public class OpenFileGroup : ControlsGroup {

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
        class DataGridSourceItem {
            public string TypeOrNamespaceName { get; set; }
            public int ModelEstimation { get; set; }
            public DataGridSourceItem(string name, int est) {
                TypeOrNamespaceName = name;
                ModelEstimation = est;
            }
        }

        string curent_model_file;

        bool _is_blocked = false;

        const int defaultMetricValue = -1;

        private bool BlockedGroup {
            get {
                return _is_blocked;
            }
            set {
                _is_blocked = value;
                refreshBlocked();
            }
        }

        public void refreshBlocked() {
            foreach (var control in current_controls) control.Enabled = !_is_blocked;
        }

        void TryInitAll() {
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
        private void InitListOfUniqueNames(Compilation compilation) {

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

        private void InitGridView() {
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

        void CalculateModel() {
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
            this.symbolView.Location = new System.Drawing.Point(33, y+55);
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
            this.comboProjects.Location = new System.Drawing.Point(413, y+270);
            this.comboProjects.Name = "comboProjects";
            this.comboProjects.Size = new System.Drawing.Size(337, 24);
           
            
            //this.comboProjects.TabIndex = 11;
            // 
            // loadProjectBut
            // 
            this.loadProjectBut.Location = new System.Drawing.Point(625, y+300);
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
            this.symbolTableView.Location = new System.Drawing.Point(43, y+330);
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
