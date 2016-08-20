using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using XPRES.DAL;

namespace XPRES.Helpers
{
    public class InbActivityCharts
    {
        public List<KeyValuePair<string, double>> ActsPerHour = new List<KeyValuePair<string, double>>();

        public void GetActivityChart(DateTime StartDate, DateTime EndDate, string Type)
        {
            XpresEntities xps = new XpresEntities();
            ActsPerHour = new List<KeyValuePair<string, double>>();
            double? _aph = 0;
            double _aphCount = 0;
            double _avg = 0;
            List<int?> aph = new List<int?>();
            List<DateTime> dateCnv = new List<DateTime>();

            try
            {
                var acts = (from a in xps.InboundActivities
                            where a.Finish >= StartDate && a.Finish <= EndDate && a.Type == Type
                            select a);

                foreach (var a in acts)
                {
                    dateCnv.Add((Convert.ToDateTime(a.Finish).Date));
                }

                dateCnv = dateCnv.Distinct().ToList();

                foreach (var d in dateCnv)
                {
                    foreach (var a in acts)
                    {
                        if (a.Finish >= d && a.Finish < d.AddDays(1))
                        {
                            _aph += a.LPH;
                            _aphCount++;
                        }
                    }
                    _avg = Convert.ToDouble(_aph) / _aphCount;
                    _aph = 0;
                    _aphCount = 0;
                    ActsPerHour.Add(new KeyValuePair<string, double>(d.ToShortDateString(), _avg));
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while retrieving Inbound Activity Data: " + ex.Message);
            }
        }
    }
}