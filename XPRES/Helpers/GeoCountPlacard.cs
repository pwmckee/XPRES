using System;
using System.Collections.Generic;
using System.Linq;
using XPRES.DAL;

namespace XPRES.Helpers
{
    public class GeoCountPlacard
    {
        private XpresEntities xps;
        private List<int?> counts = new List<int?>();
        public int Counts = 0;

        public int GetCounts(string Status)
        {
            try
            {
                xps = new XpresEntities();
                counts = (from a in xps.CountSchedules
                          where a.CountStatus == Status
                          select a.CountID).Distinct().ToList();

                Counts = counts.Count;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error retrieving Geo Count info: " + ex.Message);
                return 0;
            }
            return Counts;
        }
    }
}