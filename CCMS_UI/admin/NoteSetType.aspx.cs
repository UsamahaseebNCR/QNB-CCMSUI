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
using System.Reflection;
using System.Diagnostics;


using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;


namespace CCMSUI.admin
{
    public partial class NoteSetType : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewNoteSetType))
                {
                    Response.StatusCode = 401;
                    Response.Redirect("AccessDenied.aspx");
                }

                if (!CheckRight((int)Permissions.ModifyNoteSetType))
                    GridView_NoteSetType.Columns[16].Visible = false;

                if (!CheckRight((int)Permissions.DeleteNoteSetType))
                    GridView_NoteSetType.Columns[17].Visible = false;

                if (!CheckRight((int)Permissions.CreateNoteSetType))
                    HyperLink_newNoteSetType.Visible = false;

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "region.region_id";
                //BindGrid(ViewState["SortExpression"].ToString() + ViniteewState["sortDirection"].ToString());

                //   AppSetting objappsetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                //if (!objappsetting.IsGasperexchangeConfigured.Value)
                //  GridView_ATMs.Columns[3].Visible = true;
                AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                while (userReader.Read())
                    DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();


                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());


            }

        }
        string Filter()
        {
            string filter = "";
            if (TextBox_NoteSetTypeID.Text.Length > 0)
                filter += "  note_set_type_name like '%" + TextBox_NoteSetTypeID.Text + "%' and ";

            if (DropDownList_creator.SelectedIndex > 0)
                filter += "  app_user.user_id = " + DropDownList_creator.SelectedValue + " and ";

            string selectedRegion = null;
            if (Session[SessionVars.SelectedRegions.ToString()] == null)
                filter += " region.region_id =-1 and ";
            else
                filter += " region.region_id in (" + (string)Session[SessionVars.SelectedRegions.ToString()] + ") and ";


            if (filter.Length > 0)
                return " and " + filter.Substring(0, filter.LastIndexOf("and "));
            else
                return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                ///<Summary>Edited by Ali Shah on 11th Oct, 2019
                ///To dynamically handling of cassettes more than 4
                EnableGridRespectiveColumns();

                ValidateInputs();
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                cmd.CommandText = @"select note_set_type_id,region.region_name,region.region_id,note_set_type_name,denomination_type_1,
denomination_type_2,denomination_type_3,denomination_type_4,denomination_type_5,denomination_type_6,
denomination_type_7,denomination_type_1_title,denomination_type_2_title,
denomination_type_3_title,denomination_type_4_title,denomination_type_5_title 
,denomination_type_6_title ,denomination_type_7_title from note_set_type,region, app_user where app_user.user_id = note_set_type.created_by and  note_set_type.region_id = region.region_id  " + Filter() + "  order by " + orderby;


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtNoteSetType = new DataTable();
                adapter.Fill(dtNoteSetType);
                cmd.Connection.Close();
                this.GridView_NoteSetType.DataSource = dtNoteSetType; ;
                this.GridView_NoteSetType.DataBind();
                Literal_count.Text = dtNoteSetType.Rows.Count + " records.";

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_NoteSetType_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_NoteSetType_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            SqlConnection conn = null;
            SqlTransaction trxn = null;
            bool trxnCommitted = false;
            if (e.CommandName == "deleteNoteSetType")
            {
                task = LogableTask.NewTask("delete Note Set Type");

                try
                {

                    conn = ConnectionFactory.GetNewConnection();
                    conn.Open();
                    trxn = conn.BeginTransaction();

                    if ((int)ConnectionFactory.ExecuteScalar("select count(note_set_type_id) from atm where note_set_type_id=" + e.CommandArgument.ToString()) != 0)
                        throw new Exception("Cannot delete this note set as it is assigned to some atms");


                    //ConnectionFactory.ExecuteQuery("delete note_set_type where note_set_type_id = " + e.CommandArgument.ToString());

                    Avanza.CCMS.DAL.NoteSetType noteSetType = Avanza.CCMS.DAL.NoteSetType.LoadNoteSetTypeByPk(int.Parse(e.CommandArgument.ToString()));
                    if (noteSetType != null)
                    {
                        ValidateInputs();
                        noteSetType.Delete(conn, trxn);
                        ConnectionFactory.ExecuteQuery(@"delete ccms_org_note_set_item where org_note_set_id = (
                                                        select id from ccms_org_note_set where name = '" + noteSetType.NoteSetTypeName + "')" +
                                                        ";delete ccms_org_note_set where name = '" + noteSetType.NoteSetTypeName + "'", trxn);
                        Utility.BuildAuditLog("Node Set Type " + noteSetType.NoteSetTypeName + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteNoteSetType);
                    }

                    //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                    trxn.Commit();
                    Session[SessionVars.notice.ToString()] = "Selected Note Type Set was deleted!";
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "note type set deleted");

                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    if (!trxnCommitted)
                        trxn.Rollback();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting Note Set Type. " + ex.Message + "</li>";

                }
                finally
                {
                    if (conn != null)
                        conn.Close();
                    task.EndTask();

                }
            }
        }

        protected void GridView_NoteSetType_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_NoteSetType.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_NoteSetType_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteNoteSetType") != null)


                ((ImageButton)e.Row.FindControl("Button_deleteNoteSetType")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete selected note set type " + ((DataRowView)e.Row.DataItem).Row["note_set_type_name"] + "?','CCMS')");


        }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            try
            {
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }

        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_NoteSetTypeID.Text = "";
            DropDownList_creator.SelectedIndex = 0;


        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }


        public void ValidateInputs()
        {
           
            bool isNoteSetTypeIDContainSpecialChars = CheckSpecialChars(TextBox_NoteSetTypeID.Text);
            if (isNoteSetTypeIDContainSpecialChars == true)
            {
                throw new Exception ("Note Set Type ID has invalid character");
            }
            
        }

        private void EnableGridRespectiveColumns()
        {
            int noOfCassettes = ValidateNoOfCassettes(ConfigurationManager.AppSettings["NoOfCassettes"]);
            int columnIndexDenomination = -1;
            //int columnIndexDispensed = -1;
            //int columnIndexRemaining = -1;
            columnIndexDenomination = GetColumnIndexByName(this.GridView_NoteSetType, "Type 5");
            //columnIndexDispensed = GetColumnIndexByName(this.GridView_CountsCleared, "Dispensed Type 5");
            //columnIndexRemaining = columnIndexDenomination + 1;// GetColumnIndexByName(this.GridView_CashPositions, "Remaining");
            for (int i = 5; i <= noOfCassettes; i++)
            {
                if (columnIndexDenomination >= 0)
                {
                    this.GridView_NoteSetType.Columns[columnIndexDenomination].Visible = true;
                    columnIndexDenomination += 2;
                }

                //if (columnIndexDispensed >= 0)
                //{
                //    this.GridView_CountsCleared.Columns[columnIndexDispensed].Visible = true;
                //    columnIndexDispensed++;
                //}
                //if (columnIndexRemaining >= 0)
                //{
                //    this.GridView_CashPositions.Columns[columnIndexRemaining].Visible = true;
                //    columnIndexRemaining += 2;
                //}
            }
        }
    }
}