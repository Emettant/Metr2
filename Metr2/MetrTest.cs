using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MetrTest
{
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
        static public void Test_Lorenz(String nameOfThisMethod)
        {
            try
            {
                var compilation = _compilation;
                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(10, 
                Metr.MetricCalculator.CS(compilation, compilation.GetTypeByMetadataName(ExampleNamespaceName + ".C")),"Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(1,
                Metr.MetricCalculator.NOO(compilation, compilation.GetTypeByMetadataName(ExampleNamespaceName + ".C")),"Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(3,
                Metr.MetricCalculator.NOA(compilation, compilation.GetTypeByMetadataName(ExampleNamespaceName + ".C")),"Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(27,
                Metr.MetricCalculator.SI100(compilation, compilation.GetTypeByMetadataName(ExampleNamespaceName + ".C")), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(10,
                Metr.MetricCalculator.OS(compilation, compilation.GetTypeByMetadataName(ExampleNamespaceName + ".ClassLOC")), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(12,
                Metr.MetricCalculator.OC(compilation, compilation.GetTypeByMetadataName(ExampleNamespaceName + ".CyclomaticClass")), "Blah");

                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(14,
                Metr.MetricCalculator.NP100(compilation, compilation.GetTypeByMetadataName(ExampleNamespaceName + ".NumParam")), "Blah");

                Console.WriteLine(nameOfThisMethod + " - OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static public void Test_OC(String nameOfThisMethod)
        {
            try
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.
                Assert.AreEqual(12,
                Metr.MetricCalculator.OC( _compilation, _compilation.GetTypeByMetadataName(ExampleNamespaceName + ".CyclomaticClass")), "Blah");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        static public void Template(String nameOfThisMethod)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }

}

