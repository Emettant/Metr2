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

}
namespace MetrLearn
{
    using System.Xml.Serialization;
    using System.IO;
    using MetrXML;
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

        public List<PropertyInfo> properties_metrics_list { get; private set; }

        private void InitComponents() {
            properties_metrics_list = this.GetType().GetProperties().Where(x => x.Name != "className").ToList();
        }
        public TrainPoint() {
            InitComponents();
        }
        public TrainPoint(List<int> list, string _className)
        {
            InitComponents();
            int k = 0;
            foreach (var pro in this.GetType().GetProperties())
            {
                pro.SetValue(this, list[k++]);
            }
            //RS = list[0];
            //DIT = list[1];
            //NOC = list[2];
            //CBO = list[3];
            
            //Answer = list.Last();

            className = _className;
        }

        List<int> _cached_list;
        List<int> cached_list
        {
            get
            {
                if (_cached_list == null)
                {
                    var propertiesInfos = properties_metrics_list;
                    _cached_list = propertiesInfos.Select(x => (Int32)x.GetValue(this)).ToList();
                }
                return _cached_list;
            }
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
    public class TrainedModelElement
    {
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
        public TrainedModel(List<double> _list, string _methodName)
        {
            Items = _list.Select(x => new TrainedModelElement(x)).ToList();
            methodName = _methodName;
        }

        public List<double> ToList()
        {
            return Items.Select(x => x.Val).ToList();
        }

    }


    public interface IMySerializable
    {
        void SerializeTo(string fileName);


    }

    [XmlRoot("Model")]
    public class ModelParent : IMySerializable
    {
        [XmlElement("ApproachName")]
        public string ApproachName { get; set; }
        public ModelParent(string name) { ApproachName = name; }
        public ModelParent() { }

        public static ModelParent DeserializeFrom(string fileName)
        {
            ModelParent model;
            XmlSerializer serializer = new XmlSerializer(typeof(ModelParent));
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    model = (ModelParent)serializer.Deserialize(fs);
                }
                catch
                {
                    model = null;
                }
            }
            return model;
        }
        public virtual void SerializeTo(string fileName)
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

    }

    [XmlRoot("MyLeastSquaresModel")]
    [XmlInclude(typeof(TrainedModelElement))]
    public class LeastSquaresModel : ModelParent
    {
        [XmlArray("LeastSquaresModelList")]
        [XmlArrayItem("LeastSquaresModelListItem")]
        public List<TrainedModelElement> Items = new List<TrainedModelElement>();
        public LeastSquaresModel(string name, IEnumerable<double> list) : base(name)
        {
            Items = list.Select(x => new TrainedModelElement(x)).ToList();
        }
        public LeastSquaresModel(string name) : base(name) { }
        public LeastSquaresModel() { }
        public static new LeastSquaresModel DeserializeFrom(string fileName)
        {
            LeastSquaresModel model;
            XmlSerializer serializer = new XmlSerializer(typeof(LeastSquaresModel));
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    model = (LeastSquaresModel)serializer.Deserialize(fs);
                }
                catch
                {
                    model = null;
                }
            }
            return model;
        }
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
    }

    [XmlInclude(typeof(TrainPoint))]
    public class KNNModel : ModelParent
    {
        [XmlElement("MethodName")]
        public int Distance { get; set; }

        [XmlElement("kNeighbour")]
        public int kNeighbour { get; set; }

        [XmlArray("KNNModelList")]
        [XmlArrayItem("KNNModelListItem")]
        public List<TrainPoint> Points = new List<TrainPoint>();

        public KNNModel(string name) : base(name) { }
    }

    class GoodSerializer
    {

        public static void loadFromFile(string fileName, out ModelParent model)
        {
            model = ModelParent.DeserializeFrom(fileName);
            if (model != null) return;
            model = LeastSquaresModel.DeserializeFrom(fileName);
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
            //MetrLearn.GoodSerializer.saveToFile(@"C:\temp3\model.xml", new MetrLearn.ModelParent("VitModel"));//, new double[] { 2, 3, 1 }));

            MetrLearn.GoodSerializer.saveToFile(@"C:\temp3\model.xml", new MetrLearn.LeastSquaresModel("VitModel", new double[] { 2, 3, 1 }));
            MetrLearn.ModelParent model;
            MetrLearn.GoodSerializer.loadFromFile(@"C:\temp3\model.xml", out model);
            int t = 1;
        }
    }

}