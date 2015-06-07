using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Y2L_IDE
{
	public partial class ToolBox : Form
	{
		int _expanedIndex=0;
		ExpandedPanel[] _exPanels;
		public event TextSelectedEventHandler TextSelected;
        ClipboardMonitor _clipboard;
		public ToolBox()
		{
			InitializeComponent();

			LoadPanels();

            _clipboard = new ClipboardMonitor();
            _clipboard.ClipboardChanged += (sender, e) =>
                {
                  _exPanels[0].AddItem(e.Text);
                };
		}

  
		void LoadPanels(){            
			string dir=Path.Combine(Application.StartupPath,StringTable.CODESNIPPLET_DIR);
			string[] files=Directory.GetFiles(dir);
			if(files.Length==0)
				return;

            _exPanels = new ExpandedPanel[files.Length + 1];
            AddExPanel(0);
            _exPanels[0].HeaderText = "Clipboard Ring";

            for(int i=1;i<_exPanels.Length;i++)
			{
                AddExPanel(i);

                _exPanels[i].HeaderText = Path.GetFileNameWithoutExtension(files[i-1]);
				_exPanels[i].LoadItems(LoadFile(files[i-1]));
			}

			_exPanels[0].Expand();			
		}
        void AddExPanel(int index)
        {
            _exPanels[index] = new ExpandedPanel();
            _exPanels[index].Dock = DockStyle.Top;
            _exPanels[index].HeaderClick += ExPanels_HeaderClick;
            _exPanels[index].TextSelected += (e) =>
            {
                if (TextSelected != null)
                    TextSelected(e);
            };
            this.Controls.Add(_exPanels[index]);
            _exPanels[index].Collapse();
            _exPanels[index].BringToFront();
            _exPanels[index].Index = index;
            
        }
		protected override void OnResize(EventArgs e)
		{
			if(_exPanels!=null)
				_exPanels[_expanedIndex].Expand();
			base.OnResize(e);
		}
		IEnumerable<string> LoadFile(string filePath)
		{
			StreamReader reader=new StreamReader(filePath);
			string buffer=reader.ReadToEnd();
			reader.Dispose();
			return buffer.Split("\r\n".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
		}
		
		void ExPanels_HeaderClick(object sender, EventArgs e)
		{
			ExpandedPanel p=(ExpandedPanel)sender;
			if(p.IsExpanded)
				return;
			_exPanels[_expanedIndex].Collapse();
			p.Expand();
			_expanedIndex=p.Index;
		}
	}
}
