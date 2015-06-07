
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Y2L_IDE
{
	/// <summary>
	/// Description of AboutBox.
	/// </summary>
	public partial class AboutBox : Form
	{
		public AboutBox()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
            this.Text = "About " + Application.ProductName;
            
            tabControl1.TabPages[0].Text = this.Text;

			lblVersion.Text="Version: "+Application.ProductVersion;
			
			StringBuilder s=new StringBuilder();
			
			s.Append(Application.ProductName).Append("\t\t:  ").AppendLine(Application.ProductVersion);
			s.Append(".Net Version\t\t:  ").AppendLine(Environment.Version.ToString());
			s.Append("OS Version\t\t:  ").AppendLine(Environment.OSVersion.ToString());
			s.Append("Current culture\t\t:  ").AppendLine(Application.CurrentCulture.DisplayName);
			s.Append("Working Set Memory\t:  ").AppendLine(Process.GetCurrentProcess().WorkingSet64+"kb");
			
			txtVersion.Text=s.ToString();
			
			ShowLoadedAssemblies();
			
			Refresh();
		}
		void ShowLoadedAssemblies()
		{
			listView1.Items.Clear();
			
			Assembly[] assemblies =  AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly asm in assemblies)
			{
				AssemblyName name=asm.GetName();
				ListViewItem item= listView1.Items.Add(name.Name);
				item.SubItems.Add(name.Version.ToString());
				item.SubItems.Add(asm.Location);
			}
		}
	}
}
