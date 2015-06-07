using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Y2LCore;
using Y2LCore.SyntaxTree;

namespace Y2L_IDE
{
	public partial class SyntaxTreeForm : Form
	{
		private const string MODULE="module";
		private const string FUNCTION="method";
		private const string VARIABLE="variable";
		
		Module _module;
		
		public SyntaxTreeForm()
		{
			InitializeComponent();
		}
		public void LoadTree(Module mod)
		{
			_module=mod;
			
			if (mod == null)
			{
				return;
			}
			treeView1.Nodes.Clear();

			TreeNode node = treeView1.Nodes.Add(mod.Name);
			node.ImageKey=MODULE;
			node.SelectedImageKey=MODULE;
			node.Expand();
			LoadBlock(node,mod);			
			
		}
		void LoadBlock(TreeNode node,CodeBlock block)
		{
			if(block==null)
				return;
			
			if(node.Level<5)
				node.Expand();
			if(block is Function)
			{
			node.ImageKey=FUNCTION;
			node.SelectedImageKey=FUNCTION;
			}
			foreach(var stmt in block)
			{
				TreeNode n=node.Nodes.Add(stmt.ToString());
				//if(stmt is DeclareVar)
					n.ImageKey=VARIABLE;	
					n.SelectedImageKey=VARIABLE;
				LoadBlock(n,stmt as CodeBlock);
			}
		}
		
	}
}
