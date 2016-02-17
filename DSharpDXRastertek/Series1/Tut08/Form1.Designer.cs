namespace DSharpDXRastertek.Tut08
{
    partial class Tut08LoadMaya2011ModelForm
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
            this.buttonImportOBJ = new System.Windows.Forms.Button();
            this.textBoxFrom = new System.Windows.Forms.TextBox();
            this.textBoxDestination = new System.Windows.Forms.TextBox();
            this.buttonSource = new System.Windows.Forms.Button();
            this.buttonDestination = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // buttonImportOBJ
            // 
            this.buttonImportOBJ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonImportOBJ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonImportOBJ.ForeColor = System.Drawing.Color.White;
            this.buttonImportOBJ.Location = new System.Drawing.Point(80, 191);
            this.buttonImportOBJ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonImportOBJ.Name = "buttonImportOBJ";
            this.buttonImportOBJ.Size = new System.Drawing.Size(256, 69);
            this.buttonImportOBJ.TabIndex = 0;
            this.buttonImportOBJ.Text = "Import OBJ";
            this.buttonImportOBJ.UseVisualStyleBackColor = false;
            this.buttonImportOBJ.Click += new System.EventHandler(this.buttonImportOBJ_Click);
            // 
            // textBoxFrom
            // 
            this.textBoxFrom.BackColor = System.Drawing.Color.Black;
            this.textBoxFrom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxFrom.ForeColor = System.Drawing.Color.White;
            this.textBoxFrom.Location = new System.Drawing.Point(39, 75);
            this.textBoxFrom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxFrom.Name = "textBoxFrom";
            this.textBoxFrom.Size = new System.Drawing.Size(230, 26);
            this.textBoxFrom.TabIndex = 1;
            // 
            // textBoxDestination
            // 
            this.textBoxDestination.BackColor = System.Drawing.Color.Black;
            this.textBoxDestination.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxDestination.ForeColor = System.Drawing.Color.White;
            this.textBoxDestination.Location = new System.Drawing.Point(39, 115);
            this.textBoxDestination.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxDestination.Name = "textBoxDestination";
            this.textBoxDestination.Size = new System.Drawing.Size(230, 26);
            this.textBoxDestination.TabIndex = 1;
            // 
            // buttonSource
            // 
            this.buttonSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonSource.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSource.ForeColor = System.Drawing.Color.White;
            this.buttonSource.Location = new System.Drawing.Point(279, 75);
            this.buttonSource.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(112, 35);
            this.buttonSource.TabIndex = 0;
            this.buttonSource.Text = "Source";
            this.buttonSource.UseVisualStyleBackColor = false;
            this.buttonSource.Click += new System.EventHandler(this.buttonSource_Click);
            // 
            // buttonDestination
            // 
            this.buttonDestination.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonDestination.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDestination.ForeColor = System.Drawing.Color.White;
            this.buttonDestination.Location = new System.Drawing.Point(279, 112);
            this.buttonDestination.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonDestination.Name = "buttonDestination";
            this.buttonDestination.Size = new System.Drawing.Size(112, 35);
            this.buttonDestination.TabIndex = 0;
            this.buttonDestination.Text = "Destination";
            this.buttonDestination.UseVisualStyleBackColor = false;
            this.buttonDestination.Click += new System.EventHandler(this.buttonDestination_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(428, 294);
            this.Controls.Add(this.textBoxDestination);
            this.Controls.Add(this.textBoxFrom);
            this.Controls.Add(this.buttonDestination);
            this.Controls.Add(this.buttonSource);
            this.Controls.Add(this.buttonImportOBJ);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonImportOBJ;
        private System.Windows.Forms.TextBox textBoxFrom;
        private System.Windows.Forms.TextBox textBoxDestination;
        private System.Windows.Forms.Button buttonSource;
        private System.Windows.Forms.Button buttonDestination;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}