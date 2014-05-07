using System;
using Microsoft.Web.Administration;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Net;
using System.Xml;
using System.ServiceProcess;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Web.Configuration;
using System.DirectoryServices;



namespace SecurityTS
{
    public class TS
    {
        public DataSet TestDataSet = new DataSet();
        public DataSet TestsDataSet = new DataSet();
        public string InstallPath = "";
        public DataSet LUDataSet = new DataSet();
        public List<String> Tests = new List<string>();
        public string OS = System.Environment.OSVersion.ToString();

        //Installed apps variables
        public int[] installed = new int[24];          //-1 not installed #= # found 
        public string[] ListApps = new string[] { "SalesLogix Client", "SalesLogix Administrator", "SalesLogix Architect", "Application Architect" };
        public string[] ListAppFiles = new string[] { "saleslogix.exe", "admin.exe", "architect.exe", "sageapparchitect.exe" };

        //Connections
        public SqlConnection cn = new SqlConnection();          //SQL Connection
        public OleDbConnection scn = new OleDbConnection();          //SQL Connection
        public OracleConnection ocn = new OracleConnection();   //Oracle Connection

        //create DB object
        public DB SLXDB = new DB();
        
        //SQL components
        //Builders
        public SqlCommandBuilder cmdBuilder;                //Basic Command Builder SQL
        public OracleCommandBuilder ocmdBuilder;            //Basic Command Builder Oracle
        public SqlCommandBuilder CBcmdBuilder;              //Secondary Command Builder SQL
        public OracleCommandBuilder oCBcmdBuilder;          //Secondary Command Builder Oracle
        public OleDbCommandBuilder scmdBuilder;
        public OleDbCommandBuilder sCBcmdBuilder;

        //Data Adapters
        public SqlDataAdapter CBda;                         //Basic Adapter for SQL
        public OracleDataAdapter oCBda;                     //Basic Adapter for Oracle
        public SqlDataAdapter da;                           //Secondary Adapter for SQL 
        public OracleDataAdapter oda;
        public OleDbDataAdapter sda;
        public OleDbDataAdapter sCBda;
        //VersionFinder variables
        public List<string> filename = new List<string>();
        public List<string> files = new List<string>();
        public List<string> files2 = new List<string>();
        public FileVersionInfo[] myFI = new FileVersionInfo[10000];

        //DBC Test Variables
        public struct Test
        {
            public string Name;
            public string Family;
            public string Select;
            public string From;
            public string Where;

            public Test(string name, string family, string select, string from, string where)
            {
                Name = name;
                Family = family;
                Select = select;
                From = from;
                Where = where;
            }
        }

        //list for websites and applications
        public WebServer SLXWebServer = new WebServer();
       
        public Test[] DBChecker = new Test[200];

        public sealed class InstallProperties
        {
            public const string INSTALLPROPERTY_VERSIONSTRING = "VersionString";
            public const string INSTALLPROPERTY_TRANSFORMS = "Transforms";
            public const string INSTALLPROPERTY_DISPLAYNAME = "DisplayName";
            public const string INSTALLPROPERTY_INSTALLLOCATION = "InstallLocation";
            public const string INSTALLPROPERTY_INSTALLEDPRODUCTNAME = "InstalledProductName";
        }
        public sealed class ProductCodes
        {

            public static ArrayList PRODUCTCODES = new ArrayList { 
      // Pre 7.5.4
      "{C6453AB1-6151-4E68-B56E-66ADE85EDA98}", // Admin
      "{D7DA4DDF-BA95-4D92-BB49-56F0AFAA096F}", // Provider
      "{E5AFD400-B1ED-483E-8FB9-49D1F85153D1}", // Slxclient
      "{B1285A4C-000F-47BF-BD0E-549B27997559}", // Office
      "{B0A9F47F-5CAE-429D-8AA7-894DAEDB6FD3}", // WebMgr
      "{BC214FFB-29FB-41ED-84CA-6B167A44D811}", // WebHost
      "{F8561B1C-079B-442F-A286-07B5DF11DD96}", // WebRpt
      "{854B4275-4891-44DD-929A-FFA37E43CC23}", // DCWC
      "{8D88C982-B762-4233-9B77-70DDCE385C43}", // VSWF
      // 7.5.4
      "{99FA46D3-0ED0-4FB1-9FB5-B323C6AFBBCE}", // Admin
      "{AC7C3598-0510-43FA-9EEF-21258DB950D5}", // Provider
      "{893F65B1-C697-4149-A766-B3D80D9B2A49}", // SlxNetClient
      "{1F99C35B-ED7B-44C4-BB04-CF59CCDB709F}", // SlxRmtClient
      "{B1285A4C-000F-47BF-BD0E-549B27997559}", // Office
      "{993D2130-BBEA-4E37-9EF6-86BC3D49476C}", // WebHost
      "{19EC4F36-83FD-44A1-BE3A-AE484D2D9E49}", // WebRpt
      "{C6E5E5DE-3662-4BC9-8EF4-3060ED4B0435}", // DCWC
      "{F9AC44B9-8F81-4958-A7B2-BCAC75C824D1}", // VSWF
      "{BFBFB7CD-505E-4087-A5A5-730709546D40}", // DI
      // 8.0.0
      "{F9F69823-985C-442B-92B9-C16D151DCC72}", // Admin
      "{53F61904-5808-4201-B01C-9D7DC8C7C92C}", // Provider
      "{066F4634-F353-4A6B-84D0-F25BD4288C12}", // OfflineClient
      "{E4A3836C-2A26-474A-B996-C9DD4A384066}", // RemoteOffice
      "{BAFB0CC0-46DB-461D-B85A-E00E554A987F}", // NetworkClient
      "{AB16611D-1DA7-472E-A010-2B6E1CD77647}", // RemoteClient
      "{E403032B-2108-4A29-8FE3-8CE70BB10C62}", // WebHost
      "{DE91CB8D-7F2C-438A-9202-47E87C8E4BBD}", // WebReporting
      "{A615A6AF-1975-474A-9105-0BECF2AE2A40}", // DesktopIntegration
      // 8.1.0
      "{296C5E94-AB76-4203-919B-9D925B91A5B9}", // Admin
      "{2C42F644-8E01-4C16-AB6D-A1FEAB7ABCFA}", // Provider
      "{7CA16FA5-3D3E-4BD1-ACA9-A26BBC552D33}", // OfflineClient
      "{36F67339-70FD-4DD7-8066-95F15FD6AFEF}", // RemoteOffice
      "{C28FCFEB-A94B-43B7-8C0B-D3054EDBF541}", // NetworkClient
     "{B2E0B0E4-34DD-4928-BB61-2CE01A1673DE}", // RemoteClient
      "{16E16E12-6B0A-4D28-AED0-C8F98F79B785}", // WebHost
      "{5062B8AC-5FF5-4B1A-BA4B-0733BD140217}"  // DesktopIntegration
    };
        }
        enum MSIINSTALLCONTEXT
        {
            UserManaged = 1,

            UserUnmanaged = 2,

            Machine = 4
        }
        ArrayList _installedProducts = new ArrayList();
        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern Int32 MsiGetProductInfo(string product, string property, [Out] StringBuilder valueBuf, ref Int32 len);
        [DllImport("Msi.dll", SetLastError = true)]
        static extern uint MsiGetPatchInfoEx(String szPatchCode, String szProductCode, String szUserSid, MSIINSTALLCONTEXT dwContext, String szProperty, [Out] StringBuilder lpValue, ref uint pcchValue);
        [DllImport("Msi.dll", SetLastError = true)]
        static extern uint MsiEnumPatches(String szProduct, int iPatchIndex, StringBuilder lpPatchBuf, StringBuilder lpTransformsBuf, ref int pcchValue);

        public TS()
        {
            Tests.Add("All");
            Tests.Add("Missing RESYNCTABLEDEFS entries");
            Tests.Add("Missing SECTABLEDEFS entries");
            Tests.Add("Tables listed in RESYNCTABLEDEFS that do not exist in the database");
            Tests.Add("Fields listed in SECTABLEDEFS that do not exist in the database");
            Tests.Add("Recursive Joins");
            Tests.Add("Duplicate Joins in the JOINDATA table");
            Tests.Add("Joins To tables that do not exist in RESYNCTABLEDEFS");
            Tests.Add("Joins From tables that do not exist in RESYNCTABLEDEFS");
            Tests.Add("Duplicate FIELDINDEX values in SECTABLEDEFS");
            Tests.Add("PLUGINS based on another PLUGIN that no longer exists");
        }
        public void WriteToConsole(string message)
        {
            AttachConsole(-1);
            Console.WriteLine(message);
        }
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);
        public DataSet RunDBIntegrityTest(string Test, DB conn)
        {
            if (Test == "All")
            {
                TestsDataSet.Tables.Clear();
                GetDataset("SELECT NAME AS  \"NOT IN RESYNCTABLEDEFS\" FROM SYSOBJECTS WHERE NAME NOT IN (SELECT TABLENAME FROM sysdba.RESYNCTABLEDEFS) AND XTYPE = 'U' AND NAME <> 'DTPROPERTIES' AND NAME NOT IN ('ACCOUNT_LOOKUP', 'ACCT_XREF', 'CONTACT_LOOKUP', 'KSYNC', 'KSYNC_RELS', 'KSYNC_USER_CONVERT', 'KSYNCTABLES') ORDER BY NAME", "Missing RESYNCTABLEDEFS entries", TestsDataSet, conn);
                //TestsDataSet.Tables.Add(TestDataSet.Tables[0]);
                GetDataset("SELECT SO.NAME AS  \"TableName\", SC.NAME AS  \"FieldName\" FROM SYSOBJECTS SO LEFT OUTER JOIN SYSCOLUMNS SC ON SO.ID = SC.ID WHERE (SO.XTYPE = 'U') AND  (SO.NAME <> 'DTPROPERTIES') AND (SO.NAME + SC.NAME NOT IN (SELECT TABLENAME + FIELDNAME FROM sysdba.SECTABLEDEFS)) AND SO.NAME <> 'DTPROPERTIES' AND SO.NAME NOT IN ('ACCOUNT_LOOKUP', 'ACCT_XREF', 'CONTACT_LOOKUP', 'KSYNC', 'KSYNC_RELS', 'KSYNC_USER_CONVERT', 'KSYNCTABLES') ORDER BY SO.NAME, SC.NAME", "Missing SECTABLEDEFS entries", TestsDataSet, conn);
                //TestsDataSet.Tables.Add(TestDataSet.Tables[0]);
                GetDataset("SELECT TABLENAME AS  \"NOT IN SYSOBJECTS\" FROM sysdba.RESYNCTABLEDEFS WHERE TABLENAME NOT IN (SELECT NAME FROM SYSOBJECTS WHERE XTYPE = 'U' AND NAME <> 'DTPROPERTIES')", "Tables listed in RESYNCTABLEDEFS that do not exist in the database", TestsDataSet, conn);
                //TestsDataSet.Tables.Add(TestDataSet.Tables[0]);
                GetDataset("SELECT TABLENAME, FIELDNAME FROM sysdba.SECTABLEDEFS WHERE TABLENAME+FIELDNAME NOT IN (SELECT SYSOBJECTS.NAME + SYSCOLUMNS.NAME AS TABLEFIELD FROM SYSOBJECTS INNER JOIN SYSCOLUMNS ON SYSOBJECTS.ID=SYSCOLUMNS.ID WHERE SYSOBJECTS.XTYPE = 'U' AND SYSOBJECTS.NAME <> 'DTPROPERTIES')", "Fields listed in SECTABLEDEFS that do not exist in the database", TestsDataSet, conn);
                //TestsDataSet.Tables.Add(TestDataSet.Tables[0]);
                GetDataset("SELECT J1.FROMTABLE FROMTABLE1, J1.FROMFIELD FROMFIELD1, J1.TOTABLE TOTABLE1, J1.TOFIELD TOFIELD1 FROM sysdba.JOINDATA J1, sysdba.JOINDATA J2 WHERE J1.FROMTABLE = J2.TOTABLE AND J1.TOTABLE = J2.FROMTABLE AND J1.FROMFIELD=J2.TOFIELD AND J1.TOFIELD=J2.FROMFIELD", "Recursive Joins", TestsDataSet, conn);
                //TestsDataSet.Tables.Add(TestDataSet.Tables[0]);
                GetDataset("SELECT sysdba.JOINDATA.FROMTABLE, sysdba.JOINDATA.FROMFIELD, sysdba.JOINDATA.TOTABLE, sysdba.JOINDATA.TOFIELD, sysdba.JOINDATA.JOINID, sysdba.JOINDATA.SECONDARY, sysdba.JOINDATA.KEYGENERATOR, sysdba.JOINDATA.CASCADETYPE, sysdba.JOINDATA.USEBYDEFAULT, sysdba.JOINDATA.JOINTYPE FROM sysdba.JOINDATA WHERE (((sysdba.JOINDATA.FROMTABLE) In (SELECT FROMTABLE FROM sysdba.JOINDATA As Tmp GROUP BY FROMTABLE, FROMFIELD, TOTABLE, TOFIELD HAVING Count(*)>1  And FROMFIELD = sysdba.JOINDATA.FROMFIELD And TOTABLE = sysdba.JOINDATA.TOTABLE And TOFIELD = sysdba.JOINDATA.TOFIELD))) ORDER BY sysdba.JOINDATA.FROMTABLE, sysdba.JOINDATA.FROMFIELD, sysdba.JOINDATA.TOTABLE, sysdba.JOINDATA.TOFIELD ", "Duplicate Joins in the JOINDATA table", TestsDataSet, conn);
                //TestsDataSet.Tables.Add(TestDataSet.Tables[0]);
                GetDataset("SELECT DISTINCT TOTABLE FROM sysdba.JOINDATA WHERE TOTABLE <> '*' AND TOTABLE NOT IN (SELECT TABLENAME FROM sysdba. RESYNCTABLEDEFS) ORDER BY TOTABLE", "Joins To tables that do not exist in RESYNCTABLEDEFS", TestsDataSet, conn);
                //TestsDataSet.Tables.Add(TestDataSet.Tables[0]);
                GetDataset("SELECT DISTINCT FROMTABLE FROM sysdba.JOINDATA WHERE FROMTABLE <> '*' AND FROMTABLE NOT IN (SELECT TABLENAME FROM sysdba.RESYNCTABLEDEFS) ORDER BY FROMTABLE", "Joins From tables that do not exist in RESYNCTABLEDEFS", TestsDataSet, conn);
                //TestsDataSet.Tables.Add(TestDataSet.Tables[0]);
                GetDataset("SELECT A2.TABLENAME, A2.FIELDNAME, A1.FIELDINDEX AS FIELDINDEX, Count(A1.FIELDINDEX) AS DUPLICATES FROM sysdba.SECTABLEDEFS AS A1 INNER JOIN sysdba.SECTABLEDEFS AS A2 ON A1.FIELDINDEX = A2.FIELDINDEX GROUP BY A1.FIELDINDEX, A2.TABLENAME, A2.FIELDNAME HAVING (((Count(A1.FIELDINDEX))>1)) ORDER BY A1.FIELDINDEX", "Duplicate FIELDINDEX values in SECTABLEDEFS", TestsDataSet, conn);
                //TestsDataSet.Tables.Add(TestDataSet.Tables[0]);
                GetDataset("SELECT PLUGINID, BASEDON, NAME, FAMILY, TYPE, USERID FROM sysdba.PLUGIN WHERE BASEDON NOT IN (SELECT PLUGINID FROM sysdba.PLUGIN) AND BASEDON NOT IN ('', NULL, 'STANDARD', 'X') ORDER BY NAME, FAMILY, TYPE, USERID", "PLUGINS based on another PLUGIN that no longer exists", TestsDataSet, conn);
                //TestsDataSet.Tables.Add(TestDataSet.Tables[0]);
            }
            else
            {
                switch (Test)
                {
                    case "Missing RESYNCTABLEDEFS entries":
                        {
                            GetDataset("SELECT NAME AS  \"NOT IN RESYNCTABLEDEFS\" FROM SYSOBJECTS WHERE NAME NOT IN (SELECT TABLENAME FROM sysdba.RESYNCTABLEDEFS) AND XTYPE = 'U' AND NAME <> 'DTPROPERTIES' AND NAME NOT IN ('ACCOUNT_LOOKUP', 'ACCT_XREF', 'CONTACT_LOOKUP', 'KSYNC', 'KSYNC_RELS', 'KSYNC_USER_CONVERT', 'KSYNCTABLES') ORDER BY NAME", Test, TestsDataSet);
                            break;
                        }
                    case "Missing SECTABLEDEFS entries":
                        {
                            GetDataset("SELECT SO.NAME AS  \"TableName\", SC.NAME AS  \"FieldName\" FROM SYSOBJECTS SO LEFT OUTER JOIN SYSCOLUMNS SC ON SO.ID = SC.ID WHERE (SO.XTYPE = 'U') AND  (SO.NAME <> 'DTPROPERTIES') AND (SO.NAME + SC.NAME NOT IN (SELECT TABLENAME + FIELDNAME FROM sysdba.SECTABLEDEFS)) AND SO.NAME <> 'DTPROPERTIES' AND SO.NAME NOT IN ('ACCOUNT_LOOKUP', 'ACCT_XREF', 'CONTACT_LOOKUP', 'KSYNC', 'KSYNC_RELS', 'KSYNC_USER_CONVERT', 'KSYNCTABLES') ORDER BY SO.NAME, SC.NAME", Test, TestsDataSet);
                            break;
                        }
                    case "Tables listed in RESYNCTABLEDEFS that do not exist in the database":
                        {
                            GetDataset("SELECT TABLENAME AS  \"NOT IN SYSOBJECTS\" FROM sysdba.RESYNCTABLEDEFS WHERE TABLENAME NOT IN (SELECT NAME FROM SYSOBJECTS WHERE XTYPE = 'U' AND NAME <> 'DTPROPERTIES')", Test, TestsDataSet);
                            break;
                        }
                    case "Fields listed in SECTABLEDEFS that do not exist in the database":
                        {
                            GetDataset("SELECT TABLENAME, FIELDNAME FROM sysdba.SECTABLEDEFS WHERE TABLENAME+FIELDNAME NOT IN (SELECT SYSOBJECTS.NAME + SYSCOLUMNS.NAME AS TABLEFIELD FROM SYSOBJECTS INNER JOIN SYSCOLUMNS ON SYSOBJECTS.ID=SYSCOLUMNS.ID WHERE SYSOBJECTS.XTYPE = 'U' AND SYSOBJECTS.NAME <> 'DTPROPERTIES')", Test, TestsDataSet);
                            break;
                        }
                    case "Recursive Joins":
                        {
                            GetDataset("SELECT J1.FROMTABLE FROMTABLE1, J1.FROMFIELD FROMFIELD1, J1.TOTABLE TOTABLE1, J1.TOFIELD TOFIELD1 FROM sysdba.JOINDATA J1, sysdba.JOINDATA J2 WHERE J1.FROMTABLE = J2.TOTABLE AND J1.TOTABLE = J2.FROMTABLE AND J1.FROMFIELD=J2.TOFIELD AND J1.TOFIELD=J2.FROMFIELD", Test, TestsDataSet);
                            break;
                        }
                    case "Duplicate Joins in the JOINDATA table":
                        {
                            GetDataset("SELECT sysdba.JOINDATA.FROMTABLE, sysdba.JOINDATA.FROMFIELD, sysdba.JOINDATA.TOTABLE, sysdba.JOINDATA.TOFIELD, sysdba.JOINDATA.JOINID, sysdba.JOINDATA.SECONDARY, sysdba.JOINDATA.KEYGENERATOR, sysdba.JOINDATA.CASCADETYPE, sysdba.JOINDATA.USEBYDEFAULT, sysdba.JOINDATA.JOINTYPE FROM sysdba.JOINDATA WHERE (((sysdba.JOINDATA.FROMTABLE) In (SELECT FROMTABLE FROM sysdba.JOINDATA As Tmp GROUP BY FROMTABLE, FROMFIELD, TOTABLE, TOFIELD HAVING Count(*)>1  And FROMFIELD = sysdba.JOINDATA.FROMFIELD And TOTABLE = sysdba.JOINDATA.TOTABLE And TOFIELD = sysdba.JOINDATA.TOFIELD))) ORDER BY sysdba.JOINDATA.FROMTABLE, sysdba.JOINDATA.FROMFIELD, sysdba.JOINDATA.TOTABLE, sysdba.JOINDATA.TOFIELD ", Test, TestsDataSet);
                            break;
                        }
                    case "Joins To tables that do not exist in RESYNCTABLEDEFS":
                        {
                            GetDataset("SELECT DISTINCT TOTABLE FROM sysdba.JOINDATA WHERE TOTABLE <> '*' AND TOTABLE NOT IN (SELECT TABLENAME FROM sysdba. RESYNCTABLEDEFS) ORDER BY TOTABLE", Test, TestsDataSet);
                            break;
                        }
                    case "Joins From tables that do not exist in RESYNCTABLEDEFS":
                        {
                            GetDataset("SELECT DISTINCT FROMTABLE FROM sysdba.JOINDATA WHERE FROMTABLE <> '*' AND FROMTABLE NOT IN (SELECT TABLENAME FROM sysdba.RESYNCTABLEDEFS) ORDER BY FROMTABLE", Test, TestsDataSet);
                            break;
                        }
                    case "Duplicate FIELDINDEX values in SECTABLEDEFS":
                        {
                            GetDataset("SELECT A2.TABLENAME, A2.FIELDNAME, A1.FIELDINDEX AS FIELDINDEX, Count(A1.FIELDINDEX) AS DUPLICATES FROM sysdba.SECTABLEDEFS AS A1 INNER JOIN sysdba.SECTABLEDEFS AS A2 ON A1.FIELDINDEX = A2.FIELDINDEX GROUP BY A1.FIELDINDEX, A2.TABLENAME, A2.FIELDNAME HAVING (((Count(A1.FIELDINDEX))>1)) ORDER BY A1.FIELDINDEX", Test, TestsDataSet);
                            break;
                        }
                    case "PLUGINS based on another PLUGIN that no longer exists":
                        {
                            GetDataset("SELECT PLUGINID, BASEDON, NAME, FAMILY, TYPE, USERID FROM sysdba.PLUGIN WHERE BASEDON NOT IN (SELECT PLUGINID FROM sysdba.PLUGIN) AND BASEDON NOT IN ('', NULL, 'STANDARD', 'X') ORDER BY NAME, FAMILY, TYPE, USERID", Test, TestsDataSet);
                            break;
                        }
                }

            }
            return TestsDataSet;
        }
        public DataSet RunDBICTest(string Test,DB Conn)
        {
            DataSet ICTestsDataSet=new DataSet();
            string[] testing = Test.Split('|');
            try
            {
                GetDataset(testing[2], testing[1], ICTestsDataSet,Conn);
            }
            catch(Exception Ex)
            {
                if (ICTestsDataSet.Tables.Count==0)
                {
                    DataTable ErrorTable=new DataTable();
                    ErrorTable.TableName=testing[1];
                    ErrorTable.Columns.Add("Error", typeof(string));
                    ErrorTable.Rows.Add(Ex.Message.ToString());
                   
                    ICTestsDataSet.Tables.Add(ErrorTable);
                }
            }
            return ICTestsDataSet;
        }
        public Boolean TestConn(string sConnection, OracleConnection OConnection)
        {
            //Connection test method for Oracle
            try
            {
                OConnection.ConnectionString = sConnection;
                OConnection.Open();
                // Try to close the connection
                if (OConnection != null)
                {
                    OConnection.Dispose();
                }
                return true;
            }
            catch (Exception Ex)
            {
                // Create a (useful) error message
                string ErrorMessage = "A error occurred while trying to connect to the server.";
                ErrorMessage += Environment.NewLine;
                ErrorMessage += Environment.NewLine;
                ErrorMessage += Ex.Message;

                // Show error message (this = the parent Form object)
                MessageBox.Show(ErrorMessage, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                // Stop here
                return false;
            }
        }
        public Boolean TestConn(string sConnection, SqlConnection SQLConnection)
        {
            //Connection test method for SQL
            try
            {
                SQLConnection.ConnectionString = sConnection;
                SQLConnection.Open();
                // Try to close the connection
                if (SQLConnection != null)
                {
                    SQLConnection.Dispose();
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        public Boolean TestConn(string sConnection, OleDbConnection SLXConnection)
        {
            //Connection test method for SQL
            try
            {
                SLXConnection.ConnectionString = sConnection;
                SLXConnection.Open();
                // Try to close the connection
                if (SLXConnection != null)
                {
                    SLXConnection.Dispose();
                }
                return true;
            }
            catch (Exception Ex)
            {
                // Create a (useful) error message
                string ErrorMessage = "A error occurred while trying to connect to the server.";
                ErrorMessage += Environment.NewLine;
                ErrorMessage += Environment.NewLine;
                ErrorMessage += Ex.Message;

                // Show error message (this = the parent Form object)
                MessageBox.Show(ErrorMessage, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                // Stop here
                return false;
            }

        }
        public Boolean SQLOpen(DB Conn)
        {
            //Close existing connections
            ocn.Close();
            cn.Close();
            scn.Close();


            //set variable for result
            Boolean result=false;

            //test the connection
            switch (Conn.DBType)
            {
                case DB.connectType.None:
                    break;
                case DB.connectType.SalesLogix:
                   {
                        if (TestConn(Conn.ConnString, scn) == true)
                        {
                            scn = new OleDbConnection(Conn.ConnString);
                            scn.Open();
                            result = true;
                        }
                        break;
                    }
                case DB.connectType.SQL:
                    {
                        if (TestConn(Conn.ConnString, cn) == true)
                        {
                            cn = new SqlConnection(Conn.ConnString);
                            cn.Open();
                            result = true;
                        }
                        break;
                    }
                case DB.connectType.Oracle:
                    {
                        if (TestConn(Conn.ConnString, ocn) == true)
                        {
                            ocn = new OracleConnection(Conn.ConnString);
                            ocn.Open();
                            result = true;
                        }
                        break;
                    }
            }
          
            //return result if it opened successfully
            return result;
        }

        public void SQLClose()
        {
            //closes the connections
            cn.Close();
            ocn.Close();
            scn.Close();
        }
        
       
        public void GetLookupDataset(string strSQL, string DSName)
        {
            //locate existing dataset
            if (LUDataSet.Tables.IndexOf(DSName) > -1)
            {
                //clear is found
                LUDataSet.Tables[DSName].Clear();
            }

            //request dataset
            switch (SLXDB.DBType)
            {
                case DB.connectType.None:
                    break;
                case DB.connectType.SalesLogix:
                    {
                        sda = new OleDbDataAdapter(strSQL, scn);
                        scmdBuilder = new OleDbCommandBuilder(sda);
                        sda.Fill(LUDataSet, DSName);
                        break;
                    }
                case DB.connectType.SQL:
                    {
                        da = new SqlDataAdapter(strSQL, cn);
                        cmdBuilder = new SqlCommandBuilder(da);
                        da.Fill(LUDataSet, DSName);
                        break;
                    }
                case DB.connectType.Oracle:
                    {
                        oda = new OracleDataAdapter(strSQL.Replace("+", "||"), ocn);
                        ocmdBuilder = new OracleCommandBuilder(oda);
                        oda.Fill(LUDataSet, DSName);
                        break;
                    }
                
            }
           
        }
        public DataSet GetDataset(string strSQL, string DSName, DataSet BDataSet)
        {
            return BDataSet = GetDataset(strSQL, DSName, BDataSet, SLXDB);
        }
        
        public DataSet GetDataset(string strSQL, string DSName, DataSet BDataSet,DB Conn)
        {
            //locate existing dataset
            if (BDataSet.Tables.IndexOf(DSName) > -1)
            {
                //clear if found
                BDataSet.Tables[DSName].Clear();
            }

            //load new dataset
            switch (Conn.DBType)
            {
                case DB.connectType.None:
                    break;
                case DB.connectType.SalesLogix:
                    {
                        sda = new OleDbDataAdapter(strSQL, scn);
                        scmdBuilder = new OleDbCommandBuilder(sda);
                        sda.Fill(BDataSet, DSName);
                        break;
                    }
                case DB.connectType.SQL:
                    {
                        da = new SqlDataAdapter(strSQL, cn);
                        cmdBuilder = new SqlCommandBuilder(da);
                        da.Fill(BDataSet, DSName);
                        break;
                    }
                case DB.connectType.Oracle:
                    {
                        oda = new OracleDataAdapter(strSQL.Replace("+", "||"), ocn);
                        ocmdBuilder = new OracleCommandBuilder(oda);
                        oda.Fill(BDataSet, DSName);
                        break;
                    }
            }
            
            
            //return the dataset
            return BDataSet;
        }
        public void launchGroupTester()
        {

        }
        public DataTable getdotnet()
        {
            const string regLocation = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP";

            DataTable results = new DataTable();
            results.Columns.Add("Name");
            results.Columns.Add("SP");
            results.Columns.Add("Version");

            RegistryKey masterkey = Registry.LocalMachine.OpenSubKey(regLocation);
            RegistryKey tempKey;
            RegistryKey clientKey;
            string version = "";

            if (masterkey != null)
            {
                string[] SubKeyNames = masterkey.GetSubKeyNames();
                for (int i = 0; i < SubKeyNames.Length; i++)
                {
                    tempKey = Registry.LocalMachine.OpenSubKey(regLocation + "\\" + SubKeyNames[i]);
                    if (tempKey.GetValue("Version") == null)
                    {
                        foreach (string item in tempKey.GetSubKeyNames())
                        {
                            if (item == "Client")
                            {
                                clientKey = Registry.LocalMachine.OpenSubKey(regLocation + "\\" + SubKeyNames[i] + "\\" + item);
                                version = clientKey.GetValue("Version").ToString();
                            }
                        }
                    }
                    else
                    {
                        version = tempKey.GetValue("Version").ToString();
                    }
                    results.Rows.Add(SubKeyNames[i], tempKey.GetValue("SP"), version);
                }
            }
            return results;
        }
        public ArrayList GetProductsInstalled()
        {
            _installedProducts.Clear();
            System.Text.StringBuilder ipath = new System.Text.StringBuilder(512);

            foreach (string product in ProductCodes.PRODUCTCODES)
            {
                Int32 len = 512;
                System.Text.StringBuilder version = new System.Text.StringBuilder(len);
                int result = MsiGetProductInfo(product, InstallProperties.INSTALLPROPERTY_VERSIONSTRING, version, ref len);
                if (result == 0)
                {
                    // product info
                    len = 512;
                    System.Text.StringBuilder name = new System.Text.StringBuilder(len);
                    MsiGetProductInfo(product, InstallProperties.INSTALLPROPERTY_INSTALLEDPRODUCTNAME, name, ref len);
                    _installedProducts.Add(name.ToString() + "  -  " + version.ToString());

                    //install path
                    if (ipath.Length > 8)
                    {

                    }
                    else
                    {
                        MsiGetProductInfo(product, InstallProperties.INSTALLPROPERTY_INSTALLLOCATION, ipath, ref len);
                    }
                    // is it patched?
                    len = 512;
                    System.Text.StringBuilder patchGuid = new System.Text.StringBuilder(len);
                    System.Text.StringBuilder patchTransforms = new System.Text.StringBuilder(len);
                    //for (int patchIndex = 0; WinError.ERROR_NO_MORE_ITEMS != MsiEnumPatches(product, patchIndex, patchGuid, patchTransforms, ref len); patchIndex++)
                    for (int patchIndex = 0; 259 != MsiEnumPatches(product, patchIndex, patchGuid, patchTransforms, ref len); patchIndex++)
                    {
                        // yes, get patch info
                        len = 512;
                        uint len2 = (uint)len;
                        System.Text.StringBuilder patchName = new System.Text.StringBuilder(len);
                        MsiGetPatchInfoEx(patchGuid.ToString(), product, null, MSIINSTALLCONTEXT.Machine, InstallProperties.INSTALLPROPERTY_DISPLAYNAME, patchName, ref len2);
                        _installedProducts.Add("  + " + patchName.ToString());
                    }
                    _installedProducts.Add(" ");
                }
            }
            InstallPath = ipath.ToString();
            return _installedProducts;
        }
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);
        public bool CheckLibrary(string fileName)
        {
            return LoadLibrary(fileName) != IntPtr.Zero;
        }
        public List<string> ComputersInDomain()
        {
            List<string> ComputerNames = new List<string>();
            string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            DirectoryEntry de = new DirectoryEntry("LDAP://" + domainName);
            DirectorySearcher objSearcher = new DirectorySearcher(de);
            objSearcher.Filter = ("(ObjectClass=computer)");
            foreach (SearchResult resEnt in objSearcher.FindAll())
            {
                string ComputerName = resEnt.GetDirectoryEntry().Name;
                if (ComputerName.StartsWith("CN="))
                    ComputerName = ComputerName.Remove(0, "CN=".Length);
                ComputerNames.Add(ComputerName);
            }

            objSearcher.Dispose();
            de.Dispose();

            return ComputerNames;

        }
        public DataTable BrowserVersion()
        {
            //Declare the DataTable:
            DataTable Software = new DataTable();
            DataColumn Name = new DataColumn();
            DataColumn Version = new DataColumn();
            Software.Columns.Add(Name);
            Software.Columns.Add(Version);

            //The registry key:
            string SoftwareKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            string SoftwareKey32 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

            string ieVer = "";
            try
            {
                ieVer = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer").GetValue("DisplayVersion").ToString();
            }
            catch { }
            if (ieVer.Length == 0)
            {
                try
                {
                    ieVer = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer").GetValue("svcVersion").ToString();
                }
                catch { }
                if (ieVer.Length == 0)
                {
                    try
                    {
                        ieVer = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer").GetValue("Version").ToString();
                    }
                    catch
                    {
                    }
                }
            }
            bool FFFound = false;
            bool OpFound = false;
            bool ChFound = false;

            Software.Rows.Add("Internet Explorer", ieVer);
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(SoftwareKey))
            {
                //Let's go through the registry keys and get the info we need:
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        try
                        {
                            //If the key has value, continue, if not, skip it:
                            if (!(sk.GetValue("DisplayName") == null))
                            {
                                if (sk.GetValue("DisplayName").ToString().Contains("Firefox") && !FFFound)
                                {
                                    Software.Rows.Add("Firefox", sk.GetValue("DisplayVersion").ToString());
                                    FFFound = true;
                                }
                                if (sk.GetValue("DisplayName").ToString().Contains("Chrome") && !ChFound)
                                {
                                    Software.Rows.Add("Chrome", sk.GetValue("DisplayVersion").ToString());
                                    ChFound = true;
                                }
                                if (sk.GetValue("DisplayName").ToString().Contains("Opera") && !OpFound)
                                {
                                    Software.Rows.Add("Opera", sk.GetValue("DisplayVersion").ToString());
                                    OpFound = true;
                                }
                            }
                        }
                        catch
                        {
                            //No, that exception is not getting away... :P/>
                        }
                    }
                }
            }
            using (RegistryKey rk2 = Registry.LocalMachine.OpenSubKey(SoftwareKey32))
            {
                //Let's go through the registry keys and get the info we need:
                foreach (string skName in rk2.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk2.OpenSubKey(skName))
                    {
                        try
                        {
                            //If the key has value, continue, if not, skip it:
                            if (!(sk.GetValue("DisplayName") == null))
                            {
                                if (sk.GetValue("DisplayName").ToString().Contains("Firefox") && !FFFound)
                                {
                                    Software.Rows.Add("Firefox", sk.GetValue("DisplayVersion").ToString());
                                    FFFound = true;
                                }
                                if (sk.GetValue("DisplayName").ToString().Contains("Chrome") && !ChFound)
                                {
                                    Software.Rows.Add("Chrome", sk.GetValue("DisplayVersion").ToString());
                                    ChFound = true;
                                }
                                if (sk.GetValue("DisplayName").ToString().Contains("Opera") && !OpFound)
                                {
                                    Software.Rows.Add("Opera", sk.GetValue("DisplayVersion").ToString());
                                    OpFound = true;
                                }
                            }
                        }
                        catch
                        {
                            //No, that exception is not getting away... :P/>
                        }
                    }
                }
            }
            return Software;
        }
        public void AppInstalled()
        {
            //Zero out the installed counter list
            for (int y = 0; y < 24; y++)
            {
                installed[y] = 0;
            }

            //get count info for Installed applications
            for (int i = 0; i < ListApps.Length; i++)
            {
                //set count to 0
                int count = 0;

                //get index of file
                int test = filename.IndexOf(ListAppFiles[i]);

                //while file is found
                while (test > -1)
                {
                    //add to count
                    count += 1;

                    //see if there is another file
                    test = filename.IndexOf(ListAppFiles[i], test + 1);
                }

                //set count to installed
                installed[i] = count;
            }
        }
        public string AppsCheck()
        {
            //holder for result
            string slx = "";

            //Populates list of number of installed apps of each name 
            AppInstalled();


            //cycle through and list apps
            for (int i = 0; i < installed.GetUpperBound(0); i++)
            {
                if (installed[i] > 0)  //app is installed
                {
                    int position = filename.IndexOf(ListAppFiles[i]);  //locate file in list of files

                    //while file is found add to result and look again
                    while (position > -1)
                    {
                        slx += ListApps[i] + "\r\n" + "   " + myFI[position].FileVersion + "\r\n";
                        position = filename.IndexOf(ListAppFiles[i], position + 1);
                    }
                }
            }

            //add saleslogix version
            slx += "\r\nSalesLogix Version:  ";
            if (filename.Count > 0)
            {
                if (filename.IndexOf("saleslogix.exe") > -1)
                {
                    int count = 0;
                    int test = filename.IndexOf("saleslogix.exe");
                    while (test > -1)
                    {
                        count += 1;
                        test = filename.IndexOf("saleslogix.exe", test + 1);
                    }

                    //report if more than one client found
                    if (count > 1)
                    {
                        slx += count.ToString() + " installs of SalesLogix in the test area\n\r";
                    }

                    //determine version and service pack
                    string MainVersion = myFI[filename.IndexOf("saleslogix.exe")].FileMajorPart.ToString() + "." + myFI[filename.IndexOf("saleslogix.exe")].FileMinorPart.ToString();
                    string ServicePack = myFI[filename.IndexOf("saleslogix.exe")].FileBuildPart.ToString();

                    //add version and SP to result
                    slx += MainVersion + "\r\n";
                    if (ServicePack == "0" || ServicePack == null)
                    {
                        slx += "No Service Pack found\r\n";
                        ServicePack = "0";
                    }
                    else
                    {
                        slx += "Service Pack: " + ServicePack + " found";
                    }
                }

            }
            return slx;
        }
        public void GetFiles(string path)
        {
            filename.Clear();
            Array.Clear(myFI, 0, myFI.GetUpperBound(0));
            if (path.Length > 0)
            {
                files = GetFilesRecursive(path);
            }
            else
            {
                if (InstallPath.Length>0)
                {
                    files = GetFilesRecursive(InstallPath);
                }
            }
        }
        public string GetInstalled()
        {
            string result = "";
            ArrayList products = GetProductsInstalled();
            foreach (var product in products)
            {
                result += product.ToString() + "\r\n";
            }
            return result;
        }

        public List<string> GetFilesRecursive(string directory)
        {
            //List and stack to hold files
            List<string> result = new List<string>();
            Stack<string> stack = new Stack<string>();

            //add directory to stack
            stack.Push(directory);

            //if stack has contents
            while (stack.Count > 0)
            {
                //get directory from top of stack
                string dir = stack.Pop();
                try
                {
                    //add files from directory to result
                    result.AddRange(Directory.GetFiles(dir, "*.*"));

                    //add sub-directories to stack
                    foreach (string dn in Directory.GetDirectories(dir))
                    {
                        stack.Push(dn);
                    }
                }
                catch
                {
                    // Could not open the directory
                }
            }
            //return list of files
            return result;
        }
        public string GetFileByName(string fileName)
        {
            string result = "";
            //test to find occurence
            int test = filename.IndexOf(fileName.ToLower());

            //if found add to string and find next occurance
            while (test > -1)
            {
                result += myFI[test].OriginalFilename + "(" + myFI[test].FileVersion + " --- Build: " + myFI[test].FileBuildPart + ")\r\n";
                test = filename.IndexOf(fileName.ToLower(), test + 1);
            }
            return result;
        }
        public void Export(string fileName)
        {
            //create string to new file to store version info
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\" + fileName))
            {
                //write header
                string header = "File Name,File Version,Major Part,Minor Part,Build Part,Private Part,Product Name,Product Version,Major Part,Minor Part,Build Part,Private Part";
                file.WriteLine(header);

                //walk through files
                foreach (string File in files)
                {
                    // Show the complete file path (including file name)
                    FileVersionInfo FI = FileVersionInfo.GetVersionInfo(File);

                    //make sure Internal name exists
                    if ((FI.InternalName != null) && (FI.InternalName.Length > 0))
                    {
                        //build versioning line
                        string line = "";
                        line += FI.FileName.Substring(FI.FileName.LastIndexOf('\\') + 1).ToLower() + ",";
                        line += FI.FileVersion + ",";
                        line += FI.FileMajorPart + ",";
                        line += FI.FileMinorPart + ",";
                        line += FI.FileBuildPart + ",";
                        line += FI.FilePrivatePart + ",";
                        line += FI.ProductName + ",";
                        line += FI.ProductVersion + ",";
                        line += FI.ProductMajorPart + ",";
                        line += FI.ProductMinorPart + ",";
                        line += FI.ProductBuildPart + ",";
                        line += FI.ProductPrivatePart;


                        //write line to stream
                        file.WriteLine(line);
                    }
                }
                file.Close();
            }
        }
        public List<string> GetIni(string filename)
        {
            String line = "";
            List<string> lines = new List<string>();

            using (StreamReader sr = new StreamReader(filename))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }
        public void ProcessIni(List<string> ini)
        {
            string path = "";
            string ofile = "";
            string slxsrv = "";
            string sqlsrv = "";
            string websrv = "";
            string slxuser = "";
            string sqluser = "";
            string slxalias = "";
            string sqldb = "";
            string instance = "";
            string slxpass = "";
            string sqlpass = "";
            int i = 0;
            while (i<ini.Count)
            {

                if (ini[i].StartsWith("["))
                {
                    switch (ini[i])
                    {
                        case "[Output Path]":
                            {
                                i += 1;
                                path = ini[i];
                                break;
                            }
                        case "[Output File]":
                            {
                                i += 1;
                                ofile = ini[i];
                                break;
                            }
                        case "[SLX Server Name]":
                            {
                                i += 1;
                                slxsrv = ini[i];
                                break;
                            }
                        case "[Web Server]":
                            {
                                i += 1;
                                websrv = ini[i];
                                break;
                            }
                        case "[SQL Server]":
                            {
                                i += 1;
                                sqlsrv = ini[i];
                                break;
                            }
                        case "[Instance]":
                            {
                                i += 1;
                                instance = ini[i];
                                break;
                            }
                        case "[DB Alias]":
                            {
                                i += 1;
                                slxalias = ini[i];
                                break;
                            }
                        case "[Database]":
                            {
                                i += 1;
                                sqldb = ini[i];
                                break;
                            }
                        case "[SLX User]":
                            {
                                i += 1;
                                slxuser = ini[i];
                                break;
                            }
                        case "[SQL User]":
                            {
                                i += 1;
                                sqluser = ini[i];
                                break;
                            }
                        case "[SLX Password]":
                            {
                                i += 1;
                                slxpass = ini[i];
                                break;
                            }
                        case "[SQL Password]":
                            {
                                i += 1;
                                sqlpass = ini[i];
                                break;
                            }
                        case "[Tests]":
                            {
                                //Create Connections
                                DB sqlconn = new DB();
                                DB slxconn = new DB();
                                
                                //SQL Connection
                                sqlconn.srv = sqlsrv;
                                if(instance!="")
                                {
                                    sqlconn.db = sqldb+"\\"+instance;
                                }
                                else
                                {
                                    sqlconn.db = sqldb;
                                }
                                sqlconn.user=sqluser;
                                sqlconn.password = sqlpass;
                                sqlconn.DBType = SecurityTS.DB.connectType.SQL;
                                sqlconn.BuildConn();
                                SQLOpen(sqlconn);

                                //SLX Connection
                                slxconn.srv = slxsrv;
                                slxconn.db = slxalias;
                                slxconn.user = slxuser;
                                slxconn.password = slxpass;
                                slxconn.DBType = SecurityTS.DB.connectType.SalesLogix;
                                slxconn.BuildConn();
                                SQLOpen(slxconn);
                                
                                //Open output file
                                string pathFile = path + ofile;
                                using (StreamWriter OutFile = new StreamWriter(pathFile))
                                {

                                    i += 1;
                                    while (ini[i] != "[End]")
                                    {
                                        switch (ini[i])
                                        {
                                            case "System":
                                                {
                                                    OutFile.WriteLine("System Settings");
                                                    OutFile.WriteLine("Machine Name:  " + Environment.MachineName);
                                                    OutFile.WriteLine("Operating System:  " + Environment.OSVersion);
                                                    try
                                                    {
                                                        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                                                        foreach (ManagementObject queryObj in searcher.Get())
                                                        {
                                                            double dblMemory;
                                                            if (double.TryParse(Convert.ToString(queryObj["TotalPhysicalMemory"]), out dblMemory))
                                                            {
                                                                OutFile.WriteLine("TotalPhysicalMemory is: {0} GB", Convert.ToInt32(dblMemory / (1024 * 1024 * 1024)));
                                                            }
                                                        }
                                                    }
                                                    catch (ManagementException e)
                                                    {
                                                        throw e;
                                                    }
                                                    DataTable dotnet=getdotnet();
                                                    DataTable browser=BrowserVersion();
                                                    OutFile.WriteLine("");
                                                    
                                                    OutFile.WriteLine(".Net Versions");
                                                    for (int dn = 0; dn < dotnet.Rows.Count; dn++)
                                                    {
                                                        if (dotnet.Rows[dn].ItemArray[1].ToString().Length > 0)
                                                        {
                                                            OutFile.WriteLine(dotnet.Rows[dn].ItemArray[0].ToString().PadRight(25) + dotnet.Rows[dn].ItemArray[2].ToString().PadRight(10) +  dotnet.Rows[dn].ItemArray[1].ToString());
                                                        }
                                                        else
                                                        {
                                                            OutFile.WriteLine(dotnet.Rows[dn].ItemArray[0].ToString().PadRight(25) + dotnet.Rows[dn].ItemArray[2]);
                                                        }
                                                    }
                                                    OutFile.WriteLine("");
                                                    
                                                    OutFile.WriteLine("Browser(Version)");
                                                    for (int br = 0; br < browser.Rows.Count; br++)
                                                    {
                                                        OutFile.WriteLine(browser.Rows[br].ItemArray[0].ToString().PadRight(25) + browser.Rows[br].ItemArray[1]);
                                                    }
                                                    OutFile.WriteLine("");
                                                    break;
                                                }
                                            case "DBIntegrity":
                                                {
                                                    OutFile.WriteLine("Database Metadata Tests:");
                                                    DataSet DBI=RunDBIntegrityTest("All",sqlconn);
                                                    //cycle through tables and display results
                                                    for (int j = 0; j < DBI.Tables.Count; j++)
                                                    {
                                                        //use table name as header
                                                        OutFile.WriteLine(DBI.Tables[j].TableName);

                                                        //cycle through rows and add to results
                                                        foreach (DataRow row in DBI.Tables[j].Rows)
                                                        {
                                                            string rowstring = "    ";
                                                                
                                                            foreach (var item in row.ItemArray)
                                                            {
                                                                rowstring+=item.ToString() + "\t";
                                                            }
                                                            OutFile.WriteLine(rowstring);
                                                        }

                                                        //add blank row
                                                        OutFile.WriteLine("");
                                                    }
                                                    break;
                                                }
                                            case "IntegrityChecker":
                                                {
                                                    OutFile.WriteLine("Integrity Checker Tests:");
                                                    GetDBICTest("SalesLogix.sxc");
                                                    //create list for tests
                                                    List<string> DBICTests = new List<string>();

                                                    DataSet dsDBICTest = new DataSet();

                                                    for (int k = 0; k < DBChecker.Length; k++)
                                                    {
                                                                string SQL = DBChecker[k].Select + DBChecker[k].From + DBChecker[k].Where;

                                                                if (DBChecker[k].Name!=null)
                                                                {
                                                                    //add to list
                                                                    DBICTests.Add(DBChecker[k].Family + "|" + DBChecker[k].Name + "|" + SQL);
                                                    
                                                                }
                                                    }

                                                    //pass list to test
                                                    foreach (string testName in DBICTests)
                                                    {
                                                        dsDBICTest = RunDBICTest(testName,slxconn);
                                                        OutFile.WriteLine(dsDBICTest.Tables[0].TableName);
                                                        //cycle through rows and add to results
                                                        foreach (DataRow row in dsDBICTest.Tables[0].Rows)
                                                        {
                                                            string rowstring = "";
                                                            foreach (var item in row.ItemArray)
                                                            {
                                                                rowstring += item.ToString() + "\t";
                                                            }
                                                            OutFile.WriteLine(rowstring);
                                                        }

                                                        //add blank row
                                                        OutFile.WriteLine("");

                                                        dsDBICTest.Tables.Clear();
                                                    }
                                                    break;
                                                }
                                            case "Services":
                                                {
                                                    OutFile.WriteLine("Services Information:");
                                                    //get service controller
                                                    ServiceController[] scServices;

                                                    //get services
                                                    scServices = ServiceController.GetServices();

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
                                                            OutFile.WriteLine(scTemp.DisplayName.PadRight(36) + scTemp.Status.ToString().Trim().PadRight(16) + wmiService["StartName"].ToString().Trim());
                                                        }
                                                    }
                                                    OutFile.WriteLine("");
                                                    break;
                                                }
                                            case "Web":
                                                {
                                                    OutFile.WriteLine("Web Site Information:");
                                                    GetWebServer(websrv);
                                                    OutFile.WriteLine("Web Server:  " + SLXWebServer.ServerName);
                                                    OutFile.WriteLine("Web Sites");
                                                    for (int ws = 0; ws < SLXWebServer.websites.Count; ws++)
                                                    {
                                                        //get website
                                                        OutFile.WriteLine(SLXWebServer.websites[ws].SiteName.PadRight(30) + SLXWebServer.websites[ws].State);
                                                        string binding = "Bindings:  " + SLXWebServer.websites[ws].bindings[0].ToString();
                                                        for (int bd = 1; bd < SLXWebServer.websites.Count; bd++)
                                                        {
                                                            binding += ", " + SLXWebServer.websites[ws].bindings[bd].ToString();
                                                        }
                                                        OutFile.WriteLine(binding);
                                                        OutFile.WriteLine("");
                                                        OutFile.WriteLine("Application Pools");
                                                        //get App Pools
                                                        for (int AP = 0; AP < SLXWebServer.apppools.Count; AP++)
                                                        {
                                                            OutFile.WriteLine(SLXWebServer.apppools[AP].PoolName.PadRight(25) + SLXWebServer.apppools[AP].User.PadRight(15) + SLXWebServer.apppools[AP].Version.PadRight(8) + SLXWebServer.apppools[AP].BitEnabled.ToString().PadRight(8) + SLXWebServer.apppools[AP].Mode.PadRight(15) + SLXWebServer.apppools[AP].State);
                                                        }

                                                        //get Applications
                                                        OutFile.WriteLine("");
                                                        OutFile.WriteLine("Applications");
                                                        for (int App = 0; App < SLXWebServer.websites[ws].applications.Count; App++)
                                                        {
                                                            OutFile.WriteLine(SLXWebServer.websites[ws].applications[App].Name.PadRight(25) + SLXWebServer.websites[ws].applications[App].PhysicalPath.PadRight(35) + SLXWebServer.websites[ws].applications[App].UserName.PadRight(15) + SLXWebServer.websites[ws].applications[App].AppPool.PadRight(25) + SLXWebServer.websites[ws].applications[App].AuthString());
                                                        }
                                                    }

                                                    OutFile.WriteLine("");
                                                    break;
                                                }
                                            case "Versions":
                                                {
                                                    OutFile.WriteLine("Version Information:");
                                                    ArrayList products = GetProductsInstalled();
                                                    foreach (var product in products)
                                                    {
                                                        OutFile.WriteLine(product.ToString());
                                                    }
                                                    OutFile.WriteLine("");
                                                    break;
                                                }
                                        }
                                        i += 1;
                                    }
                                }

                                break;
                            }
                    }
                    
                }
                if (i < ini.Count)
                {
                    i += 1;
                }
            }
        }
        public ServerManager GetIIS(string server)
        {
            //set return variable
            ServerManager iisManager = new ServerManager();
            
            //create instance of Server Manager
            if (server.Length > 0)
            {
                //override server if Web server contains a servername
                iisManager = ServerManager.OpenRemote(server);
            }
            else
            {
                //use local server
                iisManager = new ServerManager();
            }

            //return servermanager
            return iisManager;
        }
        public void GetWebServer(string webserver)
        {
            //locate Websites
            ServerManager iisManager = GetIIS(webserver);

            //Clear Web Server
            SLXWebServer.Clear();

            //Create Web Server
            SLXWebServer.ServerName = webserver;

            //sitecounter
            int i = 0;

            //get websites
            foreach (var site in iisManager.Sites)
            {
                try
                {
                    //store site data
                    WebServer.WebSite newSite = new WebServer.WebSite(site.Name, site.State.ToString());

                    //add site to collection
                    SLXWebServer.websites.Add(newSite);
                }
                catch
                {
                    //store site data
                    WebServer.WebSite newSite = new WebServer.WebSite(site.Name, "unknown");

                    //add site to collection
                    SLXWebServer.websites.Add(newSite);
                }
                finally
                {
                    //populate Applications
                    SLXWebServer.websites[i].PopulateApplications(i, GetIIS(SLXWebServer.ServerName), SLXWebServer.ServerName);
                    //cycle through bindings
                    foreach (Microsoft.Web.Administration.Binding bind in site.Bindings)
                    {
                        //add binding to listbox
                        SLXWebServer.websites[i].bindings.Add(bind.ToString());
                    }
                    i += 1;
                }
            }
            //populate Application Pool
            SLXWebServer.GetApplicationPools(iisManager);
        }
        
        public void GetDBICTest(string fileName)
        {
            //create string to new file to store version info
            if (File.Exists(@"C:\programdata\SalesLogix\Integrity Checker\" + fileName))
            {
                string[] lines = File.ReadAllLines(@"C:\programdata\SalesLogix\Integrity Checker\" + fileName);
                int i = -1;
                string[] lineResult = new string[2];
                foreach (string line in lines)
                {
                    lineResult = ProcessLine(line);
                    switch (lineResult[0])
                    {
                        case "Name":
                            {
                                i += 1;
                                DBChecker[i].Name = lineResult[1];
                                break;
                            }
                        case "Family":
                            {
                                DBChecker[i].Family = lineResult[1];
                                break;
                            }
                        case "Select":
                            {
                                DBChecker[i].Select += lineResult[1] + " ";

                                break;
                            }
                        case "From":
                            {
                                DBChecker[i].From += lineResult[1] + " ";
                                break;
                            }
                        case "Where":
                            {
                                DBChecker[i].Where += lineResult[1] + " ";
                                break;
                            }
                        case "Other":
                            {
                                break;
                            }
                    }
                }
                
            }
            
        }
        public string[] ProcessLine(string line)
        {
            string[] result=new string[2];
            if (line == "[OPTIONS]")
            {
                result[0] = "Other";
                result[1] = "";
            }
            if (line.StartsWith("[") && line!="[OPTIONS]")
            {
                result[0] = "Name";
                result[1] = line.Replace("[", "").Replace("]", "");
                result[1] = result[1].Replace('/', '_');
            
            }
            if (line.StartsWith("Family"))
            {
                result[0] = "Family";
                result[1] = line.Substring(line.IndexOf('=') + 1);
            }
            if (line.StartsWith("Select"))
            {
                result[0] = "Select";
                result[1] = line.Substring(line.IndexOf('=') + 1);
            }
            if (line.StartsWith("From"))
            {
                result[0] = "From";
                result[1] = line.Substring(line.IndexOf('=') + 1);
            }
            if (line.StartsWith("Where"))
            {
                result[0] = "Where";
                result[1] = line.Substring(line.IndexOf('=') + 1);
            }
            if (line.Contains("//"))
            {
                result[1] = line.Substring(line.IndexOf('=') + 1);
                result[1] = result[1].Substring(0,result[1].IndexOf("//"));
                if (result[1].Trim().Length==0)
                {
                    result[0] = "Other";
                }
            }
            if (result[0]=="")
            {
                result[0] = "Other";
            }
            return result;
        }
        public ArrayList GetServerList()
        {
            ArrayList list = new ArrayList();
            //string connstring = "Provider=SLXOLEDB.1;Data Source=;Initial Catalog=NO_ALIAS;Extended Properties=\"Port=1706;Log=On\"";
            string connstring = "Provider=SLXOLEDB.1;Password=;Persist Security Info=True;Initial Catalog=NO_ALIAS;Data Source=;Extended Properties=\"PORT=1706;LOG=ON;CASEINSENSITIVEFIND=ON;AUTOINCBATCHSIZE=1;SVRCERT=;\"";
            OleDbConnection conn = new OleDbConnection(connstring);

            try
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("slx_getServerList()", conn);
                OleDbDataReader r = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (r.Read())
                {
                    list.Add(r.GetString(0));
                }
                r.Close();
                cmd.Dispose();
                r.Dispose();
            }
            catch
            {
                int i =0;
                i += 1;
            }
            finally
            {
                conn.Dispose();
            }
            return list;
        }
        public ArrayList GetAliasList(string server)
        {
	        ArrayList list = new ArrayList();
	        if (!server.Equals(string.Empty))
	        {
		        string connstring = string.Format("Provider=SLXOLEDB.1;Data Source={0};Initial Catalog=NO_ALIAS;Extended Properties=\"Port=1706;Log=On\"", server);

        		OleDbConnection conn = new OleDbConnection(connstring);

		        try
		        {
			        conn.Open();
			        OleDbCommand cmd = new OleDbCommand("sp_aliasList()", conn);
			        OleDbDataReader r = cmd.ExecuteReader(CommandBehavior.CloseConnection);

			        while (r.Read())
			        {
				        list.Add(r.GetString(0));
			        }
			        r.Close();
		        }
		        finally
		        {
			        conn.Dispose();
		        }
	        }
	    return list;
}
        private List<string> GetAppPools(string webserver)
        {
            ServerManager iisManager = new ServerManager();
            List<string> apools=new List<string>();
            
            //walk through application pools
            if (webserver.Length > 0)
            {
                iisManager = ServerManager.OpenRemote(webserver);
            }
            else
            {
                iisManager = new ServerManager();
            }
            using (iisManager)
            {
                string apool = "";
                foreach (var pool in iisManager.ApplicationPools)
                {
                    //check IdentityType
                    //add to grid as Specific user
                        apool = pool.Name;
                        if (pool.ProcessModel.IdentityType.ToString() == "SpecificUser")
                        {
                            apool += pool.ProcessModel.UserName;
                        }
                        else
                        {
                            apool += "Pass Through";
                        }
                        apool += pool.ManagedRuntimeVersion;
                        apool += pool.Enable32BitAppOnWin64.ToString();
                        apool += pool.ManagedPipelineMode.ToString();
                        apool += pool.State.ToString();
                        apools.Add(apool);
                  
                }
            }
            return apools;
        }
    }

}
