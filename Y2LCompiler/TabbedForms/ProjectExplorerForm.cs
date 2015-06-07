using System;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Y2L_IDE
{
	public partial class ProjectExplorerForm : Form
	{
		public event EventHandler<FileOpenedEventArgs> FileOpened;
		
		private const string PROJECT="Project";
		private const string FILE="File";
		private const string UNKNOWFILE="UnknownFile";
		private const string FOLDER="Folder";
		private const string UNKNOWFOLDER="unknownFolder";
		
		public ProjectExplorerForm()
		{
			InitializeComponent();
		}

		public void ExploreProject()
		{
			ExploreProject(tsbShowAll.Checked);

		}
		
		public void ExploreProject(bool showAll)
		{
			treeView1.Nodes.Clear();
			TreeNode root = treeView1.Nodes.Add(Global.ProjectName);
			root.ImageKey = PROJECT;
			root.SelectedImageKey = PROJECT;
			
			if(showAll)
				ExploreFolder(root,Global.ProjectPath);
			else
			{
				foreach (string file in Global.Files)
				{
					TreeNode fn = root.Nodes.Add(Path.GetFileName(file));
					fn.ImageKey = FILE;
					fn.SelectedImageKey = FILE;
				}
			}
			
			root.Expand();
		}
		void ExploreFolder(TreeNode node,string dirPath)
		{
			foreach(string dir in Directory.GetDirectories(dirPath))
			{
				string d=Path.GetFileName(dir);
				TreeNode dn=node.Nodes.Add(d,d,FOLDER,FOLDER);
				ExploreFolder(dn,dir);
			}
			foreach (string file in Directory.GetFiles(dirPath))
			{
				TreeNode fn = node.Nodes.Add(Path.GetFileName(file));
				string ex=file.Substring(file.LastIndexOf(".")+1).ToLower();
				
				string imageKey=UNKNOWFILE;
				if(ex==StringTable.FILE_PROJECT_EXTENSION)
					imageKey=PROJECT;
				else
				{
					foreach(string f in Global.Files)
					{
						if(file.EndsWith(f,StringComparison.OrdinalIgnoreCase))
						{
							imageKey=FILE;
							break;
						}
					}
				}
				fn.ImageKey = imageKey;
				fn.SelectedImageKey = imageKey;
			}
		}
        void OnFileOpened(object sender, FileOpenedEventArgs p)
		{
			if (FileOpened != null)
				FileOpened(sender,p);
		}
		string GetFullPath(TreeNode node)
		{
			return Path.Combine(Global.ProjectPath,node.FullPath.Remove(0,treeView1.Nodes[0].Text.Length+1));
		}

		private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if(e.Node.Level>0 && e.Node.ImageKey==FILE)
				OnFileOpened(this, new FileOpenedEventArgs(GetFullPath(e.Node)));
		}
		
		void TsbShowAllClick(object sender, EventArgs e)
		{
			ExploreProject();
		}
		
		void TsbRefreshClick(object sender, EventArgs e)
		{
			ExploreProject();
		}
		
		void TsbCollapseAllClick(object sender, EventArgs e)
		{
			if(treeView1.Nodes.Count>0)
			{
                treeView1.Nodes[0].Collapse();
			}
		}
		
		void TsbOpenClick(object sender, EventArgs e)
		{
			if(treeView1.SelectedNode!=null && treeView1.SelectedNode.ImageKey=="File")
				OnFileOpened(this,new FileOpenedEventArgs(GetFullPath(treeView1.SelectedNode)));
		}
		
		void TreeView1AfterSelect(object sender, TreeViewEventArgs e)
		{
			if(treeView1.SelectedNode!=null)
			{
				bool b=(treeView1.SelectedNode.ImageKey==FILE) || (treeView1.SelectedNode.ImageKey==UNKNOWFILE);
				tsbOpen.Visible=b;
				toolStripSeparator1.Visible=b;
			}
			
		}
	}
}
