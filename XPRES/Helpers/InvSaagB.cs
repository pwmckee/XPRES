using System.Data;

namespace XPRES.Helpers
{
    public class InvSaagB
    {
        public DataTable dtSaagB = new DataTable();

        public object CreateTable()
        {
            dtSaagB.Columns.Add("Period");
            dtSaagB.Columns.Add("NetGoal");
            dtSaagB.Columns.Add("NetAct");
            dtSaagB.Columns.Add("AbsGoal");
            dtSaagB.Columns.Add("AbsAct");
            dtSaagB.Columns.Add("LocGoal");
            dtSaagB.Columns.Add("LocAct");
            dtSaagB.Columns.Add("Comments");
            for (int i = 0; i < 3; i++)
            {
                DataRow dr = dtSaagB.Rows.Add();
            }

            return dtSaagB;
        }

        public object FillRows(string Range, double Net, double Abs, double Loc)
        {
            int _row = 0;
            DataRow w = dtSaagB.Rows[0];
            w[0] = "WTD";
            DataRow m = dtSaagB.Rows[1];
            m[0] = "MTD";
            DataRow y = dtSaagB.Rows[2];
            y[0] = "YTD";

            if (Range == "WTD")
                _row = 0;
            if (Range == "MTD")
                _row = 1;
            if (Range == "YTD")
                _row = 2;

            dtSaagB.Rows[_row][1] = 99;
            dtSaagB.Rows[_row][2] = Net;
            dtSaagB.Rows[_row][3] = 99;
            dtSaagB.Rows[_row][4] = Abs;
            dtSaagB.Rows[_row][5] = 99;
            dtSaagB.Rows[_row][6] = Loc;

            return dtSaagB;
        }
    }
}