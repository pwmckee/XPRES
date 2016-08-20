using System;
using System.Collections.Generic;
using System.Linq;
using XPRES.DAL;

namespace XPRES.Helpers
{
    public class AdjValueChart : IDisposable
    {
        private XpresEntities xps = new XpresEntities();
        public List<KeyValuePair<string, double?>> NetTotals = new List<KeyValuePair<string, double?>>();
        public List<KeyValuePair<string, double?>> WriteOffTotals = new List<KeyValuePair<string, double?>>();
        public List<KeyValuePair<string, double?>> WriteOnTotals = new List<KeyValuePair<string, double?>>();

        public void Dispose()
        {
            xps.Dispose();
        }

        public void GetAdjValues(DateTime StartDate, DateTime EndDate)
        {
            NetTotals = new List<KeyValuePair<string, double?>>();
            WriteOffTotals = new List<KeyValuePair<string, double?>>();
            WriteOnTotals = new List<KeyValuePair<string, double?>>();
            xps = new XpresEntities();
            double? _net = 0;
            List<MetricsAdj> _adjs = new List<MetricsAdj>();

            double? _totOff = 0;
            double? _totOn = 0;

            try
            {
                var adj = (from a in xps.MetricsAdjs
                           where a.Date >= StartDate && a.Date <= EndDate
                           select a);

                _net = adj.Sum(a => a.AdjValue);

                IEnumerable<MetricsAdj> _off = adj.Where(a => a.AdjValue < 0);
                _totOff = _off.Sum(a => a.AdjValue);

                IEnumerable<MetricsAdj> _on = adj.Where(a => a.AdjValue > 0);
                _totOn = _on.Sum(a => a.AdjValue);
                if (_net == null && _totOff == 0 && _totOn == 0)
                {
                    return;
                }
                NetTotals.Add(new KeyValuePair<string, double?>("Net", _net));
                WriteOffTotals.Add(new KeyValuePair<string, double?>("Write Offs", _totOff));
                WriteOnTotals.Add(new KeyValuePair<string, double?>("Write Ons", _totOn));
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error retrieving adjustment values: " + ex.Message);
                Dispose();
                return;
            }

            Dispose();
        }
    }
}