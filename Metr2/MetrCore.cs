using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.Generic;



namespace Metr
{
    class MetricCalculatorCore
    {


        static private Dictionary<Compilation, Dictionary<ITypeSymbol, HashSet<ITypeSymbol>>> _cacheTypesHierarchyField;
        static protected Dictionary<Compilation, Dictionary<ITypeSymbol, HashSet<ITypeSymbol>>> _cacheTypesHierarchy
        {
            get
            {
                if (_cacheTypesHierarchyField == null) _cacheTypesHierarchyField = new Dictionary<Compilation, Dictionary<ITypeSymbol, HashSet<ITypeSymbol>>>();
                return _cacheTypesHierarchyField;
            }
        }

        /// <summary>
        /// d.op() { b.meth(); } // calling method
        /// b.op() { } // called method
        ///calling Method -> called Method
        /// it could be Method only (Property get or set method) 
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
            int ttt = Environment.TickCount;
            _cacheTypesHierarchy.Add(compilation, new Dictionary<ITypeSymbol, HashSet<ITypeSymbol>>());
            _cacheMethodsCoupling.Add(compilation, new Dictionary<ISymbol, HashSet<ISymbol>>());

            //for every SyntaxTree was analysed in current compilation
            //for every "global-declared" namespace member was present in this compilation

            //var semanticModels = compilation.SyntaxTrees.Select(x => compilation.GetSemanticModel(x));
            
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                foreach (var member in ((CompilationUnitSyntax)syntaxTree.GetRoot()).Members)
                {
                    var ns = member as NamespaceDeclarationSyntax;
                    dfs(solution, compilation, compilation.GlobalNamespace.GetMembers(ns.Name.ToString()).FirstOrDefault());
                }
            }
            int ttt2 = Environment.TickCount - ttt;
            
        }

        static private void dfs(Solution solution, Compilation compilation,  INamespaceOrTypeSymbol v)
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
                                _cacheTypesHierarchy[compilation].Add(toAsType.BaseType, new HashSet<ITypeSymbol>());

                            _cacheTypesHierarchy[compilation][toAsType.BaseType].Add(toAsType);
                        }
                    }
                    dfs(solution, compilation,  to);
                }

                var asMeth = cur as IMethodSymbol;
                //if (asMeth != null)
                //{
                //    if (asMeth.PartialImplementationPart != null)
                //        asMeth = asMeth.PartialImplementationPart; // if partial method => work with implementation

                //    var declarings = asMeth.DeclaringSyntaxReferences;
                //    if (declarings != null && declarings.Length == 1)// it is a method, so it could not be implemented in two places.
                //        foreach (var invocationExpression in
                //                 declarings
                //                 .First()
                //                 .GetSyntax()
                //                 .DescendantNodes()
                //                 .OfType<InvocationExpressionSyntax>())
                //            foreach (var model in models)
                //            {
                //                try
                //                {
                //                    //var t1 = invocationExpression.Parent;
                //                    //var t2 = invocationExpression.Parent.Parent;
                //                    //var t3 = invocationExpression.Parent.Parent.Parent;
                //                    //var t4 = invocationExpression.Parent.Parent.Parent.Parent;


                //                    SymbolInfo symbolInfo = model.GetSymbolInfo(invocationExpression);
                //                    var calledMethod = (IMethodSymbol)symbolInfo.Symbol;


                //                    if (!_cacheMethodsCoupling[compilation].ContainsKey(asMeth))
                //                        _cacheMethodsCoupling[compilation].Add(asMeth, new HashSet<ISymbol>());

                //                    _cacheMethodsCoupling[compilation][asMeth].Add(calledMethod);

                //                    break;
                //                }
                //                catch
                //                { }
                //            }


                //}


                var asPro = cur as IPropertySymbol;
                var MethodList = new List<IMethodSymbol> { asMeth };
                //we do not need analyse property, because its methods already is members to see
                //if (asPro != null) MethodList = new List<IMethodSymbol> { asPro.GetMethod, asPro.SetMethod};

                foreach (var meth in MethodList)
                {
                    if (meth != null)
                    {
                        //TODO : make it to parallel processes
                        var too = SymbolFinder.FindCallersAsync(meth, solution);
                        too.Wait();
                        var CallingMethods = new List<IMethodSymbol>();
                        foreach (var el in too.Result)
                        {
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

                ///////////////////////////////////////////////////////////////
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
            foreach (ISymbol mem in cur.GetMembers())
            {
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
            return CBO(solution, compilation, cur);
        }

        //we do not need any RFC because RFC = CS + CBO
        static public Int32 RFC(Solution solution, Compilation compilation, String className)
        {
            return 0;//CBO(Solution solution, Compilation compilation, String className) 
        }


        static private IEnumerable<IMethodSymbol> mergeChildParentMethodList(IEnumerable<IMethodSymbol> list1, IEnumerable<IMethodSymbol> list2)
        {
            //var together = new Dictionary<Tuple<string,ITypeSymbol,System.Collections.Immutable.ImmutableArray<ITypeParameterSymbol>>,IMethodSymbol>();

            var list2_dif = new List<IMethodSymbol>();

            foreach (var meth in list2)
            {

                var t = list1.Where((IMethodSymbol bmeth) =>
                {

                    if (bmeth.PartialImplementationPart != null) bmeth = bmeth.PartialImplementationPart;
                    var declarations = bmeth.DeclaringSyntaxReferences;
                    if (declarations != null && declarations.Length == 1)
                    {
                        var mods = ((BaseMethodDeclarationSyntax)declarations.First().GetSyntax()).Modifiers;//.Where(x => x.Text == "public" || x.Text == "protected");
                        return (mods.Count() > 0)
                && bmeth.Name == meth.Name
                && bmeth.ReturnType == meth.ReturnType
                && bmeth.TypeParameters == meth.TypeParameters;
                    }

                    return false;

                }
                 ).FirstOrDefault();

                if (t == null) list2_dif.Add(t);

            }
            //return list1;
            foreach (var el in list1) yield return el;
            foreach (var el in list2_dif) yield return el;
            //foreach (IMethodSymbol meth in cur.GetMembers().Where(x => x is IMethodSymbol))
            //{
            //    var t = cur.BaseType.GetMembers().Where(x => x is IMethodSymbol).Select(x=>(IMethodSymbol)x).Where((IMethodSymbol bmeth) =>
            //    {
            //        return bmeth.Name == meth.Name && bmeth.ReturnType == meth.ReturnType && bmeth.TypeParameters == meth.TypeParameters;
            //    }
            //    ).FirstOrDefault();
            //    if (t != null) yield return t;
            //}
        }

        /// <summary>
        /// Class size.
        /// All class methods and properties
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="cur"></param>
        /// <returns></returns>
        static public Int32 CS(Compilation compilation, ITypeSymbol cur)
        {
            var curType = RoslynAPI.emitAndLoad(compilation, cur);
            //var ms = curTypeInfo.GetMethods();
            //var ps = curTypeInfo.GetProperties();
            return curType.GetMethods().Count() + curType.GetProperties().Count();
        }

        /// <summary>
        /// overrided methods
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="cur"></param>
        /// <returns></returns>
        static public Int32 NOO(Compilation compilation, ITypeSymbol cur) {
            int sm = 0;
            foreach (var meth in RoslynAPI.getMethods(cur)) sm += meth.IsOverride ? 1 : 0;
            return sm;
        }

        /// <summary>
        /// added methods.
        /// New methods does not count.
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="cur"></param>
        /// <returns></returns>
        static public Int32 NOA(Compilation compilation, ITypeSymbol cur)
        {
            var curType = RoslynAPI.emitAndLoad(compilation, cur);
            var baseType = curType.BaseType;
            var mc = curType.GetMethods().Count();
            //TODO exclude extention methods
            int mb = 0;
            if (baseType != null)
            mb = baseType.GetMethods().Where(x => !x.IsPrivate).Count();
            return Math.Max(mc - mb, 0); 
        }

        /// <summary>
        /// Metric 4
        /// metric SI * 100
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="cur"></param>
        /// <returns></returns>
        static public Int32 SI100(Compilation compilation, ITypeSymbol cur) {
            var curType = RoslynAPI.emitAndLoad(compilation, cur);
            var count = (curType.GetMethods() != null) ? curType.GetMethods().Count() : 0;
            return 100 * (NOO(compilation, cur) * DIT(compilation, cur)) / (count + 1);
        }

       
        const int bad_value = -1;

        /// <summary>
        /// metric 5
        /// average LOC
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="cur"></param>
        /// <returns></returns>
        static public Int32 OS(Compilation compilation, ITypeSymbol cur) {
            Int32 sm = 0;
            int k = 0;
            var ttt = RoslynAPI.getMethods(cur);
            foreach (var el in RoslynAPI.getMethods(cur))
            {
                var temp = LOC(el);

                if (temp != bad_value)
                {
                    k++;
                    sm += temp;
                }
            }
            return (k != 0) ? sm / k : 0;
        }
        /// <summary>
        /// Metric 6
        /// Kind of cyclomatic complexity
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="cur"></param>
        /// <returns></returns>
        static public Int32 OC(Compilation compilation, ITypeSymbol cur)
        {
            Int32 sm = 0;
            foreach (var el in RoslynAPI.getMethods(cur))
            {
                var decl = RoslynAPI.getMethodDeclaration(el);
                if (decl == null) continue;
                var body = decl.DescendantNodes();

                var dos = body.OfType<DoStatementSyntax>();
                var whiles = body.OfType<WhileStatementSyntax>();
                var ifs = body.OfType<IfStatementSyntax>();
                var fors = body.OfType<ForStatementSyntax>();
                var foreaches = body.OfType<ForEachStatementSyntax>();
                var switches = body.OfType<SwitchSectionSyntax>();
                var gotos = body.OfType<GotoStatementSyntax>();
                sm += dos.Count() + whiles.Count() + ifs.Count() + fors.Count() + foreaches.Count() + switches.Count() + gotos.Count();
            }
            return sm;
        }

        static public Int32 NP100(IMethodSymbol el)
        {
            var temp = LOC(el);
            return (temp != 0) ? 100 * el.Parameters.Count() / LOC(el) : bad_value;
        }


       
        /// <summary>
        /// Metric 7
        /// Average Number of Parameters per operation
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="cur"></param>
        /// <returns></returns>
        static public Int32 NP100(Compilation compilation, ITypeSymbol cur) {
            Int32 sm = 0;
            int k = 0;
            foreach (var el in RoslynAPI.getMethods(cur))
            {
                var temp = NP100(el);
                if (temp != bad_value)
                {
                    k++;
                    sm += temp;
                }
            }
            return (k != 0) ? sm / k : 0;
        }


        static public Int32 LOC(IMethodSymbol el) {
            //TODO: kill comments
            var decl = RoslynAPI.getMethodDeclaration(el);
            if (decl == null) return bad_value;
            //Console.WriteLine(decl.ToFullString());
            return  decl.ToFullString().Count(x => x == '\n');
        }

       
    }
    public class RoslynAPI
    {


        static public Solution GetSolution(String solutionPath)
        {
            Solution solution = null;
            try
            { solution = MSBuildWorkspace.Create().OpenSolutionAsync(solutionPath).Result; }
            catch
            { }
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

        static public Compilation GetCompilation(String solutionPath, String projectToPick)
        {
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

        static public IEnumerable<IMethodSymbol> getMethods(ITypeSymbol cur)
        {
            return cur.GetMembers().Select(x => x as IMethodSymbol).Where(x => x != null);
        }

        static  Dictionary<Compilation, System.Reflection.Assembly> _compilationAssembly_cache;
        static Dictionary<Compilation, System.Reflection.Assembly> compilationAssembly_cache {
            get {
                if (_compilationAssembly_cache == null) _compilationAssembly_cache = new Dictionary<Compilation, System.Reflection.Assembly>();
                return _compilationAssembly_cache;
            }
        }

        static private HashSet<Compilation> SetOfEmmitedCompilations = new HashSet<Compilation>();

        static public Type emitAndLoad(Compilation compilation, ITypeSymbol cur)
        {
            System.Reflection.Assembly assembly;
            if (compilationAssembly_cache.Keys.Contains(compilation)) assembly = compilationAssembly_cache[compilation];
            else {
                var name = "emittedOne1.dll";
                var filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), name);
                if (!SetOfEmmitedCompilations.Contains(compilation))
                {
                    compilation.Emit(filePath);
                    SetOfEmmitedCompilations.Add(compilation);
                }
                assembly = System.Reflection.Assembly.LoadFrom(filePath);
                compilationAssembly_cache[compilation] = assembly;
            }
            return assembly.GetTypes().Where(x => x.Name == cur.Name).First();
        }

        static public SyntaxNode getMethodDeclaration(IMethodSymbol el) {
            var meth = (el.PartialImplementationPart != null) ? el.PartialImplementationPart : el;
            var declarings = meth.DeclaringSyntaxReferences;
            if (declarings == null || declarings.Count() == 0) return null;
            return declarings.First().GetSyntax();
        }
    }


}


