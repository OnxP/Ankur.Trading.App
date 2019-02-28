using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kx;
using static kx.kdb;

namespace Ankur.KDB.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            //kdb.ReceiveTimeout = 1000;

            //kdb c = new kdb("localhost", 5000); c.k(".u.sub[`trade;`MSFT.O`IBM.N]");
            //while (true)
            //{
            //    object l = c.k();
            //    O(n(at(l, 2)));
            //}

            kdb c = new kdb("localhost", 5000);

            kdb.e = System.Text.Encoding.UTF8;

            //O("Unicode " + c.k("`$\"c\"$0x52616e627920426ac3b6726b6c756e64204142"));

            //object[] x = new object[4];
            //x[0] = t();
            //x[1] = "xx";
            //x[2] = (double) 93.5;
            //x[3] = 300;
            //tm();
            //for (int i = 0; i < 1000; ++i) c.k("insert", "trade", x);
            //tm();

            Flip r = td(c.k("select sum price by sym from Prices"));
            O("cols: " + n(r.x));
            O("rows: " + n(r.y[0]));

            c.Close();

        }
    }
    
}
