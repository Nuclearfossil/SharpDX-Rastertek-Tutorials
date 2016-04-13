using System;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SharpDXWinForm
{
    public partial class SharpDXRastertekTutorialsForm : Form
    {
        public SharpDXRastertekTutorialsForm()
        {
            InitializeComponent();    
            
            numericUpDownWidth.Maximum = Screen.PrimaryScreen.Bounds.Width;
            numericUpDownHeight.Maximum = Screen.PrimaryScreen.Bounds.Height;
        }
        private void ToogleAllButtons()
        {
            Thread.Sleep(500);

            if (this.Enabled)
            {
                // Disallow user to press another Test Button while this is running.
                this.Enabled = false;
                Thread.Sleep(500);
                this.Hide();
                Thread.Sleep(500);
            }
            else
            {
                // needed for quick redisplay of form in a disabled state without screen flashing.
                this.Enabled = true;
                this.Show();
                this.Enabled = false;
                Thread.Sleep(500);
                this.Enabled = true;
            }
        }
        private void DisplayTestResults()
        {
            using (PerfTestResultForm form = new PerfTestResultForm())
                form.ShowDialog(this);
        }
        private void checkBoxScreenSize_Click(object sender, EventArgs e)
        {
            if (checkBoxScreenSize.CheckState == CheckState.Checked)
            {
                checkBoxScreenSize.Text = "Full Screen";
                numericUpDownWidth.Enabled = numericUpDownHeight.Enabled = labelWidth.Enabled = labelHeight.Enabled = false;
            }
            else if (checkBoxScreenSize.CheckState == CheckState.Unchecked)
                checkBoxScreenSize.Text = "Windowed";
            else if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
            {
                checkBoxScreenSize.Text = "Both";
                numericUpDownWidth.Enabled = numericUpDownHeight.Enabled = labelWidth.Enabled = labelHeight.Enabled = true;
            }
        }
        private void checkBoxTimer_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownTimeInSeconds.Value = (!checkBoxTimer.Checked) ? 0 : 8;
            numericUpDownTimeInSeconds.Enabled = checkBoxTimer.Checked;
        }
        private void buttonRunPerfTests_Click(object sender, EventArgs e)
        {
            // First select the Main Series I Tutorials Page.
            tabControl1.SelectedTab = tabPage1;
            PressAllButtons();        
            // Now run The Terrain Tutorial Performance Tests as well.
            tabControl1.SelectedTab = tabPage2;
            PressAllButtons();
            // And finally the last Series 2 Tutorials as well.
            tabControl1.SelectedTab = tabPage3;
            PressAllButtons();

            // Display Performance Test Results
            DisplayTestResults();
        }
        private void PressAllButtons()
        {
            List<Button> sorted = new List<Button>();

            // iterate through all the buttons for clicking a Tutorial Performace Test.
            foreach (Control control in tabControl1.SelectedTab.Controls)
                if (control is Button)
                    sorted.Add((control as Button)); 

            // Sort the buttons in Order and trim out Tutorial 8 for loading Maya objects does not execute as a running engine..
            sorted = sorted.OrderBy(e => e.Name).Where(e => !e.Name.Equals("buttonTutorial08")).ToList();

            // Now iterate through the trimmed and sorted buttons for execution of a performance test.
            foreach (Button but in sorted)
                but.PerformClick();
        }

        // Rastertek Tutorials
        private void buttonTutorial2_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 2: Creating a Framework and Window    -   278 lines   - (C++: 0 FPS C#: 0 FPS)
            DSharpDXRastertek.Tut02.System.DSystem.StartRenderForm("Tutorial 2: Creating a Framework and Window", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut02.System.DSystem.StartRenderForm("Tutorial 2: Creating a Framework and Window", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial3_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 3: Initializing DirectX 11            -   563 lines   - (C++: 2255 FPS C#: 2290 FPS)
            DSharpDXRastertek.Tut03.System.DSystem.StartRenderForm("Tutorial 3: Initializing DirectX 11", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut03.System.DSystem.StartRenderForm("Tutorial 3: Initializing DirectX 11", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial4_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 4: Buffers, Shaders, and HLSL         -   997 lines   - (C++: 1482 FPS C#: 1488 FPS)
            DSharpDXRastertek.Tut04.System.DSystem.StartRenderForm("Tutorial 4: Buffers, Shaders, and HLSL", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut04.System.DSystem.StartRenderForm("Tutorial 4: Buffers, Shaders, and HLSL", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial5_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 5: Texturing                          -  1081 lines   - (C++: 1436 FPS C#: 1457 FPS)
            DSharpDXRastertek.Tut05.System.DSystem.StartRenderForm("Tutorial 5: Texturing", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut05.System.DSystem.StartRenderForm("Tutorial 5: Texturing", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial6_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 6: Diffuse Lighting                   -  1203 lines   - (C++: 1445 FPS C#: 1493 FPS)
            DSharpDXRastertek.Tut06.System.DSystem.StartRenderForm("Tutorial 6: Diffuse Lightingg", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut06.System.DSystem.StartRenderForm("Tutorial 6: Diffuse Lightingg", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial7_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 7: 3D Model Rendering                 -  1241 lines   - (C++: 1386 FPS C#: 1400 FPS)
            DSharpDXRastertek.Tut07.System.DSystem.StartRenderForm("Tutorial 7: 3D Model Rendering", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut07.System.DSystem.StartRenderForm("Tutorial 7: 3D Model Rendering", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial8_Click(object sender, EventArgs e)
        {
            // Tutorial 8: Loading Maya 2011 Models
            DSharpDXRastertek.Tut08.Tut08LoadMaya2011ModelForm form = new DSharpDXRastertek.Tut08.Tut08LoadMaya2011ModelForm();
            form.ShowDialog(this);
        }
        private void buttonTutorial9_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 9: Ambient Lighting                   -  1248 lines   - (C++: 1387 FPS C#: 1400 FPS)
            DSharpDXRastertek.Tut09.System.DSystem.StartRenderForm("Tutorial 9: Ambient Lighting", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut09.System.DSystem.StartRenderForm("Tutorial 9: Ambient Lighting", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial10_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 10: Specular Lighting                 -  1281 lines   - (C++: 1380 FPS C#: 1394 FPS)
            DSharpDXRastertek.Tut10.System.DSystem.StartRenderForm("Tutorial 10: Specular Lighting", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut10.System.DSystem.StartRenderForm("Tutorial 10: Specular Lighting", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial11_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 11: 2D Rendering                      -  1208 lines   - (C++: 1375 FPS C#: 1395 FPS)
            DSharpDXRastertek.Tut11.System.DSystem.StartRenderForm("Tutorial 11: 2D Rendering", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut11.System.DSystem.StartRenderForm("Tutorial 11: 2D Rendering", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial12_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 12: Font Engine                       -  1523 lines   - (C++: 1480 FPS C#: 1488 FPS)
            DSharpDXRastertek.Tut12.System.DSystem.StartRenderForm("Tutorial 12: Font Engine", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut12.System.DSystem.StartRenderForm("Tutorial 12: Font Engine", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial13_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 13: Direct Input                      -  1763 lines   - (C++: 1443 FPS C#: 1492 FPS)
            DSharpDXRastertek.Tut13.System.DSystem.StartRenderForm("Tutorial 13: Direct Input", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut13.System.DSystem.StartRenderForm("Tutorial 13: Direct Input", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial14_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 14: Direct Sound                      -  1111 lines   - (C++: 2267 FPS C#: 2288 FPS)
            DSharpDXRastertek.Tut14.System.DSystem.StartRenderForm("Tutorial 14: Direct Sound", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut14.System.DSystem.StartRenderForm("Tutorial 14: Direct Sound", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial15_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 15: FPS, CPU Usage, and Timers        -  1875 lines   - (C++: 1466 FPS C#: 1466 FPS)
            DSharpDXRastertek.Tut15.System.DSystem.StartRenderForm("Tutorial 15: FPS, CPU Usage, and Timers", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut15.System.DSystem.StartRenderForm("Tutorial 15: FPS, CPU Usage, and Timers", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial16_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 16: Frustum Culling  Render Count 21  -  2608 lines   - (C++: 340 FPS  C#: 343 FPS)
            DSharpDXRastertek.Tut16.System.DSystem.StartRenderForm("Tutorial 16: Frustum Culling", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut16.System.DSystem.StartRenderForm("Tutorial 16: Frustum Culling", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial17_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 17: Multitexturing and Texture Arrays -  1409 lines   - (C++: 975 FPS  C#: 962 FPS)
            DSharpDXRastertek.Tut17.System.DSystem.StartRenderForm("Tutorial 17: Multitexturing and Texture Array", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut17.System.DSystem.StartRenderForm("Tutorial 17: Multitexturing and Texture Array", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial18_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 18: Light Maps                        -1357 lines - (C++: 953 FPS C#: 960 FPS)
            DSharpDXRastertek.Tut18.System.DSystem.StartRenderForm("Tutorial 18: Light Mapsy", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut18.System.DSystem.StartRenderForm("Tutorial 18: Light Mapsy", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial19_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 19: Alpha Mapping                     -  1444 lines   - (C++: 828 FPS  C#: 838 FPS)
            DSharpDXRastertek.Tut19.System.DSystem.StartRenderForm("Tutorial 19: Alpha Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut19.System.DSystem.StartRenderForm("Tutorial 19: Alpha Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial20_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 20: Bump Mapping                      -  1691 lines   - (C++: 962 FPS  C#: 975 FPS)
            DSharpDXRastertek.Tut20.System.DSystem.StartRenderForm("Tutorial 20: Bump Mapping or Normal Mappin", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut20.System.DSystem.StartRenderForm("Tutorial 20: Bump Mapping or Normal Mappin", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial21_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 21: Specular Mapping                  -  1785 lines   - (C++: 840 FPS  C#: 852 FPS)
            DSharpDXRastertek.Tut21.System.DSystem.StartRenderForm("Tutorial 21: Specular Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut21.System.DSystem.StartRenderForm("Tutorial 21: Specular Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial22_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 22: Render to Texture                 -  2391 lines   - (C++: 480 FPS  C#: 485 FPS)
            DSharpDXRastertek.Tut22.System.DSystem.StartRenderForm("Tutorial 22: Render to Texture", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut22.System.DSystem.StartRenderForm("Tutorial 22: Render to Texture", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial23_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 23: Fog                               -  1496 lines   - (C++: 1248 FPS C#: 1270 FPS)
            DSharpDXRastertek.Tut23.System.DSystem.StartRenderForm("Tutorial 23: Fog", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut23.System.DSystem.StartRenderForm("Tutorial 23: Fog", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial24_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 24: Clipping Planes                   -  1450 lines   - (C++: 1424 FPS C#: 1440 FPS)
            DSharpDXRastertek.Tut24.System.DSystem.StartRenderForm("Tutorial 24: Clipping Planes", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut24.System.DSystem.StartRenderForm("Tutorial 24: Clipping Planes", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial25_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 25: Texture Translation               -  1488 lines   - (C++: 1448 FPS C#: 1455 FPS)
            DSharpDXRastertek.Tut25.System.DSystem.StartRenderForm("Tutorial 25: Texture Translation", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut25.System.DSystem.StartRenderForm("Tutorial 25: Texture Translation", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial26_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 26: Transparency                      -  1804 lines   - (C++: 878 FPS  C#: 888 FPS)
            DSharpDXRastertek.Tut26.System.DSystem.StartRenderForm("Tutorial 26: Transparency", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut26.System.DSystem.StartRenderForm("Tutorial 26: Transparency", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial27_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 27: Reflection (Projective Texturing) -  1933 lines   - (C++: 514 FPS  C#: 520 FPS)
            DSharpDXRastertek.Tut27.System.DSystem.StartRenderForm("Tutorial 27: Reflection", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut27.System.DSystem.StartRenderForm("Tutorial 27: Reflection", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial28_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 28: Screen Fades                      -  2219 lines   - (C++: 1410 FPS C#: 1410 FPS)
            DSharpDXRastertek.Tut28.System.DSystem.StartRenderForm("Tutorial 28: Screen Fades", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut28.System.DSystem.StartRenderForm("Tutorial 28: Screen Fades", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial29_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 29: Water                             -  2667 lines   - (C++: 277 FPS  C#: 275 FPS)
            DSharpDXRastertek.Tut29.System.DSystem.StartRenderForm("Tutorial 29: Water", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut29.System.DSystem.StartRenderForm("Tutorial 29: Water", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial30_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 30: Multiple Point Lights             -  1628 lines   - (C++: 1210 FPS C#: 1220 FPS)
            DSharpDXRastertek.Tut30.System.DSystem.StartRenderForm("Tutorial 30: Multiple Point Lights", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut30.System.DSystem.StartRenderForm("Tutorial 30: Multiple Point Lights", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial31_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 31: 3D Sound                          -  1026 lines   - (C++: 2256 FPS C#: 2290 FPS)
            DSharpDXRastertek.Tut31.System.DSystem.StartRenderForm("Tutorial 31: 3D Sound", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut31.System.DSystem.StartRenderForm("Tutorial 31: 3D Sound", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial32_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 32: Glass and Ice                     -  1888 lines   - (C++: 381 FPS  C#: 377 FPS)
            DSharpDXRastertek.Tut32.System.DSystem.StartRenderForm("Tutorial 32: Glass and Ice", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut32.System.DSystem.StartRenderForm("Tutorial 32: Glass and Ice", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial33_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 33: Fire                              -  1511 lines   - (C++: 895 FPS  C#: 892 FPS)
            DSharpDXRastertek.Tut33.System.DSystem.StartRenderForm("Tutorial 33: Fire", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut33.System.DSystem.StartRenderForm("Tutorial 33: Fire", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial34_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 34: Billboarding                      -  1567 lines   - (C++: 1220 FPS C#: 1220 FPS)
            DSharpDXRastertek.Tut34.System.DSystem.StartRenderForm("Tutorial 34: Billboarding", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut34.System.DSystem.StartRenderForm("Tutorial 34: Billboarding", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial35_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 35: Depth Buffers                     -  1213 lines   - (C++: 1350 FPS C#: 1350 FPS)
            DSharpDXRastertek.Tut35.System.DSystem.StartRenderForm("Tutorial 35: Depth Buffers", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut35.System.DSystem.StartRenderForm("Tutorial 35: Depth Buffers", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial36_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 36: Blur                              -  2646 lines   - (C++: 126 FPS  C#: 126 FPS)
            DSharpDXRastertek.Tut36.System.DSystem.StartRenderForm("Tutorial 36: Blur", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut36.System.DSystem.StartRenderForm("Tutorial 36: Blur", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial37_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 37: Instancing                        -  1307 lines   - (C++: 1316 FPS C#: 1312 FPS)
            DSharpDXRastertek.Tut37.System.DSystem.StartRenderForm("Tutorial 37: Instancing", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut37.System.DSystem.StartRenderForm("Tutorial 37: Instancing", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial38_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 38: Hardware Tessellation             -  1256 lines   - (C++:  745 FPS C#:  715 FPS)
            DSharpDXRastertek.Tut38.System.DSystem.StartRenderForm("Tutorial 38: Hardware Tessellation", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut38.System.DSystem.StartRenderForm("Tutorial 38: Hardware Tessellation", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial39_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 39: Particle Systems                  -  1565 lines   - (C++:  335 FPS C#:  435 FPS)
            DSharpDXRastertek.Tut39.System.DSystem.StartRenderForm("Tutorial 39: Particle Systems", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut39.System.DSystem.StartRenderForm("Tutorial 39: Particle Systems", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial40_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 40: Shadow Mapping                    -  2205 lines  - (C++:  270 FPS C#:  270 FPS)
            DSharpDXRastertek.Tut40.System.DSystem.StartRenderForm("Tutorial 40: Shadow Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut40.System.DSystem.StartRenderForm("Tutorial 40: Shadow Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial41_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 41: Multiple Light Shadow Mapping     -  2584 lines   - (C++:  124 FPS C#:  124 FPS)
            DSharpDXRastertek.Tut41.System.DSystem.StartRenderForm("Tutorial 41: Multiple Light Shadow Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut41.System.DSystem.StartRenderForm("Tutorial 41: Multiple Light Shadow Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial42_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 42: Soft Shadows                      -  3994 lines   - (C++:   55 FPS C#:  56 FPS)
            DSharpDXRastertek.Tut42.System.DSystem.StartRenderForm("Tutorial 42: Soft Shadows", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut42.System.DSystem.StartRenderForm("Tutorial 42: Soft Shadows", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked,false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial43_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 43: Projective Texturing              -  1467 lines   - (C++: 1045 FPS C#: 1058 FPS)
            DSharpDXRastertek.Tut43.System.DSystem.StartRenderForm("Tutorial 43: Projective Texturing", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut43.System.DSystem.StartRenderForm("Tutorial 43: Projective Texturing", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial44_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 44: Projected Light Maps              -  1384 lines    - (C++: 960 FPS C#:  964 FPS)
            DSharpDXRastertek.Tut44.System.DSystem.StartRenderForm("Tutorial 44: Projected Light Maps", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut44.System.DSystem.StartRenderForm("Tutorial 44: Projected Light Maps", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial45_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 45: Managing Multiple Shaders         -  2518 lines    - (C++: 1155 FPS C#: 1155 FPS)
            DSharpDXRastertek.Tut45.System.DSystem.StartRenderForm("Tutorial 45: Managing Multiple Shaders", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut45.System.DSystem.StartRenderForm("Tutorial 45: Managing Multiple Shaders", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial46_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 46: Glow                              -  3203 lines     - (C++:  104 FPS C#:  105 FPS)
            DSharpDXRastertek.Tut46.System.DSystem.StartRenderForm("Tutorial 46: Glow", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut46.System.DSystem.StartRenderForm("Tutorial 46: Glow", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial47_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 47: Picking                           -  2883 lines     - (C++: 1290 FPS C#: 1293 FPS)
            DSharpDXRastertek.Tut47.System.DSystem.StartRenderForm("Tutorial 47: Picking", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut47.System.DSystem.StartRenderForm("Tutorial 47: Picking", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial48_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 48: Directional Shadow Maps           -  2300 lines     - (C++:  300 FPS C#:  300 FPS)
            DSharpDXRastertek.Tut48.System.DSystem.StartRenderForm("Tutorial 48: Directional Shadow Maps", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut48.System.DSystem.StartRenderForm("Tutorial 48: Directional Shadow Maps", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial49_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 49: Shadow Mapping and Transparency   -  2845 lines     - (C++:  110 FPS C#:  130 FPS)
            DSharpDXRastertek.Tut49.System.DSystem.StartRenderForm("Tutorial 49: Shadow Mapping and Transparency", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut49.System.DSystem.StartRenderForm("Tutorial 49: Shadow Mapping and Transparency", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTutorial50_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Tutorial 50: Deferred Shading                  -  2147 lines     - (C++:  240 FPS C#:  240 FPS)
            DSharpDXRastertek.Tut50.System.DSystem.StartRenderForm("Tutorial 50: Deferred Shading", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Tut50.System.DSystem.StartRenderForm("Tutorial 50: Deferred Shading", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }

        // Rastertek Terrain Tutorials
        private void buttonTerrainTutorial1_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 1: Grid and Camera Movement   -  2673 lines     - (C++:  687 FPS C#:  692 FPS)
            DSharpDXRastertek.TutTerr01.System.DSystem.StartRenderForm("Terrain Tutorial 1: Grid and Camera Movement", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr01.System.DSystem.StartRenderForm("Terrain Tutorial 1: Grid and Camera Movement", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial2_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 2: Height Maps                -  2796 lines   - (C++: 148 FPS  C#: 148 FPS)
            DSharpDXRastertek.TutTerr02.System.DSystem.StartRenderForm("Terrain Tutorial 2: Height Maps", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr02.System.DSystem.StartRenderForm("Terrain Tutorial 2: Height Maps", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial3_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 3: Terrain Lighting           -  2907 lines   - (C++:  294 FPS C#:  292 FPS)
            DSharpDXRastertek.TutTerr03.System.DSystem.StartRenderForm("Terrain Tutorial 3: Terrain Lighting", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr03.System.DSystem.StartRenderForm("Terrain Tutorial 3: Terrain Lighting", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial4_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 4: Terrain Texturing          -  2945 lines   - (C++:  293 FPS C#:  291 FPS)
            DSharpDXRastertek.TutTerr04.System.DSystem.StartRenderForm("Terrain Tutorial 4: Terrain Texturing", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr04.System.DSystem.StartRenderForm("Terrain Tutorial 4: Terrain Texturing", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial5_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 5: Quad Trees                 -  3684 lines   - (C++:  371 FPS C#:  380 FPS)
            DSharpDXRastertek.TutTerr05.System.DSystem.StartRenderForm("Terrain Tutorial 5: Quad Trees", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr05.System.DSystem.StartRenderForm("Terrain Tutorial 5: Quad Trees", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial6_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 6: Height Based Movement      -  3831 lines   - (C++:  371 FPS C#:  390 FPS)
            DSharpDXRastertek.TutTerr06.System.DSystem.StartRenderForm("Terrain Tutorial 6: Height Based Movement", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr06.System.DSystem.StartRenderForm("Terrain Tutorial 6: Height Based Movement", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial7_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 7: Color Mapped Terrain       -  3268 lines   - (C++:  287 FPS C#:  297 FPS)
            DSharpDXRastertek.TutTerr07.System.DSystem.StartRenderForm("Terrain Tutorial 7: Color Mapped Terrain", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr07.System.DSystem.StartRenderForm("Terrain Tutorial 7: Color Mapped Terrain", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial8_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 8: Terrain Mini-Maps          -  3894 lines   - (C++  281 FPS C#:  291 FPS)
            DSharpDXRastertek.TutTerr08.System.DSystem.StartRenderForm("Terrain Tutorial 8: Terrain Mini-Maps", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr08.System.DSystem.StartRenderForm("Terrain Tutorial 8: Terrain Mini-Maps", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial9_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 9: Terrain Blending           -  3380 lines   - (C++:  796 FPS C#:  860 FPS)
            DSharpDXRastertek.TutTerr09.System.DSystem.StartRenderForm("Terrain Tutorial 9: Terrain Blending", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr09.System.DSystem.StartRenderForm("Terrain Tutorial 9: Terrain Blending", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial10_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 10: Sky Domes                 -  3888 lines   - (C++:  270 FPS C#:  280 FPS)
            DSharpDXRastertek.TutTerr10.System.DSystem.StartRenderForm("Terrain Tutorial 10: Sky Domes", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr10.System.DSystem.StartRenderForm("Terrain Tutorial 10: Sky Domes", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial11_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 11: Bitmap Clouds             -  4400 lines   - (C++: 250 FPS  C#: 259 FPS)
            DSharpDXRastertek.TutTerr11.System.DSystem.StartRenderForm("Terrain Tutorial 11: Bitmap Clouds", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr11.System.DSystem.StartRenderForm("Terrain Tutorial 11: Bitmap Clouds", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial12_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 12: Perturbed Clouds          -  4373 lines   - (C++:  246 FPS C#:  255 FPS)
            DSharpDXRastertek.TutTerr12.System.DSystem.StartRenderForm("Terrain Tutorial 12: Perturbed Clouds", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr12.System.DSystem.StartRenderForm("Terrain Tutorial 12: Perturbed Clouds", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial13_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 13: Terrain Detail Mapping    -  4141 lines   - (C++:  115 FPS C#:  115 FPS)
            DSharpDXRastertek.TutTerr13.System.DSystem.StartRenderForm("Terrain Tutorial 13: Terrain Detail Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr13.System.DSystem.StartRenderForm("Terrain Tutorial 13: Terrain Detail Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial14_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 14: Slope Based Texturing     -  3257 lines   - (C++:  267 FPS C#:  270 FPS)
            DSharpDXRastertek.TutTerr14.System.DSystem.StartRenderForm("Terrain Tutorial 14: Slope Based Texturing", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr14.System.DSystem.StartRenderForm("Terrain Tutorial 14: Slope Based Texturing", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial15_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 15: Terrain Bump Mapping      -  3367 lines   - (C++:  265 FPS C#:  269 FPS)
            DSharpDXRastertek.TutTerr15.System.DSystem.StartRenderForm("Terrain Tutorial 15: Terrain Bump Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr15.System.DSystem.StartRenderForm("Terrain Tutorial 15: Terrain Bump Mapping", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial16_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();
            Thread.Sleep(2000);

            // Terrain Tutorial 16: Small Body Water          -  5732 lines   - (C++:   19 FPS C#:   19 FPS)
            DSharpDXRastertek.TutTerr16.System.DSystem.StartRenderForm("Terrain Tutorial 16: Small Body Water", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr16.System.DSystem.StartRenderForm("Terrain Tutorial 16: Small Body Water", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial17_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 17: Terrain Texture Layers    -  2191 lines   - (C++:  370 FPS C#:  355 FPS)
            DSharpDXRastertek.TutTerr17.System.DSystem.StartRenderForm("Terrain Tutorial 17: Terrain Texture Layers", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr17.System.DSystem.StartRenderForm("Terrain Tutorial 17: Terrain Texture Layers", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonTerrainTutorial19_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Terrain Tutorial 19: Foliage                   -  3458 lines   - (C++:  119 FPS C#:  120 FPS)
            DSharpDXRastertek.TutTerr19.System.DSystem.StartRenderForm("Terrain Tutorial 19: Foliag", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.TutTerr19.System.DSystem.StartRenderForm("Terrain Tutorial 19: Foliag", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonS2Tutorial2_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Series 2 Tutorial 2: Creating a Framework and Window - 278 lines - (C++: 0 FPS C#: 0 FPS)
            DSharpDXRastertek.Series2.Tut02.System.DSystem.StartRenderForm("Series 2 Tutorial 2: Creating a Framework and Window", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Series2.Tut02.System.DSystem.StartRenderForm("Series 2 Tutorial 2: Creating a Framework and Window", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
        private void buttonS2Tutorial3_Click(object sender, EventArgs e)
        {
            ToogleAllButtons();

            // Series 2 Tutorial 3: Initializing DirectX 11    -   563 lines   - (C++: 2255 FPS C#: 2290 FPS)
            DSharpDXRastertek.Series2.Tut03.System.DSystem.StartRenderForm("Series 2 Tutorial 3: Initializing DirectX 11", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, checkBoxScreenSize.Checked, (int)numericUpDownTimeInSeconds.Value);
            Thread.Sleep(3000);

            // Execute Second pass with the Full Screen off since it was first sent in Fullscreen.
            if (checkBoxScreenSize.CheckState == CheckState.Indeterminate)
                DSharpDXRastertek.Series2.Tut03.System.DSystem.StartRenderForm("Series 2 Tutorial 3: Initializing DirectX 11", (int)numericUpDownWidth.Value, (int)numericUpDownHeight.Value, checkBoxVSync.Checked, false, (int)numericUpDownTimeInSeconds.Value);

            ToogleAllButtons();
        }
    }
}
