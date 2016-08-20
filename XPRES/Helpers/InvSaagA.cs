using System;
using System.Collections.Generic;
using System.Data;

namespace XPRES.Helpers
{
    public class InvSaagA
    {
        public DataTable dtSaagA = new DataTable();

        public object CreateTable(DateTime WeekStart, DateTime WeekEnd)
        {
            dtSaagA.Columns.Add("Date");
            dtSaagA.Columns.Add("Goal");
            dtSaagA.Columns.Add("Actual");
            dtSaagA.Columns.Add("Errors");
            dtSaagA.Columns.Add("Accuracy");
            dtSaagA.Columns.Add("Comments");

            List<DateTime> _dates = new List<DateTime>();
            int _checkDay = 0;
            DateTime _entryDate = WeekStart;
            while (_entryDate <= WeekEnd)
            {
                _checkDay = (int)(_entryDate.DayOfWeek);
                if (_checkDay != 0 && _checkDay != 6)
                {
                    _dates.Add(_entryDate);
                }
                _entryDate = _entryDate.AddDays(1);
            }

            foreach (var d in _dates)
            {
                DataRow dr = dtSaagA.NewRow();
                dr[0] = d.ToString("MM/dd/yyyy");
                dtSaagA.Rows.Add(dr);
            }

            return dtSaagA;
        }
    }
}