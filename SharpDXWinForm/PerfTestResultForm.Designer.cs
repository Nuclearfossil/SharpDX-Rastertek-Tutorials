namespace SharpDXWinForm
{
    partial class PerfTestResultForm
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
            this.richTextBoxDisplay = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBoxDisplay
            // 
            this.richTextBoxDisplay.BackColor = System.Drawing.Color.Black;
            this.richTextBoxDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxDisplay.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxDisplay.ForeColor = System.Drawing.Color.White;
            this.richTextBoxDisplay.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxDisplay.Name = "richTextBoxDisplay";
            this.richTextBoxDisplay.Size = new System.Drawing.Size(426, 402);
            this.richTextBoxDisplay.TabIndex = 0;
            this.richTextBoxDisplay.Text = "";
            this.richTextBoxDisplay.WordWrap = false;
            // 
            // PerfTestResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(426, 402);
            this.Controls.Add(this.richTextBoxDisplay);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PerfTestResultForm";
            this.Text = "PerfTestResultForm";
            this.Load += new System.EventHandler(this.PerfTestResultForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxDisplay;
    }
}