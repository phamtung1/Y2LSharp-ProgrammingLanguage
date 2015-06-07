using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Y2L_IDE
{
	public partial class ImageListBox : Panel
	{

		public event EventHandler<FileOpenedEventArgs> ProjectFileOpened;

		private const int ITEM_HEIGHT = 25;

		ToolTip tooltip;
		public ImageListBox()
		{
			
			this.AutoScroll=true;
           
			tooltip=new ToolTip();
		}
		protected override void OnResize(EventArgs eventargs)
		{
			RefreshItems();
			base.OnResize(eventargs);
		}
		void RefreshItems()
		{
			int ctlWidth=this.Width-3;
			if(this.VerticalScroll.Visible)
				ctlWidth-=SystemInformation.VerticalScrollBarWidth;
			for(int i=0;i<this.Controls.Count;i++)
			{
				this.Controls[i].Width=ctlWidth;
				this.Controls[i].Top=i*ITEM_HEIGHT;
			}

		}
		public virtual void AddItem(ImageListItem item,int index)
		{
			if(!this.Controls.ContainsKey(item.FilePath))
			{
				item.Name=item.FilePath;
				AddItem(item);
			}
			else
				item=(ImageListItem)this.Controls[item.FilePath];
			
			this.Controls.SetChildIndex(item, index);
			RefreshItems();
		}
		public virtual void AddItem(ImageListItem item)
		{
			item.Name=item.FilePath;
			item.Left=0;
			item.Top=this.Controls.Count*ITEM_HEIGHT;
			item.Height=ITEM_HEIGHT;
			item.Width=this.Width;
			item.Click+= delegate
			{
				if(ProjectFileOpened!=null)
					ProjectFileOpened(this,new FileOpenedEventArgs(item.FilePath));
			};
			this.Controls.Add(item);
			tooltip.SetToolTip(item,item.FilePath);
			
			RefreshItems();
		}
		public virtual void RemoveItemAt(int index)
		{
			this.Controls.RemoveAt(index);
			RefreshItems();
		}

		void OnProjectFileOpened(object sender, FileOpenedEventArgs e)
		{
			if (ProjectFileOpened != null)
				ProjectFileOpened(sender,e);
		}

	}
	public class ImageListItem:Control
	{
		Image _Image;
		public string FilePath;        
		DateTime _lastModified;
		bool _isHover;
		LinearGradientBrush _gradientBrush =
			new LinearGradientBrush(new Rectangle(0,0, 25, 25), Color.Wheat, Color.White, LinearGradientMode.Vertical);
		Rectangle _rect;

		public ImageListItem(string text,string filePath, DateTime lastModified)
		{
			this.DoubleBuffered=true;
			Cursor=Cursors.Hand;
			Text = text;
			this.FilePath = filePath;

			this._lastModified=lastModified;
			if (System.IO.File.Exists(filePath))
				_Image = Properties.Resources.AssignCase;
			else
				_Image = Properties.Resources.delete;
			
		}
		protected override void OnResize(EventArgs e)
		{
			_rect = new Rectangle(0, 0, this.Width, this.Height);
			base.OnResize(e);
		}
		protected override void OnMouseHover(EventArgs e)
		{
			_isHover=true;
			Invalidate();
			base.OnMouseHover(e);
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			_isHover=false;
			Invalidate();
			base.OnMouseLeave(e);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g=e.Graphics;
			if (_isHover)
			{
				 g.FillRectangle(_gradientBrush, _rect);
				_rect.Inflate(-1, -1);
				g.DrawRectangle(Pens.DarkOrange, _rect);
                _rect.Inflate(1, 1);
			}
			else
				g.FillRectangle(Brushes.White, _rect);
			
			g.DrawImage(this._Image,new Rectangle(5,5,20,20));
            SizeF sf = g.MeasureString(Text, this.Font);
			g.DrawString(Text,this.Font,Brushes.Black,30,5);
            string s = _lastModified.ToString();
            
			g.DrawString(s,this.Font,Brushes.Gray,120,5);
            
            sf = g.MeasureString(s, this.Font);
                        
			g.DrawString(FilePath,this.Font,Brushes.DarkBlue,300,5);
			base.OnPaint(e);
		}
	}

}
