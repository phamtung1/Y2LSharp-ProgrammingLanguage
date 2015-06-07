namespace Y2L_IDE
{
    partial class ProjectExplorerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        	this.components = new System.ComponentModel.Container();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectExplorerForm));
        	this.treeView1 = new System.Windows.Forms.TreeView();
        	this.imageList1 = new System.Windows.Forms.ImageList(this.components);
        	this.toolStrip1 = new System.Windows.Forms.ToolStrip();
        	this.tsbShowAll = new System.Windows.Forms.ToolStripButton();
        	this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
        	this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
        	this.tsbCollapseAll = new System.Windows.Forms.ToolStripButton();
        	this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        	this.tsbOpen = new System.Windows.Forms.ToolStripButton();
        	this.toolStrip1.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// treeView1
        	// 
        	this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.treeView1.ImageIndex = 0;
        	this.treeView1.ImageList = this.imageList1;
        	this.treeView1.Location = new System.Drawing.Point(0, 25);
        	this.treeView1.Name = "treeView1";
        	this.treeView1.SelectedImageIndex = 0;
        	this.treeView1.Size = new System.Drawing.Size(208, 406);
        	this.treeView1.TabIndex = 0;
        	this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1AfterSelect);
        	this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
        	// 
        	// imageList1
        	// 
        	this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
        	this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
        	this.imageList1.Images.SetKeyName(0, "project");
        	this.imageList1.Images.SetKeyName(1, "file");
        	this.imageList1.Images.SetKeyName(2, "folder");
        	this.imageList1.Images.SetKeyName(3, "unknownfile");
        	this.imageList1.Images.SetKeyName(4, "unknownfolder");
        	// 
        	// toolStrip1
        	// 
        	this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
        	this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.tsbShowAll,
        	        	        	this.tsbRefresh,
        	        	        	this.toolStripSeparator2,
        	        	        	this.tsbCollapseAll,
        	        	        	this.toolStripSeparator1,
        	        	        	this.tsbOpen});
        	this.toolStrip1.Location = new System.Drawing.Point(0, 0);
        	this.toolStrip1.Name = "toolStrip1";
        	this.toolStrip1.Size = new System.Drawing.Size(208, 25);
        	this.toolStrip1.TabIndex = 2;
        	this.toolStrip1.Text = "toolStrip1";
        	// 
        	// tsbShowAll
        	// 
        	this.tsbShowAll.CheckOnClick = true;
        	this.tsbShowAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.tsbShowAll.Image = ((System.Drawing.Image)(resources.GetObject("tsbShowAll.Image")));
        	this.tsbShowAll.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.tsbShowAll.Name = "tsbShowAll";
        	this.tsbShowAll.Size = new System.Drawing.Size(23, 22);
        	this.tsbShowAll.Text = "toolStripButton1";
        	this.tsbShowAll.ToolTipText = "Show all files";
        	this.tsbShowAll.Click += new System.EventHandler(this.TsbShowAllClick);
        	// 
        	// tsbRefresh
        	// 
        	this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.tsbRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsbRefresh.Image")));
        	this.tsbRefresh.ImageTransparentColor = System.Drawing.Color.White;
        	this.tsbRefresh.Name = "tsbRefresh";
        	this.tsbRefresh.Size = new System.Drawing.Size(23, 22);
        	this.tsbRefresh.Text = "toolStripButton2";
        	this.tsbRefresh.ToolTipText = "Refresh";
        	this.tsbRefresh.Click += new System.EventHandler(this.TsbRefreshClick);
        	// 
        	// toolStripSeparator2
        	// 
        	this.toolStripSeparator2.Name = "toolStripSeparator2";
        	this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
        	// 
        	// tsbCollapseAll
        	// 
        	this.tsbCollapseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.tsbCollapseAll.Image = ((System.Drawing.Image)(resources.GetObject("tsbCollapseAll.Image")));
        	this.tsbCollapseAll.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.tsbCollapseAll.Name = "tsbCollapseAll";
        	this.tsbCollapseAll.Size = new System.Drawing.Size(23, 22);
        	this.tsbCollapseAll.Text = "toolStripButton3";
        	this.tsbCollapseAll.ToolTipText = "Collapse all";
        	this.tsbCollapseAll.Click += new System.EventHandler(this.TsbCollapseAllClick);
        	// 
        	// toolStripSeparator1
        	// 
        	this.toolStripSeparator1.Name = "toolStripSeparator1";
        	this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
        	// 
        	// tsbOpen
        	// 
        	this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.tsbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpen.Image")));
        	this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.tsbOpen.Name = "tsbOpen";
        	this.tsbOpen.Size = new System.Drawing.Size(23, 22);
        	this.tsbOpen.Text = "toolStripButton4";
        	this.tsbOpen.ToolTipText = "Open";
        	this.tsbOpen.Click += new System.EventHandler(this.TsbOpenClick);
        	// 
        	// ProjectExplorerForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(208, 431);
        	this.Controls.Add(this.treeView1);
        	this.Controls.Add(this.toolStrip1);
        	this.Name = "ProjectExplorerForm";
        	this.Text = "Project Explorer";
        	this.toolStrip1.ResumeLayout(false);
        	this.toolStrip1.PerformLayout();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbCollapseAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbRefresh;
        private System.Windows.Forms.ToolStripButton tsbShowAll;
        private System.Windows.Forms.ToolStrip toolStrip1;

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
    }
}