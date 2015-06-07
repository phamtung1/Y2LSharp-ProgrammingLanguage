using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Y2LCore;

namespace Y2L_IDE
{
	public partial class ErrorListForm : Form
	{
		public event EventHandler<ErrorEventArgs> ErrorNavigated;
		
		string _filePath;
        public int ErrorsCount
        {
            get { return listView1.Items.Count; }
        }
		public ErrorListForm()
		{
			InitializeComponent();
		}
		internal void ClearError()
		{
			listView1.Items.Clear();
		}
		internal void ShowError(string filePath, List<SyntaxError> list)
		{
			_filePath=filePath;
			string fileName=Path.GetFileName(filePath);
			int count = 1;
			foreach (SyntaxError error in list)
			{
				ListViewItem item = listView1.Items.Add(count.ToString());				
				item.SubItems.Add(error.Message);
				item.SubItems.Add(fileName);
				item.SubItems.Add(error.Line.ToString());
				item.SubItems.Add(error.Col.ToString());
			}
		}
		void OnErrorNavigated(ErrorEventArgs e)
		{
			if(ErrorNavigated!=null)
				ErrorNavigated(this,e);
		}
        void ListView1DoubleClick(object sender, EventArgs e)
        {
        	if(listView1.SelectedItems.Count>0)
        	{
        		int line=int.Parse(listView1.SelectedItems[0].SubItems[3].Text);
        		int col=int.Parse(listView1.SelectedItems[0].SubItems[4].Text);
        		
        		OnErrorNavigated(new ErrorEventArgs(_filePath,line,col));
        	}
        	 
        }
	}
}
