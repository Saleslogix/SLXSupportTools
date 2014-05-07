using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;

namespace SecurityTS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //TS access for functions
            TS ts = new TS();

            //set defaults for connection
            ts.SLXDB.user = "sa";
            ts.SLXDB.db = "SalesLogix";
            ts.SLXDB.srv = Environment.MachineName;

            //if no args display gui 
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMainPanel());
            }
            else
            {
                //process args
                foreach (string arg in args)
                {
                    //determine if parameters exist
                    if (arg.Contains(":"))
                    {
                        //seperate command from parameter
                        string[] argu = arg.Split(':');

                        //process command
                        switch (argu[0].ToUpper())
                        {
                            //Individual help topics
                            case "/?":
                            case "/H":
                            case "/HELP":
                                {
                                    string strcmd = argu[1].ToUpper().Trim().Replace("/", "");
                                    switch (strcmd)
                                    {
                                        case "H":
                                        case "?":
                                        case "HELP":
                                            {
                                                ts.WriteToConsole("");
                                                ts.WriteToConsole("/H | /HELP | /? | /H:<switch name> | /HELP:<switch name> | /?:<switch name>");
                                                ts.WriteToConsole("Displays help message.  Can include a parameter to display help for a given switch.");
                                                ts.WriteToConsole("Example:");
                                                ts.WriteToConsole("   /H");
                                                ts.WriteToConsole("   -Displays the help information for the switches");
                                                ts.WriteToConsole("   /? user");
                                                ts.WriteToConsole("   -Displays the help information for the /User switch");
                                                break;
                                            }
                                        case "I":
                                        case "INI":
                                            {
                                                ts.WriteToConsole("");
                                                ts.WriteToConsole("/I:<filename> | /INI:<filename>");
                                                ts.WriteToConsole("Run Commands from an INI file.");
                                                ts.WriteToConsole("Example:");
                                                ts.WriteToConsole("   /I:automate.ini");
                                                ts.WriteToConsole("   -Executes commands from the automate file");
                                                break;
                                            }
                                        default:
                                            {
                                                ts.WriteToConsole(argu[1].ToString() + "Does not seem to be a valid switch.");
                                                break;
                                            }
                                    }
                                    break;
                                }
                            //Set INI file
                            case "/I":
                            case "/INI":
                                {
                                    List<string> Lines=new List<string>();
                                    Lines=ts.GetIni(argu[1]);
                                    ts.ProcessIni(Lines);
                                    break;
                                }
                            default:
                                {
                                    //tell user of bad parameter
                                    ts.WriteToConsole(argu[0] + " is not a reconized variables.");
                                    break;
                                }
                        }
                    }
                    else
                    {
                        //command without parameters
                        switch (arg.ToUpper())
                        {
                            //display help
                            case "/?":
                            case "/H":
                            case "/HELP":
                                {
                                    ts.WriteToConsole("");
                                    ts.WriteToConsole("Connection keys:");
                                    ts.WriteToConsole("/I /INI           INI file execution");
                                    ts.WriteToConsole("    example:  /I:automate.ini");
                                    ts.WriteToConsole("/?  /H /Help      Help file");
                                    ts.WriteToConsole("    example:  /?");
                                    break;
                                }
                            case "/I":
                            case "/INI":
                            default:
                                {
                                    //report to user any invalid values
                                    ts.WriteToConsole(arg + " is not a reconized variable.");
                                    break;
                                }
                        }
                    }
                }
            }
        }
    }
}
