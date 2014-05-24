using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;



namespace MetrMath
{
    using Wolfram.NETLink;
    using System.Text;
    using Adapter;

    static class Adapter
    {
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
        public static string ListToString<T>(IEnumerable<T> list)
        {
            var sb = new StringBuilder("{");
            var separator = ", ";
            foreach (var el in list)
            {
                sb.Append(el.ToString());
                sb.Append(separator);
            }
            sb.Remove(sb.Length - separator.Length, separator.Length);
            sb.Append("}");
            return sb.ToString();
        }
        public static string ListToString(List<double> list)
        {
            return ListToString(list.Select(x => DoubleToString_ComaToDot_ForMathematica(x)));
        }
        //TODO: for every float type ComaToDot ... Environment settings
        public static string DoubleToString_ComaToDot_ForMathematica(double d)
        {
            return d.ToString().Replace(',', '.');
        }
    }

    class Model
    {


        static List<double> _model = null;
        static public List<double> model { get { return _model; } }
        static public void Build<T>(List<List<T>> coefs, List<T> results)
        {

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
        static public void Build(List<List<Int32>> coefs, List<Int32> results)
        {
            Build<Int32>(coefs, results);
        }

        static public double Apply(List<int> coefs, List<double> model)
        {
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

    class ModelTest
    {

        static public void Run()
        {
            //Model.Build(new List<List<Int32>> { new List<Int32> { 1, 2, 3 }, new List<Int32> { 2, 3, 4 }, new List<Int32> { 3, 4, 5 } }, new List<Int32> { 1, 1, 1 });

            //Model.Build(@"C:\temp2\Metr2.xml");

            //Mathematica.Calc(ListToString<Int32>(new List<Int32> { 2, 3, 4 }) + "+" + ListToString<Int32>(new List<Int32> { 2, 3, 4 }));
            //Console.WriteLine(Mathematica.Result.GetDoubleArray()[0]);
            //Mathematica.Calc("3+4");
            //Console.WriteLine(Mathematica.Result.GetDouble());
        }
    }


}

