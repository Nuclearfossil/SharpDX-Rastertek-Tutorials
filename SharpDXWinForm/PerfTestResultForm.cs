using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpDXWinForm
{
    public partial class PerfTestResultForm : Form
    {
        public PerfTestResultForm()
        {
            InitializeComponent();
        }
        private void PerfTestResultForm_Load(object sender, EventArgs e)
        {
            richTextBoxDisplay.Text = File.ReadAllText("Test.txt");
        }
    }
}
