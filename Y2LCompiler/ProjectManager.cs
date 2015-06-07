using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;

using Microsoft.Win32;

namespace Y2L_IDE
{
	class ProjectManager
	{
		public static void FirstRun()
		{
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(StringTable.REGISTRY_PATH);
                if (key.GetValue("FirstRun") == null)
                {
                    AssociateFileTypes();
                    key.SetValue("FirstRun", 1);
                }
                key.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName);
            }
		}
		public static void AssociateFileTypes()
		{
			AssociateFileType("."+StringTable.FILE_PROJECT_EXTENSION,"Y2L Project",Application.ExecutablePath,"Y2L Express Project");
			AssociateFileType("."+StringTable.FILE_CODE_EXTENSION,"Y2L Code",Application.ExecutablePath,
			                  "Y2L Code File",Application.StartupPath+"\\y2l_code_file.ico");
			AssociateFileType("."+StringTable.FILE_EXE_EXTENSION,"Y2L Executable",Application.StartupPath+"\\Y2LInterpreter.exe",
			                  "Y2L Executable File",Application.StartupPath+"\\Console.ico");
		}
		[DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
		static void AssociateFileType(string extension, string keyName, string openWith, string fileDescription)
		{
			AssociateFileType(extension,keyName,openWith,fileDescription,openWith);
		}
		static void AssociateFileType(string extension, string keyName, string openWith, string fileDescription,string iconPath)
		{
			RegistryKey key = Registry.ClassesRoot.CreateSubKey(extension);
			
			key.SetValue("", keyName);

			key = Registry.ClassesRoot.CreateSubKey(keyName);
			key.SetValue("", fileDescription);
			key.CreateSubKey("DefaultIcon").SetValue("", "\"" + iconPath+ "\",0");
			key = key.CreateSubKey("Shell");
			key.CreateSubKey("edit").CreateSubKey("command").SetValue("", "\"" + openWith + "\"" + " \"%1\"");
			key.CreateSubKey("open").CreateSubKey("command").SetValue("", "\"" + openWith + "\"" + " \"%1\"");
			key.Close();

			key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.ucs", true);
			if(key!=null){
				key.DeleteSubKey("UserChoice", false);
				key.Close();
			}
			SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
		}
		
		public static void CreateProjectFile(string projectFile, string projectName, IEnumerable<string> includingFiles)
		{
			XNamespace ns = StringTable.URL; ;
			XElement itemGroup = new XElement(ns + "ItemGroup");
			foreach (string file in includingFiles)
			{
				itemGroup.Add(new XElement(ns + "Include", file));
			}

			XElement project = new XElement(ns + "Project",
			                                new XElement(ns + "ProjectName", projectName),
			                                new XElement(ns + "Version", Application.ProductVersion),
			                                itemGroup);

			project.Save(projectFile);
		}
		public static void CreateDefaultCodeFile(string projectFolder)
		{
            string text = "module Program{\r\nfunction void Main()\r\n{\r\nWrite(\"Hello World\");\r\nReadLine();\r\n}\r\n}";

			StreamWriter writer = new StreamWriter(
				projectFolder + "\\" + StringTable.DEFAULT_CODE_FILE, false, System.Text.Encoding.ASCII);
			writer.Write(text);
			writer.Close();
		}
		public static bool LoadProject(string projectFile)
		{
			if (!File.Exists(projectFile))
			{
				return false;
			}
			XElement x = XElement.Load(projectFile);
			XElement p = x.Element(XName.Get("ProjectName", StringTable.URL));
			Global.ProjectName = p.Value;
			p = x.Element(XName.Get("ItemGroup", StringTable.URL));
			Global.Files.Clear();
			Global.ProjectPath = Path.GetDirectoryName(projectFile);

			foreach (XElement xe in p.Elements())
				Global.Files.Add(xe.Value);
			return true;
		}

		public static void LoadRecentProjects()
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(StringTable.REGISTRY_PATH + "\\" + StringTable.RECENT);
			
			Global.RecentProjects = new Y2List<string>();

			if (key == null)
				return;

			foreach (string name in key.GetValueNames())
			{
				Global.RecentProjects.AddLast(key.GetValue(name).ToString());
			}
			
			key.Dispose();
		}
	}

}
