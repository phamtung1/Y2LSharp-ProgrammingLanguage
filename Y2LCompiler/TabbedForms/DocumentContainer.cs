using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Crom.Controls.Docking;


namespace Y2L_IDE
{
    public partial class DocumentContainer : Form
    {

        private const string START_PAGE = "Start Page";

        public event EventHandler SelectedDocumentChanged;
        public event EventHandler DocumentCountChanged;
        public event EventHandler NewProject;
        public event EventHandler OpenProject;

        public event EventHandler<FileOpenedEventArgs> ProjectFileOpened;

        Y2TabControl _tabControl;
        StartPage _startPage;

        #region Properties

        public string SelectedFilePath
        {
            get
            {
                if (_tabControl.SelectedIndex == -1)
                    return null;
                return _tabControl.SelectedTab.Name;
            }
        }
        public int SelectedIndex
        {
            get
            {
                return _tabControl.SelectedIndex;
            }
        }
        public Editor SelectedEditor
        {
            get
            {
                if (_tabControl.SelectedTab == null || _tabControl.SelectedTab.Name == START_PAGE)
                    return null;
                return _tabControl.SelectedTab.Controls[0] as Editor;
            }
        }

        #endregion

        public DocumentContainer()
        {
            InitializeComponent();
            LoadControls();
        }

        void LoadControls()
        {
            _tabControl = new Y2TabControl();
            _tabControl.Dock = DockStyle.Fill;
            _tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;            

            _tabControl.ControlAdded += new ControlEventHandler(_tabControl_ControlAdded);
            _tabControl.ControlRemoved += new ControlEventHandler(_tabControl_ControlRemoved);
            _tabControl.SelectedIndexChanged += new EventHandler(_tabControl_SelectedIndexChanged);
            _tabControl.MouseClick += (sender, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show((Control)sender, e.Location);
                }
                bool isDocument = SelectedEditor != null;
                saveToolStripMenuItem.Enabled = isDocument;
                saveAsToolStripMenuItem.Enabled = isDocument;
                openToolStripMenuItem.Enabled = isDocument;
            };

            this.Controls.Add(_tabControl);

            AddStartPage();
        }
        /// <summary>
        /// Insert text in the active document at cursor position
        /// </summary>
        internal void InsertText(string text)
        {
            Editor editor = SelectedEditor;
            if (editor != null)
            {
                editor.InsertText(text);
            }
        }
        internal void LoadRecentProjects()
        {
            if (_startPage != null && !_startPage.IsDisposed)
                _startPage.LoadRecentProjects();
        }
        internal void AddStartPage()
        {
            if (_startPage == null || _startPage.IsDisposed)
            {
                _tabControl.TabPages.Add(START_PAGE, START_PAGE);

                _startPage = new StartPage();

                _startPage.Dock = DockStyle.Fill;
                _startPage.NewProject += OnNewProject;
                _startPage.OpenProject += OnOpenProject;
                _startPage.ProjectFileOpened += OnProjectFileOpened;

                _tabControl.TabPages[_tabControl.TabCount - 1].Controls.Add(_startPage);
            }
            else
            {
                _tabControl.SelectTab(START_PAGE);
            }
        }
        void RefreshContextMenuDocuments()
        {
            ctxMenuDocuments.Items.Clear();
            foreach (TabPage tab in _tabControl.TabPages)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(tab.Text);
                item.Tag = tab.Name;

                ctxMenuDocuments.Items.Add(item);

                item.Click += (sender, e) =>
                    {
                        string tabName = ((ToolStripMenuItem)sender).Tag.ToString();
                        if (_tabControl.TabPages.ContainsKey(tabName))
                            _tabControl.SelectedTab = _tabControl.TabPages[tabName];
                    };
            }
        }
        internal void GotoPosition(string filePath, int line, int col)
        {
            OpenDocument(filePath);
            Editor editor = SelectedEditor;
            if (editor != null)
                editor.GotoPosition(line, col);
        }
        void _tabControl_ControlAdded(object sender, ControlEventArgs e)
        {
            RefreshContextMenuDocuments();
            btnClose.Visible = true;
            btnShowMenuDocuments.Visible = true;
            OnDocumentCountChanged(this, null);
        }
        void _tabControl_ControlRemoved(object sender, ControlEventArgs e)
        {
            RefreshContextMenuDocuments();
            if (_tabControl.TabCount == 0)
            {
                btnClose.Visible = false;
                btnShowMenuDocuments.Visible = false;
            }
            e.Control.Dispose();
            OnDocumentCountChanged(this, null);
        }
        void _tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnClose.Visible = _tabControl.SelectedIndex > -1;
            btnShowMenuDocuments.Visible = _tabControl.SelectedIndex > -1;
            OnSelectedDocumentChanged(sender, e);
        }

        void OnSelectedDocumentChanged(object sender, EventArgs e)
        {
            if (SelectedDocumentChanged != null)
                SelectedDocumentChanged(sender, e);
        }
        void OnDocumentCountChanged(object sender, EventArgs e)
        {
            if (DocumentCountChanged != null)
                DocumentCountChanged(sender, e);
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
        void OnProjectFileOpened(object sender,FileOpenedEventArgs e)
        {
            if (ProjectFileOpened != null)
                ProjectFileOpened(sender,e);
        }
        
        public void OpenDocument(string filePath)
        {
            int index = _tabControl.TabPages.IndexOfKey(filePath);
            if (index != -1)
            {
                _tabControl.SelectedIndex = index;
                return;
            }

            TabPage page = new TabPage(Path.GetFileName(filePath));
            page.Name = filePath;

            Editor editor = new Editor(filePath);
            editor.Dock = DockStyle.Fill;
            editor.ContentChanged += new EventHandler(editor_ContentChanged);
            page.Controls.Add(editor);

            _tabControl.TabPages.Add(page);
            _tabControl.SelectedIndex = _tabControl.TabCount - 1;
            OnSelectedDocumentChanged(null, null);
        }
        internal void SaveActiveDocument()
        {
            Editor editor = SelectedEditor;
            if (editor != null)
            {
                editor.Save();
                _tabControl.SelectedTab.Text = _tabControl.SelectedTab.Text.Trim('*');
            }
        }


        internal void SaveAll()
        {
            foreach (TabPage page in _tabControl.TabPages)
            {
                Editor editor = page.Controls[0] as Editor;
                editor.Save();
            }
        }

        public List<string> GetOpenedDocuments()
        {
            if (_tabControl == null)
                return null;
            List<string> list = new List<string>();
            foreach (TabPage page in _tabControl.TabPages)
            {
                if (!page.IsDisposed)
                    list.Add(page.Name);
            }
            return list;
        }

        void editor_ContentChanged(object sender, EventArgs e)
        {
            if (!_tabControl.SelectedTab.Text.EndsWith("*"))
                _tabControl.SelectedTab.Text = _tabControl.SelectedTab.Text + "*";

        }

        internal void DeleteRecentProject(string projectPath)
        {
            if (_startPage == null || _startPage.IsDisposed)
                return;

            _startPage.DeleteRecentProject(projectPath);
        }
        internal void CloseActiveDocument()
        {
            CloseDocumentAt(_tabControl.SelectedIndex);
        }
        internal void CloseDocumentAt(int index)
        {
            if (_tabControl.SelectedIndex != -1)
            {
                TabPage page = _tabControl.SelectedTab;
                if (page.Name != START_PAGE)
                {
                    Editor editor = (Editor)page.Controls[0];
                    if (editor.IsChanged)
                    {
                        ConfirmSaveDialog dlg = new ConfirmSaveDialog();
                        DialogResult ret = dlg.ShowConfirm(this.FindForm(), page.Text);
                        if (ret == DialogResult.Yes)
                            editor.Save();
                        else if (ret == DialogResult.Cancel)
                            return;
                    }
                }
                _tabControl.TabPages.RemoveAt(_tabControl.SelectedIndex);
                OnSelectedDocumentChanged(null, null);
            }
        }
        internal bool CloseAllDocument()
        {
            return CloseAllDocument(null);
        }
        internal void CloseAllDocument(bool closeStartPage)
        {
            if (closeStartPage)
                CloseAllDocument(null);
            else
                CloseAllDocument(START_PAGE);
        }
        internal bool CloseAllDocument(string excludingName)
        {
            List<string> list = new List<string>();
            foreach (TabPage page in _tabControl.TabPages)
            {
                if (page.Name != excludingName && page.Name != START_PAGE)
                {
                    Editor editor = (Editor)page.Controls[0];
                    if (editor.IsChanged)
                    {
                        list.Add(page.Text);
                    }
                }
            }

            if (list.Count > 0)
            {
                ConfirmSaveDialog dlg = new ConfirmSaveDialog();
                DialogResult ret = dlg.ShowConfirm(this.FindForm(), list.ToArray());
                if (ret == DialogResult.Yes)
                {
                    foreach (TabPage page in _tabControl.TabPages)
                    {
                        if (page.Name != excludingName && page.Name != START_PAGE)
                        {
                            Editor editor = (Editor)page.Controls[0];
                            if (editor.IsChanged)
                            {
                                editor.Save();
                            }
                            page.Dispose();
                        }
                    }
                }
                else if (ret == DialogResult.Cancel)
                    return false;

                OnSelectedDocumentChanged(null, null);

                
            }

            int count = _tabControl.TabCount - 1;
            while (count >= 0)
            {
                if (_tabControl.TabPages[count].Name != excludingName)
                    _tabControl.TabPages.RemoveAt(count);
                count--;
            }
            //_tabControl.TabPages.Clear();
            //			if (!close)
            //			{
            //				AddStartPage();
            //			}

            return true;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseDocumentAt(_tabControl.SelectedIndex);
        }


        void CloseToolStripMenuItemClick(object sender, EventArgs e)
        {
            CloseActiveDocument();
        }

        void CloseAllDocumentToolStripMenuItemClick(object sender, EventArgs e)
        {
            CloseAllDocument();
        }

        void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            SaveActiveDocument();
        }

        void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            Editor editor = SelectedEditor;
            if (editor != null)
            {
                ((MainForm)MainForm.ActiveForm).SaveAs(editor);

            }
        }

        void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            TabPage page = _tabControl.SelectedTab;
            if (page != null && page.Name != START_PAGE)
                Process.Start("explorer", "/select, \"" + page.Name + "\"");
        }

        void CloseOthersDocumentToolStripMenuItemClick(object sender, EventArgs e)
        {
            TabPage page = _tabControl.SelectedTab;
            if (page != null)
                CloseAllDocument(page.Name);
        }

        private void btnShowMenuDocuments_Click(object sender, EventArgs e)
        {
            Point p = new Point(btnShowMenuDocuments.Left, btnShowMenuDocuments.Bottom);
            p = this.PointToScreen(p);
            ctxMenuDocuments.Show(p);
        }

    }
}
