using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.CCMS
{
    public partial class ParamEJBNATransactions : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fromDate = Request.Params["from"];
            string toDate = Request.Params["to"];

            //http://localhost:82/CCMSInt/GraphicalAnalysis/Reports/ParamEJBNATransactions.aspx?from=05/05/2000&to=12/05/2014
            BindGrid(fromDate,toDate);

        }


          void BindGrid(string fromDate,string toDate)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                cmd.CommandText = @"select title,trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment
                                    from ej_parsed_bna_transaction inner join atm 
                                    on ej_parsed_bna_transaction.atm_id = atm.atm_id where atm.is_active=1 
                                    and  trxn_datetime >= convert(datetime,'" + fromDate + "',103) " +
                                    " and trxn_datetime<= convert(datetime,'" + toDate + " 23:59:59',103) " + 
                                    " order by trxn_datetime";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);
                Literal_count.Text = dtTrxn.Rows.Count + " records.";
                this.GridView_Trxn.DataSource = dtTrxn;
                this.GridView_Trxn.DataBind();
            }
            catch (Exception ex)
            {
                
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }


     

    }
}