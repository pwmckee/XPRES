using System;
using System.Linq;

namespace XPRES.Helpers
{
    public class XpsDates
    {
        public DateTime Yesterday = DateTime.Today.Date.AddDays(-1);
        public DateTime Tomorrow = DateTime.Today.Date.AddDays(1);
        public DateTime OldestDate = Convert.ToDateTime("1/1/2012");
        public int CurrentQtr = (DateTime.Today.Month - 1) / 3 + 1;
        public DateTime ValidDate = new DateTime();
        public DateTime StartWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek));

        public DateTime GetWeekStart(DateTime testDate)
        {
            StartWeek = testDate.AddDays(-1 * (int)(testDate.DayOfWeek));
            return StartWeek;
        }

        public DateTime CheckDateEntry(string testDate, string Default)
        {
            ValidDate = new DateTime();
            if (!string.IsNullOrEmpty(testDate))
            {
                try
                {
                    DateTime.TryParse(testDate, out ValidDate);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show(@"Invalid Date Entered");
                    return new DateTime();
                }
            }
            else
            {
                ValidDate = Convert.ToDateTime(Default);
            }

            return ValidDate;
        }
    }
}