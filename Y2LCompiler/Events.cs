using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2L_IDE
{
	public class FileOpenedEventArgs:EventArgs
	{
		public string FilePath;

		public FileOpenedEventArgs(string fileName)
		{
			FilePath = fileName;
		}

	}
			
	public class ErrorEventArgs:EventArgs
	{
		public string FilePath;
		public int Line,Col;

		public ErrorEventArgs(string filePath,int line,int col)
		{
			FilePath=filePath;
			Line=line;
			Col=col;
		}

	}
	public delegate void TextSelectedEventHandler(TextSelectedEventArgs e);
	
	public class TextSelectedEventArgs:EventArgs
	{
		public string Text;
		
		public 	TextSelectedEventArgs(string text)
		{
		Text=text;
		}
	}
}
