using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Web.Administration;
using System.Web.Configuration;

namespace SecurityTS
{
    public class WebServer
    {
        //Public variables
        private string servername;                                 //Holds the Server name
        public List<WebSite> websites = new List<WebSite>();
        public List<AppPool> apppools=new List<AppPool>();          //AppPools for the web server
        
        public string ServerName
        {
            get
            {
                return servername;
            }
            set
            {
                servername = value;
            }
        }
        public WebServer()
        {
         
        }
        public WebServer(string servername)
        {
            ServerName = servername;
        }
        public void Clear()
        {
            servername = "";
            websites.Clear();
            apppools.Clear();
        }
        public void GetApplicationPools(ServerManager iisManager)
            {
                foreach (var pool in iisManager.ApplicationPools)
                {
                    //check IdentityType
                    if (pool.ProcessModel.IdentityType.ToString() == "SpecificUser")
                    {
                        //add apppool as Specific user
                        AppPool newap = new AppPool(pool.Name, pool.ProcessModel.UserName, pool.ManagedRuntimeVersion, pool.Enable32BitAppOnWin64, pool.ManagedPipelineMode.ToString(), pool.State.ToString());
                        apppools.Add(newap);
                    }
                    else
                    {
                        //add to datagrid as passthrough
                        AppPool newap = new AppPool(pool.Name, "Pass Through", pool.ManagedRuntimeVersion, pool.Enable32BitAppOnWin64, pool.ManagedPipelineMode.ToString(), pool.State.ToString());
                        apppools.Add(newap);
                    }
                }
            }
        public class WebSite
        {
            //Public variables
            private string sitename;                                 //Holds the website name
            private string state;                                    //Holds the site state
            public List<App> applications = new List<App>();          //Applications for the web site
            public List<string> bindings = new List<string>();

            public string SiteName
            {
                get
                {
                    return sitename;
                }
                set
                {
                    sitename = value;
                }
            }
            public string State
            {
                get
                {
                    return state;
                }
                set
                {
                    state = value;
                }
            }
            public WebSite()
            {

            }
            public WebSite(string sitename, string state)
            {
                SiteName = sitename;
                State = state;
            }

            public void PopulateApplications(int sitenum, ServerManager iisManager, string ServerName)
            {
                ConfigurationElementCollection AppCollection = new ConfigurationElementCollection();

                //use iisManger as instance of Server Manager
                using (iisManager)
                {
                    //get current site
                    Site site = iisManager.Sites[sitenum];

                    //get user credentials
                    using (ServerManager AppManager = iisManager)
                    {
                        Microsoft.Web.Administration.Configuration config = AppManager.GetApplicationHostConfiguration();
                        ConfigurationSection sitesSection = config.GetSection("system.applicationHost/sites");
                        ConfigurationElementCollection sitesCollection = sitesSection.GetCollection();
                        int x = 0;
                        bool found = false;
                        //locate site
                        while (!found && x < sitesCollection.Count)
                        {
                            if (sitesCollection[x].Attributes["Name"].Value.ToString() == site.Name.ToString())
                            {
                                found = true;
                            }
                            else
                            {
                                x += 1;
                            }
                            AppCollection = sitesCollection[x].GetCollection();
                        }

                        //cycle through site's applications
                        foreach (var sapp in site.Applications)
                        {
                            //variables for authentication
                            string wauth;
                            string aauth;
                            string forms = "False";
                            string bauth;

                            //get authentification data
                            try
                            {
                                //get host configuration
                                Microsoft.Web.Administration.Configuration aconfig = iisManager.GetApplicationHostConfiguration();

                                //get anonymous auth
                                Microsoft.Web.Administration.ConfigurationSection anonymousAuthenticationSection = aconfig.GetSection("system.webServer/security/authentication/anonymousAuthentication", ServerName);
                                aauth = anonymousAuthenticationSection["enabled"].ToString();

                                //get windows auth
                                Microsoft.Web.Administration.ConfigurationSection windowsAuthenticationSection = aconfig.GetSection("system.webServer/security/authentication/windowsAuthentication", ServerName);
                                wauth = windowsAuthenticationSection["enabled"].ToString();

                                //get basic auth
                                Microsoft.Web.Administration.ConfigurationSection basicAuthenticationSection = aconfig.GetSection("system.webServer/security/authentication/basicAuthentication", ServerName);
                                bauth = anonymousAuthenticationSection["enabled"].ToString();

                                //ConfigurationSection formsAuthenticationSection = aconfig.GetSection("system.web/authentication", servername);
                                System.Configuration.Configuration bconfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(sapp.Path, site.Name);

                                //get authentication section for forms auth
                                AuthenticationSection formsAuthenticationSection = (AuthenticationSection)bconfig.GetSection("system.web/authentication");



                                //Check if FormsAuthentication enabled 
                                if (formsAuthenticationSection.Mode.ToString() == "Forms")
                                {
                                    forms = "True";
                                }

                            }
                            catch
                            {
                                //return unknown if unable to get authentifications
                                aauth = "Unknown";
                                wauth = "Unknown";
                                bauth = "Unknown";
                                forms = "Unknown";
                            }


                            //get username for app
                            string userName = "unknown";
                            try
                            {
                                int xx = 0;
                                while (AppCollection[xx].Attributes["Path"].Value.ToString() != sapp.Attributes["path"].Value.ToString())
                                {
                                    xx += 1;
                                }
                                ConfigurationElementCollection vd = AppCollection[xx].GetCollection();
                                userName = vd[0].Attributes["username"].Value.ToString();
                            }
                            catch
                            {

                            }

                            //create app entry
                            App newApp = new App(sapp.Attributes["path"].Value.ToString(), sapp.VirtualDirectories[0].PhysicalPath, userName, wauth, aauth, bauth, forms, sapp.ApplicationPoolName);

                            //add to application
                            applications.Add(newApp);
                        }
                    }
                }
            }
        }
        public class App
        {
            //class to hold Application information
            private string name;
            private string apppool;
            private string username;
            private string physicalpath;
            private string status;
            private string windowsauthentication;
            private string aauth;
            private string bauth;
            private string fauth;

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }
            public string AppPool
            {
                get
                {
                    return apppool;
                }
                set
                {
                    apppool = value;
                }
            }
            public string UserName
            {
                get
                {
                    return username;
                }
                set
                {
                    username = value;
                }
            }
            public string PhysicalPath
            {
                get
                {
                    return physicalpath;
                }
                set
                {
                    physicalpath = value;
                }
            }
            public string Status
            {
                get
                {
                    return status;
                }
                set
                {
                    status = value;
                }
            }
            public string WindowsAuthentication
            {
                get
                {
                    return windowsauthentication;
                }
                set
                {
                    windowsauthentication = value;
                }
            }
            public string AAuth
            {
                get
                {
                    return aauth;
                }
                set
                {
                    aauth = value;
                }
            }
            public string BAuth
            {
                get
                {
                    return bauth;
                }
                set
                {
                    bauth = value;
                }
            }
            public string FAuth
            {
                get
                {
                    return fauth;
                }
                set
                {
                    fauth = value;
                }
            }

            public App()
            {
            }

            public App(string name, string physicalPath, string userName, string wauth, string aauth, string bauth, string fauth, string apppool)
            {
                Name = name;
                AppPool = apppool;
                PhysicalPath = physicalPath;
                UserName = userName;
                WindowsAuthentication = wauth;
                AAuth = aauth;
                BAuth = bauth;
                FAuth = fauth;
                Status = "Untested";
            }
            public string AuthString()
            {
                string auth = "";
                if (AAuth == "True")
                {
                        auth += "A ";
                }
                if (BAuth == "True")
                {
                    auth += "B ";
                }
                if (FAuth == "True")
                {
                    auth += "F ";
                }
                if (WindowsAuthentication == "True")
                {
                    auth += "W ";
                }

                return auth;
            }
        }
        public class AppPool
        {
            //class to hold Application information
            private string poolname;
            private string user;
            private string version;
            private bool bitenabled;
            private string mode;
            private string state;

            public string PoolName
            {
                get
                {
                    return poolname;
                }
                set
                {
                    poolname = value;
                }
            }

            public string User
            {
                get
                {
                    return user;
                }
                set
                {
                    user = value;
                }
            }
            public string Version
            {
                get
                {
                    return version;
                }
                set
                {
                    version = value;
                }
            }
            public bool BitEnabled
            {
                get
                {
                    return bitenabled;
                }
                set
                {
                    bitenabled = value;
                }
            }
            public string Mode
            {
                get
                {
                    return mode;
                }
                set
                {
                    mode = value;
                }
            }
            public string State
            {
                get
                {
                    return state;
                }
                set
                {
                    state = value;
                }
            }

            public AppPool()
            {
            }

            public AppPool(string poolname, string user, string version, bool bitenabled, string mode, string state)
            {
                PoolName = poolname;
                User = user;
                Version = version;
                BitEnabled = bitenabled;
                Mode = mode;
                State = state;
            }
        }
    }
}
