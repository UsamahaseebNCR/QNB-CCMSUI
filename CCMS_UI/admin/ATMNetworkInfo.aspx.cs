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

using Avanza.CCMS.DAL;
using System.Net.NetworkInformation;
using System.Net.Sockets;


namespace Avanza.iSuite.UI
{
    public partial class ATMNetworkInfo : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "title";

                AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                while (userReader.Read())
                    DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();


                DropDownList_ATMType.Items.Add(new ListItem("Any", "Any"));
                AddEditATM.PopulateATMTypes(DropDownList_ATMType);

                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        protected void GridView_ATMs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Atm atm = null;
            try
            {

                //GridViewRow row = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
                //if (Session["lastSelectedRow"] == null)
                //{
                //    Session["lastSelectedRow"] = row;
                //    Session["lastRowColor"] = row.BackColor;
                //}
                //else
                //{
                //    GridViewRow lastSelectedRow = (GridViewRow)Session["lastSelectedRow"];
                //    lastSelectedRow.BackColor = (System.Drawing.Color)Session["lastRowColor"];
                //    Session["lastSelectedRow"] = row;
                //}


                //row.BackColor = System.Drawing.Color.Yellow;

                if (e.CommandName == "ping")
                {
                    atm = Atm.LoadAtmByPk(int.Parse(e.CommandArgument.ToString()));
                    atm.LastPingExecutedAt = DateTime.Now;
                    System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                    PingReply reply = ping.Send(atm.IP);
                    if (reply.Status.ToString() != "Success")
                        Literal_error.Text = "Ping operation failed with status " + reply.Status.ToString() + " on " + atm.Title + " [" + atm.IP + "]";
                    else
                        Literal_ok.Text = "Ping completed successfully on " + atm.Title + " [" + atm.IP + "]";
                    atm.LastPingStatus = reply.Status.ToString();
                    atm.Save();
                }
                else if (e.CommandName == "telnet")
                {
                    try
                    {
                        atm = Atm.LoadAtmByPk(int.Parse(e.CommandArgument.ToString()));
                        atm.LastTelnetExecutedAt = DateTime.Now;
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(atm.IP, atm.Port);
                        //socket.Disconnect(false);
                        socket.Close(1000);
                        Literal_ok.Text = "Telnet completed successfully on " + atm.Title + " [" + atm.IP + "]";
                        atm.LastTelnetStatus = "Connected";

                    }
                    catch (Exception ex)
                    {
                        LogableTask.LogMonoActivityTask("execCommand", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                        Literal_error.Text = "<li>An Error occured on " + atm.Title + " [" + atm.IP + "]" + ex.Message + "</li>";
                        atm.LastTelnetStatus = "Failed";
                    }
                    atm.Save();

                }
               
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("execCommand", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured on " + atm.Title + " [" + atm.IP + "]" + ex.Message + "</li>";
            }
            finally
            {

            }
        }
        string Filter()
        {
            string filter = "";
            if (TextBox_title.Text.Length > 0)
                filter += " and title like '%" + TextBox_title.Text + "%'";
            if (TextBox_ip.Text.Length > 0)
                filter += " and ip like '%" + TextBox_ip.Text + "%'";
            if (DropDownList_creator.SelectedIndex > 0)
                filter += " and created_by = " + DropDownList_creator.SelectedValue;
            if (DropDownList_ATMType.SelectedIndex > 0)
                filter += " and atm_type = '" + DropDownList_ATMType.SelectedValue + "'";

            if (DropDownList_status.SelectedValue == "1")
                filter += " and is_active =1 and atm_id in ("
                        + Session[SessionVars.SelectedATMs.ToString()] + ")";
            else if (DropDownList_status.SelectedValue == "0")
            {
                filter += " and is_active = 0 and atm_id in ("
                    + Session[SessionVars.InactiveATMs.ToString()] + ")";
                //and atm_id in (select atm_id from user_ATMS where user_id=" + ((AppUser)Session[SessionVars.user.ToString()]).UserId + ")";
            }
            else
            {
                filter += " and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "," + Session[SessionVars.InactiveATMs.ToString()] + ")";
            }



            return filter;
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_title.Text = "";

            TextBox_ip.Text = "";
            DropDownList_creator.SelectedIndex = 0;
            DropDownList_status.SelectedIndex = 0;
            DropDownList_ATMType.SelectedIndex = 0;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"SELECT ATM_ID, atm_type,TITLE, IP, PORT,last_status_reply FROM ATM where  is_active=1 " + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtATMs = new DataTable();
                adapter.Fill(dtATMs);
                cmd.Connection.Close();
                this.GridView_ATMs.DataSource = dtATMs; ;
                this.GridView_ATMs.DataBind();
                Literal_count.Text = dtATMs.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured. " + ex.Message + "</li>";
            }
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




        protected void GridView_ATMs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_ATMs.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }



        protected void Button_search_Click(object sender, EventArgs e)
        {
            try
            {
               ValidateInput();
               
                   BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
               
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }
            
        }


        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }
       
        public void ValidateInput()
        {
            
            bool isTitleContainSpecialChars = CheckSpecialChars(TextBox_title.Text);
            if (isTitleContainSpecialChars == true)
            {
                throw new Exception("Title contain invalid character");
            
            }
            bool isIPContainSpecialChars = CheckSpecialChars(TextBox_ip.Text);
            if (isIPContainSpecialChars == true)
            {
                throw new Exception("IP contain invalid character");
            
            }
            
        }
    }
}