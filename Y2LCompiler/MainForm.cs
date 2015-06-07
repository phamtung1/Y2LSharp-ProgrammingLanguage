using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Crom.Controls.Docking;
using ICSharpCode.TextEditor.Document;
using Microsoft.Win32;
using Y2LCore;
using System.Drawing;
using Y2LCore.SyntaxTree;

namespace Y2L_IDE
{
    public partial class MainForm : Form
    {
        ProjectExplorerForm _projectForm;
        DocumentContainer _documentContainer;
        ErrorListForm _errorListForm;
        ToolBox _toolBox;
        SyntaxTreeForm _syntaxForm;
        SaveFileDialog _saveFileDialog;

        public MainForm(string[] args)
        {

            SplashScreen splash = new SplashScreen();
            splash.Show(this);
            Application.DoEvents();
            ProjectManager.FirstRun();
            InitializeComponent();

            this.Text = Application.ProductName;

            ProjectManager.AssociateFileTypes();
            LoadControls();
            if (args.Length > 0)
            {
                if (args[0].EndsWith("." + StringTable.FILE_PROJECT_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    LoadProject(args[0]);
                }
                else
                {
                    foreach (string file in args)
                        _documentContainer.OpenDocument(file);
                }
            }

            splash.Close();
            splash.Dispose();
        }


        void LoadControls()
        {

            dockContainer1.ShowContextMenu += (sender, e) =>
            {
                DockableFormInfo fInfo = dockContainer1.GetFormInfo(e.Form);
                ctxItemAutoHide.Enabled = !fInfo.IsAutoHideMode;
                ctxItemDock.Enabled = fInfo.HostContainerDock == DockStyle.None;

                contextMenuStrip1.Show(e.Form, e.MenuLocation);

            };
            dockContainer1.FormClosing += (sender, e) =>
                {
                    e.Cancel = true;
                    dockContainer1.Remove(dockContainer1.GetFormInfo(e.Form));
                };
            AddProjectForm();
            AddSyntaxForm();
            AddDocumentContainer();
            AddToolBox();
            AddErrorForm();

            AddEvents();

            LoadRecentProjects();
            _documentContainer_DocumentCountChanged(null, null);
            _documentContainer_SelectedDocumentChanged(null, null);

        }
        void AddProjectForm()
        {
            if (_projectForm == null || _projectForm.IsDisposed)
                _projectForm = new ProjectExplorerForm();
            AddForm(_projectForm, DockStyle.Right);
        }
        void AddSyntaxForm()
        {
            if (_syntaxForm == null || _syntaxForm.IsDisposed)
                _syntaxForm = new SyntaxTreeForm();
            AddForm(_syntaxForm, DockStyle.Right, zDockMode.None, true);
        }
        void AddDocumentContainer()
        {
            if (_documentContainer == null || _documentContainer.IsDisposed)
                _documentContainer = new DocumentContainer();
            AddForm(_documentContainer, DockStyle.Fill);
        }
        void AddToolBox()
        {
            if (_toolBox == null || _toolBox.IsDisposed)
                _toolBox = new ToolBox();
            AddForm(_toolBox, DockStyle.Left);
        }
        void AddErrorForm()
        {
            if (_errorListForm == null || _errorListForm.IsDisposed)
                _errorListForm = new ErrorListForm();
            AddForm(_errorListForm, DockStyle.Bottom, true);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveRecentProjects();
            if (!_documentContainer.CloseAllDocument())
                e.Cancel = true;
            else
            {
                dockContainer1.Dispose();
                Application.Exit();
            }
            base.OnClosing(e);
        }
        void AddForm(Form form, DockStyle dockStyle)
        {
            AddForm(form, dockStyle, false);
        }
        void AddForm(Form form, DockStyle dockStyle, bool autoHide)
        {
            AddForm(form, dockStyle, zDockMode.None, autoHide);
        }
        void AddForm(Form form, DockStyle dockStyle, zDockMode dockMode, bool autoHide)
        {
            if (dockContainer1.Contains(form))
                return;
            DockableFormInfo formInfo = dockContainer1.Add(form, zAllowedDock.All, Guid.NewGuid());

            dockContainer1.DockForm(formInfo, dockStyle, zDockMode.None);
            if (autoHide)
                dockContainer1.SetAutoHide(formInfo, true);


            if (form == _documentContainer)
            {
                formInfo.ShowContextMenuButton = false;
                formInfo.ShowCloseButton = false;
            }
        }
        void AddEvents()
        {
            _documentContainer.NewProject += OnNewProject;
            _documentContainer.OpenProject += OnOpenProject;
            _documentContainer.ProjectFileOpened += OnLoadProject;
            _documentContainer.DocumentCountChanged += _documentContainer_DocumentCountChanged;
            _documentContainer.SelectedDocumentChanged += _documentContainer_SelectedDocumentChanged;

            _projectForm.FileOpened += OnCodeFileOpened;

            _errorListForm.ErrorNavigated += (sender, e) =>
            {
                _documentContainer.GotoPosition(e.FilePath, e.Line - 1, e.Col);
            };
            _toolBox.TextSelected += (e) =>
            {
                _documentContainer.InsertText(e.Text);
            };
        }

        void _documentContainer_SelectedDocumentChanged(object sender, EventArgs e)
        {
            if (_documentContainer.SelectedIndex == -1)
                return;
            for (int i = 3; i < windowToolStripMenuItem.DropDownItems.Count; i++)
            {
                ((ToolStripMenuItem)windowToolStripMenuItem.DropDownItems[i]).Checked = false;
            }
            ((ToolStripMenuItem)windowToolStripMenuItem.DropDownItems[3 + _documentContainer.SelectedIndex]).Checked = true;
            saveToolStripMenuItem.Text = "Save " + Path.GetFileName(_documentContainer.SelectedFilePath);
            saveAsToolStripMenuItem.Text = "Save " + Path.GetFileName(_documentContainer.SelectedFilePath) + " As";

            bool isDocument = _documentContainer.SelectedEditor != null;
            buildToolStripMenuItem.Enabled = isDocument;
            runToolStripMenuItem.Enabled = isDocument;
            tsbRun.Enabled = isDocument;
        }
        void _documentContainer_DocumentCountChanged(object sender, EventArgs e)
        {
            int count = windowToolStripMenuItem.DropDownItems.Count;
            for (int i = 3; i < count; i++)
            {
                windowToolStripMenuItem.DropDownItems.RemoveAt(3);
            }
            List<string> list = _documentContainer.GetOpenedDocuments();
            foreach (string text in list)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(Path.GetFileName(text));
                item.Tag = text;
                item.Click += (sd, ev) =>
                {
                    _documentContainer.OpenDocument(((ToolStripItem)sd).Tag.ToString());
                };
                windowToolStripMenuItem.DropDownItems.Add(item);
            }
        }
        void OnLoadProject(object sender, FileOpenedEventArgs e)
        {
            _documentContainer.CloseAllDocument(false);
            LoadProject(e.FilePath);
        }
        void OnCodeFileOpened(object sender, FileOpenedEventArgs e)
        {
            if (_documentContainer == null)
            {
                AddForm(_documentContainer = new DocumentContainer(), DockStyle.Fill);

            }
            _documentContainer.OpenDocument(e.FilePath);

        }
        void LoadRecentProjects()
        {
            if (Global.RecentProjects == null)
                ProjectManager.LoadRecentProjects();

            recentProjectsMenuItem.DropDownItems.Clear();
            int count = 1;
            foreach (string value in Global.RecentProjects)
            {
                ToolStripItem item = recentProjectsMenuItem.DropDownItems.Add((count++) + " " + value);
                item.Tag = value;
                item.Click += (sender, e) =>
                {
                    string projectFile = item.Tag.ToString();
                    LoadProject(projectFile);

                    LoadRecentProjects();
                };
            }
            if (Global.RecentProjects.Count == 0)
                recentProjectsMenuItem.Enabled = false;
            _documentContainer.LoadRecentProjects();
        }

        void SaveRecentProjects()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(StringTable.REGISTRY_PATH);
            key.DeleteSubKey(StringTable.RECENT, false);
            key = key.CreateSubKey(StringTable.RECENT);
            for (int i = 0; i < Global.RecentProjects.Count; i++)
            {
                key.SetValue((i + 1).ToString(), Global.RecentProjects[i]);
            }
            key.Dispose();

        }
        bool LoadProject(string projectFile)
        {
            string currentProject = Global.GetFullProjectFilePath();
            if (!String.IsNullOrEmpty(currentProject) && currentProject.Equals(projectFile, StringComparison.OrdinalIgnoreCase))
                return false;
            if (!ProjectManager.LoadProject(projectFile))
            {
                string fileName = Path.GetFileNameWithoutExtension(projectFile);
                if (DialogResult.Yes == MessageBox.Show("\"" + fileName + "\" could not be opened.  Do you want to remove the reference(s) to it from the Recent list(s)?",
                                                        Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                {
                    Global.RecentProjects.Remove(projectFile);
                    LoadRecentProjects();
                }
                return false;
            }
            Global.RecentProjects.AddFirst(projectFile);
            _projectForm.ExploreProject();
            _documentContainer.CloseAllDocument(false);

            LoadRecentProjects();

            tsbAddItem.Enabled = true;
            return true;
        }

        internal void SaveAs(Editor editor)
        {
            if (editor == null)
                return;
            if (_saveFileDialog == null)
            {
                _saveFileDialog = new SaveFileDialog();
                _saveFileDialog.Filter = String.Format("{0} files {1}|*.{1}|All files|*.*",
                                                      StringTable.FILE_CODE_EXTENSION.ToUpper(), StringTable.FILE_CODE_EXTENSION);
                _saveFileDialog.InitialDirectory = Global.ProjectPath;
            }

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                editor.SaveAs(_saveFileDialog.FileName);
            }
            statusLabel1.Text = "Item Saved";
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = _documentContainer.SelectedFilePath;
            _documentContainer.SaveActiveDocument();
            if (!String.IsNullOrEmpty(filePath))
            {
                string path = BuildFile();
                statusLabel1.Text = "Ready";
                if (_errorListForm.ErrorsCount > 0)
                {
                    if (File.Exists(path))
                    {
                        if (DialogResult.No == MessageBox.Show("There were build errors. Would you like to continue and run the last success build?",
                            Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2))
                            return;

                        goto Run;
                    }
                    else
                    {
                        MessageBox.Show("There were build errors. Please fix all errors and retry.",
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
                else
                    goto Run;

                return;
            Run:
                ProcessStartInfo startInfo = new ProcessStartInfo(Application.StartupPath + "\\Y2LInterpreter.exe", "\"" + path + "\"");
                Process.Start(startInfo);

            }



        }
        protected virtual void OnNewProject(object sender, EventArgs e)
        {
            NewProjectForm form = new NewProjectForm();
            while (form.ShowNewProject() == DialogResult.OK)
            {


                string projectFile = Path.Combine(form.ProjectPath, form.ProjectName);
                projectFile += "." + StringTable.FILE_PROJECT_EXTENSION;

                string extension = "." + StringTable.FILE_PROJECT_EXTENSION;
                bool existed = false;
                foreach (string file in Directory.GetFiles(form.ProjectPath))
                {
                    if (file.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("The project cannot be saved because another project already exists in the folder '" + Global.ProjectPath +
                            "'. Please choose another name or another location.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        existed = true;
                        break;

                    }
                }
                Global.ProjectsCreatedCount++;
                if (existed)
                {
                    // LoadRecentProjects();
                    continue;
                }
                Global.ProjectName = form.ProjectName;
                Global.ProjectPath = form.ProjectPath;
                Global.Files.Clear();
                Global.Files.Add(StringTable.DEFAULT_CODE_FILE);

                Global.RecentProjects.AddFirst(projectFile);


                ProjectManager.CreateProjectFile(projectFile, form.ProjectName, Global.Files);
                ProjectManager.CreateDefaultCodeFile(form.ProjectPath);
                LoadRecentProjects();
                _projectForm.ExploreProject();

                recentProjectsMenuItem.Enabled = true;

                break;
            }

            form.Dispose();
        }
        protected virtual void OnOpenProject(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = String.Format("Y2L Project (*.{0})|*.{0}", StringTable.FILE_PROJECT_EXTENSION);
            dlg.InitialDirectory = Application.StartupPath;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                LoadProject(dlg.FileName);
            }
        }
        internal void SetStatus(int line, int col)
        {
            statusLine.Text = ("Line " + line).PadRight(10);
            statusCol.Text = ("Col " + col).PadRight(10);
        }
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewProject(sender, e);
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnOpenProject(sender, e);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _documentContainer.SaveActiveDocument();
            statusLabel1.Text = "Item Saved";
        }

        private void closeAllDocumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _documentContainer.CloseAllDocument();
        }


        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuildFile();
        }

        private string BuildFile()
        {
            statusLabel1.Text = "Build Progress";
            if (_documentContainer.SelectedEditor == null)
                return String.Empty;
            _documentContainer.SaveActiveDocument();
            _errorListForm.ClearError();

            Scanner scanner = null;
            string filePath = _documentContainer.SelectedFilePath;
            using (TextReader input = new StreamReader(filePath))
            {
                scanner = new Scanner(input);
            }
            Parser parser = new Parser(scanner);

            //try
            //{
            Module mod = parser.Parse();
            string newPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".yexe");
            if (parser.Errors.Count == 0)
            {
                _syntaxForm.LoadTree(mod);
                CodeIO.Save(mod, newPath);
                statusLabel1.Text = "Build finished successfully.";
            }
            else
            {
                DockableFormInfo forminfo = dockContainer1.GetFormInfo(_errorListForm);
                dockContainer1.SetAutoHide(forminfo, false);

                _errorListForm.ShowError(_documentContainer.SelectedFilePath, parser.Errors);
                statusLabel1.Text = "Build failed. " + parser.Errors.Count + " error(s)";
            }

            return newPath;
        }

        private void newItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProjectForm form = new NewProjectForm();

            while (form.ShowAddItem() == DialogResult.OK)
            {

               
                string sourcePath = form.SelectedFile;
                string fileName = form.ProjectName;
                if (!fileName.Contains("."))
                    fileName += "." + StringTable.FILE_CODE_EXTENSION;

                string destPath = Path.Combine(Global.ProjectPath, fileName);
                File.Copy(sourcePath, destPath);

                Global.Files.Add(fileName);
                ProjectManager.CreateProjectFile(Global.GetFullProjectFilePath(), Global.ProjectName, Global.Files);
                _projectForm.ExploreProject();

                tsbAddItem.Enabled = true;
                _documentContainer.OpenDocument(destPath);
                break;
            }
        }



        void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }


        void StartPageToolStripMenuItemClick(object sender, EventArgs e)
        {
            _documentContainer.AddStartPage();
        }

        void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            SaveAs(_documentContainer.SelectedEditor);
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _documentContainer.SaveAll();
            statusLabel1.Text = "Item(s) Saved";
        }
        void SplitDocumentsToolStripMenuItemClick(object sender, EventArgs e)
        {
            Editor editor = _documentContainer.SelectedEditor;
            if (editor != null)
                editor.Split();
        }

        private void ctxItemDock_Click(object sender, EventArgs e)
        {
            DockableFormInfo fInfo = dockContainer1.GetFormInfo((Form)contextMenuStrip1.SourceControl);
            dockContainer1.DockForm(fInfo, fInfo.Dock, fInfo.DockMode);
            dockContainer1.SetAutoHide(fInfo, false);
        }

        private void ctxItemAutoHide_Click(object sender, EventArgs e)
        {
            DockableFormInfo fInfo = dockContainer1.GetFormInfo((Form)contextMenuStrip1.SourceControl);
            dockContainer1.SetAutoHide(fInfo, true);
        }

        private void ctxItemHide_Click(object sender, EventArgs e)
        {

            //((Form)contextMenuStrip1.SourceControl).Hide();            
            dockContainer1.Remove(dockContainer1.GetFormInfo((Form)contextMenuStrip1.SourceControl));
        }

        private void viewProjectExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddProjectForm();
        }

        private void viewSyntaxTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSyntaxForm();
        }

        private void viewErrorListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddErrorForm();
        }

        private void viewToolBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToolBox();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor editor = _documentContainer.SelectedEditor;
            if (editor != null)
                editor.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor editor = _documentContainer.SelectedEditor;
            if (editor != null)
                editor.Redo();
        }

        private void restoreFileAssociatationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectManager.AssociateFileTypes();
            MessageBox.Show("All default file associations have been restored.",
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void tsbNewProject_Click(object sender, EventArgs e)
        {
            newProjectToolStripMenuItem_Click(null, null);
        }

        private void tsbOpenProject_Click(object sender, EventArgs e)
        {
            openProjectToolStripMenuItem_Click(null, null);
        }

        private void tsbAddItem_Click(object sender, EventArgs e)
        {
            newItemToolStripMenuItem_Click(null, null);
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(null, null);
        }

        private void tsbSaveAll_Click(object sender, EventArgs e)
        {
            saveAllToolStripMenuItem_Click(null, null);
        }

        private void tsbUndo_Click(object sender, EventArgs e)
        {
            undoToolStripMenuItem_Click(null, null);
        }

        private void tsbRedo_Click(object sender, EventArgs e)
        {
            redoToolStripMenuItem_Click(null, null);
        }

        private void tsbRun_Click(object sender, EventArgs e)
        {
            runToolStripMenuItem_Click(null, null);
        }



    }
}

