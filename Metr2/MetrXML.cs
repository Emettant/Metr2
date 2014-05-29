using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;





namespace MetrXML
{
    using System.Xml.Serialization;
    using System.IO;
    using MetrLearn;
    public abstract class TMySerializable
    {
        abstract public void SerializeTo(string fileName);
        static public TMySerializable DeserializeFrom(string fileName, Type type)
        {
            object model;

            XmlSerializer serializer = new XmlSerializer(type);
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    model = serializer.Deserialize(fs);
                }
                catch
                {
                    model = null;
                }
            }
            return (TMySerializable)model;
        }


    }

    [XmlType("EstimationOfElement")]
    public class EstimationOfElement
    {
        [XmlAttribute("Solution")]
        public string Solution { get; set; }

        [XmlAttribute("Project")]
        public string Project { get; set; }

        [XmlAttribute("FullName")]
        public string FullName { get; set; }

        [XmlAttribute("Estimation")]
        public int Estimation { get; set; }

        public EstimationOfElement() { }
        public EstimationOfElement(string solution, string project, string name, int est)
        {
            this.Solution = solution;
            this.Project = project;
            this.FullName = name;
            this.Estimation = est;
        }
    }

    [XmlRoot("EstimationList")]
    [XmlInclude(typeof(EstimationOfElement))] // include type class EstimationOfElement
    public class EstimationList : TMySerializable
    {
        [XmlArray("EstimationArray")]
        [XmlArrayItem("EstimationObject")]
        public List<EstimationOfElement> Estimations = new List<EstimationOfElement>();

        [XmlElement("ListName")]
        public string ListName { get; set; }

        public EstimationList() { ListName = "EstimationList"; }

        public EstimationList(IEnumerable<EstimationOfElement> list) {
            Estimations = list.ToList();
        }

        public void AddEstimation(EstimationOfElement estimation)
        {
            Estimations.Add(estimation);
        }

        override public void SerializeTo(string fileName) {
            // Serialize 
            Type[] estimationTypes = { typeof(EstimationOfElement) };
            XmlSerializer serializer = new XmlSerializer(typeof(EstimationList), estimationTypes);
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                try
                {
                    serializer.Serialize(fs, this);
                }
                catch
                { }
            }
        }
    
    }

    public class EstimationListTest
    {
        static public void Run()
        {

            var estimList = new EstimationList();
            estimList.ListName = "TestEstims";

            estimList.AddEstimation(new EstimationOfElement("Metr2", "Metr2", "MetrExamples.D", 33));
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

    public class GoodSerializer
    {

        public static Type loadFromFile(string fileName, out ModelParent model)
        {
            model = (ModelParent)TMySerializable.DeserializeFrom(fileName, typeof(ModelParent));
            if (model != null && ((ModelParent)model).ApproachName == typeof(ModelParent).ToString()) return typeof(ModelParent);

            model = (LeastSquaresModel)TMySerializable.DeserializeFrom(fileName, typeof(LeastSquaresModel));
            if (model != null && ((ModelParent)model).ApproachName == typeof(LeastSquaresModel).ToString()) return typeof(LeastSquaresModel);
            return null;
        }

        public static EstimationList loadFromFileEstimationList(string fileName)
        {
            return (EstimationList)TMySerializable.DeserializeFrom(fileName, typeof(EstimationList));
        }

        public static TrainPointsList loadFromFileTrainPointsList(string fileName)
        {
            return (TrainPointsList)TMySerializable.DeserializeFrom(fileName, typeof(TrainPointsList));
        }

        public static void saveToFile(string fileName, ModelParent model)
        {
            model.SerializeTo(fileName);
        }

    }
    class GoodSerializerTest
    {
        public static void Run()
        {
            //MetrLearn.GoodSerializer.saveToFile(@"C:\temp3\model.xml", new MetrLearn.ModelParent());//, new double[] { 2, 3, 1 }));

            GoodSerializer.saveToFile(@"C:\temp3\model.xml", new LeastSquaresModel(new double[] { 2, 3, 1 }));
            ModelParent model;
            Console.WriteLine(GoodSerializer.loadFromFile(@"C:\temp3\model.xml", out model).ToString());
            int t = 1;
        }
    }

}
namespace MetrLearn
{
    using System.Xml.Serialization;
    using System.IO;
    using MetrXML;
    using System.Reflection;
    using MetrMath.Adapter;

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
        public string not_metric_className { get; set; }

        [XmlIgnoreAttribute]
        public List<PropertyInfo> not_metric_properties_metrics_list { get; private set; }
        List<int> _cached_list;

        [XmlIgnoreAttribute]
        List<int> cached_list
        {
            get
            {
                if (_cached_list == null)
                {
                    var propertiesInfos = not_metric_properties_metrics_list;
                    _cached_list = propertiesInfos.Select(x => (Int32)x.GetValue(this)).ToList();
                }
                return _cached_list;
            }
        }
        private void InitComponents() {
            not_metric_properties_metrics_list = this.GetType().GetProperties().Where(x => !x.Name.Contains("not_metric")).ToList();
        }
        public TrainPoint() {
            InitComponents();
        }
        public TrainPoint(List<int> list, string _className)
        {
            InitComponents();

            int k = 0;
            foreach (var pro in not_metric_properties_metrics_list)
            {
                pro.SetValue(this, list[k++]);
            }

            //RS = list[0];
            //DIT = list[1];
            //NOC = list[2];
            //CBO = list[3];

            //Answer = list.Last();

            not_metric_className = _className;
        }

        public List<int> ToList()
        {
            return cached_list;
        }

        public List<int> getRequest()
        {
            return cached_list.GetRange(0, cached_list.Count - 1);
        }

        public int getAnswer()
        {
            return cached_list.Last();
        }

        [XmlIgnoreAttribute]
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
    public class TrainPointsList : MetrXML.TMySerializable
    {
        [XmlArray("TrainPointsList")]
        [XmlArrayItem("TrainPointsListItem")]
        public List<TrainPoint> Points = new List<TrainPoint>();


        public TrainPointsList() { }



        public void AddPoint(TrainPoint point)
        {
            Points.Add(point);
        }
        public override void SerializeTo(string fileName)
        {
            // throw new NotImplementedException();
            Type[] types = { };
            XmlSerializer serializer = new XmlSerializer(typeof(TrainPointsList));
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(fs, this);
                //try
                //{

                //    serializer.Serialize(fs, this);
                //}
                //catch
                //{ }
            }
        }
        /// <summary>
        /// Unpack_TrainData.nb
        /// </summary>
        [XmlIgnore]
        public static string load_UnpackTrainPointList =
@"UnpackTrainPointList[path_] := Module[{
    array,
    TrainDataArrows,
    
    },
   array = Import[path];
   array[[2, 3, 1, 3, All, 2]] // OutputForm;
   TrainDataArrows = array[[2, 3, 1, 3, All, 2]];
   TrainDataArrows[[All, 1 ;; -2, 2]]
   ];";
        /// <summary>
        /// Normalization_0_1.nb
        /// </summary>
        [XmlIgnore]
        public static string load_NormalizeMaxMin =
@"NormalizeMaxMin [Data_] := 
  Module[{NormalizationCoefs = {}, MinCoefs = {}},
   Do[AppendTo[NormalizationCoefs, 
     1/(Max[Data[[All, kkk]]] - Min[Data[[All, kkk]]])];
    AppendTo[MinCoefs, Min[Data[[All, kkk]]]];
    , {kkk, Length[Data]}];
   (# - MinCoefs)*NormalizationCoefs & /@ Data
   ];";
        public static string BuildUnpackTrainPointList(string filePath)
        {
            Mathematica.Calc(load_UnpackTrainPointList + load_NormalizeMaxMin);
            return "NormalizeMaxMin[UnpackTrainPointList[" + filePath + "]]";
        }
    }
   


    [XmlType("TrainedModelElement")]
    public class TrainedModelElement
    {
        [XmlAttribute("element")]
        public double Val { get; set; }

        public TrainedModelElement() { Val = -1; }
        public TrainedModelElement(double _val) { Val = _val; }
    }

    //[XmlRoot("TrainedModel")]
    //[XmlInclude(typeof(TrainedModelElement))]
    //public class TrainedModel
    //{
    //    [XmlElement("MethodName")]
    //    public string methodName { get; set; }

    //    [XmlArray("TrainedModelList")]
    //    [XmlArrayItem("TrainedModelListItem")]
    //    public List<TrainedModelElement> Items = new List<TrainedModelElement>();

    //    public TrainedModel() { }
    //    public TrainedModel(List<double> _list, string _methodName)
    //    {
    //        Items = _list.Select(x => new TrainedModelElement(x)).ToList();
    //        methodName = _methodName;
    //    }

    //    public List<double> ToList()
    //    {
    //        return Items.Select(x => x.Val).ToList();
    //    }

    //}


    //TODO: ModelParent == TMySerializable

    [XmlRoot("Model")]
    public class ModelParent : TMySerializable
    {
        [XmlElement("ApproachName")]
        public string ApproachName { get; set; }
        private ModelParent(string name) { ApproachName = name; }
        public ModelParent() { ApproachName = this.GetType().ToString(); }

        static private List<Type> _childs_and_self_list;
        static public List<Type> ApproachList
        {
            get {
                if (_childs_and_self_list == null)
                    _childs_and_self_list = new List<Type> { typeof(ModelParent), typeof(LeastSquaresModel), typeof(KNNModel)};
                return _childs_and_self_list;
            }
        }

        override public void SerializeTo(string fileName)
        {
            //typeof(TrainedModelElement), typeof(TrainPoint)
            Type[] types = { };
            XmlSerializer serializer = new XmlSerializer(typeof(ModelParent));
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                try
                {
                    
                    serializer.Serialize(fs, this);
                }
                catch
                { }
            }
        }
        static public ModelParent getModel(TrainPointsList list) {
            throw new NotImplementedException();
        }
        static public ModelParent getModel(string sourceFile)
        {
            throw new NotImplementedException();
        }

        virtual public double Apply(TrainPoint point) {
            throw new NotImplementedException();
        }


    }

    [XmlRoot("MyLeastSquaresModel")]
    [XmlInclude(typeof(TrainedModelElement))]
    public class LeastSquaresModel : ModelParent
    {
        [XmlArray("LeastSquaresModelList")]
        [XmlArrayItem("Item")]
        public List<TrainedModelElement> Items = new List<TrainedModelElement>();
        public LeastSquaresModel(IEnumerable<double> list) : base()
        {
            Items = list.Select(x => new TrainedModelElement(x)).ToList();
        }
        public LeastSquaresModel() : base() { }
        //public static new LeastSquaresModel DeserializeFrom(string fileName)
        //{
        //    LeastSquaresModel model;
        //    XmlSerializer serializer = new XmlSerializer(typeof(LeastSquaresModel));
        //    using (var fs = new FileStream(fileName, FileMode.Open))
        //    {
        //        try
        //        {
        //            model = (LeastSquaresModel)serializer.Deserialize(fs);
        //        }
        //        catch
        //        {
        //            model = null;
        //        }
        //    }
        //    return model;
        //}
        public override void SerializeTo(string fileName)
        {
            //
            //Type[] types = { };
            XmlSerializer serializer = new XmlSerializer(typeof(LeastSquaresModel));
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                try
                {
                    serializer.Serialize(fs, this);
                }
                catch
                { }
            }
        }
        override public double Apply(TrainPoint point)
        {
            var coefsString = ListToString(point.getRequest());
            var modelString = ListToString(Items.Select(x => x.Val));

            Mathematica.Calc(coefsString + "." + modelString);

            var res = Mathematica.Result.GetDouble();
            //Console.WriteLine(res);
            return res;
        }

        static public new LeastSquaresModel getModel(TrainPointsList trainPoints)
        {
            {
                var ReAn = trainPoints.Points.Select(x => x.ToList());
                var Request = ReAn.Select(x => x.GetRange(0, x.Count - 1)).ToList();
                var Answer = ReAn.Select(x => x.Last()).ToList();

                MetrMath.Model.Build(Request, Answer);
                return new LeastSquaresModel(MetrMath.Model.model);
            }
        }

        static public new LeastSquaresModel getModel(string sourceFile) {
            return getModel(GoodSerializer.loadFromFileTrainPointsList(sourceFile));
        }


    }

   

    [XmlRoot("KNN_Model")]
    //[XmlInclude(typeof(TrainPoint))]
    public class KNNModel : ModelParent
    {
        //[XmlElement("MethodName")]
        //public int Distance { get; set; }

        [XmlElement("kNeighbour")]
        public int kNeighbour { get; set; }

        [XmlElement("kDistance")]
        public double kDistance { get; set; }

        [XmlElement("pathToTrainData")]
        public string pathToTrainData { get; set; }
        //[XmlArray("KNNModelList")]
        //[XmlArrayItem("KNNModelListItem")]
        //public List<TrainPoint> Points = new List<TrainPoint>();

        public KNNModel() : base() { }
        public KNNModel(int kN, double kD, string path) : base() {
            kNeighbour = kN;
            kDistance = kD;
            pathToTrainData = path;
        }

        override public void SerializeTo(string fileName)
        {
            //
            //Type[] types = { };
            XmlSerializer serializer = new XmlSerializer(typeof(KNNModel));
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                try
                {
                    serializer.Serialize(fs, this);
                }
                catch
                { }
            }
        }
        override public double Apply(TrainPoint point)
        {
            throw new NotImplementedException();
        }

        static new public KNNModel getModel(string sourceFile)
        {
            var trainPointsList = GoodSerializer.loadFromFileTrainPointsList(sourceFile);
            Mathematica.Calc(
                    //TrainPointsList.load_UnpackTrainPointList + 
                    KNNModel.load_SetEuclideanDistance +
                KNNModel.load_GetApplyModel +
                "GetLearnedModelKNN[" + TrainPointsList.BuildUnpackTrainPointList(sourceFile) + ",20]"
                );
            var res = Mathematica.Result.GetDoubleArray();
            return new KNNModel((int)Math.Round(res[0]), res[1], sourceFile);
        }

        /// <summary>
        /// from ModelKNN.nb
        /// </summary>
        [XmlIgnore]
        static public string load_SetEuclideanDistance =
@"CustomMeasureDistance := EuclideanDistance;";

        /// <summary>
        /// from ModelKNN.nb
        /// </summary>
        [XmlIgnore]
        static public string load_GetApplyModel =
@"getRequest := #[[All, 1 ;; -2]] &;
getAnswer := #[[All, -1 ;; -1]] &;

GetDistanceNeighborAnswerList[BasePartRequest_, BasePartAnswer_, 
   Request_] := Module[{},
   DistanceNeighborList = 
    CustomMeasureDistance[#, Request] & /@ BasePartRequest;
   {DistanceNeighborList, Flatten[BasePartAnswer
      ]}\[Transpose]
   ];
AggregateAnswersNeighborList[SortedDistanceNeighborAnswerList_] := 
  Module[{},
   (*#&*)
   Mean[#] &
    (*Median[#]&*)
    
    /@ Take[
     FoldList[
      Append[#1, #2[[2]]] &,
      {},
      SortedDistanceNeighborAnswerList
      ], {2, -1}]
   ];

GetLearnedModelKNN[givenData_, precision_] := 
  Module[{RepeatTimes = 100,
    globalGottedK = {},
    globalGottedDistances = {},
    rndPerm,
    BasePercent = 0.6,
    BaseCount,
    BasePart,
    TryPart,
    BasePartRequest,
    BasePartAnswer,
    TryPartRequest,
    TryPartAnswer,
    TryGottedK,
    DistanceNeighborList,
    DistanceNeighborAnswerList,
    NeighborErrors,
    currentK,
    FirstArgMin
    
    },
   
   (*from KNN.nb file*)
   FirstArgMin[inList_] := Module[{MinEl = -1, ret = -1, i, list},
     list = Flatten[inList];
     MinEl = Min[list];
     For[i = Length[list], i >= 1, --i, If[list[[i]] == MinEl, ret = i]
      ];
     ret
     ];
   
   
   Do[
    rndPerm = 
     PermutationList[RandomPermutation[Length[givenData]], 
      Length[givenData]];
    BaseCount = IntegerPart[BasePercent*Length[rndPerm]];
    BasePart = Table[givenData[[rndPerm[[i]]]], {i, 1, BaseCount}];
    TryPart =  
     Table[givenData[[rndPerm[[i]]]], {i, BaseCount + 1, 
       Length[givenData]}];
    BasePartRequest = getRequest[BasePart];
    BasePartAnswer = getAnswer[BasePart];
    TryPartRequest = getRequest[TryPart];
    TryPartAnswer = getAnswer[TryPart];
    
    TryGottedK = {};
    Do[
      DistanceNeighborAnswerList = 
       Sort[GetDistanceNeighborAnswerList[BasePartRequest, 
         BasePartAnswer, TryPartRequest[[i]]]];
      
      NeighborErrors =
       Abs[TryPartAnswer[[i]] - #] & /@ 
        AggregateAnswersNeighborList[DistanceNeighborAnswerList];
      currentK = FirstArgMin[NeighborErrors];
      
      AppendTo[
       TryGottedK, {currentK, 
        DistanceNeighborAnswerList[[currentK, 1]] }];
      ,
      {i, Length[TryPartRequest]}
      ]
     AppendTo[globalGottedK, TryGottedK[[All, 1]]];
    AppendTo[globalGottedDistances, TryGottedK[[All, 2]]];
    ,
    {j, RepeatTimes}
    ];
   {N[Mean[Flatten[globalGottedK]], precision],
    N[Mean[Flatten[globalGottedDistances]], precision]}
   
   ];

ApplyLearnedModelKNN[TrainData_, Request_, Model_] := Module[{
    ModelDistance,
    myEPSILON = 0.0000001,
    SortedDistanceNeighborAnswerList,
    ModelK = -1
    },
   SortedDistanceNeighborAnswerList = 
    Sort[GetDistanceNeighborAnswerList[getRequest[TrainData], 
      getAnswer[TrainData], Request]];
   
   
   (*ModelK = Round[Model\[LeftDoubleBracket]1\[RightDoubleBracket]];*) 
   ModelDistance = Model[[2]]*(1 + myEPSILON);
   Do[
    If[ModelDistance >= SortedDistanceNeighborAnswerList[[i, 1]] , 
      ModelK = i; Break;];
    ,
    {i, Length[SortedDistanceNeighborAnswerList]}
    ];
   If[ModelK >= 2, ModelK = ModelK - 1];
   
   (*{ModelK,Model\[LeftDoubleBracket]1\[RightDoubleBracket]}*)
   
   If[ModelK == -1, 
    -1,
    AggregateAnswersNeighborList[SortedDistanceNeighborAnswerList][[
     ModelK]]
    ]
   ];";

    }

    

}