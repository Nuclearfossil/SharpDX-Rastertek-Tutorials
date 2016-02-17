using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut08
{
    public partial class Tut08LoadMaya2011ModelForm : Form
    {
        OBJImporter importedOBJ = null;

        public Tut08LoadMaya2011ModelForm()
        {
            InitializeComponent();

            this.Text = "Tutorial 8: Loading Maya 2011 Models ";
        }
        private void buttonSource_Click(object sender, EventArgs e)
        {
            // Read in the name of the model file.
            openFileDialog1.ShowDialog();
            textBoxFrom.Text = openFileDialog1.FileName;
            importedOBJ = new OBJImporter(textBoxFrom.Text);
        }
        private void buttonDestination_Click(object sender, EventArgs e)
        {
            // Get the name of the file to save in.
            saveFileDialog1.ShowDialog();
            textBoxDestination.Text = saveFileDialog1.FileName;
        }
        private void buttonImportOBJ_Click(object sender, EventArgs e)
        {
            importedOBJ.ImportOBJ(textBoxDestination.Text);
            MessageBox.Show("Conversion of " + textBoxDestination.Text + " Complete!");
        }
    }
}
