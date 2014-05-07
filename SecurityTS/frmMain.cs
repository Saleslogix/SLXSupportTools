using System;
using Microsoft.Web.Administration;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.OracleClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Net;
using System.Xml;
using System.ServiceProcess;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using System.Web.Configuration;


namespace SecurityTS
{
    public partial class frmMainPanel : Form
    {
        #region Variables
        public TS ts = new TS();            //Object for the Troubleshooter tests

        //flags to prevent control execution during data loads
        public bool siteload = false;
        public bool SQLLoad = false;
        public bool DBLoad = false;
        public bool IIS = false;
        public bool TVCheck = false;
 
        //Seccodejoins datatable components
        public DataTable SCJ = new DataTable();
        public DataColumn TOwner = new DataColumn("Team/User/Dept");
        public DataColumn Permission = new DataColumn("Profile");
        public DataColumn Source = new DataColumn("Has access from");
        
        //Secrights datatable components
        public DataTable SR = new DataTable();
        public DataColumn SROwner = new DataColumn("Team/User/Dept");
        public DataColumn SRPermission = new DataColumn("Profile");
        
        //tabpages to disable when there is no database connection
        public int[] epages = { 1, 2, 3, 4, 5};
        public int[] SQLpages = { 1, 2, 3, 4, 5};
        public int[] SLXpages = { 2, 3, 4, 5};

        //string to contain seccodes to be ignored during user security test
        public string ignorelist = "";
        
        //Create Datasets
        public DataSet test = new DataSet();
        public DataSet TSDataSet = new DataSet();
        public DataSet SRDataSet = new DataSet();
        public DataSet TestDataSet = new DataSet();

        

        //holder for IIS servermanager object
        public ServerManager iisManager;
       

        //list of servers and aliases
        ArrayList slxservers = new ArrayList();
        ArrayList slxaliases = new ArrayList();

        #endregion       
        
        public frmMainPanel()
        {
            InitializeComponent();

            //get machine name
            txtServer.Text = System.Environment.MachineName;

        
            //Set up Columns for security datatables
            SCJ.Columns.Add(TOwner);
            SCJ.Columns.Add(Permission);
            SCJ.Columns.Add(Source);
            SR.Columns.Add(SROwner);
            SR.Columns.Add(SRPermission);

            DbConnection(epages, false);            
            
            //Start logging
            rtbLog.Text = "Session Started: " + DateTime.Now.ToString() +"\r\n";
            
            //get machine name
            txtServer.Text = System.Environment.MachineName;
            rtbLog.Text += "Machine Name: " + txtServer.Text + "\r\n";
            
            //get .net versions
            DataTable dotnet = ts.getdotnet();
            
            //display in log
            rtbLog.Text += "\r\n";
            rtbLog.Text += ".Net".PadRight(16) + "SP".PadRight(8) + "Version\r\n";
            foreach (DataRow row in dotnet.Rows)
            {
                rtbLog.Text += row.ItemArray[0].ToString().PadRight(16) + row.ItemArray[1].ToString().PadRight(8) + row.ItemArray[2].ToString() + "\r\n";
            }
            
            //display in datagrid
            dgvNet.DataSource = dotnet;

            //get Browser versions
            rtbLog.Text += "\r\n";
            rtbLog.Text += "Browser".PadRight(24) + "Version\r\n";
            
            //add to grid and display in log
            foreach (DataRow row in ts.BrowserVersion().Rows)
            {
                dgvBrowsers.Rows.Add(row.ItemArray[0].ToString(), row.ItemArray[1].ToString());
                rtbLog.Text += row.ItemArray[0].ToString().PadRight(24) + row.ItemArray[1].ToString() + "\r\n";
            } 
        }
        
        //open connection
        private void btnOpen_Click(object sender, EventArgs e)
        {
            //flag for connection readiness
            bool ConnReady = false;

            //confirm data for connection
            switch (cboConnectionType.Text)
            {
                case "SQL":
                    {
                        //set connection type
                        ts.SLXDB.DBType = SecurityTS.DB.connectType.SQL;
                        
                        //test data needed exists
                        if (txtServer.Text.Length>0 && txtUser.Text.Length>0)
                        {
                            //Mark Connection as ready
                            ConnReady = true;
                        }
                        break;
                    }
                case "Saleslogix":
                    {
                        //set connection type
                        ts.SLXDB.DBType = SecurityTS.DB.connectType.SalesLogix;

                        //test data needed exists

                        if (txtServer.Text.Length > 0 && txtSLXDatabase.Text.Length > 0 && txtUser.Text.Length > 0)
                        {
                            //Mark Connection as ready
                            ConnReady = true;
                        }
                        break;
                    }
            }

            //Make connection if all information is present
            if (ConnReady)
            {
                //Populate connection info
                ts.SLXDB.srv = txtServer.Text;
                if (ts.SLXDB.DBType==SecurityTS.DB.connectType.SQL)
                {
                    ts.SLXDB.db = txtDatabase.Text;    
                }
                else
                {
                    ts.SLXDB.db = txtSLXDatabase.Text;
                }
                
                ts.SLXDB.user = txtUser.Text;
                ts.SLXDB.password = txtPassword.Text;

                //Build the connection
                ts.SLXDB.BuildConn();

                try
                {
                    //open connection
                    ts.SQLOpen(ts.SLXDB);

                    //set status
                    setstatus(true);

                    //check if connection has a DB assigned
                    if (ts.SLXDB.db.Length > 0)
                    {
                        DatabaseConnected();
                        //enable tabpages that need a database connection
                        switch (ts.SLXDB.DBType)
                        {
                            case DB.connectType.None:
                                break;
                            case DB.connectType.SalesLogix:
                                DbConnection(SLXpages, true);
                                break;
                            case DB.connectType.SQL:
                                DbConnection(SQLpages, true);
                                break;
                            case DB.connectType.Oracle:
                                break;
                            default:
                                break;
                        }
                        tsDatabase.Text = ts.SLXDB.db;
                    }

                    //Log SQL connection
                    rtbLog.Text += "Connected to " + ts.SLXDB.srv + "\r\n";

                    //populate DB cbox
                    if (ts.SLXDB.DBType == SecurityTS.DB.connectType.SQL && txtDatabase.Text.Length < 1)
                    {
                        GetDBs();
                    }
                    if (txtDatabase.Text.Length >0)
                    {
                        //Log Database connection
                        rtbLog.Text += "Connected to " + ts.SLXDB.db + " Database\r\n";
                        tsDatabase.Text = ts.SLXDB.db;
                    }
                    connectionEnable(true);  
                }
                catch
                {
                    //Show error and set status
                    setstatus(false);
                    MessageBox.Show("Failed to connect");
                    connectionEnable(false);
                }
            }
        }

        //Controls to enable\disable when a connection is active or inactive
        public void connectionEnable(bool connected)
        {
            btnOpen.Enabled = !connected;
            cboConnectionType.Enabled = !connected;
            txtServer.Enabled = !connected;
            txtUser.Enabled = !connected;
            txtPassword.Enabled = !connected;
            btnClose.Enabled = connected;
            txtDatabase.Enabled = !connected;
            txtSLXDatabase.Enabled = !connected;

        }

        //close connection
        private void btnClose_Click(object sender, EventArgs e)
        {
            //closes the connections
            ts.cn.Close();
            ts.ocn.Close();
            ts.scn.Close();

            //Set status
            setstatus(false);

            //disable controls that need a connection
            DbConnection(epages, false);
            
            //Clear controls
            dgvSCJ.Rows.Clear();
            dgvSR.Rows.Clear();
            cboSLXUser.DataSource = null;
            cboSLXUser.Items.Clear();
            txtDatabase.Text="";
            connectionEnable(false);
        }

        //get installed apps versions
        private void cmdGetFiles_Click(object sender, EventArgs e)
        {
                //display Versions found
                MessageBox.Show(ts.AppsCheck(),"SalesLogix Apps Detected");
        }

        //Find file in Version check
        private void btnFind_Click(object sender, EventArgs e)
        {
            //locate file(s) desired
            string messagestr = "";  //string to hold results

            messagestr = ts.GetFileByName(txtFind.Text);
            
            //show results
            if (messagestr.Length==0)
            {
                messagestr = "File not found.";
            }
            MessageBox.Show(messagestr, "Find file: " + txtFind.Text);
        }
        
        //Change file path
        private void btnPath_Click(object sender, EventArgs e)
        {
            //check if path is already set
            if (txtPath.Text.Length > 0)
            {
                //open to set path
                fbdOpen.SelectedPath = txtPath.Text;
            }
            //show path dialog
            if (fbdOpen.ShowDialog() == DialogResult.OK)
            {
                //set path if selected
                txtPath.Text = fbdOpen.SelectedPath;
            }
        }
        
        //Load files for version info
        private void btnSLX_Click(object sender, EventArgs e)
        {
            //holder for installed programs results
            string sb = "";

            //get Installed
            sb = ts.GetInstalled();
            
            //display on form
            txtInstalled.Text = sb;
            
            //get installpath
            if (txtPath.Text.Length==0 && ts.InstallPath.Length>0)
            {
                txtPath.Text = ts.InstallPath;
            }
           
            //display in internal log
            rtbLog.Text += sb;
            if (txtPath.Text.Length==0)
            {
                if (Directory.Exists(@"C:\program files\SalesLogix"))
                {
                    txtPath.Text = @"C:\program files\SalesLogix";
                }
                else
                {
                    if (Directory.Exists(@"C:\program files (x86)\SalesLogix"))
                    {
                        txtPath.Text = @"C:\program files\SalesLogix";
                    }
                }
                
            }
            if (txtPath.Text.Length > 0)
            {
                //get files
                ts.GetFiles(txtPath.Text);

                //clear list
                lstFiles.Items.Clear();

                //Initialize Counter
                if (ts.files.Count > 0)
                {
                    //set Progress Block size
                    float block = InitiateProgressBar(ts.files.Count);

                    //set file counter
                    int i = 0;

                    //limit results to 10000
                    if (ts.files.Count < 10001)
                    {
                        //progress counter 
                        int jj = 0;

                        //load file Information
                        foreach (string File in ts.files)
                        {
                            // Show the complete file path (including file name)
                            if (File.Contains(".")&&(File.Substring(File.LastIndexOf('.')).ToUpper() == ".EXE" || File.Substring(File.LastIndexOf('.')).ToUpper() == ".DLL"))
                            {
                                //Get FileVersionInfo
                                ts.myFI[i] = FileVersionInfo.GetVersionInfo(File);

                                //Add Filename to a list of files
                                ts.filename.Add(ts.myFI[i].FileName.Substring(ts.myFI[i].FileName.LastIndexOf('\\') + 1).ToLower());

                                //add file to listbox
                                lstFiles.Items.Add(ts.filename[i]);

                                //increment file number
                                i += 1;
                            }

                            //increment progress
                            jj += 1;

                            //test if block in progress bar needs increase
                            if (jj > block)
                            {
                                IncementProgressBar();
                                jj = 0;
                            }
                        }

                        //reset the bar
                        ResetProgressBar();

                        //enable Controls
                        btnExport.Enabled = true;
                        btnFind.Enabled = true;
                        cmdGetFiles.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("More than 10,000 files found try with a more restrictive path");
                    }
                }
            }
            
        }

        //Set values for Progressbar
        private float InitiateProgressBar(int TotalCount)
        {
            //holder for block size
            float block = 0;

            //if there are more than 100 items
            if (TotalCount >= 100)
            {
                //set block size to 1/100th
                block = (float)TotalCount / 100;
                
                //set maximum to 100
                toolStripProgressBar1.Maximum = 100;
            }

            //if less than 100
            if(TotalCount<100 && TotalCount>0)
            {
                //set block size to 1
                block = 1;
                //set maximum to Count
                toolStripProgressBar1.Maximum = TotalCount;
            }

            //return block size
            return block;
        }

        
        //Increment Progress
        private void IncementProgressBar()
        {
            toolStripProgressBar1.Value += 1;
        }

        //Reset Progress
        private void ResetProgressBar()
        {
            toolStripProgressBar1.Value = 0;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //Export Fileinfor to file
            ts.Export("FileList.csv");
            MessageBox.Show("C:\\FileList.csv created", "Export Complete");
        }
        
        private void btnHandler_Click(object sender, EventArgs e)
        {
            if (IIS)
            {
                string webserver = "";
                if (webserver.Length>0)
                {
                    webserver = txtWebServer.Text;
                }
                using (iisManager = ts.GetIIS(webserver))
                {
                    //get configuration
                    Microsoft.Web.Administration.Configuration siteConfig = iisManager.GetApplicationHostConfiguration();

                    //Get handlers section
                    Microsoft.Web.Administration.ConfigurationSection handlersSection = siteConfig.GetSection("system.webServer/handlers");

                    //Get elements
                    Microsoft.Web.Administration.ConfigurationElementCollection handlersCollection = handlersSection.GetCollection();

                    //string for report
                    string report = "";

                    //walk through handlers looking for requested one
                    foreach (Microsoft.Web.Administration.ConfigurationElement handler in handlersCollection)
                    {
                        //test for handler
                        if (handler.GetAttributeValue("Name").ToString() == "SimpleHandlerFactory-ISAPI-2.0")
                        {
                            //set up report string
                            report = "Handler Mappings\r\n";
                            report += handler.GetAttributeValue("Name") + "\r\n";
                            report += handler.GetAttributeValue("verb") + "\r\n" + "\r\n";
                        }
                    }

                    //show report
                    MessageBox.Show(report, "Handler Mapping");
                }
            }
            else
            {
                MessageBox.Show("No IIS found");
            }
        }
        private void cmdGetLogs_Click(object sender, EventArgs e)
        {
           //counters
            int infoerr = 0;
            int warnerr = 0;
            int erroerr = 0;
                      
            //Get application log records
            EventLog eventLog1 = new EventLog(cboEvent.Text, System.Environment.MachineName);
            
            //counter
            int jj = 0;


            float block = InitiateProgressBar(eventLog1.Entries.Count);
             
            //proccess each log
            foreach (System.Diagnostics.EventLogEntry entry in eventLog1.Entries)
            {
                //check for SLX/SalesLogix
                if (entry.Source.Contains("SalesLogix") || entry.Source.Contains("SLX") || entry.Source.Contains("slx") || entry.Source.Contains("saleslogix"))
                {
                    //switch by log type
                    switch (entry.EntryType.ToString())
                    {
                        case "Error":
                            {
                                //add to datagrid if error checkbox was set
                                if (cbError.Checked)
                                {
                                    dgvELog.Rows.Add(entry.Source, entry.EntryType, entry.TimeGenerated, entry.Message);
                                    erroerr += 1;
                                }
                                break;
                            }
                        case "Warning":
                            {
                                //add to datagrid if warning checkbox was set
                                if (cbWarn.Checked)
                                {
                                    dgvELog.Rows.Add(entry.Source, entry.EntryType, entry.TimeGenerated, entry.Message);
                                    warnerr += 1;
                                }
                                break;
                            }
                        case "Information":
                            {
                                //add to datagrid if Information checkbox was set
                                if (cbInfo.Checked)
                                {
                                    dgvELog.Rows.Add(entry.Source, entry.EntryType, entry.TimeGenerated, entry.Message);
                                    infoerr += 1;
                                }
                                break;
                            }
                    }
                }

                jj += 1;
                if (jj>block)
                {
                    IncementProgressBar();
                    jj = 0;
                }
            }
            ResetProgressBar();

            //set count and Find status
            lblFindQ.Text = "No";
            lblTime.Text = "none";
            
            //log events
            rtbLog.Text += "\r\nSalesLogix Events\r\n";
            if (cbError.Checked)
            {
                rtbLog.Text += "Errors:  " + erroerr + "\r\n";
            }
            if (cbWarn.Checked)
            {
                rtbLog.Text += "Warnings:  " + warnerr + "\r\n";
            }
            if (cbError.Checked)
            {
                rtbLog.Text += "Information:  " + infoerr + "\r\n";
            }
            countEvents();
        }
        public void loadServerGrids()
        {
                    if (dgvSLXServers.Rows.Count > 0)
                    {
                        //skip load
                    }
                    else
                    {
                        //Get slx servers
                        ArrayList slxservers = ts.GetServerList();

                        //holder for index of current machine
                        int rowindex = -1;

                        //populate grid
                        foreach (var server in slxservers)
                        {
                            //add row
                            dgvSLXServers.Rows.Add(server.ToString());

                            //check if row is current machine
                            if (server.ToString() == System.Environment.MachineName)
                            {
                                //store index
                                rowindex = dgvSLXServers.Rows.Count - 1;
                            }
                        }

                        //set selection to current server if possible
                        foreach (DataGridViewRow row in dgvSLXServers.SelectedRows)
                        {
                            //unselect any selected rows
                            row.Selected = false;
                        }

                        //if current machine exists
                        if (rowindex > -1)
                        {
                            //select current machine
                            dgvSLXServers.Rows[rowindex].Selected = true;
                        }
                    }
            
            if (dgvSQLServers.Rows.Count > 0)
                    {

                    }
                    else
                    {
                        //Load SQL Servers
                        //clear grid
                        dgvSQLServers.Rows.Clear();

                        //get datatable of instances
                        System.Data.DataTable SQLdataTable = LoadSQL();

                        //cycle through table and add results to datagrid
                        foreach (DataRow row in SQLdataTable.Rows)
                        {
                            dgvSQLServers.Rows.Add(row[0].ToString(), row[1].ToString(), row[3].ToString());
                        }

                        //unselect first row
                        if (dgvSQLServers.SelectedRows.Count > 0)
                        {
                            dgvSQLServers.SelectedRows[0].Selected = false;
                        }

                        //override sort
                        dgvSQLServers.Sort(dgvSQLServers.Columns[0], ListSortDirection.Ascending);
                    }
 
         }

        private void cboSLXUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Holder for seccodeid
            string seccodeid = "";

            //Lists for user records
            List<DataGridViewRow> uSCJ = new List<DataGridViewRow>();
            List<DataGridViewRow> uSR = new List<DataGridViewRow>();
                        
            //clear existing data
            dgvSCJ.Rows.Clear();
            dgvSR.Rows.Clear();
            cbUserid.Checked = false;

            try
            {
                //clear SCJ Datatable
                SCJ.Rows.Clear();

                //get selected user
                string username = cboSLXUser.SelectedItem.ToString();

                //flag for user found
                Boolean userfound = false;

                //holder for base seccode id
                string bseccodeid = "";

                //set counter
                int i = 0;


                while (!userfound)
                {
                    //loop through datatable rows till user is found
                    if (ts.LUDataSet.Tables["seccode"].Rows[i].ItemArray[1].ToString() != username)
                    {
                        //increment counter
                        i += 1;
                    }
                    else
                    {
                        //set base seccode id
                        bseccodeid = ts.LUDataSet.Tables["seccode"].Rows[i]["seccodeid"].ToString();

                        //set flag
                        userfound = true;
                    }
                }

                //set seccodeid to base seccode id
                seccodeid = bseccodeid;

                //add id to ignore list
                ignorelist = "'" + seccodeid + "'";

                //get dataset of all ids with access to the seccodeid
                TSDataSet = ts.GetDataset("select a1.parentseccodeid as SECCODEID,a2.seccodedesc as Owner, a3.profiledescription as Permission, 'Base' as Source,a1.childseccodeid as accessid from sysdba.seccodejoins a1 Left Join sysdba.seccode a2 on a1.parentseccodeid=a2.seccodeid left join sysdba.secprofile a3 on a1.profileid=a3.profileid  where a1.childseccodeid='" + seccodeid + "'", "SCJ", TSDataSet);

                //process the data
                CheckDataSet(TSDataSet.Tables["SCJ"], "SCJ",seccodeid);

                //Add data to uLists
                foreach (DataRow row in SCJ.Rows)
                {
                    if (row.ItemArray[0].ToString()==username)
                    {
                        DataGridViewRow SCJRow = new DataGridViewRow();
                        SCJRow.CreateCells(dgvSCJ);
                        SCJRow.SetValues(row.ItemArray[0], "Team Owner Profile", row.ItemArray[2]);
                        uSCJ.Add(SCJRow);
                    }
                    else
                    {
                        DataGridViewRow SCJRow = new DataGridViewRow();
                        SCJRow.CreateCells(dgvSCJ);
                        SCJRow.SetValues(row.ItemArray[0], row.ItemArray[1], row.ItemArray[2]);
                        uSCJ.Add(SCJRow);
                    }
                }

                //clear the secrights datatable
                SR.Rows.Clear();

                //set seccodeid to base seccode id
                seccodeid = bseccodeid;

                //add to ignore list
                ignorelist = "'" + seccodeid + "'";

                //Get user record for seccodeid
                ts.GetLookupDataset("Select userid from sysdba.usersecurity where defaultseccodeid='" + seccodeid + "'", "user");

                //if user is found get what they have access to
                if (ts.LUDataSet.Tables["user"].Rows.Count > 0)
                {
                    SRDataSet = ts.GetDataset("select a1.seccodeid as SECCODEID,a2.seccodedesc as Owner, a3.profiledescription as Permission,a1.accessid as accessid from sysdba.secrights a1 Left Join sysdba.seccode a2 on a1.seccodeid=a2.seccodeid left join sysdba.secprofile a3 on a1.profileid=a3.profileid  where a1.accessid='" + ts.LUDataSet.Tables["user"].Rows[0][0].ToString() + "'", "SR", SRDataSet);

                }
                else
                {
                    //if no user found use seccodeid
                    SRDataSet = ts.GetDataset("select a1.seccodeid as SECCODEID,a2.seccodedesc as Owner, a3.profiledescription as Permission,a1.accessid as accessid from sysdba.secrights a1 Left Join sysdba.seccode a2 on a1.seccodeid=a2.seccodeid left join sysdba.secprofile a3 on a1.profileid=a3.profileid  where a1.accessid='" + seccodeid + "'", "SR", SRDataSet);

                }

                //process data
                CheckDataSet(SRDataSet.Tables["SR"], "SR",seccodeid);

                //add data to grid
                foreach (DataRow row in SR.Rows)
                {
                    if (row.ItemArray[0].ToString() == "" & row.ItemArray[1].ToString() == "Team Owner Profile")
                    {
                        cbUserid.Checked = true;
                    }
                    else
                    {
                        DataGridViewRow SRRow = new DataGridViewRow();
                        SRRow.CreateCells(dgvSR);
                        SRRow.SetValues(row.ItemArray[0], row.ItemArray[1]);
                        uSR.Add(SRRow);
                    }
                }
                //Lists for mismatched records
                List<DataGridViewRow> LSR = new List<DataGridViewRow>();
                List<DataGridViewRow> LSCJ = new List<DataGridViewRow>();

                //locate matched rows
                List<bool> SCJList = new List<bool>();
                List<bool> SRList = new List<bool>();

                // add list record
                for (int ll = 0; ll < uSCJ.Count; ll++)
                {
                    SCJList.Add(false);    
                }
                for (int lm = 0; lm < uSR.Count; lm++)
                {
                    SRList.Add(false);
                }

                //cycle through seccodejoins grid
                for (int y = 0; y < uSCJ.Count; y++)
                {
                    //cycle through uSR List
                    for (int x = 0; x < uSR.Count; x++)
                    {
                        //if rows match change to true
                        if (uSCJ[y].Cells[0].Value.ToString() == uSR[x].Cells[0].Value.ToString() && uSCJ[y].Cells[1].Value.ToString() == uSR[x].Cells[1].Value.ToString())
                        {
                            //if match set true in lists
                            SCJList[y] = true;
                            SRList[x] = true;
                        }
                       
                    }
                }

                //make lists of non matched items
                int scjcount = 0;
                foreach (DataGridViewRow row in uSCJ)
                {
                    if (!SCJList[scjcount])
                    {
                        LSCJ.Add(row);
                    }
                    scjcount += 1;
                }
                int srcount = 0;
                foreach (DataGridViewRow row in uSR)
                {
                    if (!SRList[srcount])
                    {
                        LSR.Add(row);
                    }
                    srcount += 1;
                }
               
                //add to datagrids
                for (int nSCJ = 0; nSCJ < LSCJ.Count; nSCJ++)
                {
                    dgvSCJ.Rows.AddRange(LSCJ[nSCJ]);
                }
                for (int nSR = 0; nSR < LSR.Count; nSR++)
                {
                    dgvSR.Rows.AddRange(LSR[nSR]);
                }

            }

            //Using try\catch to prevent errors when no user is selected
            catch(Exception ex)
            {
            }
        }
        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string to build version display
            string versioninfo = "";

            //determine item selected
            int ii = lstFiles.SelectedIndex;

            //build string
            versioninfo += ts.myFI[ii].FileName + "\r\n";
            versioninfo += "Product Name:\t" + ts.myFI[ii].ProductName + "\r\n";
            versioninfo += "Product Version:\t" + ts.myFI[ii].ProductVersion + "\r\n";
            versioninfo += "Major Part:\t" + ts.myFI[ii].ProductMajorPart + "\r\n";
            versioninfo += "Minor Part:\t" + ts.myFI[ii].ProductMinorPart + "\r\n";
            versioninfo += "Build Part:\t" + ts.myFI[ii].ProductBuildPart + "\r\n";
            versioninfo += "Private Part:\t" + ts.myFI[ii].ProductPrivatePart;
            
            //display product info
            txtProductInfo.Text = versioninfo;
            
            //clear string
            versioninfo = "";

            //build string
            versioninfo += "File Name:\t" + ts.filename[ii] + "\r\n";
            versioninfo += "File Version:\t" + ts.myFI[ii].FileVersion + "\r\n";
            versioninfo += "Major Part:\t" + ts.myFI[ii].FileMajorPart + "\r\n";
            versioninfo += "Minor Part:\t" + ts.myFI[ii].FileMinorPart + "\r\n";
            versioninfo += "Build Part:\t" + ts.myFI[ii].FileBuildPart + "\r\n";
            versioninfo += "Private Part:\t" + ts.myFI[ii].FilePrivatePart;
            
            //display version info
            txtFileInfo.Text = versioninfo;
        }
        private void tcTroubleshooter_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (tcTroubleshooter.SelectedTab.Name == "tpIIS")
            {
                //Run Web Server population code when tab is accessed
                //if (ts.CheckLibrary("Microsoft.Web.Administration.dll"))
                try
                {
                    RunTests();
                    IIS = true;
                }
                catch
                {
                    MessageBox.Show("IIS not found");
                    IIS = false;
                }
            }
        }
        private void dgvSites_SelectionChanged(object sender, EventArgs e)
        {
            //if not during siteload populate Applications
            if (siteload == false)
            {
                PopAppsGrid(dgvSites.SelectedRows[0].Index);
            }
        }
        public void DbConnection(int[] pages,bool connected)
        {
            //cycle through the pages
            foreach (int page in pages)
            {
                //set controls.enabled property for tabpage
                foreach (Control item in tcSQL.TabPages[page].Controls)
                {
                    item.Enabled = connected;
                }
            }       
        }
        public void DbConnection(int page, bool connected)
        {
            //cycle through the pages
            //set controls.enabled property for tabpage
                foreach (Control item in tcSQL.TabPages[page].Controls)
                {
                    item.Enabled = connected;
                }
            
        }
        public void TSConnection(int page, bool connected)
        {
            //cycle through the pages
            //set controls.enabled property for tabpage
            foreach (Control item in tcTroubleshooter.TabPages[page].Controls)
            {
                item.Enabled = connected;
            }

        }       
        private void GetDBs()
        {
            DBLoad = true;
            //Get list of SQL databases
            ts.GetLookupDataset("Select name FROM sys.databases", "databases");

            //clear grid
            dgvDBs.Rows.Clear();

            //add databases to cbox
            foreach (DataRow item in ts.LUDataSet.Tables["databases"].Rows)
            {
                //is database saleslogix?
                try
                {
                    ts.GetLookupDataset("Select dbversion from [" + item.ItemArray[0].ToString() + "].sysdba.SYSTEMINFO", "version");
                    dgvDBs.Rows.Add(item.ItemArray[0].ToString(),ts.LUDataSet.Tables["version"].Rows[0].ItemArray[0].ToString());
                    dgvDBs.Sort(dgvDBs.Columns[0],ListSortDirection.Ascending);
                    dgvDBs.SelectedRows[0].Selected = false;
                }
                catch
                {
                }
            }
            DBLoad = false;
        }      
        public void CheckDataSet(DataTable testData, String Dset, string seccodeid)
        {
            //set string variable to hold type
            string typevalue = "";

            //for each member in the dataset
            for (int i = 0; i < testData.Rows.Count; i++)
            {
                //add record to dataset and set type value
                if (Dset == "SCJ")
                {
                    SCJ.Rows.Add(testData.Rows[i]["Owner"].ToString(), testData.Rows[i]["Permission"].ToString(), testData.Rows[i]["Source"].ToString());
                    typevalue = "N";
                }
                else
                {
                    SR.Rows.Add(testData.Rows[i]["Owner"].ToString(), testData.Rows[i]["Permission"].ToString());
                    typevalue = "U";
                }

                //get seccodeid from testdata
                string Tseccodeid = testData.Rows[i]["SECCODEID"].ToString();
                
                //add to ignore list for next level
                if (ignorelist.Length == 0)
                {
                    ignorelist += "'" + Tseccodeid + "'";
                }
                else
                {
                    ignorelist += ",'" + Tseccodeid + "'";
                }
                
                //if seccodeid for row does not match the seccode of the selected user and seccodeid is not of type value
                if ((testData.Rows[i]["SECCODEID"].ToString() != seccodeid) && (testData.Rows[i]["SECCODEID"].ToString().Substring(0, 1) != typevalue))
                {
                    //if Seccodejoins is the dataset
                    if (Dset == "SCJ")
                    {
                        //get a dataset of related records from seccodejoins
                        ts.GetDataset("select a1.parentseccodeid as SECCODEID,a2.seccodedesc as Owner, a3.profiledescription as Permission, '" + testData.Rows[i]["Owner"].ToString() + "' as Source,a1.childseccodeid as accessid from sysdba.seccodejoins a1 Left Join sysdba.seccode a2 on a1.parentseccodeid=a2.seccodeid left join sysdba.secprofile a3 on a1.profileid=a3.profileid  where a1.childseccodeid='" + Tseccodeid + "' and parentseccodeid not in (" + ignorelist + ")", Tseccodeid, TSDataSet);
                        
                        //if the dataset has records
                        if (TSDataSet.Tables[Tseccodeid].Rows.Count > 0)
                        {
                            //repeat above for new dataset
                            CheckDataSet(TSDataSet.Tables[Tseccodeid], "SCJ",seccodeid);
                        }
                    }
                    else
                    {
                        //get a dataset of related records from secrights
                        ts.GetDataset("select a1.seccodeid as SECCODEID,a2.seccodedesc as Owner, a3.profiledescription as Permission, a1.accessid as accessid from sysdba.secrights a1 Left Join sysdba.seccode a2 on a1.seccodeid=a2.seccodeid left join sysdba.secprofile a3 on a1.profileid=a3.profileid  where a1.accessid='" + Tseccodeid + "' and a1.seccodeid not in (" + ignorelist + ")", Tseccodeid, TSDataSet);

                        //if the dataset has records
                        if (TSDataSet.Tables[Tseccodeid].Rows.Count > 0)
                        {
                            //repeat above for new dataset
                            CheckDataSet(TSDataSet.Tables[Tseccodeid], "SR",seccodeid);
                        }
                    }
                }
            }
        }
              
        public string getBuild(string file)
        {
            //locate file
            int test = ts.filename.IndexOf(file);

            //if file exists
            if (test > -1)
            {
                //return build
                return ts.myFI[test].FilePrivatePart.ToString();
            }
            else
            {
                //return failed message
                return "Failed";
            }

        }
        public Boolean checkBuild(string file, string build)
        {
            //get location of file
            int test = ts.filename.IndexOf(file);

            //if file found
            while (test > -1)
            {
                //check if build matches
                if (ts.myFI[test].FilePrivatePart.ToString() == build)
                {
                    return true;
                }
                else
                {
                    //get next occurance file
                    test = ts.filename.IndexOf(file, test + 1);
                }

            }
            return false;

        }
        public void RunTests()
        {
            //flag of load status
            siteload = true;

            //locate Websites
            ts.GetWebServer(txtWebServer.Text);

            //clear datagrid
            dgvSites.Rows.Clear();
            dgvAppPools.Rows.Clear();
            dgvApps.Rows.Clear();
            lstPorts.Items.Clear();
            
            //process sites
            foreach (WebServer.WebSite site in ts.SLXWebServer.websites)
            {
                dgvSites.Rows.Add(site.SiteName, site.State.ToString());
            }
            
            //remove loding flag
            siteload = false;
            
            //populate bindings
            lstPorts.Items.AddRange(ts.SLXWebServer.websites[0].bindings.ToArray());
            
            //populate Applications
            PopAppsGrid(dgvSites.SelectedRows[0].Index);

            //populate AppPools
            foreach (WebServer.AppPool pool in ts.SLXWebServer.apppools)
            {
                dgvAppPools.Rows.Add(pool.PoolName,pool.User,pool.Version,pool.BitEnabled,pool.Mode,pool.State);
            }
        }
             
        private void PopAppsGrid(int site)
        {
            //clear datagrid
            dgvApps.Rows.Clear();
            
            //create authorized string for each app
            foreach (WebServer.App item in ts.SLXWebServer.websites[site].applications)
            {
                string auth = "";
                if (item.AAuth=="True")
                {
                    auth += "A ";
                }
                if (item.BAuth == "True")
                {
                    auth += "B ";
                }
                if (item.FAuth == "True")
                {
                    auth += "F ";
                }
                if (item.WindowsAuthentication == "True")
                {
                    auth += "W ";
                }
                
                //add application info to datagrid
                dgvApps.Rows.Add(item.Name,item.PhysicalPath,item.UserName,item.Status, item.AppPool,auth);
            }
        }
       
        public string TestWeb(string urltest)
        {
            //string to hold result
            string result = "";

            //ensure url is not wildcard
            if (urltest.Contains("*") == false)
            {
                //use MyClient class to limit returns
                using (MyClient client = new MyClient())
                {
                    //set url to return only header
                    client.HeadOnly = true;
                    
                    try
                    {
                        //try to get header
                        string testuri = client.DownloadString(urltest);
                        
                        //if successful set status to good
                        result = "Good";
                    }
                    catch (WebException Ex)
                    {
                        //if response is returned get statuscode
                        result = ((HttpWebResponse)Ex.Response).StatusCode.ToString();
                    }
                }
            }
            else
            {
                //set result to no port due to wildcard
                result = "no port";
            }

            //return result
            return result;
        }
        
        private void buildSearchPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show buildsearchpath form
            BuildSettings BSForm = new BuildSettings();
            BSForm.ShowDialog();
        }
        private void dgvELog_SelectionChanged(object sender, EventArgs e)
        {
            //populate log entry
            if (dgvELog.SelectedRows.Count>0)
            {
                txtMessage.Text = dgvELog.SelectedRows[0].Cells[3].Value.ToString();
            }
            
        }
        private void cmdFind_Click(object sender, EventArgs e)
        {
            //string for find
            string strFind = "";

            //list to hold found rows
            List<DataGridViewRow> findrow = new List<DataGridViewRow>();
            
            //display find dialog
            using (Find dlg = new Find())
            {
                dlg.ShowDialog();
                if (dlg.DialogResult == DialogResult.OK)
                {
                    //populate from form
                    strFind = dlg.txtFind.Text;
                }

                //if find text exist
                if (strFind.Length>0)
                {
                    //cycle through rows
                    foreach (DataGridViewRow data in dgvELog.Rows)
                    {
                        //if find data is in message of row add to list
                        if (data.Cells[3].Value.ToString().ToUpper().Contains(strFind.ToUpper()))
                        {
                            findrow.Add(data);
                        }
                    }

                    //clear grid
                    dgvELog.Rows.Clear();
                    
                    //add rows from list to grid
                    for (int i = 0; i < findrow.Count; i++)
			        {
                        dgvELog.Rows.Add(findrow[i]);	 
			        }

                    //set find status
                    lblFindQ.Text = "Yes";

                    countEvents();
                }
            }
        }
        private void cmdRun_Click(object sender, EventArgs e)
        {
            //Clear previous display
            txtResults.Text = "";
            
            //run test indicated in combobox
            test = ts.RunDBIntegrityTest(cboTest.Text,ts.SLXDB);    
            
            //cycle through tables and display results
            for (int i = 0; i < test.Tables.Count; i++)
			{
                //use table name as header
                txtResults.Text += test.Tables[i].TableName + "\r\n";
                
                //cycle through rows and add to results
                foreach (DataRow row in test.Tables[i].Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        txtResults.Text += item.ToString() + "\t";
                    }
                    txtResults.Text += "\r\n";
                }
                
                //add blank row
                txtResults.Text += "\r\n"; 
			}
            rtbLog.Text += "Tests Run:\r\n";
            rtbLog.Text += txtResults.Text;
            rtbLog.Text += "\r\n";
	    }
        private void cmbTest_Click(object sender, EventArgs e)
        {
            if (IIS)
            {

                //cycle through Apps datagrid
                for (int i = 0; i < dgvApps.Rows.Count; i++)
                {
                    //test website with a basic header only request
                    string status = TestWeb("http://localhost:" + lstPorts.Items[0].ToString().Substring(lstPorts.Items[0].ToString().IndexOf(":")).Replace(":", "") + dgvApps.Rows[i].Cells[0].Value.ToString());

                    //update datagrid with status
                    dgvApps.Rows[i].Cells[3].Value = status;

                    //find app from Applications
                    int index = dgvSites.SelectedRows[0].Index; 
                    int appindex=ts.SLXWebServer.websites[index].applications.FindIndex(x => x.Name == dgvApps.Rows[i].Cells[0].Value.ToString());

                    //set status in Applications list
                    ts.SLXWebServer.websites[index].applications[appindex].Status = status;
                }
            }
            else
            {
                MessageBox.Show("No IIS found");
            }

        }
        private void cmdServices_Click(object sender, EventArgs e)
        {
            //get service controller
            ServiceController[] scServices;
            
            //get services
            scServices = ServiceController.GetServices();
            
            //Log title
            rtbLog.Text += "\r\nSalesLogix Services\r\n";

            //counter
            int jj = 0;
            float block = InitiateProgressBar(scServices.GetUpperBound(0));
            
            //walk through services
            foreach (ServiceController scTemp in scServices)
            {
                //create management object
                ManagementObject wmiService;
                
                //set object 
                wmiService = new ManagementObject("Win32_Service.Name='" + scTemp.ServiceName + "'");
                
                //get information
                wmiService.Get();
                
                //check for sage, slx or saleslogix logs(sage is there for backwards compatibility 
                if (wmiService["Description"] != null && (wmiService["Description"].ToString().ToUpper().Contains(" SAGE") || wmiService["Description"].ToString().ToUpper().StartsWith("SAGE") || wmiService["Description"].ToString().ToUpper().Contains("SALESLOGIX") || wmiService["Description"].ToString().ToUpper().Contains("SLX")))
                {
                    //add row to saleslogix grid
                    dgvServices.Rows.Add(scTemp.DisplayName, wmiService["Description"],scTemp.Status, wmiService["StartName"]);
                    rtbLog.Text += scTemp.DisplayName.PadRight(36)+ scTemp.Status.ToString().Trim().PadRight(16)+ wmiService["StartName"].ToString().Trim()+"\r\n";
                }
                else
                {
                    //add row to other grid
                    dgvServices2.Rows.Add(scTemp.DisplayName, wmiService["Description"], scTemp.Status, wmiService["StartName"]);
                }
                jj += 1;
                if (jj>block)
                {
                    toolStripProgressBar1.Value += 1;
                    jj = 0;
                }
            }
            ResetProgressBar();

            //set default sorts
            dgvServices.Sort(dgvServices.Columns[0],ListSortDirection.Ascending);
            dgvServices2.Sort(dgvServices2.Columns[0],ListSortDirection.Ascending);
        }
        private void cmdClear_Click(object sender, EventArgs e)
        {
            //clear selected test and results
            test.Tables.Clear();
            cboTest.Text = "";
            txtResults.Text = "";
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //set about box
            AboutBox1 about = new AboutBox1();
            
            //show about dialog
            about.ShowDialog();
        }
        private void cmdProfiler_Click(object sender, EventArgs e)
        {
            //launch if not open then set focus
            string[] paths = { @"C:\Program Files (x86)\SalesLogix", @"C:\Program Files\SalesLogix" };
            showexternalwindow("SLXProfiler","slxProfiler.exe", paths);
        }
        private string locateFiles(string Filename, string[] basepaths)
        {
            //string for result
            string result = "false";

            //array for results
            string[] results;

            //initialize counter
            int x=0;

            //loop till results are found or all paths are checked
            while (result=="false"&&x<basepaths.Length)
            {
                //get files along current path
                results = Directory.GetFiles(basepaths[x], Filename, SearchOption.TopDirectoryOnly);
                
                //if files were found set result
                if (results.Length > 0)
                {
                    result = results[0].ToString();
                }
                else
                {
                    //increment counter
                    x += 1;
                }
            }

            //return result
            return result;
        }

        //load SetForegroundWindow function
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hndlr);

        //load ShowWindow function
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hndlr, ShowWindowEnum flags);
        
        //enumerate ShowWindow Flags
        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized= 2, ShowMaximized = 3,
            Maximize= 3,ShowNormalNoActivate= 4, Show= 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        //load SetActiveWindow function
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SetActiveWindow(int hwnd);

        //load SetFocus function
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SetFocus(IntPtr hwnd);
        
        //Show external application in a window
        public void showexternalwindow(string processName, string filename, string[] paths)
        {
            //flag for msc applications
            bool msc = false;

            //holder for snapon name
            string snapon = "";
            
            //holder for mainwindowtitle
            string mwt = "";

            //process holder
            Process sp;
            
            //check if MSC app
            if (processName.Contains(".msc"))
            {
                //Get name of Snapon
                snapon = processName.Replace(".msc", "").ToUpper();

                //set process to MMC to indicate that its a management console app
                processName = "MMC";

                //set flag
                msc = true;
            }
            //locate process
            Process[] sps = System.Diagnostics.Process.GetProcessesByName(processName);
            
            //If process is found
            if (sps.Length>0)
            {
                //set 1st occurence of process to holder
                sp = sps[0];

                //process is a msc?
                if (msc)
                {
                    //clear holder
                    sp = null;

                    //determine snapon and get expected MainWindow Title for snapon
                    switch (snapon)
                    {
                        case "SERVICES":
                            {
                                mwt = "Services";
                            break;
                            }
                        case "EVENTVWR":
                            {
                                mwt = "Event Viewer";
                                break;
                            }
                    }

                    //locate process with the expected mainwindowtitle
                    foreach (Process mmcp in sps)
                    {
                        if (mmcp.MainWindowTitle==mwt)
                        {
                            //set process in holder
                            sp = mmcp;
                        }
                    }
                }
            }
            else
            {
                //if no process found ensure holder is null
                sp = null;
            }

            //process was found show window
            if (sp !=null)
            {
                //get handler for window
                int hndlr = (int)sp.MainWindowHandle;

                //convert handler to pointer
                IntPtr pntr = (IntPtr)hndlr;

                //if handler found set active window
                if (hndlr != 0)
                {
                    SetActiveWindow(hndlr);
                }
                else
                {
                    //get pointer from process
                    pntr = sp.Handle;
                }

                //show window based on pointer
                ShowWindow(pntr, ShowWindowEnum.Restore);

                //set window to foreground
                SetForegroundWindow(pntr);

                //set focus to window
                SetFocus(pntr);
            }
            else
            {
                //if no running process
                if (msc)
                {
                    //start process for msc snapon
                    Process esp=Process.Start(filename,"/S");
                }
                else
                {
                    //create processStart info
                    ProcessStartInfo psi = new ProcessStartInfo();
                    
                    //get filename from paths if exists
                    psi.FileName = locateFiles(filename, paths);
                    
                    if (psi.FileName != "false")
                    {
                        //start the process
                        sp = Process.Start(psi);
                        
                        //once process starts show window
                        if (sp != null)
                        {
                            //process found show window
                            int hndlr = (int)sp.MainWindowHandle;
                            if (hndlr != 0)
                            {
                                SetActiveWindow(hndlr);
                            }
                            else
                            {
                                ShowWindow(sp.Handle, ShowWindowEnum.Restore);
                                SetForegroundWindow((IntPtr)sp.MainWindowHandle);
                            }
                        }
                        else
                        {
                            //show error
                            MessageBox.Show(filename + "is not installed where expected.  Start program manually");
                        }
                    }
                }
            }
        }
        
        private void cmdTool2_Click(object sender, EventArgs e)
        {
            string[] paths = { @"C:\Windows", @"C:\Windows\System32" };
            //launch if not open then set focus
            showexternalwindow("Eventvwr.msc", "Eventvwr.msc", paths);      
        }

        private void cmdTool3_Click(object sender, EventArgs e)
        {
            //launch if not open then set focus
            string[] paths = { @"C:\Program Files (x86)\SalesLogix", @"C:\Program Files\SalesLogix" };
            showexternalwindow("SLXDBChecker", "SLXDBChecker.exe", paths);
        }

        private void cmdTool4_Click(object sender, EventArgs e)
        {
            string[] paths = { @"C:\Windows", @"C:\Windows\System32" };
            //launch if not open then set focus
            showexternalwindow("notepad", "notepad.exe", paths);
        }

        private void cmdTool5_Click(object sender, EventArgs e)
        {
            string[] paths = { @"C:\Windows", @"C:\Windows\System32" };
            //launch if not open then set focus
            showexternalwindow("services.msc", "services.msc", paths);
        }

        private void dgvSQLServers_SelectionChanged(object sender, EventArgs e)
        {
            //bypass if loading SQL datagrids
            if(SQLLoad==false)
            {
                //set server
                if (dgvSQLServers.SelectedRows[0].Cells[1].Value.ToString().Length > 0)
                {
                    //set server text/instance
                    txtServer.Text = dgvSQLServers.SelectedRows[0].Cells[0].Value.ToString() + "\\" + dgvSQLServers.SelectedRows[0].Cells[1].Value.ToString();
                }
                else
                {
                    //set server text
                    txtServer.Text = dgvSQLServers.SelectedRows[0].Cells[0].Value.ToString();
                }
                
            }

        }
        //Set status indicators
        private void setstatus(bool connected)
        {
            //type of connection
            switch (ts.SLXDB.DBType)
            {
                case DB.connectType.None:
                    break;
                case DB.connectType.SalesLogix:
                    {
                        if (connected)
                        {
                            lblConnectionType.Text = "SLX Provider";
                            lblDBConnection.ForeColor = Color.Green;
                            lblDBConnection.Text = "Connected";
                            tsServer.Text = txtServer.Text;
                            tsDatabase.Text = txtDatabase.Text;
                            lblSLXDisconnect.Enabled = true;
                            lblSLXDisconnect.Visible = true;
                        }
                        else
                        {
                            lblConnectionType.Text = "None";
                            lblDBConnection.ForeColor = Color.Red;
                            lblDBConnection.Text = "Not Connected";
                            tsServer.Text = "";
                            tsDatabase.Text = "";
                            lblSLXDisconnect.Enabled = false;
                            lblSLXDisconnect.Visible = false;
                        }
                        break;
                    }
                case DB.connectType.SQL:
                    {
                        if (connected)
                        {
                            lblConnectionType.Text = "SQL";
                            lblDBConnection.ForeColor = Color.Green;
                            lblDBConnection.Text = "Connected";
                            tsServer.Text = txtServer.Text;
                            if (txtDatabase.Text.Length > 0)
                            {
                                tsDatabase.Text = txtDatabase.Text;
                            }
                            lblSqlMessage.Enabled = true;
                            lblSqlMessage.Visible = true;
                        }
                        else
                        {
                            lblConnectionType.Text = "None";
                            lblDBConnection.ForeColor = Color.Red;
                            lblDBConnection.Text = "Not Connected";
                            tsServer.Text = "";
                            tsDatabase.Text = "";
                            lblSqlMessage.Enabled = false;
                            lblSqlMessage.Visible = false;
                        }
                        break;
                    }
                case DB.connectType.Oracle:
                    break;
            }
            dgvSLXServers.Enabled = !connected;
            dgvSQLServers.Enabled = !connected;
        }
        private void DatabaseConnected()
        {
            //clear the cboSLXUser combobox
            cboSLXUser.Items.Clear();

            try
                    {
                        ts.GetLookupDataset("Select SERVERPATH from sysdba.SYNCSERVER", "logging");
                        lblLPath.Text = ts.LUDataSet.Tables["logging"].Rows[0].ItemArray[0].ToString();

                        //log path
                        rtbLog.Text += "Logging Path:  " + lblLPath.Text + "\r\n";
                    }
                    catch { }

                    //get files on logging path
                    List<string> infiles = new List<string>();
                    List<string> outfiles = new List<string>();
                    lblInFiles.Text=infiles.Count.ToString();
                    lblOutFiles.Text=infiles.Count.ToString();

                    //check outfiles
                    outfiles = ts.GetFilesRecursive(lblLPath.Text+ "\\outfiles");
                    infiles = ts.GetFilesRecursive(lblLPath.Text + "\\infiles");


                    //try to get seccode table
                    try
                    {
                        //get lookup dataset from seccode
                        ts.GetLookupDataset("Select * from sysdba.seccode", "seccode");

                        //proccess dataset 
                        foreach (DataRow SCitem in ts.LUDataSet.Tables["seccode"].Rows)
                        {
                            //if item is a user
                            if (SCitem.ItemArray[2].ToString() == "U")
                            {
                                //add to user combobox
                                cboSLXUser.Items.Add(SCitem.ItemArray[1]);
                            }
                        }
                    }
                    catch(Exception EX)
                    {
                        throw EX;
                    }

        }
        private void dgvDBs_SelectionChanged(object sender, EventArgs e)
        {
            if (DBLoad == false)
            {
                txtDatabase.Text=dgvDBs.SelectedRows[0].Cells[0].Value.ToString();                
                

                //set database object with server and database info
                ts.SLXDB.srv = txtServer.Text;
                ts.SLXDB.db = txtDatabase.Text;
                ts.SLXDB.user = txtUser.Text;
                ts.SLXDB.password = txtPassword.Text;
                ts.SLXDB.DBType = SecurityTS.DB.connectType.SQL;

                //Try to connect to database
                try
                {
                    //build connection
                    ts.SLXDB.BuildConn();

                    //open connection
                    ts.SQLOpen(ts.SLXDB);

                    //set connected status
                    setstatus(true);

                    //log Database
                    rtbLog.Text += "Connected to database " + ts.SLXDB.db + "\r\n";
                    
                    //enable controls for tabpages that need database connections
                    switch (ts.SLXDB.DBType)
                    {
                        case DB.connectType.None:
                            break;
                        case DB.connectType.SalesLogix:
                            DbConnection(SLXpages, true);
                            break;
                        case DB.connectType.SQL:
                            DbConnection(SQLpages, true);
                            break;
                        case DB.connectType.Oracle:
                            break;
                        default:
                            break;
                    }
                    
                    DatabaseConnected();
                    
                }
                catch
                {
                    //set the connection status and show failed message
                    setstatus(false);
                }
            }
        }

        private System.Data.DataTable LoadSQL()
        {
            System.Data.Sql.SqlDataSourceEnumerator instance = System.Data.Sql.SqlDataSourceEnumerator.Instance;
            System.Data.DataTable SQLdataTable = instance.GetDataSources();
            return SQLdataTable;
        }

        private void lblHour_Click(object sender, EventArgs e)
        {
            limitlog(1);
            lblTime.Text = "Last Hour";
        }
        public void limitlog(int limitType)
        {
            //list to hold found rows
            List<DataGridViewRow> Limitrow = new List<DataGridViewRow>();

            foreach (DataGridViewRow data in dgvELog.Rows)
            {
                //if Limit metadd to list
                if ((DateTime.Now-Convert.ToDateTime(data.Cells[2].Value.ToString())).Hours<limitType)
                {
                    Limitrow.Add(data);
                }
            }

            //clear grid
            dgvELog.Rows.Clear();

            //add rows from list to grid
            for (int i = 0; i < Limitrow.Count; i++)
            {
                dgvELog.Rows.Add(Limitrow[i]);
            }
            countEvents();
        }

        private void lbl6Hour_Click(object sender, EventArgs e)
        {
            limitlog(6);
            lblTime.Text = "Last 6 Hours";
        }
        public void countEvents()
        {
            //counters
            int erroerrH = 0;
            int erroerr6H = 0;
            int erroerrD = 0;
            int erroerrW = 0;
            int erroerr4W = 0;

            //proccess each log
            foreach (DataGridViewRow entry in dgvELog.Rows)
            {
                TimeSpan time = DateTime.Now - Convert.ToDateTime(entry.Cells[2].Value);
                int Hours = time.Hours;
                if (Hours < 1)
                {
                    erroerrH += 1;
                }
                if (Hours < 6)
                {
                    erroerr6H += 1;
                }
                if (Hours < 24)
                {
                    erroerrD += 1;
                }
                if (Hours < 168)
                {
                    erroerrW += 1;
                }
                if (Hours < 672)
                {
                    erroerr4W += 1;
                }
            }
            //set count
            lblCount.Text = dgvELog.Rows.Count.ToString();
            lblHour.Text = erroerrH.ToString();
            lbl6Hour.Text = erroerr6H.ToString();
            lblDay.Text = erroerrD.ToString();
            lblWeek.Text = erroerrW.ToString();
            lblMonth.Text = erroerr4W.ToString();
            
            //display count
            lblCount.Text = dgvELog.Rows.Count.ToString();
            
            if (dgvELog.Rows.Count == 0)
            {
                MessageBox.Show("No events Found");
            }
        }

        private void lblDay_Click(object sender, EventArgs e)
        {
            limitlog(24);
            lblTime.Text = "Last Day";
        }

        private void lblWeek_Click(object sender, EventArgs e)
        {
            limitlog(168);
            lblTime.Text = "Last Week";
        }

        private void lblMonth_Click(object sender, EventArgs e)
        {
            limitlog(672);
            lblTime.Text = "Last Month";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ts.SQLClose();
            this.Close();
        }

        private void cmdReloadIIS_Click(object sender, EventArgs e)
        {
            try
            {
                RunTests();
                IIS = true;
            }
            catch (Exception)
            {

                MessageBox.Show("Failed to locate IIS");
                IIS = false;
            }
            
        }
        
        private void tvICTests_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action!=TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count>0 && !TVCheck)
                {
                    CheckAllChildNodes(e.Node, e.Node.Checked);
                }
                if (e.Node.Parent!=null && e.Node.Parent.Checked && e.Node.Checked==false)
                {
                    TVCheck = true;
                    e.Node.Parent.Checked = false;
                    TVCheck = false;
                }
            }
        }
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively. 
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void btnICLoad_Click(object sender, EventArgs e)
        {
            ts.GetDBICTest("SalesLogix.sxc");
            int i = 0;
            while (ts.DBChecker[i].Name != "" && ts.DBChecker[i].Name != null)
            {
                TreeNode test = new TreeNode(ts.DBChecker[i].Family);
                test.Tag = test.Text;
                test.Name = test.Text;
                if (tvICTests.Nodes.IndexOfKey(ts.DBChecker[i].Family) > -1)
                {
                    tvICTests.Nodes[tvICTests.Nodes.IndexOfKey(ts.DBChecker[i].Family)].Nodes.Add(i.ToString(),ts.DBChecker[i].Name);
                }
                else
                {
                    tvICTests.Nodes.Add(test);
                    tvICTests.Nodes[tvICTests.Nodes.IndexOfKey(ts.DBChecker[i].Family)].Nodes.Add(i.ToString(),ts.DBChecker[i].Name);
                }
                i += 1;
            }
            tvICTests.Sort();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            //create list for tests
            List<string> DBICTests = new List<string>();
 
            DataSet dsDBICTest = new DataSet();
            
            for (int i = 0; i < tvICTests.Nodes.Count; i++)
			{
                TreeNode node = tvICTests.Nodes[i];
                for (int j = 0; j < node.Nodes.Count; j++)
                {
                    TreeNode testnode = node.Nodes[j];
                    if (testnode.Checked==true)
                    {
                        int testnum=Convert.ToInt16(testnode.Name);

                        string SQL = ts.DBChecker[testnum].Select + ts.DBChecker[testnum].From + ts.DBChecker[testnum].Where;
                        
                        //add to list
                        DBICTests.Add(ts.DBChecker[testnum].Family + "|" + ts.DBChecker[testnum].Name + "|" + SQL);
                        
                    }
                } 
			}

            //counter
            int jj = 0;
            float block = InitiateProgressBar(DBICTests.Count);
             
            //pass list to test
            ts.SQLOpen(ts.SLXDB);
            foreach (string testName in DBICTests)
            {
                dsDBICTest = ts.RunDBICTest(testName,ts.SLXDB);
                tbDBICResults.Text += dsDBICTest.Tables[0].TableName + "\r\n";
                //cycle through rows and add to results
                foreach (DataRow row in dsDBICTest.Tables[0].Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        tbDBICResults.Text += item.ToString() + "\t";
                    }
                    tbDBICResults.Text += "\r\n";
                }

                //add blank row
                tbDBICResults.Text += "\r\n";

                jj += 1;
                if (jj>block)
                {
                    IncementProgressBar();
                    jj = 0;
                }
                dsDBICTest.Tables.Clear();
            }
            ResetProgressBar();
        }

        private void cboConnectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadServerGrids();
            SQLLoad = true;
            switch (cboConnectionType.Text)
            {
                case "SQL":
                    {
                        pnlSLX.Visible = false;
                        int index = -1;
                        if (dgvSQLServers.SelectedRows.Count > 0)
                        {
                            index = dgvSQLServers.SelectedRows[0].Index;
                        }
                        pnlSQL.Visible = true;
                        if (index > -1)
                        {
                            dgvSQLServers.Rows[0].Selected = false;
                            dgvSQLServers.Rows[index].Selected = true;
                        }
                        else
                        {
                            dgvSQLServers.Rows[0].Selected = false;
                        }
                        lblCServer.Text = "SQL Server";
                        lblCUser.Text = "SQL User";
                        lblCDatabase.Visible = true;
                        lblSLXDBase.Visible = false;
                        txtSLXDatabase.Visible = false;
                        txtDatabase.Visible = true;
                        txtUser.Text = "sysdba";
                        gbConn.Visible = true;
                        gbConn.Text = "SQL Connection";
                        break;
                    }
                case "Saleslogix":
                    {
                        pnlSQL.Visible = false;
                        int index = -1;
                        if(dgvSLXServers.SelectedRows.Count > 0)
                        {
                            index=dgvSLXServers.SelectedRows[0].Index;
                        }
                        pnlSLX.Visible = true;
                        dgvSLXServers.Rows[0].Selected = false;
                            
                        if (index>-1)
                        {
                            dgvSLXServers.Rows[index].Selected = true;
                        }

                        dgvSLXAliases.Rows[0].Selected = false;
                        lblCServer.Text = "SLX Server";
                        lblCUser.Text = "SLX User";
                        lblCDatabase.Visible = false;
                        lblSLXDBase.Visible = true;
                        txtUser.Text = "admin";
                        txtSLXDatabase.Visible = true;
                        txtDatabase.Visible = false;
                        gbConn.Visible = true;
                        gbConn.Text = "SLX Connection";
                        break;
                    }
                case "None":
                    {
                        pnlSLX.Visible = false;
                        pnlSQL.Visible = false;
                        gbConn.Visible = false;
                        break;
                    }
               
            }
            SQLLoad = false;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dgvSLXServers_SelectionChanged(object sender, EventArgs e)
        {
            if (SQLLoad == false)
            {
                if (dgvSLXServers.Enabled == false)
                {
                    MessageBox.Show("Close connection before chosing new Saleslogix server");
                }
                else
                {
                    txtServer.Text = dgvSLXServers.SelectedRows[0].Cells[0].Value.ToString();
                    dgvSLXAliases.Rows.Clear();
                    foreach (var item in ts.GetAliasList(txtServer.Text))
                    {
                        dgvSLXAliases.Rows.Add(item.ToString());
                    }
                }
            }
        }

        private void txtServer_TextChanged(object sender, EventArgs e)
        {
            txtDatabase.Text = "";
            txtSLXDatabase.Text = "";
        }

        private void dgvSLXAliases_SelectionChanged(object sender, EventArgs e)
        {
            if (DBLoad == false && dgvSLXAliases.SelectedRows.Count>0)
            {
                txtSLXDatabase.Text = dgvSLXAliases.SelectedRows[0].Cells[0].Value.ToString();
            }
        }

        private void btnClearResults_Click(object sender, EventArgs e)
        {
            tbDBICResults.Text = "";
        }

        private void runINIFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "ini files (*.ini)|*.ini|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            List<string> lines = new List<string>();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Create an instance of StreamReader to read from a file.
                    lines = ts.GetIni(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    // Let the user know what went wrong.
                    throw ex;
                }

                if (lines.Count>0)
                {
                    ts.ProcessIni(lines);
                }
            }
        }

        private void dgvApps_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void btnGroups_Click(object sender, EventArgs e)
        {
            Performance per = new Performance();
            switch (ts.SLXDB.DBType)
	        {
		        case DB.connectType.None:
                    break;
                case DB.connectType.SalesLogix:
                    per.GroupTests(ts.scn);
                    break;
                case DB.connectType.SQL:
                    per.GroupTests(ts.cn);
                    break;
                case DB.connectType.Oracle:
                    break;
            }
            dgvAdhoc.DataSource = per.Groups.Tables[1];
            dgvDynamic.DataSource = per.Groups.Tables[2];
            dgvLegacy.DataSource = per.Groups.Tables[0];
        }

        private void btnSpS_Click(object sender, EventArgs e)
        {
            Performance per=new Performance();
            
            switch (ts.SLXDB.DBType)
            {
                case DB.connectType.None:
                    break;
                case DB.connectType.SalesLogix:
                    tbSpeedSearch.Text = per.SpeedSearchTest(ts.scn).ToString();
                    break;
                case DB.connectType.SQL:
                    tbSpeedSearch.Text = per.SpeedSearchTest(ts.cn).ToString();
                    break;
                case DB.connectType.Oracle:
                    break;
            }
        }

        private void btnIndexes_Click(object sender, EventArgs e)
        {
            if (ts.SLXDB.DBType == SecurityTS.DB.connectType.SalesLogix)
            {
                MessageBox.Show("Index Test requires a SQL connection, please disconnect any current connection and reconnect with a SQL connection");
            }
            else
            {
                Performance per = new Performance();
                per.GetIndexes(ts.cn);
                for (int i = 0; i < per.SLXIndex.Tables.Count; i++)
                {
                    TreeNode Table = new TreeNode();
                    Table.Text = per.SLXIndex.Tables[i].TableName;
                    for (int j = 0; j < per.SLXIndex.Tables[i].Rows.Count; j++)
                    {
                        string indexname = per.SLXIndex.Tables[i].Rows[j].ItemArray[0].ToString();
                        if (indexname.ToUpper().StartsWith("SALESLOGIX") || indexname.ToUpper().StartsWith("X"))
                        {
                            Table.Nodes.Add(indexname);
                        }
                    }
                    if (Table.Nodes.Count > 0)
                    {
                        tvIndexes.Nodes.Add(Table);
                    }
                }
                tvIndexes.Sort();
            }
        }

        private void btnContent_Click(object sender, EventArgs e)
        {
            Performance per=new Performance();
            List<Performance.WebExpiration> ExpirationValues = new List<Performance.WebExpiration>();
            if (cbSites.SelectedItem!=null && cbSites.SelectedItem.ToString().Length>0)
            {
                ExpirationValues = per.GetContentExpiration(cbSites.SelectedItem.ToString());
            }
            else
            {
                ExpirationValues = per.GetContentExpiration("SalesLogix");
            }
            
            foreach (Performance.WebExpiration exp in ExpirationValues)
            {
                string duration = "";
                switch (exp.cacheControlMode)
                {
                    case Performance.WebExpiration.ControlMode.NoControl:
                        break;
                    case Performance.WebExpiration.ControlMode.DisableCache:
                        break;
                    case Performance.WebExpiration.ControlMode.UseMaxAge:
                        duration = exp.cacheControlMaxAge.ToString();
                        break;
                    case Performance.WebExpiration.ControlMode.UseExpires:
                        duration = exp.HttpExpires.ToString();
                        break;
                }
                dgvContent.Rows.Add(exp.Name, exp.Expires, exp.cacheControlMode.ToString(), duration);
                
            }
        }

        private void btnLoadWebServer_Click(object sender, EventArgs e)
        {
            if (ts.SLXWebServer.ServerName!=null && ts.SLXWebServer.ServerName.Length > 0)
            {
            }
            else
            {
                ts.GetWebServer(Environment.MachineName);
            }
            tbWebServer.Text = ts.SLXWebServer.ServerName;
            if (true)
            {
                foreach (SecurityTS.WebServer.WebSite site in ts.SLXWebServer.websites)
                {
                    cbSites.Items.Add(site.SiteName);
                }
                
            }
        }

        private void rtbLog_TextChanged(object sender, EventArgs e)
        {

        }
    }
}