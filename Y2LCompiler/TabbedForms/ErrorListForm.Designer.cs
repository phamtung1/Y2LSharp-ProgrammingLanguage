﻿namespace Y2L_IDE
{
    partial class ErrorListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        	this.listView1 = new System.Windows.Forms.ListView();
        	this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
        	this.SuspendLayout();
        	// 
        	// listView1
        	// 
        	this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        	        	        	this.columnHeader1,
        	        	        	this.columnHeader2,
        	        	        	this.columnHeader3,
        	        	        	this.columnHeader4,
        	        	        	this.columnHeader5});
        	this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.listView1.FullRowSelect = true;
        	this.listView1.GridLines = true;
        	this.listView1.Location = new System.Drawing.Point(0, 0);
        	this.listView1.Name = "listView1";
        	this.listView1.Size = new System.Drawing.Size(648, 118);
        	this.listView1.TabIndex = 0;
        	this.listView1.UseCompatibleStateImageBehavior = false;
        	this.listView1.View = System.Windows.Forms.View.Details;
        	this.listView1.DoubleClick += new System.EventHandler(this.ListView1DoubleClick);
        	// 
        	// columnHeader1
        	// 
        	this.columnHeader1.Text = "";
        	this.columnHeader1.Width = 20;
        	// 
        	// columnHeader2
        	// 
        	this.columnHeader2.Text = "Descrption";
        	this.columnHeader2.Width = 400;
        	// 
        	// columnHeader3
        	// 
        	this.columnHeader3.Text = "File";
        	this.columnHeader3.Width = 120;
        	// 
        	// columnHeader4
        	// 
        	this.columnHeader4.Text = "Line";
        	// 
        	// columnHeader5
        	// 
        	this.columnHeader5.Text = "Col";
        	// 
        	// ErrorListForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(648, 118);
        	this.Controls.Add(this.listView1);
        	this.Name = "ErrorListForm";
        	this.Text = "Error List";
        	this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}