using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace Y2L_IDE
{
	public partial class Editor : Control
	{
		public event EventHandler ContentChanged;
		public string FilePath;
        TextEditorControl textEditor;

		public bool IsChanged;

		public Editor(string filePath)
		{
            this.textEditor = new ICSharpCode.TextEditor.TextEditorControl();

            // 
            // textEditor
            // 
            this.textEditor.AllowCaretBeyondEOL = true;
            
            this.textEditor.IsReadOnly = false;
            this.textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditor.LineViewerStyle = ICSharpCode.TextEditor.Document.LineViewerStyle.FullRow;
            this.textEditor.ShowTabs = true;
            this.textEditor.ShowVRuler = false;
            
            this.Controls.Add(textEditor);
   
            SetHighlighting();

			try
			{
				this.FilePath = filePath;
				textEditor.LoadFile(filePath,true,true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Application.ProductName);
			}

            IsChanged = false;
		}		
		
		void SetHighlighting()
		{			
			FileSyntaxModeProvider fsmProvider; 
			string dir=Path.Combine(Application.StartupPath, "highlighting");
			if (Directory.Exists( dir))
			{
				fsmProvider = new FileSyntaxModeProvider(dir); 
				HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmProvider); 
				textEditor.SetHighlighting("Y2L");
			}
			textEditor.TextChanged+=new EventHandler(textEditor_TextChanged);
			textEditor.Document.FormattingStrategy= new DefaultFormattingStrategy();

            textEditor.ActiveTextAreaControl.Caret.PositionChanged += (sender, e1) =>
                {
                    Caret caret=textEditor.ActiveTextAreaControl.Caret;                    
                    ((MainForm)MainForm.ActiveForm).SetStatus(caret.Line+1, caret.Column);                    
                };
	
		}

		internal void Split()
		{
			textEditor.Split();
		}
		internal void Undo()
		{
			textEditor.Undo();			
		}
		internal void Redo(){
			textEditor.Redo();
		}
		
		internal void InsertText(string text)
		{					
			text=text.Replace(@"\n","\n");
			text=text.Replace(@"\t","\t");
			text=text.Replace(@"\r","\r");
			
			textEditor.Document.Insert(textEditor.ActiveTextAreaControl.Caret.Offset,text);
		}
		
		public void GotoPosition(int line,int col)
		{
			textEditor.ActiveTextAreaControl.Caret.Position=new TextLocation(col,line);			
		}
		public void Save()
		{
			textEditor.SaveFile(FilePath);
			IsChanged = false;
		}
		public void SaveAs(string filePath)
		{
			textEditor.SaveFile(filePath);			
		}
		void OnContentChanged(object sender, EventArgs e)
		{
			if (ContentChanged != null)
				ContentChanged(sender, e);
		}
		void textEditor_TextChanged(object sender, EventArgs e)
		{
			if (!IsChanged)
			{
				OnContentChanged(sender, e);
				IsChanged = true;
			}
		}

	}
}
