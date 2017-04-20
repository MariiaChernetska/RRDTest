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
            this.StartProcess(arguments);
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
}
