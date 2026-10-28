using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Avanza.iSuite.DAL;
using Avanza.CCMS.DAL;


namespace Avanza.iSuite.UI
{
    public partial class ProblemBucketDescription : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (Request.QueryString["i"] == null)
                {
                    //if (!CheckRight((int)Permissions.ModifyATM))
                    //    GridView_ATMs.Columns[12].Visible = false;

                    //if (!CheckRight((int)Permissions.DeleteATM))
                    //    GridView_ATMs.Columns[13].Visible = false;

                    //if (!CheckRight((int)Permissions.CreateATMs))
                    //    this.HyperLink_newATM2.Visible = false;


                    //if (!CheckRight((int)Permissions.ImportATMsFromIMAC))
                    //    this.HyperLink_importFromIMAC.Visible = false;

                }
                else
                {
                    GridView_ATMs.Columns[13].Visible = false;
                    //this.HyperLink_newATM2.Visible = false;
                    //this.HyperLink_importFromIMAC.Visible = false;
                }

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "problem_bucket_desc";
                //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

             //   AppSetting objappsetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                //if (!objappsetting.IsGasperexchangeConfigured.Value)
                  //  GridView_ATMs.Columns[3].Visible = true;
                //AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                //DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                //while (userReader.Read())
                //    DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                //userReader.Close();


                //DropDownList_ATMType.Items.Add(new ListItem("Any", "Any"));
                //AddEditATM.PopulateATMTypes(DropDownList_ATMType);
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
            
        }
        string Filter()
        {
            string filter = "";
            //if (TextBox_title.Text.Length > 0)
            //    filter += " and title like '%" + TextBox_title.Text + "%'";
            //if (TextBox_ip.Text.Length > 0)
            //    filter += " and ip like '%" + TextBox_ip.Text + "%'";
            //if (DropDownList_creator.SelectedIndex > 0)
            //    filter += " and created_by = " + DropDownList_creator.SelectedValue;
            //if (DropDownList_ATMType.SelectedIndex > 0)
            //    filter += " and atm_type = '" + DropDownList_ATMType.SelectedValue + "'";
            
            //if (DropDownList_status.SelectedValue=="1")
            //filter += " and is_active =1 and atm_id in ("
            //        + Session[SessionVars.SelectedATMs.ToString()] + ")";
            //else if (DropDownList_status.SelectedValue == "0")
            //{
            //    filter += " and is_active = 0 and atm_id in ("
            //        + Session[SessionVars.InactiveATMs.ToString()] + ")";
            //    //and atm_id in (select atm_id from user_ATMS where user_id=" + ((AppUser)Session[SessionVars.user.ToString()]).UserId + ")";
            //}
            //else
            //{
            //    filter += " and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "," + Session[SessionVars.InactiveATMs.ToString()] + ")";
            //}

            //if (Request.QueryString["i"] != null)
            //    filter += " and atm_id in (select distinct(atm_id) from atm_general_inventory) ";

            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select * from problem_bucket_desc order by "+orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtATMs = new DataTable();
                adapter.Fill(dtATMs);
                cmd.Connection.Close();
                this.GridView_ATMs.DataSource = dtATMs; ;
                this.GridView_ATMs.DataBind();
                Literal_count.Text = dtATMs.Rows.Count + " records.";
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
            finally
            { task.EndTask(); }
        }

        protected void GridView_ATMs_Sorting(object sender, GridViewSortEventArgs e)
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

        
        protected void GridView_ATMs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "editATM")
            {
                //if (Request.QueryString["i"] == null)
                //    Response.Redirect("AddEditATM.aspx?atm_id="+e.CommandArgument);
                //else
                //    Response.Redirect("AddEditATM.aspx?atm_id=" + e.CommandArgument+"&i=1");

            }
            else if (e.CommandName == "deleteATM")
            {
                SqlCommand cmd = null;
                SqlTransaction trx = null;
                try
                {
                    //task = LogableTask.NewTask("delete atm");
                    //cmd = ConnectionFactory.GetNewCommand(true);
                    //cmd.CommandTimeout = 0;
                    //Atm atm = Atm.LoadAtmByPk(int.Parse(e.CommandArgument.ToString()));

                    //trx = cmd.Connection.BeginTransaction();
                    //cmd.CommandText = "DeleteATM";
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add("@atm_id", SqlDbType.Int);
                    //cmd.Parameters[0].Value = int.Parse(e.CommandArgument.ToString());
                    //cmd.Transaction = trx;
                    //cmd.ExecuteNonQuery();
                    //trx.Commit();
                    //if (atm != null)
                    //{
                    //    Utility.BuildAuditLog(atm.Title + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteATM);

                    //    //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                    //    Session[SessionVars.notice.ToString()] = "Selected atm was deleted!<script>window.parent.DeleteATM(" + atm.ATMId + ")</script>";
                    //}

                    //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "atm deleted");
                    //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting ATM. " + ex.Message + "</li>";
                    trx.Rollback();
                }
                finally
                {
                    cmd.Connection.Close();
                    task.EndTask();

                }
            }
        }

        protected void GridView_ATMs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_ATMs.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_ATMs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteATM") != null)
                ((ImageButton)e.Row.FindControl("Button_deleteATM")).Attributes.Add("onClick","return confirm('Are you sure you want to delete ATM " + ((DataRowView)e.Row.DataItem).Row["Title"] + "?','CCMS')");
        }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_title.Text = "";
           
            TextBox_ip.Text = "";
            
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }


    }
}