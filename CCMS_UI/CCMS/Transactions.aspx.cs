using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;


namespace CCMSUI.CCMS
{
    public partial class Transactions : System.Web.UI.Page
    {

        private void AddRow(DataTable dtGrid, string name, int counter1, int counter2)
        {
            DataRow drNewRow = dtGrid.NewRow();
            drNewRow[0] = name;
            drNewRow[1] = counter1;
            drNewRow[2] = counter2;
            drNewRow[3] = counter1 + counter2;
            dtGrid.Rows.Add(drNewRow);

        }
        //private void UpdateCounters(ref int counterEI, ref int counterEmirates, string columnName, DataTable dt)
        //{
        //    DataRow[] drEI = dt.Select(columnName+"=1");
        //    foreach (DataRow dr in drEI)
        //        counterEI += int.Parse(dr.ToString());

        //    DataRow[] drEmirates = dt.Select(columnName+"=0");
        //        counterEmirates += int.Parse(drEmirates.ToString());            
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid()
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select location,COUNT(parsed_transaction_id) trxn_count, SUM(amount) amount
from parsed_transaction inner join atm 
on atm.atm_id = parsed_transaction.atm_id where is_active=1
group by location";

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            GridView_Top10Machine.DataSource = dt;
            GridView_Top10Machine.DataBind();

            foreach (DataRow dr in dt.Rows)
            {
                Chart_Top10Locations.Series[0].Points.AddXY(dr["location"], dr["trxn_count"]);
                Chart_Top10Locations.Series[1].Points.AddXY(dr["location"], dr["amount"]);
            }
           
            GridView_Bottom10Locations.DataSource = dt;
            GridView_Bottom10Locations.DataBind();

            foreach (DataRow dr in dt.Rows)
            {
                Chart_Bottom10Locations.Series[0].Points.AddXY(dr["location"], dr["trxn_count"]);
                Chart_Bottom10Locations.Series[1].Points.AddXY(dr["location"], dr["amount"]);
            }



        }
    }
}