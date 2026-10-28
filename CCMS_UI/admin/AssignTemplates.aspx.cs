using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avanza.iSuite.UI;
using System.Data.SqlClient;
using Avanza.iSuite.DAL;
using System.Data;
using ComponentArt.Web.UI;
using System.Collections;
using Avanza.CCMS.DAL;
using System.Web.UI.WebControls;
//using System.Web.UI.WebControls;

namespace CCMSUI.admin
{
   public partial class AssignTemplates : BasePage
{
   
       private DataSet atmsDS;
    // Methods
    protected void BuildTree()
    {
        SqlCommand selectCommand = null;
        SqlDataAdapter adapter = null;
        this.atmsDS = new DataSet();
        try
        {
            selectCommand = ConnectionFactory.GetNewCommand(false);
            //BasePage.\\UILogger.Log(SeverityLevel.Information, base.PageName + MethodBase.GetCurrentMethod(), "BuildTree");
            selectCommand.CommandText = "select title, atm_id, region_id from atm where is_active = 1 order by title;select region_name, region_id, parent_region_id from region order by region_name";
            adapter = new SqlDataAdapter(selectCommand);
            adapter.Fill(this.atmsDS);
            this.atmsDS.Relations.Add("ATMRegionRelation", this.atmsDS.Tables[1].Columns["region_id"], this.atmsDS.Tables[0].Columns["region_id"]);
            this.atmsDS.Relations.Add("RegionRegionRelation", this.atmsDS.Tables[1].Columns["parent_region_id"], this.atmsDS.Tables[1].Columns["region_id"], false);
            foreach (DataRow row in this.atmsDS.Tables[1].Rows)
            {
                if (row.IsNull("parent_region_id"))
                {
                    ComponentArt.Web.UI.TreeViewNode node = this.CreateRegionNode(row["region_name"].ToString(), true, (int)row["region_id"]);
                    node.EditingEnabled = node.DraggingEnabled = false;
                    node.Enabled = false;
                    this.TreeView_atms.Nodes.Add(node);
                    this.PopulateSubTree(row, node, false, false);
                }
            }
            foreach (DataRow row2 in this.atmsDS.Tables[0].Rows)
            {
                if (row2.IsNull("region_id"))
                {
                    TreeViewNode node2 = this.CreateATMNode(row2["title"].ToString(), (int) row2["atm_id"]);
                    node2.EditingEnabled = node2.DraggingEnabled = false;
                    node2.Nodes.Add(node2);
                }
            }
            //if (this.TreeView_atms.Nodes.Count > 0)
            //{
            //    this.TreeView_atms.Height = 180;
            //}
            //else
            //{
            //    this.TreeView_atms.Height = 5;
            //}
        }
        catch (Exception exception)
        {
            //BasePage.\\UILogger.Log(SeverityLevel.Information, base.PageName + MethodBase.GetCurrentMethod(), "BuildTree Could not build ATM tree. ", exception);
            throw new Exception("Could not build ATM tree. " + exception.Message);
        }
        finally
        {
            if (adapter != null)
            {
                adapter.Dispose();
            }
            if (selectCommand != null)
            {
                selectCommand.Connection.Close();
                selectCommand.Dispose();
            }
        }
    }

    protected void Button_back_Click(object sender, EventArgs e)
    {
        try
        {
            base.Response.Redirect("~/admin/administration.aspx");
        }
        catch (Exception)
        {
        }
    }

    protected void Button_Save_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.DropDownList_template.SelectedValue == "-1")
            {
                this.Literal_error.Text = "Select a valid template.";
            }
            else
            {
                string selectedValue = this.DropDownList_template.SelectedValue;
                foreach (TreeViewNode node in GetAtmNodes(this.TreeView_atms.Nodes))
                {
                    string str2 = node.ID.Substring(1);
                    if (node.Checked)
                    {
                        ConnectionFactory.ExecuteQuery(string.Format("delete atm_devices where atm_id={0}; insert atm_devices(atm_id,device_template_id,device_id,is_active,is_critical,is_present)\r\n                                                    select {0},device_template_id,device_id,1,1,is_present\r\n                                                    from device_template_devices where is_present=1 and device_template_id={1}", str2, selectedValue));
                        //BasePage.\\UILogger.Log(SeverityLevel.Trace, string.Format("Template {0} was assigned to atm {1} successfully", selectedValue, str2));
                    }
                    else
                    {
                        ConnectionFactory.ExecuteQuery(string.Format("delete atm_devices where atm_id={0} and device_template_id={1}", str2, selectedValue));
                        //BasePage.\\UILogger.Log(SeverityLevel.Trace, string.Format("Template {0} was deleted from atm {1} successfully", selectedValue, str2));
                    }
                }
                this.Literal_ok.Text = "<li>Templates assigned to ATMs successfully!</li>";
            }
        }
        catch (Exception exception)
        {
            //BasePage.\\UILogger.Log(SeverityLevel.Error, base.PageName + MethodBase.GetCurrentMethod(), "An Error Occured", exception);
            if (exception.Message == "Please select atleast one ATM")
            {
                this.Literal_error.Text = "<li>" + exception.Message + "</li>";
            }
            else
            {
                this.Literal_error.Text = "<li>An error occurred while assigning Template: [" + exception.Message + "]</li>";
            }
        }
    }

    protected TreeViewNode CreateATMNode(string text, int id)
    {
        TreeViewNode node = new TreeViewNode();
        node.Text = text;
        node.ID = "a" + id;
        node.ImageUrl = "treeATM.jpg";
        node.DroppingEnabled = false;
        node.ShowCheckBox = true;
        return node;
    }

    protected TreeViewNode CreateRegionNode(string text, bool expanded, int id)
    {
        TreeViewNode node = new TreeViewNode();
        node.Text = text;
        node.ID = "r" + id;
        node.ImageUrl = "treeRegion.jpg";
        node.Expanded = expanded;
        node.ShowCheckBox = true;
        return node;
    }

    protected void DropDownList_template_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.FillATMs(Convert.ToInt32(this.DropDownList_template.SelectedValue));
    }

    protected void FillATMs(int Id)
    {
        ArrayList selectedItem = new ArrayList();
        SqlCommand newCommand = null;
        SqlDataReader reader = null;
        try
        {
            //BasePage.\\UILogger.Log(SeverityLevel.Information, base.PageName + MethodBase.GetCurrentMethod(), "FillATMs");
            newCommand = ConnectionFactory.GetNewCommand(true);
            newCommand.CommandText = "select distinct(atm_id) from atm_devices where device_template_id=" + Id;
            reader = newCommand.ExecuteReader();
            while (reader.Read())
            {
                selectedItem.Add(reader.GetInt32(0));
            }
            if (selectedItem.Count > 0)
            {
                for (int i = 0; i < this.TreeView_atms.Nodes.Count; i++)
                {
                    this.setCheck(this.TreeView_atms.Nodes[i], selectedItem);
                }
            }
            else
            {
                this.TreeView_atms.UnCheckAll();
            }
        }
        catch (Exception exception)
        {
            //BasePage.\\UILogger.Log(SeverityLevel.Error, base.PageName + MethodBase.GetCurrentMethod(), "FillATMs Could not build ATM tree. ", exception);
            throw new Exception("Could not build ATM tree. " + exception.Message);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
            if (newCommand != null)
            {
                newCommand.Connection.Close();
                newCommand.Dispose();
            }
        }
    }

    private void FillTemplate(string templateName)
    {
        DeviceTemplate.DeviceTemplateReader reader = null;
        try
        {
            this.DropDownList_template.Items.Add(new ListItem(templateName, "-1"));
            reader = DeviceTemplate.ExecuteReader("is_active = 1");
            while (reader.Read())
            {
                this.DropDownList_template.Items.Add(new ListItem(reader.CurrentDeviceTemplate.DeviceTemplateName, reader.CurrentDeviceTemplate.DeviceTemplateId.ToString()));
            }
        }
        catch (Exception exception)
        {
            this.Literal_error.Text = "<li>No active device templates found</li>";
            //BasePage.\\UILogger.Log(SeverityLevel.Error, base.PageName + MethodBase.GetCurrentMethod(), "An Error Occured", exception);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Page.SetFocus(this.DropDownList_template);
        if (!base.IsPostBack)
        {
            this.BuildTree();
            if (!CheckRight((int)Permissions.AssignTemplate))
            {
                Response.Redirect("../admin/AccessDenied.aspx");
            }
            this.FillTemplate("--Select One--");
        }
    }

    protected void PopulateSubTree(DataRow parentRegionRow, TreeViewNode node, bool allowEditingRegion, bool allowEditingATM)
    {
        //BasePage.\\UILogger.Log(SeverityLevel.Information, base.PageName + MethodBase.GetCurrentMethod(), "PopulateSubTree");
        foreach (DataRow row in this.atmsDS.Tables[1].Select("parent_region_id=" + parentRegionRow["region_id"]))
        {
            TreeViewNode node2 = this.CreateRegionNode(row["region_name"].ToString(), false, (int) row["region_id"]);
            node.Nodes.Add(node2);
            node2.EditingEnabled = node2.DraggingEnabled = allowEditingRegion;
            this.PopulateSubTree(row, node2, allowEditingRegion, allowEditingATM);
        }
        foreach (DataRow row2 in parentRegionRow.GetChildRows("ATMRegionRelation"))
        {
            TreeViewNode node3 = this.CreateATMNode(row2["title"].ToString(), (int) row2["atm_id"]);
            node.Nodes.Add(node3);
            node3.EditingEnabled = node3.DraggingEnabled = allowEditingATM;
        }
    }

    private void setCheck(TreeViewNode node, ArrayList selectedItem)
    {
        TreeViewNodeCollection nodes = node.Nodes;
        int num = 0;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].ID.IndexOf('r') < 0)
            {
                if (selectedItem.IndexOf(int.Parse(nodes[i].ID.Substring(1))) >= 0)
                {
                    nodes[i].Checked = true;
                    num++;
                }
                else
                {
                    nodes[i].Checked = false;
                }
            }
            else if (nodes[i].Nodes.Count > 0)
            {
                this.setCheck(nodes[i], selectedItem);
                if (nodes[i].Checked)
                {
                    num++;
                }
            }
            else
            {
                nodes[i].Checked = true;
                num++;
            }
        }
        if ((num == nodes.Count) && (num != 0))
        {
            nodes[0].ParentNode.Checked = true;
        }
    }
    public static List<TreeViewNode> GetAtmNodes(TreeViewNodeCollection nodes)
    {
        List<TreeViewNode> list = new List<TreeViewNode>();
        Stack<TreeViewNode> stack = new Stack<TreeViewNode>();
        foreach (TreeViewNode node in nodes)
        {
            stack.Push(node);
        }
        while (stack.Count > 0)
        {
            TreeViewNode item = stack.Pop();
            if (item.ID[0] == 'r')
            {
                foreach (TreeViewNode node3 in item.Nodes)
                {
                    stack.Push(node3);
                }
            }
            else
            {
                list.Add(item);
            }
        }
        return list;
    }

 

 


}

 

 

}