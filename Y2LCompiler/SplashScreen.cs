
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Y2L_IDE
{

	public partial class SplashScreen : Form
	{

        Font _font = new Font("Tahoma", 24);

		public SplashScreen()
		{
			InitializeComponent();

            
		}
		protected override void OnPaint(PaintEventArgs e)
		{
            
		}
		protected override void OnPaintBackground(PaintEventArgs e)
		{
            Graphics g = e.Graphics;
			g.DrawImage(Properties.Resources.splash_01,new Rectangle(0,0,this.Width,this.Height));
            g.DrawString(Application.ProductName, _font, Brushes.MidnightBlue, 300, 100);
            g.DrawString(Application.ProductVersion, this.Font, Brushes.MidnightBlue, 500, 140);
            g.DrawString("Copyright © 2011 Yin Yang", this.Font, Brushes.Black, 30, this.Bottom - 240);
		}		
	}
}
