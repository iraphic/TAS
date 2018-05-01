using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DirectShowLib;
using Emgu.Util;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.UI;

namespace TAS
{
    public partial class Main : Form
    {
        private Capture cap = null;
        Mat frame = new Mat();
        List<KeyValuePair<int, string>> listCameraData = new List<KeyValuePair<int, string>>();
        int cameraIndex;
        double width = 0;
        double height = 0;

        double lowThr;
        double accThr;
        double minRad,
               maxRad;
               
        int radiusThr,
            thick,
            blue,
            green,
            red,
            ov,
            gb;
        int hitung;
        
        float[] rad = new float[8];
        double[] radd = new double[8];

        double[] luas = new double[8];
        double[] luass = new double[8];
        double[] jarak = new double[8];
        double[] jarakk = new double[8];

        

        string method = "";
        bool cameraSel = false;

        bool status = false;

        public Main()
        {
            InitializeComponent();
        }

        public void readIndex()
        {
            listCameraData = new List<KeyValuePair<int, string>>();
            DsDevice[] systemCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            int deviceIndex = 0;
            foreach (DirectShowLib.DsDevice systemCamera in systemCameras)
            {
                listCameraData.Add(new KeyValuePair<int, string>(deviceIndex, systemCamera.Name));
                cameraListMenu.DropDownItems.Add(systemCamera.Name + " - (" + deviceIndex + ")", null, indexSelect);
                deviceIndex++;
            }
        }

        private void indexSelect(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            try
            {
                cameraIndex = cameraListMenu.DropDownItems.IndexOf(menu);
                menu.Checked = true;
                cameraSel = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void resolutionSelect(object sender, EventArgs e)
        {
            x256Res.Checked = false;
            x480Res.Checked = false;
            x780Res.Checked = false;
            x912Res.Checked = false;
            x1200Res.Checked = false;

            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            menu.Checked = true;
            string[] res = menu.ToString().Split('x');
            width = Convert.ToDouble(res[0]);
            height = Convert.ToDouble(res[1]);
        }

        private void methodeCelect(object sender, EventArgs e)
        {
            houghCircleToolStripMenuItem.Checked = false;
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            menu.Checked = true;
            method = menu.ToString();
            groupBox3.Text = method;
            groupBox7.Text = " Parameter " + method;
        }

        private void processFrame(object sender, EventArgs e)
        {
            cap.Retrieve(frame, 0);

            Mat convertClr = new Mat();
            Mat gBlur = new Mat();

            CvInvoke.CvtColor(frame, convertClr, ColorConversion.Bgr2Gray);

            Mat copy = frame.Clone();
            
            CvInvoke.GaussianBlur(convertClr, gBlur, new Size(gb, gb), 0, 0);
            
            CircleF[] circles = CvInvoke.HoughCircles(gBlur, HoughType.Gradient, minRad, maxRad, lowThr, accThr, ov);
            
            int i = 0;

            foreach (CircleF circle in circles)
            {
                rad[i] = circle.Radius;
                radd[i] = Math.Round(rad[i]/2, 0);
                //luas[i] = circle.Area / 4;
                luas[i] = 3.14 * radd[i] * radd[i];
                luass[i] = Math.Round(luas[i], 0);
                CvInvoke.Circle(copy, new Point((int)circle.Center.X, (int)circle.Center.Y), radiusThr, new MCvScalar(blue, green, red), thick, LineType.EightConnected, 0);
                hitung++;
                i++;
            }

            label21.Invoke((MethodInvoker)(() => label21.Text = hitung.ToString()));
            hitung = 0;
            status = true;
            
            imageBox1.Image = frame;
            imageBox2.Image = convertClr;
            imageBox3.Image = copy;
            imageBox4.Image = gBlur;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            domainUpDown1.Text = "9";
            gb = Convert.ToInt32(domainUpDown1.Text);

            readIndex();

            startBtn.Enabled = true;
            pauseBtn.Enabled = false;
            stopBtn.Enabled = false;
            viewCodeBtn.Enabled = false;
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (width == 0 && height == 0)
            {
                MessageBox.Show("Pilih resolusi kamera yang diinginkan!");
            }
            if (method == "")
            {
                MessageBox.Show("Pilih metode deteksi yang diinginkan!");
            }
            if (cameraSel == false)
            {
                MessageBox.Show("Pilih kamera yang akan digunakan!");
            }

            if (width != 0 && height != 0 && method != "" && cameraSel != false)
            {
                cap = new Capture(cameraIndex);
                cap.SetCaptureProperty(CapProp.FrameWidth, width);
                cap.SetCaptureProperty(CapProp.FrameHeight, height);
                cap.ImageGrabbed += processFrame;
                cap.Start();
                timer1.Start();
                startBtn.Enabled = false;
                pauseBtn.Enabled = true;
                stopBtn.Enabled = true;
                viewCodeBtn.Enabled = true;
            }
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            cap.Pause();
            timer1.Stop();
            startBtn.Enabled = true;
            pauseBtn.Enabled = false;
            stopBtn.Enabled = false;
        }

        private void exitMenu_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
            this.Close();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(pictureBox1, "Gaussian Blur hanya bisa dengan angka ganjil, minimal bernilai 3 \n Tips : turunkan Gaussian Blur, naikkan Higher Threshold");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lowThr = trackThr.Value;
            accThr = accThrBar.Value;
            radiusThr = radiusBar.Value;
            thick = thickBar.Value;
            minRad = minRadiusBar.Value;
            maxRad = maxRadiusBar.Value;
            blue = blueBar.Value;
            green = greenBar.Value;
            red = redBar.Value;
            gb = Convert.ToInt32(domainUpDown1.Text);
            ov = ovBar.Value;

            label3.Text = lowThr.ToString();
            label4.Text = accThr.ToString();
            label7.Text = radiusThr.ToString();
            label9.Text = thick.ToString();
            label12.Text = minRad.ToString();
            label13.Text = maxRad.ToString();
            label15.Text = blue.ToString();
            label17.Text = green.ToString();
            label19.Text = red.ToString();
            label1.Text = ov.ToString();

            
           
            try
            {
                    label53.Text = radd[0].ToString();
                    label52.Text = radd[1].ToString();
                    label51.Text = radd[2].ToString();
                    label50.Text = radd[3].ToString();
                    label57.Text = radd[4].ToString();
                    label56.Text = radd[5].ToString();
                    label55.Text = radd[6].ToString();
                    label54.Text = radd[7].ToString();
                    label46.Text = luass[0].ToString();
                    label47.Text = luass[1].ToString();
                    label48.Text = luass[2].ToString();
                    label49.Text = luass[3].ToString();
                    label61.Text = luass[4].ToString();
                    label60.Text = luass[5].ToString();
                    label59.Text = luass[6].ToString();
                    label58.Text = luass[7].ToString();
                    
            }
            catch (Exception ex) { }

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    rad[i] = 0;
                    radd[i] = 0;
                    luas[i] = 0;
                    luass[i] = 0;
                }
            }
            catch (Exception ex) { }
            if (status == true)
            {
                hitung += 0;
            }
            else
            {
                hitung = 0;
            }   
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void viewCodeBtn_Click(object sender, EventArgs e)
        {
            viewCode tampil = new viewCode();
            tampil.Show();
        }

        private void aboutMenu_Click(object sender, EventArgs e)
        {
            about tampil2 = new about();
            tampil2.Show();
        }

        private void instructionMenu_Click_1(object sender, EventArgs e)
        {
            instruction tampil3 = new instruction();
            tampil3.Show();
        }
    }
}