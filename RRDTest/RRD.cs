using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRDTest
{
    class RRD
    {
        private string _filePath = @"C:\Program Files (x86)\RRDtool\rrdtool.exe";
        

        public RRD() {
            
            if (!File.Exists(_filePath)) throw new Exception("File does not exist");
        }

        public void StartProcess(string arguments) {
            Process rrdProc = new Process();
            rrdProc.StartInfo = new ProcessStartInfo
            {
                FileName = _filePath,
                Arguments = arguments
            };
            rrdProc.Start();
        }
        public void CreateRRD(CreationParameters crParams) {
            string arguments = "create "+crParams.FileName;
            if (crParams.Start != 0)
            {
                arguments = String.Concat(arguments, " --start "+crParams.Start);
            }
            if (crParams.DSs.Count != 0)
            {
                
                foreach (DS ds in crParams.DSs)
                {
                    string dsType = "";
                    switch (ds.Type)
                    {
                        case DSTypes.GAUGE: dsType = "GAUGE"; break;
                        case DSTypes.ABSOLUTE: dsType = "ABSOLUTE"; break;
                        case DSTypes.COUNTER: dsType = "COUNTER"; break;
                        case DSTypes.DCOUNTER: dsType = "DCOUNTER"; break;
                        case DSTypes.DDERIVE: dsType = "DDERIVE"; break;
                        case DSTypes.DERIVE: dsType = "DERIVE"; break;
                        
                    }
                    arguments = arguments += " DS:" + ds.Name + ":" + dsType + ":" + ds.Heartbeat + ":" + ds.Min + ":" + ds.Max;
                }
                foreach (RRA rra in crParams.RRAs)
                {
                    string cfType = "";
                    switch (rra.CF)
                    {
                        case CFTypes.AVERAGE: cfType = "AVERAGE"; break;
                        case CFTypes.LAST: cfType = "LAST"; break;
                        case CFTypes.MAX: cfType = "MAX"; break;
                        case CFTypes.MIN: cfType = "MIN"; break;
                    }
                    arguments = arguments += " RRA:" + cfType + ":" + rra.Xff + ":" + rra.Steps + ":" + rra.Rows;
                        
                }
            }
            StartProcess(arguments);
        }

        public void UpdateRRD(UpdateParameters upParams)
        {
            string arguments = "update " + upParams.FileName;
            foreach (InsertPair pair in upParams.Pairs)
            {
                arguments += " " + pair.TimeStamp + ":" + pair.Value;
            }
            StartProcess(arguments);
        }
        public void DrawGraph(GraphParameters gp)
        {
            string arguments = "graph " + gp.ImageFileName;
            arguments += " --start " + gp.TimeStart + " --end " + gp.TimeEnd;
            foreach (GraphItem gi in gp.Items)
            {
                arguments += " DEF:" + gi.VarName + "=" + gi.DEF.RRDName + ":" + gi.DEF.RRDField + ":" + gi.DEF.CFType;
                arguments += " LINE" + gi.Line.LineWidth + ":" + gi.VarName + gi.Line.Color;
            }

            StartProcess(arguments);
        }
       
    }
    public class CreationParameters
    {
        public string FileName { get; }
        public int Step { get; }
        public int Start { get; }
        public string Daemon { get; }
        public List<DS> DSs { get; set; }

        public List<RRA> RRAs { get; set; }

        public CreationParameters(string fileName, int step, int start, string daemon = "")
        {
            FileName = fileName;
            Step = step;
            Start = start;
            Daemon = daemon;
            DSs = new List<DS>();
            RRAs = new List<RRA>();
        }
    }
    public class DS
    {
        public string Name { get; }
        public DSTypes Type { get; }
        public int Heartbeat { get; }
        public string Min { get; }
        public string Max { get; }
        public DS(string name, DSTypes type, int heartbeat, string min, string max)
        {
            Name = name;
            Type = type;
            Heartbeat = heartbeat;
            Min = min;
            Max = max;
        }

    }
    public class RRA
    {
        public double Xff { get; }
        public int Steps { get; }
        public int Rows { get; }
        public CFTypes CF { get; }
        public RRA(CFTypes cf, double xff, int steps, int rows)
        {
            Xff = xff;
            Steps = steps;
            Rows = rows;
            CF = cf;
        }
    }
    public enum DSTypes {
        GAUGE,
        COUNTER,
        DERIVE,
        DCOUNTER,
        DDERIVE,
        ABSOLUTE
    }
    public enum CFTypes
    {
        AVERAGE,
        MIN,
        MAX,
        LAST
    }

    public class UpdateParameters
    {
        public string FileName { get; }
        public List<InsertPair> Pairs { get; }
        public UpdateParameters(string fileName){
            FileName = fileName;
            Pairs = new List<InsertPair>();
        }

}
    public class InsertPair
    {
        public string TimeStamp { get; }
        public string Value { get; }

        
        public InsertPair(int value, DateTime time)
        {
            Value = value.ToString();
            TimeStamp = TimeStampConverter.ConvertToTimestamp(time).ToString();
        }
        public InsertPair(int value, string time)
        {
            Value = value.ToString();
            TimeStamp = time;
        }
        public InsertPair(int value)
        {
            Value = value.ToString();
            TimeStamp = "N"; 
        }
    }
    public class GraphParameters
    {
        public string ImageFileName { get; }
        public string TimeStart { get; }
        public string TimeEnd { get; }
        public List<GraphItem> Items { get; }

        public GraphParameters(string image, string timeStart, string timeEnd)
        {
            ImageFileName = image;
            TimeStart = timeStart;
            TimeEnd = timeEnd;
            Items = new List<GraphItem>();
        }
        public GraphParameters(string image, DateTime timeStart, DateTime timeEnd)
        {
            ImageFileName = image;
            TimeStart = TimeStampConverter.ConvertToTimestamp(timeStart).ToString();
            TimeEnd = TimeStampConverter.ConvertToTimestamp(timeStart).ToString();
            Items = new List<GraphItem>();
        }
    }
    public class GraphItem
    {
        public string VarName { get; }
        public GraphDEF DEF { get; }
        public GraphLine Line { get; }
        public GraphItem(string varName, GraphDEF def, GraphLine line)
        {
            VarName = varName;
            DEF = def;
            Line = line;
        }
    }
    public class GraphDEF
    {

        public string RRDName { get; }
        public string RRDField { get; }
        public CFTypes CFType { get; }
        public GraphDEF(string rrdName, string rrdField, CFTypes cfType)
        {
            RRDName = rrdName;
            RRDField = rrdField;
            CFType = cfType;
        }
    }
    public class GraphLine
    {
        public int LineWidth { get; }
        public string Color { get; }
        public GraphLine(int lineWidth, string color)
        {
            LineWidth = lineWidth;
            Color = color;
        }
    }




    public class TimeStampConverter
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ConvertToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }
    }
}
