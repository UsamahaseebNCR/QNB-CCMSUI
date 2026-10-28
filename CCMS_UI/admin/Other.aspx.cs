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


namespace CCMSUI.admin
{
    public partial class Other : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CheckRight((int)Permissions.ViewGraphList, false))
                DivGraph.Visible = false;

            object res = ConnectionFactory.ExecuteScalar(@"SELECT COUNT(*) 
                                                           FROM dashboard_template 
                                                           WHERE is_active = 1 AND id in (
                                                                 SELECT dashboard_id FROM dashboard_graph WHERE is_active = 1   
                                                                            )");
            if (res != null && (int)res > 0)
            {
                DivCustomDashboard.InnerHtml = @"<table cellpadding='0' cellspacing='0' border='0' class='DIVHomePage'>
                                                    <tr style='background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                                        width: 158px'>
                                                        <td style='width: 158px; cursor: pointer' onclick=location.href='../ccms/dashboardlist.aspx'>
                                                            Dashboard<br />
                                                            <img src='../images/icon_reports.jpg' />
                                                        </td>
                                                    </tr>
                                                </table>";
            }
        }
    }
}