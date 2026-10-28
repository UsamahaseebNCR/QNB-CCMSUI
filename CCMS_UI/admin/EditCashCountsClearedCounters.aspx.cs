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
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;

using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;

using ComponentArt.Web.UI;

namespace CCMSUI.admin
{
    public partial class EditCashCountsClearedCounters : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.SetFocus(TextBox_DenomType1);
            if (!Page.IsPostBack)
            {
                AtmSettlement atmSettlement = AtmSettlement.LoadAtmSettlementByPk(int.Parse(Request.QueryString["id"]));
                if (atmSettlement != null)
                {
                    if (atmSettlement.CashRepDenominationType1.HasValue)
                        TextBox_DenomType1.Text = atmSettlement.CashRepDenominationType1.Value.ToString();
                    else
                        TextBox_DenomType1.Text = "0";

                    if (atmSettlement.CashRepDenominationType2.HasValue)
                        TextBox_DenomType2.Text = atmSettlement.CashRepDenominationType2.Value.ToString();
                    else
                        TextBox_DenomType2.Text = "0";

                    if (atmSettlement.CashRepDenominationType3.HasValue)
                        TextBox_DenomType3.Text = atmSettlement.CashRepDenominationType3.Value.ToString();
                    else
                        TextBox_DenomType3.Text = "0";

                    if (atmSettlement.CashRepDenominationType4.HasValue)
                        TextBox_DenomType4.Text = atmSettlement.CashRepDenominationType4.Value.ToString();
                    else
                        TextBox_DenomType4.Text = "0";

                    if (atmSettlement.CashRepCountersType1.HasValue)
                        TextBox_NoOfNotes1.Text = atmSettlement.CashRepCountersType1.Value.ToString();
                    else
                        TextBox_NoOfNotes1.Text = "0";

                    if (atmSettlement.CashRepCountersType2.HasValue)
                        TextBox_NoOfNotes2.Text = atmSettlement.CashRepCountersType2.Value.ToString();
                    else
                        TextBox_NoOfNotes2.Text = "0";

                    if (atmSettlement.CashRepCountersType3.HasValue)
                        TextBox_NoOfNotes3.Text = atmSettlement.CashRepCountersType3.Value.ToString();
                    else
                        TextBox_NoOfNotes3.Text = "0";

                    if (atmSettlement.CashRepCountersType4.HasValue)
                        TextBox_NoOfNotes4.Text = atmSettlement.CashRepCountersType4.Value.ToString();
                    else
                        TextBox_NoOfNotes4.Text = "0";

                    TextBox_RepDateTime.Text = atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy HH:mm:ss");
                    TextBox_PreviousRepDate.Text = atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy HH:mm:ss");

                    if (atmSettlement.CashRejectedDenominationType1.HasValue)
                        TextBox_RejDenom1.Text = atmSettlement.CashRejectedDenominationType1.Value.ToString();
                    else
                        TextBox_RejDenom1.Text = "0";

                    if (atmSettlement.CashRejectedDenominationType2.HasValue)
                        TextBox_RejDenom2.Text = atmSettlement.CashRejectedDenominationType2.Value.ToString();
                    else
                        TextBox_RejDenom2.Text = "0";

                    if (atmSettlement.CashRejectedDenominationType3.HasValue)
                        TextBox_RejDenom3.Text = atmSettlement.CashRejectedDenominationType3.Value.ToString();
                    else
                        TextBox_RejDenom3.Text = "0";

                    if (atmSettlement.CashRejectedDenominationType4.HasValue)
                        TextBox_RejDenom4.Text = atmSettlement.CashRejectedDenominationType4.Value.ToString();
                    else
                        TextBox_RejDenom4.Text = "0";

                    if (atmSettlement.CashRejectedCountersType1.HasValue)
                        TextBox_RejNoOfNotes1.Text = atmSettlement.CashRejectedCountersType1.Value.ToString();
                    else
                        TextBox_RejNoOfNotes1.Text = "0";

                    if (atmSettlement.CashRejectedCountersType2.HasValue)
                        TextBox_RejNoOfNotes2.Text = atmSettlement.CashRejectedCountersType2.Value.ToString();
                    else
                        TextBox_RejNoOfNotes2.Text = "0";

                    if (atmSettlement.CashRejectedCountersType3.HasValue)
                        TextBox_RejNoOfNotes3.Text = atmSettlement.CashRejectedCountersType3.Value.ToString();
                    else
                        TextBox_RejNoOfNotes3.Text = "0";

                    if (atmSettlement.CashRejectedCountersType4.HasValue)
                        TextBox_RejNoOfNotes4.Text = atmSettlement.CashRejectedCountersType4.Value.ToString();
                    else
                        TextBox_RejNoOfNotes4.Text = "0";




                }
            }


        }


        protected void Button_UpdateATMSettlement_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("UpdateATMSettlement");
            SqlConnection conn = null;
            SqlTransaction trx = null;
            AtmSettlement atmSettlement;
            try
            {

                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();

                if (Request.QueryString["id"] != null)
                {
                    atmSettlement = Avanza.CCMS.DAL.AtmSettlement.LoadAtmSettlementByPk(int.Parse(Request.QueryString["id"]));
                    atmSettlement.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    atmSettlement.ModifiedOn = DateTime.Now;

                    atmSettlement.CashRepDenominationType1 = int.Parse(TextBox_DenomType1.Text);
                    atmSettlement.CashRepDenominationType2 = int.Parse(TextBox_DenomType2.Text);
                    atmSettlement.CashRepDenominationType3 = int.Parse(TextBox_DenomType3.Text);
                    atmSettlement.CashRepDenominationType4 = int.Parse(TextBox_DenomType4.Text);
                    atmSettlement.CashRepCountersType1 = int.Parse(TextBox_NoOfNotes1.Text);
                    atmSettlement.CashRepCountersType2 = int.Parse(TextBox_NoOfNotes2.Text);
                    atmSettlement.CashRepCountersType3 = int.Parse(TextBox_NoOfNotes3.Text);
                    atmSettlement.CashRepCountersType4 = int.Parse(TextBox_NoOfNotes4.Text);
                    
                    atmSettlement.TotalReplenished = atmSettlement.CashRepDenominationType1 * atmSettlement.CashRepCountersType1 +
                        atmSettlement.CashRepDenominationType2 * atmSettlement.CashRepCountersType2 +
                        atmSettlement.CashRepDenominationType3 * atmSettlement.CashRepCountersType3 +
                        atmSettlement.CashRepDenominationType4 * atmSettlement.CashRepCountersType4;





                    atmSettlement.RepDatetime = DateTime.ParseExact(TextBox_RepDateTime.Text, "dd/MM/yyyy HH:mm:ss", null);
                    atmSettlement.PreviousRepDate = DateTime.ParseExact(TextBox_PreviousRepDate.Text, "dd/MM/yyyy HH:mm:ss", null);

                    atmSettlement.CashRejectedDenominationType1 = int.Parse(TextBox_RejDenom1.Text);
                    atmSettlement.CashRejectedDenominationType2 = int.Parse(TextBox_RejDenom2.Text);
                    atmSettlement.CashRejectedDenominationType3 = int.Parse(TextBox_RejDenom3.Text);
                    atmSettlement.CashRejectedDenominationType4 = int.Parse(TextBox_RejDenom4.Text);

                    atmSettlement.CashRejectedCountersType1 = int.Parse(TextBox_RejNoOfNotes1.Text);
                    atmSettlement.CashRejectedCountersType2 = int.Parse(TextBox_RejNoOfNotes2.Text);
                    atmSettlement.CashRejectedCountersType3 = int.Parse(TextBox_RejNoOfNotes3.Text);
                    atmSettlement.CashRejectedCountersType4 = int.Parse(TextBox_RejNoOfNotes4.Text);


                    atmSettlement.TotalRejected = atmSettlement.CashRejectedDenominationType1 * atmSettlement.CashRejectedCountersType1 +
                        atmSettlement.CashRejectedDenominationType2 * atmSettlement.CashRejectedCountersType2 +
                        atmSettlement.CashRejectedDenominationType3 * atmSettlement.CashRejectedCountersType3 +
                        atmSettlement.CashRejectedDenominationType4 * atmSettlement.CashRejectedCountersType4;

                    atmSettlement.Save(trx.Connection, trx);
                    Utility.BuildAuditLog("AtmSettlement having replenishment date " + atmSettlement.RepDatetime + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyATMSettlement, trx.Connection, trx);
                    trx.Commit();

                }
                Response.Redirect("~/ccms/ATMSettlementList.aspx", false);
            }
            catch (Exception ex)
            {
                if (trx != null)
                    trx.Rollback();
         
                Literal_error.Text = "<li>An error occured while saving CIT." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }


    }
}