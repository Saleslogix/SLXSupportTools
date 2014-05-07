using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace SecurityTS
{
    public partial class BuildSettings : Form
    {
        public BuildSettings()
        {
            InitializeComponent();
        }
        public static List<string> GetFilesRecursive(string b)
        {
            List<string> result = new List<string>();
            Stack<string> stack = new Stack<string>();


            stack.Push(b);

            while (stack.Count > 0)
            {
                string dir = stack.Pop();
                try
                {
                    result.AddRange(Directory.GetFiles(dir, "*.*"));
                    foreach (string dn in Directory.GetDirectories(dir))
                    {
                        stack.Push(dn);
                    }
                }
                catch
                {
                    // D
                    // Could not open the directory
                }
            }
            return result;
        }
        private void BuildSettings_Load(object sender, EventArgs e)
        {
             //set variable to hold files
            List<string> filesxml = new List<string>();

            //populate the files(may need to change for 8.1) 
            filesxml = GetFilesRecursive("C:\\ProgramData\\Sage\\Platform\\Configuration\\Machine_User\\");
            
            //process files
            foreach (string file in filesxml)
            {
                //locate files desired
                if (file.Contains("buildsettings.xml"))
                {
                    //split files to get directories
                    string[] folder=file.Split('\\');
                    
                    //get xml file contents
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file);
                    
                    //variables for file contents and expected values
                    string bsp="";
                    string ebsp = "";
                    string Ver="";
                    string AF = "";
                    string SF = "";

                    //locate and set variables from xml
                    foreach (XmlAttribute attr in doc.DocumentElement.Attributes)
                    {
                        if (attr.Name=="BuildSearchPath")
                        {
                            bsp = attr.Value;
                        }
                        //solutionFolder -- AssembliesFolder
                        if (attr.Name=="version")
                        {
                            Ver=attr.Value;
                        }
                        if (attr.Name == "solutionFolder")
                        {
                            SF = attr.Value;
                        }
                        if (attr.Name == "AssembliesFolder")
                        {
                            AF = attr.Value;
                        }
                    }

                    //get base portion of version
                    string bVer = Ver.Substring(0, 3);
                    
                    //get expected build path by version
                    switch (bVer)
                    {
                      
                        case "8.0":
                            {
                                ebsp = "%BASESAGEINSTALLPATH%\\SalesLogix;%BASESAGEINSTALLPATH%\\Platform;%BASESAGEINSTALLPATH%\\SupportFiles;%BASESAGEINSTALLPATH%";
                                break;
                            }
                        case "8.1":
                            {
                                ebsp = "%BASEBUILDPATH%\\assemblies,%BASEBUILDPATH%\\interfaces\\bin,%BASEBUILDPATH%\\forminterfaces\\bin";
                                break;
                            }
                        default:
                            {
                                ebsp = "%BASESAGEINSTALLPATH%\\SalesLogix;%BASESAGEINSTALLPATH%\\Platform;%BASESAGEINSTALLPATH%\\SupportFiles;%BASESAGEINSTALLPATH%";
                                break;
                            }
                    }

                    //determine match and populate grid
                    if (bsp==ebsp)
                    {
                        dgvBSettings.Rows.Add(folder[folder.Length - 4], folder[folder.Length - 3], folder[folder.Length - 2], Ver, "Match", bsp, ebsp, SF, AF);
                    }
                    else
                    {
                        dgvBSettings.Rows.Add(folder[folder.Length - 4], folder[folder.Length - 3], folder[folder.Length - 2], Ver, "Not a Match", bsp, ebsp,SF,AF); ; 
                    }
                }
            }

            for (int i = 0; i < dgvBSettings.RowCount; i++)
            {
                if (dgvBSettings.Rows[i].Cells[4].Value.ToString()=="Not a Match")
                {
                    dgvBSettings.Rows[i].Cells[4].Style.BackColor = Color.Red; 
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvBSettings_SelectionChanged(object sender, EventArgs e)
        {
            txtbs.Text = dgvBSettings.SelectedRows[0].Cells[5].Value.ToString();
            txtebs.Text = dgvBSettings.SelectedRows[0].Cells[6].Value.ToString();
            txtAssemblies.Text = dgvBSettings.SelectedRows[0].Cells[8].Value.ToString();
            txtBuildPath.Text = dgvBSettings.SelectedRows[0].Cells[7].Value.ToString();
        }
    }
}
