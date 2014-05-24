using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

using System.Collections.Generic;



namespace Metr
{
    class MetricCalculatorCore
    {
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
            _cacheTypesHierarchy.Add(compilation, new Dictionary<ITypeSymbol, List<ITypeSymbol>>());
            _cacheMethodsCoupling.Add(compilation, new Dictionary<ISymbol, HashSet<ISymbol>>());

            //for every SyntaxTree was analysed in current compilation
            //for every "global-declared" namespace member was present in this compilation
            var semanticModels = compilation.SyntaxTrees.Select(x => compilation.GetSemanticModel(x));

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                foreach (var member in ((CompilationUnitSyntax)syntaxTree.GetRoot()).Members)
                {
                    var ns = member as NamespaceDeclarationSyntax;
                    dfs(solution, compilation, semanticModels, compilation.GlobalNamespace.GetMembers(ns.Name.ToString()).FirstOrDefault());
                }
            }
            Console.WriteLine(Environment.TickCount - ttt);
        }

        static private void dfs(Solution solution, Compilation compilation, IEnumerable<SemanticModel> models, INamespaceOrTypeSymbol v)
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
                    dfs(solution, compilation, models, to);
                }

                var asMeth = cur as IMethodSymbol;
                if (asMeth != null)
                {
                    if (asMeth.PartialImplementationPart != null)
                        asMeth = asMeth.PartialImplementationPart; // if partial method => work with implementation

                    var declarings = asMeth.DeclaringSyntaxReferences;
                    if (declarings != null && declarings.Length == 1)// it is a method, so it could not be implemented in two places.
                        foreach (var invocationExpression in
                                 declarings
                                 .First()
                                 .GetSyntax()
                                 .DescendantNodes()
                                 .OfType<InvocationExpressionSyntax>())
                            foreach (var model in models)
                            {
                                try
                                {
                                    //var t1 = invocationExpression.Parent;
                                    //var t2 = invocationExpression.Parent.Parent;
                                    //var t3 = invocationExpression.Parent.Parent.Parent;
                                    //var t4 = invocationExpression.Parent.Parent.Parent.Parent;


                                    SymbolInfo symbolInfo = model.GetSymbolInfo(invocationExpression);
                                    var calledMethod = (IMethodSymbol)symbolInfo.Symbol;


                                    if (!_cacheMethodsCoupling[compilation].ContainsKey(asMeth))
                                        _cacheMethodsCoupling[compilation].Add(asMeth, new HashSet<ISymbol>());

                                    _cacheMethodsCoupling[compilation][asMeth].Add(calledMethod);

                                    break;
                                }
                                catch
                                { }
                            }


                }


                //var asPro = cur as IPropertySymbol;
                //var MethodList = new List<IMethodSymbol> { asMeth };
                ////we do not need analyse property, because its methods already is members to see
                ////if (asPro != null) MethodList = new List<IMethodSymbol> { asPro.GetMethod, asPro.SetMethod};

                //foreach (var meth in MethodList)
                //{
                //    if (meth != null)
                //    {
                //        var too = SymbolFinder.FindCallersAsync(meth, solution);
                //        too.Wait();
                //        var CallingMethods = new List<IMethodSymbol>();
                //        foreach (var el in too.Result)
                //        {
                //            var method = el.CallingSymbol as IMethodSymbol;
                //            if (method != null) CallingMethods.Add(method);
                //            var property = el.CallingSymbol as IPropertySymbol;
                //            if (property != null)
                //            {
                //                if (property.GetMethod != null) CallingMethods.Add(property.GetMethod);
                //                if (property.SetMethod != null) CallingMethods.Add(property.SetMethod);
                //            }
                //        }

                //        foreach (var el in CallingMethods)
                //        {
                //            if (el.ContainingType != meth.ContainingType)
                //            {
                //                if (!_cacheMethodsCoupling[compilation].ContainsKey(el))
                //                    _cacheMethodsCoupling[compilation].Add(el, new HashSet<ISymbol>());

                //                _cacheMethodsCoupling[compilation][el].Add(meth);
                //            }
                //        }

                //    }
                // }

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
            return NOC(solution, compilation, cur);
        }

        static public Int32 RFC(Solution solution, Compilation compilation, String className)
        {
            return 0;//CBO(Solution solution, Compilation compilation, String className) 
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


    }


}


