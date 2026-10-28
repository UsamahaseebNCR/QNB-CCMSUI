using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;

namespace CCMSUI.CCMS
{
    public partial class Reconciliation_details : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ReconciliationBatch batch = ReconciliationBatch.LoadReconciliationBatchByPk(int.Parse(Request.QueryString["id"]));
                Literal_Heading.Text = "Reconciliation Batch Details";

                Label_RetryCount2.Text = batch.RetryCount.ToString();
                Label_CreationTimestamp.Text = batch.CreationTime.ToString("dd/MM/yyyy HH:mm:ss");
                if (batch.EndTime.HasValue)
                    Label_FinishedTimestamp2.Text = batch.EndTime.Value.ToString("dd/MM/yyyy HH:mm:ss");
                Label_Status.Text = batch.Status;
                if (batch.LastInvokedAt.HasValue)
                    Label_LastInvoked2.Text = batch.LastInvokedAt.Value.ToString("dd/MM/yyyy HH:mm:ss");

                Label_LastAttemptFailedReason2.Text = batch.FailureReason;
                Label_TrxnStartDate.Text = batch.TransactionStartDate.ToString("dd/MM/yyyy");
                Label_TrxnEndDate.Text = batch.TransactionEndDate.ToString("dd/MM/yyyy");

                if (Label_LastAttemptFailedReason2.Text.Length == 0) Label_LastAttemptFailedReason2.Text = "-";

                if (batch.Status == "Scheduled")
                    Button_Cancel.Visible = true;
                else
                    Button_Cancel.Visible = false;

            }
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            ReconciliationBatch batch = ReconciliationBatch.LoadReconciliationBatchByPk(int.Parse(Request.QueryString["id"]));
            batch.Status = "Cancelled";
            batch.Save();
            Response.Redirect("Reconciliation.aspx");
        }
    }
}