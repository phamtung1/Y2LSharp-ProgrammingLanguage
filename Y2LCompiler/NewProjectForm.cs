using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;

namespace Y2L_IDE
{
    public partial class NewProjectForm : Form
    {

        public string ProjectPath { get; private set; }
        public string ProjectName { get; private set; }
        // full path
        public string SelectedFile { get; private set; }

        public NewProjectForm()
        {
            InitializeComponent();

            txtLocation.Text = Application.StartupPath + "\\Projects";

            string[] names = System.Enum.GetNames(typeof(View));
            cboViewMode.Items.AddRange(names);
            cboViewMode.SelectedIndex = (int)View.Tile;
        }


        public DialogResult ShowNewProject()
        {
            lblContent.Enabled = false;
            listView1.Items.Clear();
            listView1.Items.Add("Hello World Application", 0);
            listView1.Items[0].Selected = true;


            string projectName = "Y2LProject" + Global.ProjectsCreatedCount;

            create:

            foreach (string project in Global.RecentProjects)
            {
                if (Path.GetFileNameWithoutExtension(project).Equals(projectName,StringComparison.OrdinalIgnoreCase))
                {
                    Global.ProjectsCreatedCount++;
                    projectName = "Y2LProject" + Global.ProjectsCreatedCount;
                    goto create;
                }
            }

            txtProjectName.Text = projectName;
            txtProjectName.Focus();
            return this.ShowDialog();
        }
        public DialogResult ShowAddItem()
        {
            lblContent.Enabled = true;
            this.Text = "Add New Item";
            txtLocation.Enabled = false;
            btnBrowse.Enabled = false;
            checkBox1.Enabled = false;

            listView1.Items.Clear();
            foreach (string file in Directory.GetFiles(Path.Combine(Application.StartupPath, StringTable.TEMPLATES_DIR), "*." + StringTable.FILE_CODE_EXTENSION))
            {
                ListViewItem item = listView1.Items.Add(Path.GetFileName(file),1);
                item.Tag = file;
            }
            if (listView1.Items.Count > 0)
            {
                listView1.Items[0].Selected = true;
            }
            listView1.Sorting = SortOrder.Ascending;

            listView1.Sort();

            if(String.IsNullOrEmpty(txtProjectName.Text))
            txtProjectName.Text = "Program" + Global.Files.Count + "." + StringTable.FILE_CODE_EXTENSION;
            txtProjectName.Focus();
            return this.ShowDialog();
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtLocation.Text = dlg.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string path = txtLocation.Text;

            if (checkBox1.Enabled && checkBox1.Checked)
            {
                path = Path.Combine(path, txtProjectName.Text);
            }
            try
            {
                Directory.CreateDirectory(path);
                if (listView1.SelectedItems.Count > 0)
                    this.SelectedFile = Application.StartupPath + "\\" +
                        StringTable.TEMPLATES_DIR + "\\" + listView1.SelectedItems[0].Text;
                this.ProjectPath = path;
                this.ProjectName = txtProjectName.Text;
                this.DialogResult = DialogResult.OK;
            }
            catch
            {
                MessageBox.Show("The location specified cannot be created. Check that the location is not a reserved system name, and that the disk is writeable and that there is enough room on the disk.", Application.ProductName);
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = txtLocation.Text.Trim() != String.Empty &&
                txtProjectName.Text.Trim() != String.Empty;
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            Graphics gfx = e.Graphics;
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            gfx.DrawLine(Pens.Gray, 0, 0, panel4.Width, panel4.Height);
            gfx.DrawLine(Pens.Gray, panel4.Width, 0, 0, panel4.Height);
            string text = "Not Yet Implemented";
            Size size = gfx.MeasureString(text, panel4.Font).ToSize();
            Point p = new Point((panel4.Width - size.Width) / 2, (panel4.Height - size.Height) / 2);
            gfx.DrawString(text, panel4.Font, Brushes.Gray, p);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            if (lblContent.Enabled)
            {
                lblContent.Text = File.ReadAllText(listView1.SelectedItems[0].Tag.ToString());
            }
        }

        private void radSortAsc_CheckedChanged(object sender, EventArgs e)
        {
            listView1.Sorting = radSortAsc.Checked ? SortOrder.Descending : SortOrder.Ascending;
            listView1.Sort();
        }

        private void cboViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboViewMode.SelectedIndex >= 0)
                listView1.View = (View)cboViewMode.SelectedIndex;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                btnOK_Click(null, null);
            }
        }

    }
}
