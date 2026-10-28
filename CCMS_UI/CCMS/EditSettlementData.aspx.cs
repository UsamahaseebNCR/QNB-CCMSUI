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
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using Avanza.iSuite.UI;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class EditSettlementData : BasePage
    {
        protected void Button_AddNewComments_click(object sender, EventArgs e)
        {
            if (this.TextBox_NewComments.Text.Trim().Length > 0)
            {
                AtmSettlement settlement = AtmSettlement.LoadAtmSettlementByPk(int.Parse(base.Request.QueryString["sid"]));
                Avanza.CCMS.DAL.UserTask task = Avanza.CCMS.DAL.UserTask.LoadUserTask("atm_settlement_id=" + settlement.AtmSettlementId);
                UserTaskComments comments = new UserTaskComments();
                comments.Comments = this.TextBox_NewComments.Text;
                comments.UserTaskId = task.UserTaskId;
                comments.CreationTime = DateTime.Now;
                comments.UserId = ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
                comments.Save();
                this.TextBox_NewComments.Text = "";
            }
            this.ReloadComments();
        }

        protected void Button_cancel_click(object sender, EventArgs e)
        {
            base.Response.Redirect("~/ccms/UserTask.aspx");
        }

        protected void Button_Update_Click(object sender, EventArgs e)
        {
            LogableTask task = null;
            SqlCommand cmd = null;
            SqlTransaction trx = null;
            AtmSettlement settlement = AtmSettlement.LoadAtmSettlementByPk(int.Parse(base.Request.QueryString["sid"]));
            if (settlement != null)
            {
                try
                {
                    task = LogableTask.NewTask("updateSettlement");
                    AtmSettlementHistory history = new AtmSettlementHistory();
                    history.AtmSiteAndNumber = settlement.AtmSiteAndNumber;
                    history.AtmSettlementId = settlement.AtmSettlementId.Value;
                    history.CashDispensedCountersType1 = settlement.CashDispensedCountersType1;
                    history.CashDispensedCountersType2 = settlement.CashDispensedCountersType2;
                    history.CashDispensedCountersType3 = settlement.CashDispensedCountersType3;
                    history.CashDispensedCountersType4 = settlement.CashDispensedCountersType4;
                    history.CashRejectedCountersType1 = settlement.CashRejectedCountersType1;
                    history.CashRejectedCountersType2 = settlement.CashRejectedCountersType2;
                    history.CashRejectedCountersType3 = settlement.CashRejectedCountersType3;
                    history.CashRejectedCountersType4 = settlement.CashRejectedCountersType4;
                    history.CashRepCountersType1 = settlement.CashRepCountersType1;
                    history.CashRepCountersType2 = settlement.CashRepCountersType2;
                    history.CashRepCountersType3 = settlement.CashRepCountersType3;
                    history.CashRepCountersType4 = settlement.CashRepCountersType4;
                    history.CashReturnCountersType1 = settlement.CashReturnCountersType1;
                    history.CashReturnCountersType2 = settlement.CashReturnCountersType2;
                    history.CashReturnCountersType3 = settlement.CashReturnCountersType3;
                    history.CashReturnCountersType4 = settlement.CashReturnCountersType4;
                    history.GlNo = settlement.GlNo;
                    history.RepDatetime = settlement.RepDatetime;
                    history.SealNumber = settlement.SealNumber;
                    history.CashRepDenominationType1 = settlement.CashRepDenominationType1;
                    history.CashRepDenominationType2 = settlement.CashRepDenominationType2;
                    history.CashRepDenominationType3 = settlement.CashRepDenominationType3;
                    history.CashRepDenominationType4 = settlement.CashRepDenominationType4;
                    history.CashReturnDenominationType1 = settlement.CashReturnDenominationType1;
                    history.CashReturnDenominationType2 = settlement.CashReturnDenominationType2;
                    history.CashReturnDenominationType3 = settlement.CashReturnDenominationType3;
                    history.CashReturnDenominationType4 = settlement.CashReturnDenominationType4;
                    history.CashRejectedDenominationType1 = settlement.CashRejectedDenominationType1;
                    history.CashRejectedDenominationType2 = settlement.CashRejectedDenominationType2;
                    history.CashRejectedDenominationType3 = settlement.CashRejectedDenominationType3;
                    history.CashRejectedDenominationType4 = settlement.CashRejectedDenominationType4;
                    history.CashDispensedDenominationType1 = settlement.CashDispensedDenominationType1;
                    history.CashDispensedDenominationType2 = settlement.CashDispensedDenominationType2;
                    history.CashDispensedDenominationType3 = settlement.CashDispensedDenominationType3;
                    history.CashDispensedDenominationType4 = settlement.CashDispensedDenominationType4;
                    history.UploadedBy = settlement.UploadedBy;
                    history.UploadDatetime = settlement.UploadDatetime;
                    cmd = ConnectionFactory.GetNewCommand(true);
                    trx = cmd.Connection.BeginTransaction();
                    cmd.Transaction = trx;
                    history.Save(trx.Connection, trx);
                    if (this.TextBox_CITAdded1.Text.Length > 0)
                    {
                        settlement.CashRepCountersType1 = new int?(int.Parse(this.TextBox_CITAdded1.Text));
                    }
                    if (this.TextBox_CITAdded2.Text.Length > 0)
                    {
                        settlement.CashRepCountersType2 = new int?(int.Parse(this.TextBox_CITAdded2.Text));
                    }
                    if (this.TextBox_CITAdded3.Text.Length > 0)
                    {
                        settlement.CashRepCountersType3 = new int?(int.Parse(this.TextBox_CITAdded3.Text));
                    }
                    if (this.TextBox_CITAdded4.Text.Length > 0)
                    {
                        settlement.CashRepCountersType4 = new int?(int.Parse(this.TextBox_CITAdded4.Text));
                    }
                    if (this.TextBox_CITReturned1.Text.Length > 0)
                    {
                        settlement.CashReturnCountersType1 = new int?(int.Parse(this.TextBox_CITReturned1.Text));
                    }
                    if (this.TextBox_CITReturned2.Text.Length > 0)
                    {
                        settlement.CashReturnCountersType2 = new int?(int.Parse(this.TextBox_CITReturned2.Text));
                    }
                    if (this.TextBox_CITReturned3.Text.Length > 0)
                    {
                        settlement.CashReturnCountersType3 = new int?(int.Parse(this.TextBox_CITReturned3.Text));
                    }
                    if (this.TextBox_CITReturned4.Text.Length > 0)
                    {
                        settlement.CashReturnCountersType4 = new int?(int.Parse(this.TextBox_CITReturned4.Text));
                    }
                    if (this.TextBox_CITRejected1.Text.Length > 0)
                    {
                        settlement.CashRejectedCountersType1 = new int?(int.Parse(this.TextBox_CITRejected1.Text));
                    }
                    if (this.TextBox_CITRejected2.Text.Length > 0)
                    {
                        settlement.CashRejectedCountersType2 = new int?(int.Parse(this.TextBox_CITRejected2.Text));
                    }
                    if (this.TextBox_CITRejected3.Text.Length > 0)
                    {
                        settlement.CashRejectedCountersType3 = new int?(int.Parse(this.TextBox_CITRejected3.Text));
                    }
                    if (this.TextBox_CITRejected4.Text.Length > 0)
                    {
                        settlement.CashRejectedCountersType4 = new int?(int.Parse(this.TextBox_CITRejected4.Text));
                    }
                    if (this.TextBox_CITDispensed1.Text.Length > 0)
                    {
                        settlement.CashDispensedCountersType1 = new int?(int.Parse(this.TextBox_CITDispensed1.Text));
                    }
                    if (this.TextBox_CITDispensed2.Text.Length > 0)
                    {
                        settlement.CashDispensedCountersType2 = new int?(int.Parse(this.TextBox_CITDispensed2.Text));
                    }
                    if (this.TextBox_CITDispensed3.Text.Length > 0)
                    {
                        settlement.CashDispensedCountersType3 = new int?(int.Parse(this.TextBox_CITDispensed3.Text));
                    }
                    if (this.TextBox_CITDispensed4.Text.Length > 0)
                    {
                        settlement.CashDispensedCountersType4 = new int?(int.Parse(this.TextBox_CITDispensed4.Text));
                    }
                    settlement.Save(trx.Connection, trx);
                    Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTask("atm_settlement_id=" + base.Request.QueryString["sid"]);
                    userTask.Status = (((ApprovalStatus)0).ToString());
                    userTask.Save(trx.Connection, trx);
                    trx.Commit();
                    this.Session[SessionVars.notice.ToString()] = "Settlement updated successfully";
                    base.Response.Redirect("~/ccms/UserTask.aspx");
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    if (!((trx == null) || ex.Message.ToLower().Contains("thread")))
                    {
                        trx.Rollback();
                    }
                }
                finally
                {
                    task.EndTask();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!base.IsPostBack)
            {
                if (!base.CheckRight(0xbed))
                {
                    base.Response.Redirect("../admin/AccessDenied.aspx");
                }
                AtmSettlement atmSettlement = AtmSettlement.LoadAtmSettlementByPk(int.Parse(base.Request.QueryString["sid"]));
                if (atmSettlement != null)
                {
                    this.Label_ATMSiteAndNumber.Text = atmSettlement.AtmSiteAndNumber;
                    this.Label_SealNo.Text = atmSettlement.SealNumber;
                    this.Label_GLNo.Text = atmSettlement.GlNo;
                    this.Label_ReplenishmentDate.Text = atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy HH:mm:ss");
                    if (atmSettlement.CashRepCountersType1.HasValue)
                    {
                        this.TextBox_CITAdded1.Text = atmSettlement.CashRepCountersType1.Value.ToString();
                    }
                    if (atmSettlement.CashRepCountersType2.HasValue)
                    {
                        this.TextBox_CITAdded2.Text = atmSettlement.CashRepCountersType2.Value.ToString();
                    }
                    if (atmSettlement.CashRepCountersType3.HasValue)
                    {
                        this.TextBox_CITAdded3.Text = atmSettlement.CashRepCountersType3.Value.ToString();
                    }
                    if (atmSettlement.CashRepCountersType4.HasValue)
                    {
                        this.TextBox_CITAdded4.Text = atmSettlement.CashRepCountersType4.Value.ToString();
                    }
                    if (atmSettlement.CashDispensedCountersType1.HasValue)
                    {
                        this.TextBox_CITDispensed1.Text = atmSettlement.CashDispensedCountersType1.Value.ToString();
                    }
                    if (atmSettlement.CashDispensedCountersType2.HasValue)
                    {
                        this.TextBox_CITDispensed2.Text = atmSettlement.CashDispensedCountersType2.Value.ToString();
                    }
                    if (atmSettlement.CashDispensedCountersType3.HasValue)
                    {
                        this.TextBox_CITDispensed3.Text = atmSettlement.CashDispensedCountersType3.Value.ToString();
                    }
                    if (atmSettlement.CashDispensedCountersType4.HasValue)
                    {
                        this.TextBox_CITDispensed4.Text = atmSettlement.CashDispensedCountersType4.Value.ToString();
                    }
                    if (atmSettlement.CashReturnCountersType1.HasValue)
                    {
                        this.TextBox_CITReturned1.Text = atmSettlement.CashReturnCountersType1.Value.ToString();
                    }
                    if (atmSettlement.CashReturnCountersType2.HasValue)
                    {
                        this.TextBox_CITReturned2.Text = atmSettlement.CashReturnCountersType2.Value.ToString();
                    }
                    if (atmSettlement.CashReturnCountersType3.HasValue)
                    {
                        this.TextBox_CITReturned3.Text = atmSettlement.CashReturnCountersType3.Value.ToString();
                    }
                    if (atmSettlement.CashReturnCountersType4.HasValue)
                    {
                        this.TextBox_CITReturned4.Text = atmSettlement.CashReturnCountersType4.Value.ToString();
                    }
                    if (atmSettlement.CashRejectedCountersType1.HasValue)
                    {
                        this.TextBox_CITRejected1.Text = atmSettlement.CashRejectedCountersType1.Value.ToString();
                    }
                    if (atmSettlement.CashRejectedCountersType2.HasValue)
                    {
                        this.TextBox_CITRejected2.Text = atmSettlement.CashRejectedCountersType2.Value.ToString();
                    }
                    if (atmSettlement.CashRejectedCountersType3.HasValue)
                    {
                        this.TextBox_CITRejected3.Text = atmSettlement.CashRejectedCountersType3.Value.ToString();
                    }
                    if (atmSettlement.CashRejectedCountersType4.HasValue)
                    {
                        this.TextBox_CITRejected4.Text = atmSettlement.CashRejectedCountersType4.Value.ToString();
                    }
                    Atm atm = Atm.LoadAtm("gl_number='" + atmSettlement.GlNo + "'");
                    if (atm != null)
                    {
                        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                        if (noteSetType != null)
                        {
                            if (noteSetType.DenominationType1Title.Length > 0)
                            {
                                this.Label_ctype1.Text = noteSetType.DenominationType1Title;
                            }
                            if (noteSetType.DenominationType2Title.Length > 0)
                            {
                                this.Label_ctype2.Text = noteSetType.DenominationType2Title;
                            }
                            if (noteSetType.DenominationType2Title.Length > 0)
                            {
                                this.Label_ctype3.Text = noteSetType.DenominationType3Title;
                            }
                            if (noteSetType.DenominationType4Title.Length > 0)
                            {
                                this.Label_ctype4.Text = noteSetType.DenominationType4Title;
                            }
                        }

                        int result = 0;

                        if (atmSettlement.CashRepCountersType1.HasValue)
                            result = atmSettlement.CashRepCountersType1.Value * noteSetType.DenominationType1.Value;
                        if (atmSettlement.CashRepCountersType2.HasValue)
                            result += atmSettlement.CashRepCountersType2.Value * noteSetType.DenominationType2.Value;
                        if (atmSettlement.CashRepCountersType3.HasValue)
                            result += atmSettlement.CashRepCountersType3.Value * noteSetType.DenominationType3.Value;
                        if (atmSettlement.CashRepCountersType4.HasValue)
                            result += atmSettlement.CashRepCountersType4.Value * noteSetType.DenominationType4.Value;
                        this.Label_CITTotalAdded.Text = result.ToString();
                        result = 0;

                        if (atmSettlement.CashDispensedCountersType1.HasValue)
                            result = atmSettlement.CashDispensedCountersType1.Value * noteSetType.DenominationType1.Value;
                        if (atmSettlement.CashDispensedCountersType2.HasValue)
                            result += atmSettlement.CashDispensedCountersType2.Value * noteSetType.DenominationType2.Value;
                        if (atmSettlement.CashDispensedCountersType3.HasValue)
                            result += atmSettlement.CashDispensedCountersType3.Value * noteSetType.DenominationType3.Value;
                        if (atmSettlement.CashDispensedCountersType4.HasValue)
                            result += atmSettlement.CashDispensedCountersType4.Value * noteSetType.DenominationType4.Value;
                        this.Label_CITTotalDispensed.Text = result.ToString();

                        result = 0;

                        if (atmSettlement.CashRejectedCountersType1.HasValue)
                            result = atmSettlement.CashRejectedCountersType1.Value * noteSetType.DenominationType1.Value;
                        if (atmSettlement.CashRejectedCountersType2.HasValue)
                            result += atmSettlement.CashRejectedCountersType2.Value * noteSetType.DenominationType2.Value;
                        if (atmSettlement.CashRejectedCountersType3.HasValue)
                            result += atmSettlement.CashRejectedCountersType3.Value * noteSetType.DenominationType3.Value;
                        if (atmSettlement.CashRejectedCountersType4.HasValue)
                            result += atmSettlement.CashRejectedCountersType4.Value * noteSetType.DenominationType4.Value;
                        this.Label_CITTotalRejected.Text = result.ToString();



                        result = 0;

                        if (atmSettlement.CashReturnCountersType1.HasValue)
                            result = atmSettlement.CashReturnCountersType1.Value * noteSetType.DenominationType1.Value;
                        if (atmSettlement.CashReturnCountersType2.HasValue)
                            result += atmSettlement.CashReturnCountersType2.Value * noteSetType.DenominationType2.Value;
                        if (atmSettlement.CashReturnCountersType3.HasValue)
                            result += atmSettlement.CashReturnCountersType3.Value * noteSetType.DenominationType3.Value;
                        if (atmSettlement.CashReturnCountersType4.HasValue)
                            result += atmSettlement.CashReturnCountersType4.Value * noteSetType.DenominationType4.Value;
                        this.Label_CITTotalReturned.Text = result.ToString();

                        this.TextBox_CITAdded1.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",1)" }));
                        this.TextBox_CITAdded2.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",1)" }));
                        this.TextBox_CITAdded3.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",1)" }));
                        this.TextBox_CITAdded4.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",1)" }));
                        this.TextBox_CITDispensed1.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",2)" }));
                        this.TextBox_CITDispensed2.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",2)" }));
                        this.TextBox_CITDispensed3.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",2)" }));
                        this.TextBox_CITDispensed4.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",2)" }));
                        this.TextBox_CITReturned1.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",3)" }));
                        this.TextBox_CITReturned2.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",3)" }));
                        this.TextBox_CITReturned3.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",3)" }));
                        this.TextBox_CITReturned4.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",3)" }));
                        this.TextBox_CITRejected1.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",4)" }));
                        this.TextBox_CITRejected2.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",4)" }));
                        this.TextBox_CITRejected3.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",4)" }));
                        this.TextBox_CITRejected4.Attributes.Add("onchange", string.Concat(new object[] { "javascript:updateTotal(", noteSetType.DenominationType1, ",", noteSetType.DenominationType2, ",", noteSetType.DenominationType3, ",", noteSetType.DenominationType4, ",4)" }));
                        this.ReloadComments();
                    }
                }
            }
        }

        private void ReloadComments()
        {
            AtmSettlement settlement = AtmSettlement.LoadAtmSettlementByPk(int.Parse(base.Request.QueryString["sid"]));
            Avanza.CCMS.DAL.UserTask task = Avanza.CCMS.DAL.UserTask.LoadUserTask("atm_settlement_id=" + settlement.AtmSettlementId);
            UserTaskComments.UserTaskCommentsReader reader = UserTaskComments.ExecuteReader(" user_task_id="+ task.UserTaskId );
            StringBuilder builder = new StringBuilder();
            while (reader.Read())
            {
                builder.Append("<div style='width:100%'><table style='width:100%' border='0'><tr><td style='width:10%'><b>" + AppUser.LoadAppUserByPk(reader.CurrentUserTaskComments.UserId).UserFullName + ":</b></td><td style='width:80%'>" + reader.CurrentUserTaskComments.Comments + "</td><td align='right' width='10%'>" + reader.CurrentUserTaskComments.CreationTime.ToString("dd/MM/yyyy HH:mm") + "</td></table></div><br/>");
            }
            reader.Close();
            this.Literal_Comments.Text = builder.ToString();
        }
    }


}