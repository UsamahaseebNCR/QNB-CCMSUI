using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

using System.Data;
using System.Text.RegularExpressions;
using Avanza.iSuite.UI;
namespace CCMSUI
{
    public partial class DisplayData : BasePage
    {
        Regex regex = new Regex(@"(?<FromDate>\d{2}\/\d{2}/\d{4})(-(?<ToDate>\d{2}\/\d{2}/\d{4}))?");
        private DateTime GetToDate(string heading)
        {
            Match match = regex.Match(heading);
            string val = string.Empty;
            if (match.Success)
            {
                if (match.Groups["ToDate"].Success)
                    val = match.Groups["ToDate"].Value;
                else if (match.Groups["FromDate"].Success)
                    val = match.Groups["FromDate"].Value;
            }
            return DateTime.ParseExact(val, "dd/MM/yyyy", null);
        }

        private DateTime GetFromDate(string heading)
        {
            Match match = regex.Match(heading);
            string val = string.Empty;
            if (match.Success)
            {
                if (match.Groups["FromDate"].Success)
                    val = match.Groups["FromDate"].Value;
            }
            return DateTime.ParseExact(val, "dd/MM/yyyy", null);
        }
        private long GetUpperBound(string heading)
        {
            return 0;
        }
        private long GetLowerBound(string heading)
        {
            return 0;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            string receivedString = (string)Request.QueryString["data"];

            SqlConnection conn = ConnectionFactory.GetNewConnection();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
            cmd.Parameters[0].Value = GetFromDate(receivedString);
            cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
            cmd.Parameters[1].Value = GetToDate(receivedString);
            switch ((string)Request.QueryString["gid"])
            {
                case "1":
                    cmd.CommandText = "TransactionVersusHourDetail";
                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];
                    break;
                case "20":
                    cmd.CommandText = "ATMTransactionsDetail";
                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[3].Value = Session[SessionVars.SelectedATMs.ToString()];
                    cmd.Parameters.Add("@LowerBound", SqlDbType.Int);
                    cmd.Parameters[4].Value = GetLowerBound(receivedString);
                    cmd.Parameters.Add("@UpperBound", SqlDbType.Int);
                    cmd.Parameters[5].Value = GetUpperBound(receivedString);
                    break;
                case "21":
                    cmd.CommandText = "RegionWiseTransactionDetail";
                    cmd.Parameters.Add("@OrganizationId", SqlDbType.Int);
                    cmd.Parameters[2].Value = 1;
                    break;
                case "23":
                    cmd.CommandText = "RegionWiseReplenishmentDetail";
                    cmd.Parameters.Add("@OrganizationId", SqlDbType.Int);
                    cmd.Parameters[2].Value = 1;
                    break;
                case "22":
                    cmd.CommandText = "ReplenishmentsDetail";
                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];
                    break;
                case "24":
                    cmd.CommandText = "ATMUtilizationDetail";
                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];
                    break;
                case "25":
                    cmd.CommandText = "ATMUtilizationByRegionDetail";
                    break;
            }
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            GridView_Data.DataSource = dt;
            GridView_Data.DataBind();



        }
    }
}