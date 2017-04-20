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
            

            try
            {
                RRD myprocess = new RRD();
                CreationParameters cp = new CreationParameters("test18.rrd", 0, 920804400);
                DS d = new DS("speed", DSTypes.COUNTER, 600, "U", "U");
                cp.DSs.Add(d);
                cp.RRAs.Add(new RRA(CFTypes.AVERAGE, 0.5, 1, 24));
                myprocess.CreateRRD(cp);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
