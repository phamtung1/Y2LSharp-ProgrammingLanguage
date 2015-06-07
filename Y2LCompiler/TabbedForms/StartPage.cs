using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Y2L_IDE
{
	public partial class StartPage : UserControl
	{

		public event EventHandler NewProject;
		public event EventHandler OpenProject;
        public event EventHandler<FileOpenedEventArgs> ProjectFileOpened;

		public StartPage()
		{
			InitializeComponent();
			
			lblProductName.Text=Application.ProductName;
		}

		private void StartPageForm_Load(object sender, EventArgs e)
		{
			recentListBox.ProjectFileOpened += OnProjectFileOpened;
			
			LoadRecentProjects();
		}
		protected override void OnResize(EventArgs e)
		{
			pictureBox1.Left=this.Width-pictureBox1.Width;
            if (pictureBox1.Left < lblProductName.Right)
            {
                pictureBox1.Left = lblProductName.Right + 20;
                this.BackgroundImageLayout = ImageLayout.None;
            }else
                this.BackgroundImageLayout = ImageLayout.Stretch;
			
			
			base.OnResize(e);
		}
        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);
        }
		void OnNewProject(object sender, EventArgs e)
		{
			if (NewProject != null)
				NewProject(sender, e);
		}
		void OnOpenProject(object sender, EventArgs e)
		{
			if (OpenProject != null)
				OpenProject(sender, e);
		}
		void OnProjectFileOpened(object sender, FileOpenedEventArgs e)
		{
			if (ProjectFileOpened != null)
			{
				ProjectFileOpened(sender,e);
			}
		}

		internal void LoadRecentProjects()
		{
			if (Global.RecentProjects == null)
				ProjectManager.LoadRecentProjects();
			int a=Global.RecentProjects.Count;
			recentListBox.Controls.Clear();
			foreach (string value in Global.RecentProjects)
			{
				DateTime date=File.GetLastWriteTime(value);
				ImageListItem item = new ImageListItem(Path.GetFileNameWithoutExtension(value),value,date);
				recentListBox.AddItem(item);
			}
		}

		internal void DeleteRecentProject(string projectPath)
		{
			for(int i=0;i<recentListBox.Controls.Count;i++)
			{
				ImageListItem item=recentListBox.Controls[i] as ImageListItem;
				if (item.FilePath.Equals(projectPath, StringComparison.OrdinalIgnoreCase))
				{
					recentListBox.RemoveItemAt(i);
					Global.RecentProjects.RemoveAt(i);
					break;
				}
			}
			
		}
		private void lnkNewProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OnNewProject(sender, e);
		}

		private void lnkOpenProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OnOpenProject(sender, e);
		}

	}
}
