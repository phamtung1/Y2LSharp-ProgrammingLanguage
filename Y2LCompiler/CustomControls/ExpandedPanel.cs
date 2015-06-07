using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Y2L_IDE
{
	/// <summary>
	/// Description of ExPanel.
	/// </summary>
	public partial class ExpandedPanel : Control
	{
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Button btnHeader;
		
		private const int ITEM_HEIGHT=25;
		
		public int Index{get; set;}
		
		public event TextSelectedEventHandler TextSelected;
		public event EventHandler HeaderClick;
		
		public string HeaderText{
			get {return btnHeader.Text;}
			set {btnHeader.Text=value;}
		}
		public string SelectedItem
		{
			get {
				if(listView1.SelectedItems.Count==0)
				return String.Empty;
				return listView1.SelectedItems[0].Text;
			}
		}
		public bool IsExpanded{get;private set;}
		
		public ExpandedPanel()
		{			
			
			this.btnHeader = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// btnHeader
			// 
			this.btnHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnHeader.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnHeader.Location = new System.Drawing.Point(0, 0);			
			this.btnHeader.Size = new System.Drawing.Size(208, 25);			
			this.btnHeader.UseVisualStyleBackColor = true;
            this.btnHeader.BackColor = Color.Azure;
			this.btnHeader.Click += new System.EventHandler(this.BtnHeaderClick);
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			                                	this.columnHeader1});
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listView1.FullRowSelect = true;
			this.listView1.Location = new System.Drawing.Point(0, 0);			
			this.listView1.Size = new System.Drawing.Size(208, 496);			
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.DoubleClick+= listView1_DoubleClick;
			// 
			// ExpandedPanel
			// 			
			
			this.Controls.Add(this.btnHeader);
			this.Controls.Add(this.listView1);
			this.Name = "ExpandedPanel";
			this.Size = new System.Drawing.Size(208, 496);
			this.ResumeLayout(false);
		}
		
		
		protected override void OnResize(EventArgs e)
		{
			if(listView1!=null)			
			listView1.Columns[0].Width=this.Width-30;
			base.OnResize(e);
		}
        internal void AddItem(string text)
        {
            if (listView1.Items.ContainsKey(text))
                listView1.Items.RemoveByKey(text);

            listView1.Items.Insert(0, text).Name = text;
            if (listView1.Items.Count > 20)
                listView1.Items.RemoveAt(listView1.Items.Count - 1);
        }

		void listView1_DoubleClick(object sender, EventArgs e)
		{
			if(listView1.SelectedItems.Count==0)
			return;			 
			
			 if(TextSelected!=null)
			 	TextSelected(new TextSelectedEventArgs(listView1.SelectedItems[0].Text));
		}
		void ToggleExpand()
		{
			IsExpanded=!IsExpanded;
			if(IsExpanded)
				Expand();
			else
				Collapse();
		}
		internal void Collapse(){
			listView1.Hide();
			this.Height=btnHeader.Height;
		}
		internal void Expand()
		{			
			this.Height=this.Parent.Height-this.Parent.Controls.Count*ExpandedPanel.ITEM_HEIGHT;
			listView1.Show();
		}
		public void LoadItems(IEnumerable<string> items)
		{		
			listView1.Items.Clear();			
			foreach (var item in items) {                
				listView1.Items.Add(item).ToolTipText=item;
			}
		}				
		void BtnHeaderClick(object sender, EventArgs e)
		{
			if(HeaderClick!=null)
				HeaderClick(this,null);
		}
	}
}
