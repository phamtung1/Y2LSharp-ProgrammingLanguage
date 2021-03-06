using System;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace Y2L_IDE
{
    public partial class ExceptionForm : Form
    {

        Exception exception;
        private ExceptionForm()
        {
            InitializeComponent();
        }
        public ExceptionForm(Exception ex)
            : this()
        {
            this.exception = ex;
            this.Text = Application.ProductName;

            txtExMessage.Text = ex.Message;
            txtException.Text = ex.ToString();
            txtException.SelectionLength = 0;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("mailto:yinyang.it@gmail.com");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (txtException.Height == 0)
            {
                Clipboard.SetText(txtExMessage.Text);
            }
            else
                Clipboard.SetText(txtException.Text);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

    }
}