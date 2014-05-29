using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


using System.Collections.Generic;



namespace MetrLearn {
    using System.Xml.Serialization;
    using System.IO;
    using MetrXML;
    public class Train
    {
       


        //TODO: Serialization/Deserialization from toTrainPoints have to move to MetrXML

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

                int ttt = Environment.TickCount;

                foreach (var v in votesList.Estimations)
                {
                    int tt = Environment.TickCount;

                    if (v.Solution != solutionPath || v.Project != projectToPick)
                        Metr.RoslynAPI.ProjectCompile((solutionPath = v.Solution), (projectToPick = v.Project), out solution, out compilation);

                    var curClass = compilation.GetTypeByMetadataName(v.FullName);
                    int tt2 = Environment.TickCount - tt;


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

                int ttt2 = Environment.TickCount - ttt;
                coefs.SerializeTo(targetFileName);
                //Type[] estimationTypes2 = { typeof(TrainPoint) };
                //XmlSerializer serializer2 = new XmlSerializer(typeof(TrainPointsList), estimationTypes2);
                //FileStream fs2 = new FileStream(targetFileName, FileMode.Create);
                //serializer2.Serialize(fs2, coefs);
                //fs.Close();


                //Build(coefs, ress);
                //save to new file
            }
            finally
            {
                fs.Close();
            }
        }

        static public void toTrainedModel(string sourceFileName, string targetFileName, Type method)
        {

            Type[] Types1 = { typeof(TrainPoint) };
            XmlSerializer serializer1 = new XmlSerializer(typeof(TrainPointsList), Types1);
            TrainPointsList trainPoints = null;
            using (var fs = new FileStream(sourceFileName, FileMode.Open))
            {
                //try { // TODO: write file not exist, not deserialize
                trainPoints = (TrainPointsList)serializer1.Deserialize(fs);
            }

            
            var model = getTrainedModel(trainPoints, method);

            GoodSerializer.saveToFile(targetFileName, model);

            //Type[] Types2 = { typeof(TrainedModelElement) };
            //XmlSerializer serializer2 = new XmlSerializer(typeof(TrainedModel), Types2);
            //using (var fs = new FileStream(targetFileName, FileMode.Create))
            //{
            //    try
            //    {
            //        serializer2.Serialize(fs, model);
            //    }
            //    catch
            //    { }
            //}
            //model = null;



        }

        static Dictionary<string, ModelParent> _cached_model;
        static Dictionary<string, ModelParent> cached_model
        {
            get
            {
                if (_cached_model == null)
                {
                    _cached_model = new Dictionary<string, ModelParent>();
                }
                return _cached_model;
            }
        }
        static public int getAnswerForClassByModel(string sourceFile, string solutionPath, string projectToPick, string className)
        {
            ModelParent model = null;
            if (cached_model.Keys.Contains(sourceFile))
                model = cached_model[sourceFile];
            else
            {

                //Type[] Types2 = { typeof(TrainedModelElement) };
                //XmlSerializer serializer2 = new XmlSerializer(typeof(TrainedModel), Types2);
                //using (var fs = new FileStream(sourceFile, FileMode.Open))
                //{
                //    try
                //    {
                //        model = (TrainedModel)serializer2.Deserialize(fs);
                //    }
                //    catch
                //    { }
                //}
                GoodSerializer.loadFromFile(sourceFile, out model);

                cached_model[sourceFile] = model;
            }

            Compilation compilation = Metr.RoslynAPI.GetCompilation(solutionPath, projectToPick);
            var point = TrainPoint.getClassRequestAnswerPoint(Metr.RoslynAPI.GetSolution(solutionPath), compilation, className);
            if (point == null) return -1;// if (className is namespace name)
            else return (int)Math.Round(model.Apply(point));
        }



        static public ModelParent getTrainedModel(TrainPointsList trainPoints, Type method)
        {
            if (method == typeof(ModelParent))
                return ModelParent.getModel(trainPoints);

            if (method == typeof(LeastSquaresModel))
                return LeastSquaresModel.getModel(trainPoints);
           
           return null;
        }




        static public void RunMathFunction()
        {
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

    class TrainTest
    {
        static public void Run()
        {
            // Train.toTrainPoints(@"C:\temp2\Another-Metric.xml", "");

            Train.RunMathFunction();

            //Train.toTrainedModel("C:\\temp2\\Another-Metric - points.xml", "C:\\temp2\\test.xml",Train.ModelToTrainMethod.LeastSquaresMethod);

            //var ttt = new TrainPoint(new List<int> { 1, 2, 3, 4, 5 });
            //var myList = ttt.ToList();

        }
    }
}
