using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;

using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace CCMSUI.admin
{
    public partial class AddEditATMBandwidth : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.SetFocus(TextBox_title);
            if (Page.IsPostBack)
                return;


            if (Request.QueryString["id"] == null)
            {
                if (!CheckRight((int)Permissions.CreateATMBandwidth))
                    Response.Redirect("AccessDenied.aspx");
          
            }
            else
            {
                if (!CheckRight((int)Permissions.ModifyATMBandwidth))
                    Response.Redirect("AccessDenied.aspx");

                Avanza.CCMS.DAL.AtmBandwidth bandwidth = Avanza.CCMS.DAL.AtmBandwidth.LoadAtmBandwidthByPk(int.Parse(Request.QueryString["id"]));
                TextBox_title.Text = bandwidth.AtmBandwidthName;
                TextBox_BandwidthSpeed.Text = bandwidth.LinkSpeedKbps.Value.ToString();
             }
        }

        protected void Button_SaveUpdateBandwidth_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_SaveUpdateBandwidth");
            Avanza.CCMS.DAL.AtmBandwidth bandwidth;
            Avanza.CCMS.DAL.AtmBandwidth oldbandwidth = null;
            bool editMode = false;
            bool isNameContainSpecialChars = CheckSpecialChars(TextBox_title.Text);
            SqlConnection conn = null;
            SqlTransaction trx = null;

            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                if (isNameContainSpecialChars == true)
                {
                    Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Name contain invalid character</img> </li>";
                    return;
                }
                else
                {
                    if (Request.QueryString["id"] == null)
                    {
                        bandwidth = new Avanza.CCMS.DAL.AtmBandwidth();
                        bandwidth.CreationTime = DateTime.Now;
                        bandwidth.CreatedBy = (int)((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    }
                    else
                    {
                        bandwidth = Avanza.CCMS.DAL.AtmBandwidth.LoadAtmBandwidthByPk(int.Parse(Request.QueryString["id"]));
                        oldbandwidth = Avanza.CCMS.DAL.AtmBandwidth.LoadAtmBandwidthByPk(int.Parse(Request.QueryString["id"]));
                        bandwidth.ModificationTime = DateTime.Now;
                        bandwidth.ModifiedBy = (int)((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        editMode = true;
                    }

                    object res = ConnectionFactory.ExecuteScalar("select top 1 classification_name from classification where classification_name ='" + TextBox_title.Text + "'"
                         + (editMode ? " and classification_id != " + bandwidth.AtmBandwidthId : ""));
                    if (res != null)
                    {
                        Literal_error.Text = "<li>This name is already defined for ATM Classification '" + res + "'. Please, provide different classification name.</li>";
                        return;
                    }

                    bandwidth.AtmBandwidthName = TextBox_title.Text;
                    bandwidth.LinkSpeedKbps = int.Parse(TextBox_BandwidthSpeed.Text);
                    bandwidth.Save();

                    if (!editMode)
                        Utility.BuildAuditLog("ATM Bandwidth: "+bandwidth.AtmBandwidthName + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateATMBandwidth);
                    else
                        editAuditLog(bandwidth, oldbandwidth, trx);
                    //Utility.BuildAuditLog("ATM Bandwidth: "+bandwidth.AtmBandwidthName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyATMBandwidth);
                    if (trx != null)
                    {
                        trx.Commit();
                    }
                    Response.Redirect("ATMBandwidth.aspx", false);
                }
                
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }
                Literal_error.Text = "<li>An error occured while saving ATM Bandwidth." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }

        public void editAuditLog(Avanza.CCMS.DAL.AtmBandwidth objCurrent, Avanza.CCMS.DAL.AtmBandwidth objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "ATM Bandwidth: " + objPrevious.AtmBandwidthName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyATMBandwidth, trx);
            }

        }



    }
}