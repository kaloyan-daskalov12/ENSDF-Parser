using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENSDF_Parser
{
    public class Value
    {
        public double Val { get; set; }
        double units;
        public double DVal
        {
            get => dVal;
            internal set
            {
                dVal = value * units;
                dVal = Math.Round(dVal, (int)Math.Abs(Math.Log10(units)));
            }
        }
        double dVal;

        public void SetValue(string val)
        {
            units = LeastSignificantUnit(val);
            Val = double.Parse(val);
        }

        static double LeastSignificantUnit(string s)
        {
            int dot = s.IndexOf('.');

            if (dot < 0)
                return 1.0;

            int decimals = s.Length - dot - 1;

            return Math.Pow(10, -decimals);
        }
    }
}
