using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRDTest
{
    class Program
    {

        
        static void Main(string[] args)
        {
            string fileName = "test20.rrd";
            try
            {
                RRD myprocess = new RRD();
                CreationParameters cp = new CreationParameters(fileName, 0, 920804400);
                DS d = new DS("speed", DSTypes.COUNTER, 600, "U", "U");
                cp.DSs.Add(d);
                cp.RRAs.Add(new RRA(CFTypes.AVERAGE, 0.5, 1, 24));
                cp.RRAs.Add(new RRA(CFTypes.AVERAGE, 0.5, 6, 10));
                myprocess.CreateRRD(cp);
                UpdateParameters up = new UpdateParameters(fileName);
                up.Pairs.Add(new InsertPair(12363, "920805600"));
                up.Pairs.Add(new InsertPair(12363, "920805900"));
                up.Pairs.Add(new InsertPair(12373, "920806200"));
                myprocess.UpdateRRD(up);
                GraphParameters gp = new GraphParameters("speed.png", "920804400", "920808000");
                
                GraphDEF def = new GraphDEF(fileName, "speed", CFTypes.AVERAGE);
                GraphLine line = new GraphLine(2, "#0000FF");
                GraphItem gi = new GraphItem("myspeed", def, line);
                gp.Items.Add(gi);
                myprocess.DrawGraph(gp);



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
