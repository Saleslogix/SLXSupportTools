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
    class Performance
    {
        //Legacy Database Indices
        public struct Indexes
        {
            public string Name;
            public string Table;
            public string SQL;
        }

        public DataSet SLXIndex = new DataSet();
        List<Indexes> SQLIndexes = new List<Indexes>();

        //Web Expiration
        public struct WebExpiration
        {
            public string Name;
            public bool Expires;
            public enum ControlMode {NoControl,DisableCache,UseMaxAge,UseExpires};
            public ControlMode cacheControlMode;
            public string cacheControlMaxAge;
            public string HttpExpires;
        }
        public DataSet Groups = new DataSet();
        
        //Speedsearch test
        public int SpeedSearchTest(SqlConnection conn)
        {
            int a = 0;
            DataSet SpeedSearch = new DataSet();
            SpeedSearch = GetDataset("select count(*) from sysdba.IndexUpdates", "SpeedSearch", SpeedSearch, conn);
            a = Convert.ToInt32(SpeedSearch.Tables[0].Rows[0].ItemArray[0].ToString());
            return a;
        }

        public int SpeedSearchTest(OleDbConnection conn)
        {
            int a = 0;
            DataSet SpeedSearch = new DataSet();
            SpeedSearch = GetDataset("select count(*) from sysdba.IndexUpdates", "SpeedSearch", SpeedSearch, conn);
            a = Convert.ToInt32(SpeedSearch.Tables[0].Rows[0].ItemArray[0].ToString());
            return a;
        }

        //Test groups
        public void GroupTests(SqlConnection conn)
        {
            Groups = GetDataset("select name, category, userid from sysdba.useroptions where name = 'creategroupinsert'", "Legacy Options", Groups, conn);
            Groups = GetDataset("select groupid,entityid,createuser from sysdba.ADHOCGROUP where groupid in (select P.Pluginid from sysdba.Plugin P Left Join sysdba.Plugin C on c.basedon = P.pluginid where p.basedOn is NULL and P.Type in (8,23) and c.Pluginid is NULL and P.UserID in (Select userid from sysdba.usersecurity where type='R'))", "ADHOC", Groups, conn);
            Groups = GetDataset("select pluginid, userid, family, name from sysdba.plugin where pluginid in (Select P.Pluginid from sysdba.Plugin P Left Join sysdba.Plugin C ON C.BasedOn =P.PluginID WHERE P.BasedOn is NULL AND P.Type in (8,23) and c.Pluginid is NULL and P.UserID in (Select userid from sysdba.usersecurity where type='R'))", "DYNAMIC Groups", Groups, conn);
        }
        public void GroupTests(OleDbConnection conn)
        {
            Groups = GetDataset("select name, category, userid from sysdba.useroptions where name = 'creategroupinsert'", "Legacy Options", Groups, conn);
            Groups = GetDataset("select groupid,entityid,createuser from sysdba.ADHOCGROUP where groupid in (select P.Pluginid from sysdba.Plugin P Left Join sysdba.Plugin C on c.basedon = P.pluginid where p.basedOn is NULL and P.Type in (8,23) and c.Pluginid is NULL and P.UserID in (Select userid from sysdba.usersecurity where type='R'))", "ADHOC", Groups, conn);
            Groups = GetDataset("select pluginid, userid, family, name from sysdba.plugin where pluginid in (Select P.Pluginid from sysdba.Plugin P Left Join sysdba.Plugin C ON C.BasedOn =P.PluginID WHERE P.BasedOn is NULL AND P.Type in (8,23) and c.Pluginid is NULL and P.UserID in (Select userid from sysdba.usersecurity where type='R'))", "DYNAMIC Groups", Groups, conn);
        }


        //get Indexes
        public void GetIndexes(SqlConnection conn)
        {
            DataSet indexds = new DataSet();
            DataTable tablesds = conn.GetSchema("Tables");

            foreach (DataRow row in tablesds.Rows)
            {
                if (row.ItemArray[1].ToString().ToUpper() == "SYSDBA")
                {
                    GetDataset("EXECUTE sp_helpindex 'SysDBA." + row.ItemArray[2] + "'", row.ItemArray[2].ToString(), SLXIndex, conn);
                }
            }
        }

        //Get Web content expiration main
        public List<WebExpiration> GetContentExpiration(string webserver)
        {
            List<WebExpiration> results = new List<WebExpiration>();

            results.Add(GetExpiration(webserver, "/SLXClient/CSS"));
            results.Add(GetExpiration(webserver, "/SLXClient/Images"));
            results.Add(GetExpiration(webserver, "/SLXClient/Jscript"));
            results.Add(GetExpiration(webserver, "/SLXClient/Library"));
           
            return results;
        }

        //get web content expiration by location
        public WebExpiration GetExpiration(string webserver, string location)
        {
            WebExpiration result = new WebExpiration();
            using (ServerManager iisManager = new ServerManager())
            {
                Configuration config = iisManager.GetWebConfiguration(webserver, location);
                ConfigurationSection staticContentSection = config.GetSection("system.webServer/staticContent");
                ConfigurationElement clientCacheElement = staticContentSection.GetChildElement("clientCache");
                result.cacheControlMode=(Performance.WebExpiration.ControlMode)Convert.ToInt32(clientCacheElement.Attributes["cacheControlMode"].Value);
                result.Name = location.Substring(location.LastIndexOf('/'));
                result.Expires = true;
                switch (result.cacheControlMode)
                {
                    case WebExpiration.ControlMode.NoControl:
                        result.Expires = false;
                        break;
                    case WebExpiration.ControlMode.DisableCache:
                        break;
                    case WebExpiration.ControlMode.UseMaxAge:
                        result.cacheControlMaxAge = clientCacheElement.Attributes["cacheControlMaxAge"].Value.ToString();
                        break;
                    case WebExpiration.ControlMode.UseExpires:
                        result.HttpExpires= clientCacheElement.Attributes["httpExpires"].Value.ToString();
                        break;
                }
            }
            return result;
        }

        //get SQL dataset
        public DataSet GetDataset(string strSQL, string DSName, DataSet BDataSet, SqlConnection conn)
        {
            //locate existing dataset
            if (BDataSet.Tables.IndexOf(DSName) > -1)
            {
                //clear if found
                BDataSet.Tables[DSName].Clear();
            }

            //load new dataset
            SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(da);
            da.Fill(BDataSet, DSName);

            //return the dataset
            return BDataSet;
        }

        public DataSet GetDataset(string strSQL, string DSName, DataSet BDataSet, OleDbConnection conn)
        {
            //locate existing dataset
            if (BDataSet.Tables.IndexOf(DSName) > -1)
            {
                //clear if found
                BDataSet.Tables[DSName].Clear();
            }

            //load new dataset
            OleDbDataAdapter da = new OleDbDataAdapter(strSQL, conn);
            OleDbCommandBuilder cmdBuilder = new OleDbCommandBuilder(da);
            da.Fill(BDataSet, DSName);

            //return the dataset
            return BDataSet;
        }
        
    }
}
