using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Y2L_IDE
{
    public partial class ConfirmSaveDialog : Form
    {
        public ConfirmSaveDialog()
        {
            InitializeComponent();
            this.Text = Application.ProductName;
        }
        public DialogResult ShowConfirm(IWin32Window parent, params string[] items)
        {
            listBox1.Items.AddRange(items.ToArray());
           return this.ShowDialog(parent);
        }
    }
}
