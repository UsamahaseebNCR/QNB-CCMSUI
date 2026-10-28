using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI.CCMS
{
    public partial class DashboardList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DashboardTemplate.DashboardTemplateReader reader = DashboardTemplate.ExecuteReader(@"is_active=1 AND id IN (
                SELECT dashboard_id FROM dashboard_graph where is_active = 1
                )");
            string customReportComponent = string.Empty;
            while (reader.Read())
            {
                customReportComponent = string.Empty;
                customReportComponent += string.Format(@"<div class='DIVHomePage'>
                            <table cellpadding='0' cellspacing='0' border='0'>
                                <tr style='background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px;'>
                                    <td style='width: 158px; cursor: pointer; height: 100px;' onclick='{0}'>
                                       {1}<br />
                                        <img alt='' src='../images/icon_reports.jpg' style='cursor: pointer' />
                                    </td>
                                </tr>
                            </table>
                        </div>", string.Format("window.open('dashboardrenderer.aspx?id={0}', ' ', 'location=0,status=1,resizable=yes,scrollbars=yes')", reader.CurrentDashboardTemplate.Id).Replace('\'', '"'), reader.CurrentDashboardTemplate.Title);
                dDashboardList.InnerHtml += customReportComponent;
            }
            reader.Close();
        }
    }
}