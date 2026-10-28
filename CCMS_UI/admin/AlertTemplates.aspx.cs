using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;


using Avanza.CCMS.DAL;




namespace Avanza.iSuite.UI
{
	/// <summary>
	/// Displays the information about alert templates.
	/// </summary>
	public partial class AlertTemplates : BasePage
	{
		

		
		protected void SetDefaultValues()
		{
			try
			{
				//\\UILogger.StepInContext("SetDefaultValues");

				TextBox_name.Text = "";

				GridView_AlertTemplates.DataSource = new DataTable();
				GridView_AlertTemplates.DataBind();

				Literal_count.Text = "0 records.";
			}
			catch (Exception ex)
			{
				//\\UILogger.Log(SeverityLevel.Error, "An error occurred while setting default values.", ex);
				Literal_error.Text = "<li>An error occurred while setting default values: [" + ex.Message + "]</li>";
			}
			finally
			{
				//\\UILogger.StepOutContext();
			}
		}

		
		protected void BindGrid()
		{
			SqlDataAdapter adapter = null;
			SqlCommand cmd = null;
			DataTable dtAlertTemplates = null;
			StringBuilder selectQuery = null; 
			string orderBy = "";

			try
			{
				//\\UILogger.StepInContext("BindGrid");

				cmd = ConnectionFactory.GetNewCommand(false);

				selectQuery = new StringBuilder("select * from alert_template ");
				if (TextBox_name.Text != String.Empty)
				{
					selectQuery.Append("where alert_template_name like @alert_template_name ");
					cmd.Parameters.AddWithValue("@alert_template_name", "%" + TextBox_name.Text + "%");
				}

				orderBy = ViewState["SortExpression"].ToString() + " " + ViewState["SortDirection"].ToString();
				selectQuery.Append("order by " + orderBy);

				cmd.CommandText = selectQuery.ToString();
				adapter = new SqlDataAdapter(cmd);
				dtAlertTemplates = new DataTable();
				adapter.Fill(dtAlertTemplates);

				GridView_AlertTemplates.DataSource = dtAlertTemplates;
				GridView_AlertTemplates.DataBind();
				GridView_AlertTemplates.Visible = true;

				Literal_count.Text = dtAlertTemplates.Rows.Count + " records.";
			}
			catch (Exception ex)
			{
				//\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding grid.", ex);
				Literal_error.Text = "<li>An error occurred while binding grid: [" + ex.Message + "]</li>";
			}
			finally
			{
				//\\UILogger.StepOutContext();

				if (adapter != null)
					adapter.Dispose();

				if (cmd != null)
				{
					cmd.Connection.Close();
					cmd.Dispose();
				}
			}
		}

		

		

	

		
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
                //\\UILogger.OpenContext("AlertTemplates");
                //\\UILogger.StepInContext("Page_Load");

				if (!CheckRight((int)Permissions.ViewAlertTemplatesList))
					Response.Redirect("AccessDenied.aspx");
				else if (!Page.IsPostBack)
				{
					if (!CheckRight((int)Permissions.AddAlertTemplate))
						HyperLink_AddAlertTemplate.Visible = false;
					if (!CheckRight((int)Permissions.EditAlertTemplate))
						GridView_AlertTemplates.Columns[2].Visible = false;
					if (!CheckRight((int)Permissions.DeleteAlertTemplate))
						GridView_AlertTemplates.Columns[3].Visible = false;

					SetDefaultValues();

					ViewState["SortDirection"] = "asc";
					ViewState["SortExpression"] = "alert_template_name";
					BindGrid();

					Page.SetFocus(Button_search);
				}
			}
			catch (Exception ex)
			{
				//\\UILogger.Log(SeverityLevel.Error, "An error occurred while loading page.", ex);
				Literal_error.Text = "<li>An error occurred while loading page: [" + ex.Message + "]</li>";
			}
			finally
			{
				//\\UILogger.CloseContext();
			}
		}

		

		

		
		protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
		{
			try
			{
                //\\UILogger.OpenContext("AlertTemplates");
                //\\UILogger.StepInContext("GridView_Sorting");

				if (ViewState["SortExpression"].ToString() == e.SortExpression)
				{
					if (ViewState["SortDirection"].ToString() == "asc")
						ViewState["SortDirection"] = "desc";
					else
						ViewState["SortDirection"] = "asc";
				}
				else
				{
					ViewState["SortExpression"] = e.SortExpression;
					ViewState["SortDirection"] = "asc";
				}

				BindGrid();
			}
			catch (Exception ex)
			{
				//\\UILogger.Log(SeverityLevel.Error, "An error occurred while sorting grid.", ex);
				Literal_error.Text = "<li>An error occurred while sorting grid: [" + ex.Message + "]</li>";
			}
			finally
			{
				//\\UILogger.CloseContext();
			}
		}

	
		protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			try
			{
                //\\UILogger.OpenContext("AlertTemplates");
                //\\UILogger.StepInContext("GridView_PageIndexChanging");

				GridView_AlertTemplates.PageIndex = e.NewPageIndex;
				BindGrid();
			}
			catch (Exception ex)
			{
				//\\UILogger.Log(SeverityLevel.Error, "An error occurred while changing page index.", ex);
				Literal_error.Text = "<li>An error occurred while changing page index: [" + ex.Message + "]</li>";
			}
			finally
			{
				//\\UILogger.CloseContext();
			}
		}

		
		protected void GridView_RowCreated(object sender, GridViewRowEventArgs e)
		{
			try
			{
                //\\UILogger.OpenContext("AlertTemplates");
                //\\UILogger.StepInContext("GridView_RowCreated");

				if (ViewState["SortDirection"] != null && ViewState["SortExpression"] != null)
				{
					if (e.Row.RowType == DataControlRowType.Header)
					{
						foreach (TableCell tc in e.Row.Cells)
						{
							if (tc.HasControls())
							{
								LinkButton lnk = (LinkButton)tc.Controls[0];
								if (lnk != null)
								{
									System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
									img.ImageUrl = "../images/gridimages/" + ViewState["SortDirection"].ToString().Trim() + ".gif";

									if (ViewState["SortExpression"].ToString().Trim() == lnk.CommandArgument)
									{
										tc.Controls.Add(new LiteralControl(" "));
										tc.Controls.Add(img);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				//\\UILogger.Log(SeverityLevel.Error, "An error occurred while creating row.", ex);
				Literal_error.Text = "<li>An error occurred while creating row: [" + ex.Message + "]</li>";
			}
			finally
			{
				//\\UILogger.CloseContext();
			}
		}

		
		protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
                //\\UILogger.OpenContext("AlertTemplates");
                //\\UILogger.StepInContext("GridView_RowDataBound");

				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					if (e.Row.FindControl("Button_Delete") != null)
						((ImageButton)e.Row.FindControl("Button_Delete")).Attributes.Add("onClick", "return confirm('Are you sure you want to delete Template " + ((DataRowView)e.Row.DataItem).Row["alert_template_name"] + "?', 'iWatch')");
				}
			}
			catch (Exception ex)
			{
				//\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding row.", ex);
				Literal_error.Text = "<li>An error occurred while binding row: [" + ex.Message + "]</li>";
			}
			finally
			{
				//\\UILogger.CloseContext();
			}
		}

	
		protected void GridView_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			SqlCommand cmd = null;
			SqlTransaction trx = null;

			try
			{
                //\\UILogger.OpenContext("AlertTemplates");
                //\\UILogger.StepInContext("GridView_RowCommand");

				if (e.CommandName == "DeleteTemplate")
				{
					if (CheckRight((int)Permissions.DeleteAlertTemplate))
					{
						Avanza.CCMS.DAL.AlertTemplate alertTemplate = Avanza.CCMS.DAL.AlertTemplate.LoadAlertTemplateByPk(int.Parse(e.CommandArgument.ToString()));

						cmd = ConnectionFactory.GetNewCommand(true);
						trx = cmd.Connection.BeginTransaction();
						cmd.Transaction = trx;

						cmd.CommandText = "delete alert_template_devices where alert_template_id = " + e.CommandArgument.ToString();
						cmd.ExecuteNonQuery();
						cmd.CommandText = "delete Alert_escalation where alert_template_id = " + e.CommandArgument.ToString();
						cmd.ExecuteNonQuery();
						cmd.CommandText = "delete Alert_template where alert_template_id = " + e.CommandArgument.ToString();
						cmd.ExecuteNonQuery();

						trx.Commit();

						if (alertTemplate != null)
							Utility.BuildAuditLog("Alert Template " + alertTemplate.AlertTemplateName + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteAlertTemplate);

						//\\UILogger.Log(SeverityLevel.Information, "Alert Template " + alertTemplate.AlertTemplateName + " deleted.");

						Literal_ok.Text = "<li>Selected Alert Template was deleted!</li>";

						BindGrid();
					}
				}
			}
			catch (Exception ex)
			{
				if (trx != null)
					trx.Rollback();

				//\\UILogger.Log(SeverityLevel.Error, "An error occurred while executing row command.", ex);
				Literal_error.Text = "<li>An error occurred while executing row command: [" + ex.Message + "]</li>";
			}
			finally
			{
				//\\UILogger.CloseContext();

				if (cmd != null)
				{
					cmd.Connection.Close();
					cmd.Dispose();
				}
			}
		}

		

		

	
		protected void Button_search_Click(object sender, EventArgs e)
		{
			BindGrid();
		}

		
		protected void Button_reset_Click(object sender, EventArgs e)
		{
			SetDefaultValues();

			ViewState["SortDirection"] = "asc";
			ViewState["SortExpression"] = "alert_template_name";
		}

	
	

	

		protected void AddAlertTemplate(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
		{
			SqlConnection conn = null;
			SqlTransaction trx = null;
			Avanza.CCMS.DAL.AlertTemplate alertTemplate = null;
			Avanza.CCMS.DAL.SeverityLevel.SeverityLevelReader reader = null;
			Avanza.CCMS.DAL.AlertEscalation alertEscalation = null;
			object result = null;

			try
			{
				//\\UILogger.StepInContext("AddAlertTemplate");

                //if (!BasePage.isSqlInjectionFree(e.Parameters[0]) || !BasePage.isSqlInjectionFree(e.Parameters[1]))
                //{
                //    e.Output.Write("<script>ShowErrorMessage('Invalid characters entered.')</script>");
                //    return;
                //} 

				result = ConnectionFactory.ExecuteScalar("select top 1 alert_template_id from alert_template where alert_template_name like '" + e.Parameters[0] + "'");
				if (result != null)
				{
					e.Output.Write("<script>ShowErrorMessage('Alert Template name already exists.')</script>");
					return;
				}

				conn = ConnectionFactory.GetNewConnection();
				conn.Open();
				trx = conn.BeginTransaction();

				alertTemplate = new Avanza.CCMS.DAL.AlertTemplate();

				alertTemplate.AlertTemplateName = e.Parameters[0];
				alertTemplate.AlertTemplateDesc = e.Parameters[1];
				alertTemplate.Save(conn, trx);

				ConnectionFactory.ExecuteQuery("delete from alert_template_devices where alert_template_id = " + alertTemplate.AlertTemplateId, trx);

				reader = Avanza.CCMS.DAL.SeverityLevel.ExecuteReader("1=1");

				while (reader.Read())
				{
					alertEscalation = new CCMS.DAL.AlertEscalation();

					alertEscalation.AlertTemplateId = alertTemplate.AlertTemplateId;
					alertEscalation.SeverityId = reader.CurrentSeverityLevel.SeverityId;

					alertEscalation.Level0Duration = 0;
					alertEscalation.Level1Duration = 0;
					alertEscalation.Level2Duration = 0;
					alertEscalation.Level3Duration = 0;
					alertEscalation.Level4Duration = 0;

					alertEscalation.Level1NotificationCount = 0;
					alertEscalation.Level2NotificationCount = 0;
					alertEscalation.Level3NotificationCount = 0;
					alertEscalation.Level4NotificationCount = 0;
					alertEscalation.Level5NotificationCount = 0;

					alertEscalation.Level1ReminderInterval = 0;
					alertEscalation.Level2ReminderInterval = 0;
					alertEscalation.Level3ReminderInterval = 0;
					alertEscalation.Level4ReminderInterval = 0;
					alertEscalation.Level5ReminderInterval = 0;

					alertEscalation.Level1EmailEnabled = false;
					alertEscalation.Level2EmailEnabled = false;
					alertEscalation.Level3EmailEnabled = false;
					alertEscalation.Level4EmailEnabled = false;
					alertEscalation.Level5EmailEnabled = false;

					alertEscalation.Level1FaxEnabled = false;
					alertEscalation.Level2FaxEnabled = false;
					alertEscalation.Level3FaxEnabled = false;
					alertEscalation.Level4FaxEnabled = false;
					alertEscalation.Level5FaxEnabled = false;

					alertEscalation.Level1SmsEnabled = false;
					alertEscalation.Level2SmsEnabled = false;
					alertEscalation.Level3SmsEnabled = false;
					alertEscalation.Level4SmsEnabled = false;
					alertEscalation.Level5SmsEnabled = false;

					alertEscalation.Save(conn, trx);
				}

				trx.Commit();

				Utility.BuildAuditLog("Alert Template " + alertTemplate.AlertTemplateName + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.AddAlertTemplate);

				//\\UILogger.Log(SeverityLevel.Information, "Alert Template " + alertTemplate.AlertTemplateName + " saved.");

				e.Output.Write("<script>Dialog_AddAlertTemplate.Close();window.location = 'AddEditAlertTemplate.aspx?alert_template_id=' + " + alertTemplate.AlertTemplateId + ";</script>");
			}
			catch (Exception ex)
			{
				if (trx != null)
					trx.Rollback();

				//\\UILogger.Log(SeverityLevel.Error, "An error occurred while adding alert template.", ex);
				e.Output.Write("<script>ShowErrorMessage('An error occurred while adding alert template. " + ex.Message.Replace("'", "") + "')</script>");
			}
			finally
			{
				//\\UILogger.StepOutContext();

				if (reader != null)
					reader.Close();

				if (conn != null)
					conn.Close();
			}
		}

		
	}
}