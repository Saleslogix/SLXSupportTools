using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityTS
{
    public class DB
    {
        //Database variables
        public string db;                   //Holds the database or Alias name
        public string srv;                  //Holds the server name
        public string user;                 //Holds the user name
        public string password;             //Holds the password
        public enum connectType {None,SalesLogix,SQL,Oracle};
        public connectType DBType;
        public Boolean sqlconnected=false;        //Holds SQL connection status
        public Boolean dbconnected=false;         //Holds DB connection status
        public string ConnString;           //Holds the Connection string
        
        public void BuildConn()
        {
            //Builds the connection string based on the database information
            switch (DBType)
            {
                case connectType.SalesLogix:
                    {
                        if (password=="")
                        {
                            ConnString = "Provider=SLXOLEDB.1;Password=\"\";";
                        }
                        else
                        {
                            ConnString = "Provider=SLXOLEDB.1;Password=" + password + ";";
                        }
                        ConnString += "Persist Security Info=True;User ID=" + user + ";";
                        ConnString += "Initial Catalog=" + db + ";";
                        ConnString += "Data Source=" + srv + ";Extended Properties=\"PORT=1706;LOG=ON;CASEINSENSITIVEFIND=ON;AUTOINCBATCHSIZE=1;SVRCERT=;\"";
                        break;
                    }
                case connectType.SQL:
                    {
                        ConnString = "User ID=" + user + ";";
                        ConnString += "PASSWORD=" + password + ";";
                        if (db.Length > 1)
                        {
                            ConnString += "Initial Catalog=" + db + ";";
                        }
                        ConnString += "Data Source=" + srv + ";";
                        break;
                    }
                case connectType.Oracle:
                    {
                        ConnString = "Data Source = " + srv + "; User Id = " + user + "; Password = " + password + "; Integrated Security = no;";
                        break;
                    }
                case connectType.None:
                    {
                        ConnString = "";
                        break;
                    }
                default:
                    break;
            }
        }
        
        public void Clear()
        {
            db = "";
            srv = "";
            user = "";                  
            password = "";
            dbconnected = false;        
            ConnString = "";
            DBType = connectType.None;
        }
        public void Disconnect()
        {
            //disconnects DB and Sql connected statuses
            dbconnected = false;
            sqlconnected = false;
        }
        public void Connect()
        {
            //checks if database is set and set connection status based on results
            switch (DBType)
            {
                case connectType.None:
                    {
                        break;
                    }
                case connectType.SalesLogix:
                    {
                        if (db.Length > 0)
                        {
                            dbconnected = true;
                        }
                        sqlconnected = true;
                        break;
                    }
                case connectType.SQL:
                    {
                        if (db.Length > 0)
                        {
                            dbconnected = true;
                        }
                        sqlconnected = true;
                        break;
                    }
                case connectType.Oracle:
                    break;
                default:
                    break;
            }
            
        }
    }
}
