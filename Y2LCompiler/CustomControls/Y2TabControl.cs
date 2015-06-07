using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Y2L_IDE
{
    class Y2TabControl : TabControl
    {
        LinearGradientBrush brush = new LinearGradientBrush(
        new Rectangle(0, 0, 25, 20),
        Color.LightYellow,
        Color.Wheat,
        LinearGradientMode.Vertical);

        Image _y2lImage = Properties.Resources.Y2l_code_file.ToBitmap();
        Image _unknownImage = Properties.Resources.Mimetypes_unknown.ToBitmap();
        public Y2TabControl()
        {

        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            e.Control.Text= " ".PadRight(6)+e.Control.Text;
            base.OnControlAdded(e);
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;


            if (e.Index == this.SelectedIndex)
            {
                g.FillRectangle(brush, e.Bounds);
                
            }
             
            Point pos = e.Bounds.Location;
            pos.X += 3;
            pos.Y += 3;

            g.DrawString(this.TabPages[e.Index].Text, this.Font, Brushes.Black, pos);

           // icon
            Rectangle rect = e.Bounds;
            rect.Offset(3, 3);
            rect.Width = 13;
            rect.Height = 15;
            if(TabPages[e.Index].Name.EndsWith(StringTable.FILE_CODE_EXTENSION))
            g.DrawImage(_y2lImage, rect);
            else
                g.DrawImage(_unknownImage, rect);
            base.OnDrawItem(e);
        }

    }
}
