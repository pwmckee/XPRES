using System;
using System.Data;
using System.Linq;
using XPRES.DAL;

namespace XPRES.Helpers
{
    public class InbDashInfo : IDisposable
    {
        private XpresEntities xps = new XpresEntities();
        public int Count = 0;

        public void Dispose()
        {
            xps.Dispose();
        }

        public void GetLtlInfo(string Type)
        {
            xps = new XpresEntities();
            Count = 0;
            DateTime _today = DateTime.Today.Date;

            try
            {
                var ltl = (from a in xps.RcvSchedules
                           where a.Appt >= _today && a.Appt < _today.AddDays(1)
                           select a);
                if (Type == "schedTrucks")
                {
                    Count = ltl.Count();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error retrieving LTL Schedule data: " + ex.Message);
                Dispose();
                return;
            }
            Dispose();
        }
    }
}