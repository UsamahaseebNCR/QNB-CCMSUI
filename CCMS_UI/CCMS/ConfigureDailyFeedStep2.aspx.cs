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

using System.Data.SqlClient;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using Newtonsoft.Json;

namespace CCMSUI.CCMS
{
    public partial class ConfigureDailyFeedStep2 : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (!Page.IsPostBack)
                {
                    if (Session["mcn"] != null)
                        Label_SelectedMcn.Text = (string)Session["mcn"];

                    LoadSelectedMcn();

                }
            }
            catch (Exception ex)
            {

            }
        }
        private void LoadSelectedMcn()
        {
            Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegion("mcn='" + Label_SelectedMcn.Text + "'");
            Checkbox_DFFVersion2Configured.Checked = region.IsDffVersion2Configured;

            Avanza.CCMS.DAL.DailyFeedScheme dailyFeedScheme =
                Avanza.CCMS.DAL.DailyFeedScheme.LoadDailyFeedScheme("mcn='" + Label_SelectedMcn.Text + "'");


            if (dailyFeedScheme == null)
            {
                if (RadioButton_SchemeType.SelectedIndex == 0)
                    AddSingleRow();
                else
                    AddMultipleRows();
                // return;
            }
            RadioButton_SchemeType.SelectedIndex = bool.Parse(dailyFeedScheme.IsSplitByCountry.ToString()) ? 1 : 0;

            DataTable dt = new DataTable();
            Initialize(dt);



            //TextBox_FtpUri.Text = dailyFeedScheme.FtpUri;

            Avanza.CCMS.DAL.DailyFeedConfig.DailyFeedConfigReader reader =
Avanza.CCMS.DAL.DailyFeedConfig.ExecuteReader("daily_feed_scheme_id = " + dailyFeedScheme.DailyFeedSchemeId);
            while (reader.Read())
            {
                if (dailyFeedScheme.IsSplitByCountry)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = Region.LoadRegionByPk(reader.CurrentDailyFeedConfig.RegionId).RegionName;
                    dr[1] = reader.CurrentDailyFeedConfig.DailyFeedFilePrefix;
                    //dr[2] = reader.CurrentDailyFeedConfig.DailyFeedFolderPath;
                    dt.Rows.Add(dr);
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = reader.CurrentDailyFeedConfig.DailyFeedFilePrefix;
                    //dr[1] = reader.CurrentDailyFeedConfig.DailyFeedFolderPath;
                    dt.Rows.Add(dr);

                }
            }
            reader.Close();
            if (dailyFeedScheme.IsSplitByCountry)
            {
                GridView_DFConfig.DataSource = dt;
                GridView_DFConfig.DataBind();
            }
            else
            {
                GridView_Single.DataSource = dt;
                GridView_Single.DataBind();
            }
        }
        protected void Button_SaveUpdateDailyFeedScheme_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();

                Avanza.CCMS.DAL.DailyFeedScheme olddailyFeedScheme = Avanza.CCMS.DAL.DailyFeedScheme.LoadDailyFeedScheme("mcn='" + Label_SelectedMcn.Text + "'");
                Avanza.CCMS.DAL.DailyFeedConfig olddailyFeedConfig = Avanza.CCMS.DAL.DailyFeedConfig.LoadDailyFeedConfig("daily_feed_scheme_id=" + olddailyFeedScheme.DailyFeedSchemeId.ToString());
                if (RadioButton_SchemeType.SelectedIndex == 0)
                {

                    Avanza.CCMS.DAL.DailyFeedScheme dailyFeedScheme = null;
                    ConnectionFactory.ExecuteQuery("delete daily_feed_config where daily_feed_scheme_id in (select daily_feed_scheme_id from daily_feed_scheme where mcn ='" + Label_SelectedMcn.Text + "')");
                    ConnectionFactory.ExecuteQuery("delete daily_feed_scheme where mcn ='" + Label_SelectedMcn.Text + "'");
                    dailyFeedScheme = new Avanza.CCMS.DAL.DailyFeedScheme();
                    //dailyFeedScheme.FtpUri = TextBox_FtpUri.Text;
                    dailyFeedScheme.IsSplitByCountry = bool.Parse(RadioButton_SchemeType.SelectedItem.Value);
                    dailyFeedScheme.Mcn = Label_SelectedMcn.Text;
                    dailyFeedScheme.Save();
                    Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegion("mcn='" + dailyFeedScheme.Mcn + "'");
                    region.IsDffVersion2Configured = Checkbox_DFFVersion2Configured.Checked;
                    region.Save();

                    for (int i = 0; i < GridView_Single.Rows.Count; i++)
                    {
                        bool isMCNContainSpecialChars = CheckSpecialChars(((TextBox)GridView_Single.Rows[i].FindControl("TextBox_DailyFeedFilePrefix")).Text);
                        if (isMCNContainSpecialChars == true)
                        {
                            Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;DailyFeed File Prefix contain invalid character</img> </li>";
                            return;
                        }
                        Avanza.CCMS.DAL.DailyFeedConfig config = new Avanza.CCMS.DAL.DailyFeedConfig();
                        config.DailyFeedSchemeId = dailyFeedScheme.DailyFeedSchemeId;
                        config.DailyFeedFilePrefix = ((TextBox)GridView_Single.Rows[i].FindControl("TextBox_DailyFeedFilePrefix")).Text;
                        //config.DailyFeedFolderPath = ((TextBox)GridView_Single.Rows[i].FindControl("TextBox_DailyFeedFolderPath")).Text;
                        config.Save();
                        editAuditLog(config, olddailyFeedConfig, "Configure Daily Feed: " + olddailyFeedScheme.Mcn + " modified", trx);
                    }

                    editAuditLog(dailyFeedScheme, olddailyFeedScheme, "Configure Daily Feed: " + olddailyFeedScheme.Mcn + " modified", trx);
                    
                }
                else
                {
                    Avanza.CCMS.DAL.DailyFeedScheme dailyFeedScheme = null;
                    ConnectionFactory.ExecuteQuery("delete daily_feed_config where daily_feed_scheme_id in (select daily_feed_scheme_id from daily_feed_scheme where mcn ='" + Label_SelectedMcn.Text + "')");
                    ConnectionFactory.ExecuteQuery("delete daily_feed_scheme where mcn ='" + Label_SelectedMcn.Text + "'");
                    dailyFeedScheme = new Avanza.CCMS.DAL.DailyFeedScheme();
                    //dailyFeedScheme.FtpUri = TextBox_FtpUri.Text;
                    dailyFeedScheme.IsSplitByCountry = bool.Parse(RadioButton_SchemeType.SelectedItem.Value);
                    dailyFeedScheme.Mcn = Label_SelectedMcn.Text;
                    dailyFeedScheme.Save();
                    for (int i = 0; i < GridView_DFConfig.Rows.Count; i++)
                    {
                        Avanza.CCMS.DAL.DailyFeedConfig config = new Avanza.CCMS.DAL.DailyFeedConfig();
                        config.DailyFeedSchemeId = dailyFeedScheme.DailyFeedSchemeId;
                        config.DailyFeedFilePrefix = ((TextBox)GridView_DFConfig.Rows[i].FindControl("TextBox_DailyFeedFilePrefix")).Text;
                        //config.DailyFeedFolderPath = ((TextBox)GridView_DFConfig.Rows[i].FindControl("TextBox_DailyFeedFolderPath")).Text;
                        Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegion("region_name='" + ((Label)GridView_DFConfig.Rows[i].FindControl("Label_Organization")).Text + "'");
                        region.IsDffVersion2Configured = Checkbox_DFFVersion2Configured.Checked;
                        region.Save();
                        config.RegionId = region.RegionId;
                        config.Save();
                        editAuditLog(config, olddailyFeedConfig, "Configure Daily Feed: " + olddailyFeedScheme.Mcn + " modified", trx);
                    }
                    editAuditLog(dailyFeedScheme, olddailyFeedScheme, "Configure Daily Feed: " + olddailyFeedScheme.Mcn + " modified", trx);
                }
                if (trx != null)
                {
                    trx.Commit();
                }
                Session[SessionVars.notice.ToString()] = "Configuration saved successfully!";
                Response.Redirect("ConfigureDailyFeed.aspx");
            }catch(Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }

                Session[SessionVars.notice.ToString()] = ex.Message.ToString();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void editAuditLog(dynamic objCurrent, dynamic objPrevious, string message, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, message, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyDailyFeedGenerationScheme, trx);
            }

        }


        private void Initialize(DataTable dt)
        {
            if (RadioButton_SchemeType.SelectedIndex != 0)
                dt.Columns.Add("organization");

            dt.Columns.Add("dailyfeedfileprefix");
            dt.Columns.Add("dailyfeedfolderpath");
        }
        private void AddMultipleRows()
        {
            SqlCommand cmd = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);
                if (Label_SelectedMcn.Text.Length == 0)
                    cmd.CommandText = "select mcn, country,region_name from region where is_organization=1 and mcn = '" + Request.QueryString["mcn"] + "'";
                else
                    cmd.CommandText = "select mcn, country,region_name from region where is_organization=1 and mcn = '" + Label_SelectedMcn.Text + "'";
                //int counter = 0;

                DataTable dt = new DataTable();
                Initialize(dt);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = reader.GetString(2);
                        dt.Rows.Add(dr);
                    }
                }
                GridView_DFConfig.DataSource = dt;
                GridView_DFConfig.DataBind();
                cmd.Connection.Close();
                GridView_Single.Visible = false;
                GridView_DFConfig.Visible = true;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }

        }
        private void AddSingleRow()
        {
            DataTable dt = new DataTable();
            Initialize(dt);
            dt.Rows.Add(dt.NewRow());
            GridView_Single.DataSource = dt;
            GridView_Single.DataBind();
            GridView_DFConfig.Visible = false;
            GridView_Single.Visible = true;
        }
        protected void DropDownList_SplitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (DropDownList_SplitType.SelectedIndex == 0)
            //    AddSingleRow();
            //else
            //    AddMultipleRows();


        }

        protected void RadioButton_SchemeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RadioButton_SchemeType.SelectedIndex == 0)
                AddSingleRow();
            else
                AddMultipleRows();





        }

      

    }
}