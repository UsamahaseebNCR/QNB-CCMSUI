using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI
{
    public partial class RegionWiseComparison : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "no-cache";

            Chart1.Url = "Data.aspx?param=" + Request.QueryString["gid"] + "@" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "@0@0@167AD4";
        }

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            Chart1.Url = "Data.aspx?param=" + Request.QueryString["gid"] + "@" + TextBox_From.Text + "@" + TextBox_To.Text + "@0@0@167AD4";
        }

        protected void Chart1_Load(object sender, EventArgs e)
        {

        }
    }
}