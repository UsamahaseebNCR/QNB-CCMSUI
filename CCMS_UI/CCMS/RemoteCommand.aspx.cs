using Avanza.iSuite.UI;
using ComponentArt.Web.UI;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace CCMSUI.CCMS
{
    public class RemoteCommand : BasePage
    {

        private string snmpcommunity = "ABBA";

        private string inserviceSNMPobject;

        private int inserviceSNMPValue;

        private int outserviceSNMPValue;

        private string RestartSNMPobject;

        private int RestartSNMPValue;

        private int Rowcount;

        private List<string> arrip;

        public string AtmIP = "";

        public string Vender = "";

        protected DataSet atmsDS = null;

        protected Literal Literal_error;

        protected ValidationSummary ValidationSummary_ATM;

        protected Literal Literal_ok;

        protected Label Label1;

        protected DropDownList DropDownList_command;

        protected HtmlGenericControl Div_TreeView;

        protected ComponentArt.Web.UI.TreeView TreeView_ATM;

        protected Button Button_send;

        public int INSERVICESNMPINTVALUE
        {
            get
            {
                return this.inserviceSNMPValue;
            }
            set
            {
                this.inserviceSNMPValue = value;
            }
        }

        public string INSERVICESNMPOBJECT
        {
            get
            {
                return this.inserviceSNMPobject;
            }
            set
            {
                this.inserviceSNMPobject = value;
            }
        }

        public int OUTSERVICESNMPINTVALUE
        {
            get
            {
                return this.outserviceSNMPValue;
            }
            set
            {
                this.outserviceSNMPValue = value;
            }
        }

        public string RestartSNMPINTOBJECT
        {
            get
            {
                return this.RestartSNMPobject;
            }
            set
            {
                this.RestartSNMPobject = value;
            }
        }

        public int RestartSNMPINTVALUE
        {
            get
            {
                return this.RestartSNMPValue;
            }
            set
            {
                this.RestartSNMPValue = value;
            }
        }

        public int ROWCOUNT
        {
            get
            {
                return this.Rowcount;
            }
            set
            {
                this.Rowcount = value;
            }
        }

        public string SNMPCOM
        {
            get
            {
                return this.snmpcommunity;
            }
            set
            {
                this.snmpcommunity = value;
            }
        }

        public RemoteCommand()
        {
        }

        protected void BuildSubTree(DataRow parentRegionRow, TreeViewNode node, bool allowEditingRegion, bool allowEditingATM)
        {
            bool flag;
            DataRow[] dataRowArray = this.atmsDS.Tables[1].Select(string.Concat("parent_region_id = ", parentRegionRow["region_id"]));
            for (int i = 0; i < (int)dataRowArray.Length; i++)
            {
                DataRow childRow = dataRowArray[i];
                TreeViewNode childNode = this.CreateRegionNode(childRow["region_name"].ToString(), true, (int)childRow["region_id"]);
                node.Nodes.Add(childNode);
                bool flag1 = allowEditingRegion;
                flag = flag1;
                childNode.DraggingEnabled = flag1;
                childNode.EditingEnabled = flag;
                this.BuildSubTree(childRow, childNode, allowEditingRegion, allowEditingATM);
            }
            DataRow[] childRows = parentRegionRow.GetChildRows("ATMRegionRelation");
            for (int j = 0; j < (int)childRows.Length; j++)
            {
                DataRow childRow = childRows[j];
                TreeViewNode childNode = this.CreateATMNode(childRow["title"].ToString(), (int)childRow["atm_id"]);
                node.Nodes.Add(childNode);
                bool flag2 = allowEditingATM;
                flag = flag2;
                childNode.DraggingEnabled = flag2;
                childNode.EditingEnabled = flag;
            }
        }

        protected void BuildTree()
        {
            bool flag;
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            try
            {
                try
                {
                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandText = "select title, atm_id, region_id from atm where is_active = 1  order by title; select region_name, region_id, parent_region_id from region order by region_name";
                    adapter = new SqlDataAdapter(cmd);
                    this.atmsDS = new DataSet();
                    adapter.Fill(this.atmsDS);
                    this.Rowcount = this.atmsDS.Tables[0].Rows.Count;
                    this.atmsDS.Relations.Add("ATMRegionRelation", this.atmsDS.Tables[1].Columns["region_id"], this.atmsDS.Tables[0].Columns["region_id"]);
                    this.atmsDS.Relations.Add("RegionRegionRelation", this.atmsDS.Tables[1].Columns["parent_region_id"], this.atmsDS.Tables[1].Columns["region_id"], false);
                    foreach (DataRow dbRow in this.atmsDS.Tables[1].Rows)
                    {
                        if (dbRow.IsNull("parent_region_id"))
                        {
                            TreeViewNode newNode = this.CreateRegionNode(dbRow["region_name"].ToString(), true, (int)dbRow["region_id"]);
                            newNode.DraggingEnabled = false;
                            newNode.EditingEnabled = false;
                            newNode.Enabled = false;
                            this.TreeView_ATM.Nodes.Add(newNode);
                            this.BuildSubTree(dbRow, newNode, false, false);
                        }
                    }
                    foreach (DataRow dbRow in this.atmsDS.Tables[0].Rows)
                    {
                        if (dbRow.IsNull("region_id"))
                        {
                            TreeViewNode childNode = this.CreateATMNode(dbRow["title"].ToString(), (int)dbRow["atm_id"]);
                            childNode.DraggingEnabled = false;
                            childNode.EditingEnabled = false;
                            childNode.Nodes.Add(childNode);
                        }
                    }
                    if (this.TreeView_ATM.Nodes.Count <= 0)
                    {
                        this.TreeView_ATM.Height = 5;
                    }
                    else
                    {
                        this.TreeView_ATM.Height = 180;
                    }
                }
                catch (Exception exception)
                {
                    Exception ex = exception;
                    LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    this.Literal_error.Text = string.Concat("<li>An error occurred while building tree: [", ex.Message, "]</li>");
                }
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Connection.Close();
                    cmd.Dispose();
                }
                if (adapter != null)
                {
                    adapter.Dispose();
                }
            }
        }

        protected void Button_send_Click(object sender, EventArgs e)
        {
            try
            {
                this.arrip = new List<string>();
                this.setsnmp();
                foreach (TreeViewNode node in TreeView_ATM.CheckedNodes)
                {

                    this.getipandvender(int.Parse(node.ID.Substring(1)));
                    this.loadSnmpCommunity(this.AtmIP);
                    LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Info, string.Concat(this.AtmIP, " has been selected for remote command"));
                    this.arrip.Add(this.AtmIP);

                }
                if (arrip.Count > 0)
                {
                    RemoteCommand remoteCommand = this;
                    (new Thread(new ThreadStart(remoteCommand.ProcessSNMPIntSet))).Start();
                }
                //if (this.arrip.Count <= 0)
                //{
                //    this.Literal_error.Text = "<li> Remote command are currently active for NCR ATMs only </li>";
                //}
                //else
                //{
                //    this.Literal_error.Text = "<li>Command has been sent successfully</li>";
                //}
                //LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Info, string.Format("Going tO start SNMP Command Thread for {0} terminals", this.arrip.Count));

            }
            catch (Exception exception)
            {
                Exception ex = exception;
                LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_error.Text = string.Concat("<li>An error occurred while sending remote command: [", ex.Message, "]</li>");
            }
        }

        protected TreeViewNode CreateATMNode(string text, int id)
        {
            TreeViewNode node = new TreeViewNode()
            {
                Text = text,
                ID = string.Concat("a", id),
                ImageUrl = "treeATM.jpg",
                DroppingEnabled = false,
                ShowCheckBox = true
            };
            return node;
        }

        protected TreeViewNode CreateRegionNode(string text, bool expanded, int id)
        {
            TreeViewNode node = new TreeViewNode()
            {
                Text = text,
                ID = string.Concat("r", id),
                Expanded = expanded,
                ShowCheckBox = true,
                ImageUrl = "treeRegion.jpg"
            };
            return node;
        }

        public void getipandvender(int atmid)
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            try
            {
                //this.logger.Info("Set IP of ATMs");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = string.Concat("select ip from atm where atm_id = ", atmid) ?? "";
                (new SqlDataAdapter(cmd)).Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    this.AtmIP = dt.Rows[0][0].ToString();
                    //this.Vender = dt.Rows[0][1].ToString();
                }
            }
            catch (Exception exception)
            {
                Exception ex = exception;
                LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_error.Text = string.Concat("<li>An error occurred while get IP: [", ex.Message, "]</li>");
            }
        }

        private void loadSnmpCommunity(string ip)
        {
            //Check it later about SNMP Community

            //using (ISuiteDataContext dc = new ISuiteDataContext(ConnectionFactory.getConnection(DatabaseType.iSuite)))
            //{
            //    IQueryable<string> list = 
            //        from x in dc.servers
            //        where x.product_name == "iWatch"
            //        select x.dbtype_name;
            //    foreach (string serverName in list)
            //    {
            //        if (IwAtm.LoadIwAtm(string.Concat("ip = '", ip, "'"), (DatabaseType)Enum.Parse(typeof(DatabaseType), serverName)) != null)
            //        {
            //            IwAppSetting iWatchAppSettingObject = IwAppSetting.LoadIwAppSetting("1=1", (DatabaseType)Enum.Parse(typeof(DatabaseType), serverName));
            //            this.snmpcommunity = iWatchAppSettingObject.SnmpCommunityName;
            //            this.logger.Info(string.Concat(new string[] { "Community name[", this.snmpcommunity, "], atm ip [", ip, "]" }));
            //            break;
            //        }
            //    }
            //}
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.Page.IsPostBack)
                {
                    //this.logger.Info("Page_Load");
                    this.BuildTree();
                    this.Page.SetFocus(this.Button_send);
                }
            }
            catch (Exception exception)
            {
                Exception ex = exception;
                LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_error.Text = string.Concat("<li>An error occurred while loading page: [", ex.Message, "]</li>");
            }
        }

        public virtual void ProcessSNMPIntSet()
        {
            int nSERVICESNMPINTVALUE;
            LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Info, "GO TO RUN REMOTE COMMANDS ON ATMS");

            for (int i = 0; i < this.arrip.Count; i++)
            {
                try
                {
                    if (this.arrip[i].ToString() != "0.0.0.0")
                    {
                        if (this.DropDownList_command.SelectedValue == "InService")
                        {
                            LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Info, string.Concat("GO TO RUN INSERVICE COMMAND ON IP", this.arrip[i].ToString()));

                            string[] str = new string[] { this.arrip[i].ToString(), this.INSERVICESNMPOBJECT, "i", null };
                            nSERVICESNMPINTVALUE = this.INSERVICESNMPINTVALUE;
                            str[3] = nSERVICESNMPINTVALUE.ToString();
                            this.SNMPSet(str);
                        }
                        if (this.DropDownList_command.SelectedValue == "OutOfService")
                        {
                            LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Info, string.Concat("GO TO RUN OUTOFSERVICE COMMAND ON IP", this.arrip[i].ToString()));
                            string[] strArrays = new string[] { this.arrip[i].ToString(), this.INSERVICESNMPOBJECT, "i", null };
                            nSERVICESNMPINTVALUE = this.OUTSERVICESNMPINTVALUE;
                            strArrays[3] = nSERVICESNMPINTVALUE.ToString();
                            this.SNMPSet(strArrays);
                        }
                        if (this.DropDownList_command.SelectedValue == "Restart")
                        {
                            LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Info, string.Concat("GO TO RUN RESTART COMMAND ON IP", this.arrip[i].ToString()));
                            string[] str1 = new string[] { this.arrip[i].ToString(), this.RestartSNMPINTOBJECT, "i", null };
                            nSERVICESNMPINTVALUE = this.RestartSNMPINTVALUE;
                            str1[3] = nSERVICESNMPINTVALUE.ToString();
                            this.SNMPSet(str1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                }
            }
        }

        protected void setCheck(TreeViewNode node, ArrayList selectedItem)
        {
            TreeViewNodeCollection nodeCol = node.Nodes;
            int atms = 0;
            for (int counter = 0; counter < nodeCol.Count; counter++)
            {
                if (nodeCol[counter].ID.IndexOf('r') < 0)
                {
                    if (selectedItem.IndexOf(int.Parse(nodeCol[counter].ID.Substring(1))) >= 0)
                    {
                        nodeCol[counter].Checked = true;
                        atms++;
                    }
                }
                else if (nodeCol[counter].Nodes.Count <= 0)
                {
                    nodeCol[counter].Checked = true;
                    atms++;
                }
                else
                {
                    this.setCheck(nodeCol[counter], selectedItem);
                    if (nodeCol[counter].Checked)
                    {
                        atms++;
                    }
                }
            }
            if ((atms != nodeCol.Count ? false : atms != 0))
            {
                nodeCol[0].ParentNode.Checked = true;
            }
        }


        public void setsnmp()
        {
            try
            {
                this.inserviceSNMPobject = ".1.3.6.1.4.1.191.39.13.2.1.7.0";
                this.inserviceSNMPValue = 1;
                this.INSERVICESNMPOBJECT = ".1.3.6.1.4.1.191.39.13.2.1.7.0";
                this.INSERVICESNMPINTVALUE = 1;
                this.outserviceSNMPValue = 2;
                this.RestartSNMPobject = ".1.3.6.1.4.1.191.39.13.2.1.8.0";
                this.RestartSNMPValue = 2;
                this.RestartSNMPINTOBJECT = ".1.3.6.1.4.1.191.39.13.2.1.8.0";
                this.RestartSNMPINTVALUE = 2;
            }
            catch (Exception exception)
            {
                Exception ex = exception;
                LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_error.Text = string.Concat("<li>An error occurred while configuring snmp: [", ex.Message, "]</li>");
            }
        }

        public void SNMPSet(string[] args)
        {
            IPAddress ip;
            ISnmpData data;
            this.loadSnmpCommunity(args[0]);
            VersionCode version = VersionCode.V1;
            int timeout = 90000;
            if (args.Length != 0)
            {
                IPAddress.TryParse(args[0], out ip);
                try
                {
                    List<Variable> vList = new List<Variable>();
                    char chr = args[2][0];
                    if (chr <= 'd')
                    {
                        if (chr == 'a')
                        {
                            data = new IP(args[3]);
                        }
                        else
                        {
                            if (chr != 'd')
                            {
                                Console.WriteLine(string.Concat("unknown type string: ", args[2]));
                                return;
                            }
                            data = new OctetString(ByteTool.ConvertDecimal(args[3]));
                        }
                    }
                    else if (chr == 'i')
                    {
                        data = new Integer32(int.Parse(args[3]));
                    }
                    else
                    {
                        switch (chr)
                        {
                            case 'n':
                                {
                                    data = new Null();
                                    break;
                                }
                            case 'o':
                                {
                                    data = new ObjectIdentifier(args[3]);
                                    break;
                                }
                            case 'p':
                            case 'q':
                            case 'r':
                            case 'v':
                            case 'w':
                                {
                                    Console.WriteLine(string.Concat("unknown type string: ", args[2]));
                                    return;
                                }
                            case 's':
                                {
                                    data = new OctetString(args[3]);
                                    break;
                                }
                            case 't':
                                {
                                    data = new TimeTicks(uint.Parse(args[3]));
                                    break;
                                }
                            case 'u':
                                {
                                    data = new Gauge32(uint.Parse(args[3]));
                                    break;
                                }
                            case 'x':
                                {
                                    data = new OctetString(ByteTool.Convert(args[3]));
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine(string.Concat("unknown type string: ", args[2]));
                                    return;
                                }
                        }
                    }
                    Variable command = new Variable(new ObjectIdentifier(args[1]), data);
                    vList.Add(command);
                    //this.logger.Info(string.Concat("Setting snmp command to atm: ", ip.ToString()));
                    IPEndPoint receiver = new IPEndPoint(ip, 161);
                    if (version != VersionCode.V3)
                    {
                        foreach (Variable variable in Messenger.Set(version, receiver, new OctetString(this.snmpcommunity), vList, timeout))
                        {
                            LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Info, variable.ToString());

                         }
                    }
                }
                catch (Exception ex)
                {
                    Literal_error.Text = ex.Message;
                    LogableTask.LogMonoActivityTask("RemoteCommand", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

                }

            }
        }
    }
}