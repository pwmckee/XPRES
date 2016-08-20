using System;

namespace XPRES.Helpers
{
    public class GetPercentage
    {
        public double PerNet = 0;
        public double PerAbs = 0;
        public double PerLoc = 0;

        public double CalcNet(double Total, double Errors)
        {
            if (Total != 0)
            {
                if (Errors < 0)
                {
                    PerNet = ((Total + Errors) / Total) * 100;
                }
                else
                {
                    PerNet = ((Total - Errors) / Total) * 100;
                }
                PerNet = Math.Round(Convert.ToDouble(PerNet), 2);
            }

            return PerNet;
        }

        public double CalcAbs(double Total, double Errors)
        {
            if (Total != 0)
            {
                PerAbs = ((Total - Errors) / Total) * 100;
                PerAbs = Math.Round(Convert.ToDouble(PerAbs), 2);
            }
            return PerAbs;
        }

        public double CalcLoc(double Total, double Errors)
        {
            if (Total != 0)
            {
                PerLoc = ((Total - Errors) / Total) * 100;
                PerLoc = Math.Round(Convert.ToDouble(PerLoc), 2);
            }
            return PerLoc;
        }
    }
}