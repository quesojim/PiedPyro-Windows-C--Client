//Author: Colby J. Conkling
//Copyright 2016. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Renci.SshNet;
using System.Text.RegularExpressions;
using System.IO;
using WMPLib;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PIroTech
{
    public partial class Main : Form
    {
        public Main()
        {
      
            InitializeComponent();
            StartClock();
            log.AppendText(DateTime.Now.ToString("hh:mm:ss tt") + "  System Started.  Welcome to Pied Pyro!\r");
            StartSequenceButton.Enabled = false;
            StopSequenceButton.Enabled = false;
            TestAudioButton.Visible = true;
            stopTestbutton.Visible = false;
            disarm_button.Visible = false;
            arm_system_button.Visible = false;
            arm_system_button.Enabled = false;
            Control.CheckForIllegalCrossThreadCalls = false;
            //FormBorderStyle = FormBorderStyle.None;
            //WindowState = FormWindowState.Maximized;

        }

        public string IP;
        public string inputTo;
        public string outputFrom;
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password = "pied";
        public static SshClient client { get; set; }
        public ShellStream shellStream { get; set; }
        public string filepath;
        public string audiofilepath;
        public Boolean SystemArmed = false;
        public int NumberofModulesConnected = 0;
        public long startTime = 0;
        WindowsMediaPlayer wplayer = new WindowsMediaPlayer();
        public long startTimeMS = 0;
        public long startTimeS = 0;
        public long startTimeM = 0;
        public long microseconds = 0;
        public long seconds = 0;
        public int minutes = 0;
        public System.Windows.Forms.Timer t;
        public int currentCueinList = 0;
        public bool FireCue = true;
        int NumberofCues = 0;
        string[] FiringTimes;
        bool LastCueFired = false;
        bool audionotloaded = true;
        bool ASelected = false;
        bool BSelected = false;
        bool CSelected = false;
        bool DSelected = false;
        bool ESelected = false;
        PictureBox[] ABoxes = new PictureBox[16];
        PictureBox[] BBoxes = new PictureBox[16];
        PictureBox[] CBoxes = new PictureBox[16];
        PictureBox[] DBoxes = new PictureBox[16];
        PictureBox[] EBoxes = new PictureBox[16];
        bool[] ANotSelected = new bool[16];
        bool[] BNotSelected = new bool[16];
        bool[] CNotSelected = new bool[16];
        bool[] DNotSelected = new bool[16];
        bool[] ENotSelected = new bool[16];
        Image OneSelected = PiedPyro.Properties.Resources._1selected;
        Image One = PiedPyro.Properties.Resources._1;
        Image TwoSelected = PiedPyro.Properties.Resources._2selected;
        Image Two = PiedPyro.Properties.Resources._2;
        Image ThreeSelected = PiedPyro.Properties.Resources._3selected;
        Image Three = PiedPyro.Properties.Resources._3;
        Image FourSelected = PiedPyro.Properties.Resources._4selected;
        Image Four = PiedPyro.Properties.Resources._4;
        Image FiveSelected = PiedPyro.Properties.Resources._5selected;
        Image Five = PiedPyro.Properties.Resources._5;
        Image SixSelected = PiedPyro.Properties.Resources._6selected;
        Image Six = PiedPyro.Properties.Resources._6;
        Image SevenSelected = PiedPyro.Properties.Resources._7selected;
        Image Seven = PiedPyro.Properties.Resources._7;
        Image EightSelected = PiedPyro.Properties.Resources._8selected;
        Image Eight = PiedPyro.Properties.Resources._8;
        Image NineSelected = PiedPyro.Properties.Resources._9selected;
        Image Nine = PiedPyro.Properties.Resources._9;
        Image TenSelected = PiedPyro.Properties.Resources._10selected;
        Image Ten = PiedPyro.Properties.Resources._10;
        Image ElevenSelected = PiedPyro.Properties.Resources._11selected;
        Image Eleven = PiedPyro.Properties.Resources._11;
        Image TwelveSelected = PiedPyro.Properties.Resources._12selected;
        Image Twelve = PiedPyro.Properties.Resources._12;
        Image ThirteenSelected = PiedPyro.Properties.Resources._13selected;
        Image Thirteen = PiedPyro.Properties.Resources._13;
        Image FourteenSelected = PiedPyro.Properties.Resources._14selected;
        Image Fourteen = PiedPyro.Properties.Resources._14;
        Image FifteenSelected = PiedPyro.Properties.Resources._15selected;
        Image Fifteen = PiedPyro.Properties.Resources._15;
        Image SixteenSelected = PiedPyro.Properties.Resources._16selected;
        Image Sixteen = PiedPyro.Properties.Resources._16;
        bool ArmSound = true;


        private void Main_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                ANotSelected[i] = true;
                BNotSelected[i] = true;
                CNotSelected[i] = true;
                DNotSelected[i] = true;
                ENotSelected[i] = true;
            }
            openFileDialog2.Filter = "All Supported Audio | *.mp3; *.wma, *.wav; *.m4a; | MP3 | *.mp3 | WMA | *.wma | WAV | *.wav | M4A | *.m4a";
            openFileDialog2.FilterIndex = 1;
            ManualFireButton.Enabled = false;
            addmodulebutton.Select();
            TestAudioButton.Enabled = false;


        }




        System.Windows.Forms.Timer ti = null;
        private void StartClock()
        {
            ti = new System.Windows.Forms.Timer();
            ti.Interval = 1000;
            ti.Tick += new EventHandler(t_Tick);
            ti.Enabled = true;
        }

        void t_Tick(object sender, EventArgs e)
        {
            TimeClock.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            filepath = openFileDialog1.FileName;
            readCSV();
        }

        private void openCSVFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV Script Files | *.csv;";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.ShowDialog();
        }

        public void connectToModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addModule();

        }

    

//Read CSV file into array and input array values into DataGridView

        public void readCSV()
        {
            arm_system_button.Enabled = true;
            StreamReader sr = new StreamReader(filepath);
            var lines = new List<string[]>();
            int Row = 0;
            while (!sr.EndOfStream)
            {
                string[] Line = sr.ReadLine().Split(',');
                lines.Add(Line);
                Row++;
            }
  
           
            //Put values into array
            var data = lines.ToArray();
            log.AppendText(DateTime.Now.ToString("hh:mm:ss tt") + "  Loaded Script File: " + Path.GetFileName(filepath) + "\r");
            //ScriptFileName.Text = "Script File: " + Path.GetFileName(filepath);

            //Construct datagrid and input array values
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView1.ColumnCount = 5;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns[0].Name = "Module #";
            dataGridView1.Columns[0].Width = (60);
            dataGridView1.Columns[1].Name = "Queue #";
            dataGridView1.Columns[1].Width = (60);
            dataGridView1.Columns[2].Name = "Product Name";
            dataGridView1.Columns[2].Width = (167);
            dataGridView1.Columns[3].Name = "Product Description";
            dataGridView1.Columns[3].Width = (210);
            dataGridView1.Columns[4].Name = "Firing Time";
            dataGridView1.Columns[4].Width = (71);
            for (int x = 1; x < Row; x++)
            {
                string[] rowAdd = new string[] { data[x][0], data[x][1], data[x][2], data[x][3], data[x][4] };
                dataGridView1.Rows.Add(rowAdd);

            }
            NumberofCues = Row - 1;
            FiringTimes = new string[Row];


            for (int x = 1; x < Row; x++)
            { 
                String minute = (data[x][4]).Substring(0, 2);
                String second = (data[x][4]).Substring(3, 2);
                String ms = (data[x][4]).Substring(6, 2);
                String strVal = (minute + second + ms);
                FiringTimes[x-1] = strVal;
                Console.WriteLine(FiringTimes[x-1]);

            }
  
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.MultiSelect = false;
            dataGridView1.Rows[0].Selected = true;
            ScriptLoadedLabel.Visible = false;
            if (audionotloaded)
            {
                askforaudio();
            }
        }

        private void askforaudio()
        {
            DialogResult result = MessageBox.Show("Would you like to load an audio file as well?", "Load Audio File?", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            { 

                openFileDialog2.ShowDialog();
            }
            else if (result == DialogResult.No)
            {
                return;
            }
        }
     
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutPIroTechToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Interaction.MsgBox("Pied Pyro is an advanced pyrotechnics firing system intended to be built by DIYers and hobbyists based on the low cost Raspberry Pi computer.  It is an open source project created by Colby J. Conkling and continues to be updated.  More information can be found at piedpyro.tech ");

        }


        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void runbutton_Click(object sender, EventArgs e)
        {
            
        }

        private void ProgressBar_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            readCSV();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }


        private void StartShowClock()
        {
            startTimeMS = startTime;
            startTimeS = startTime;
            startTimeM = startTime;
            t = new System.Windows.Forms.Timer();
            t.Interval = 1;
            t.Tick += new EventHandler(s_Tick);
            t.Enabled = true;

        }

        void s_Tick(object sender, EventArgs e)
        {
            StartSequenceButton.Visible = false;
            long time_elapsed_MS = (DateTime.Now.Ticks - startTimeMS);
            long time_elapsed_S = (DateTime.Now.Ticks - startTimeS);


            microseconds = ((time_elapsed_MS / TimeSpan.TicksPerMillisecond) / 10);
            seconds = ((time_elapsed_S / TimeSpan.TicksPerMillisecond) / 1000);
            if (microseconds > 99)
            {
                microseconds = 0;
                startTimeMS = DateTime.Now.Ticks;

            }
            if (seconds > 59)
            {
                seconds = 0;
                startTimeS = DateTime.Now.Ticks;
                minutes++;

            }
            String msText = microseconds.ToString("00");
            String sText = seconds.ToString("00");
            String mText = minutes.ToString("00");
            ShowTimeLabel.Text = (mText + ":" + sText + ":" + msText);
            int currentFiringTime = Int32.Parse(minutes.ToString("00") + seconds.ToString("00") + microseconds.ToString("00"));
            
            if (currentCueinList < NumberofCues)
            {
                int cueToFire = Int32.Parse(FiringTimes[currentCueinList]);

                if (cueToFire == currentFiringTime || cueToFire == currentFiringTime++ || cueToFire == currentFiringTime-- || cueToFire == 0  || cueToFire == currentFiringTime-2 || cueToFire == currentFiringTime+2)
                {
                    if (FireCue == true) {
                        Task.Run(() => {
                            currentCueinList++;
                            log.AppendText(DateTime.Now.ToString("hh:mm:ss tt") + "  Fired Queue " + currentCueinList + " at " + (mText + ":" + sText + ":" + msText) + "\r");
                            String response = SendData("Q" + currentCueinList);
                            String output = SendData("q" + currentCueinList + "fire");
                            Console.WriteLine(output);
                        });

                        FireCue = false;
                        nextCue();
                    }
                }

            }
            else
            {
                if (LastCueFired == false)
                {
                    log.AppendText(NumberofCues.ToString());
                    log.AppendText(DateTime.Now.ToString("hh:mm:ss tt") + "  Finished Script.  \r");
                    LastCueFired = true;
                }
            }


        }

        public void nextCue()
        {
            FireCue = true;
        }


       
  
        private void AddModuleButton_Click_1(object sender, EventArgs e)
        {
            addModule();
            
        }

//Adding a module into table and attempting shell connection

        public void addModule()
        {
            
            IP = Interaction.InputBox("Enter IP Address of Module", "Enter IP Address", "192.168.2.2");
            if (IP == " ")
            {
                MessageBox.Show("No IP address entered!", "No IP Address Entered", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (IP == "")
            {
                return;
            }

            toolStripStatusLabel1.Text = ("Connecting to module...");

            String response = SendData("Test");
            if (response == "Connected")
            {
                NumberofModulesConnected++;
                MessageBox.Show("Module " + NumberofModulesConnected + " Connected", "Connection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DialogResult result = MessageBox.Show("Connection Error.  Check IP Address.", "Connection Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Retry)
                {
                    addModule();
                    
                }
                else
                {
                    return;
                }
                return;
            }

            dataGridView2.Rows.Clear();
            dataGridView2.Refresh();
            dataGridView2.ColumnCount = 4;
            dataGridView2.ReadOnly = true;
            dataGridView2.Columns[0].Name = "Module #";
            dataGridView2.Columns[0].Width = (60);
            dataGridView2.Columns[1].Name = "IP Address";
            dataGridView2.Columns[1].Width = (100);
            dataGridView2.Columns[2].Name = "Connection Quality";
            dataGridView2.Columns[2].Width = (120);
            dataGridView2.Columns[3].Name = "Cues";
            dataGridView2.Columns[3].Width = (78);
            string[] row = new string[] { "1", IP, "Ethernet", "80" };
            dataGridView2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Rows.Add(row);
            NumberofModulesConnected++;
            arm_system_button.Enabled = true;
            arm_system_button.Visible = true;
            coverpanel.Visible = false;       

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Cancel running sequence?", "Are you sure?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                e.Cancel = true;
            }

            else e.Cancel = false;
        }


        private void openAudioFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        public void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            audiofilepath = openFileDialog2.FileName;
            wplayer.URL = audiofilepath;
            wplayer.controls.stop();
            log.AppendText(DateTime.Now.ToString("hh:mm:ss tt") + "  Loaded Audio File  " + Path.GetFileName(audiofilepath) + "\r");
            audionotloaded = false;
            TestAudioButton.Enabled = true;       

        }

        private void ManualFireButton_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }


        private void addmodulebutton_Click(object sender, EventArgs e)
        {
            addModule();
        }

        private void LoadAudioButton_Click(object sender, EventArgs e)
        {

            openFileDialog2.ShowDialog();

        }

        private void LoadScriptButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV Script Files | *.csv; ";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            TestAudioButton.Visible = false;
            wplayer.controls.play();
            stopTestbutton.Visible = true;

        }

        private void stopTestbutton_Click(object sender, EventArgs e)
        {
            TestAudioButton.Visible = true;
            stopTestbutton.Visible = false;
            wplayer.controls.stop();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
                disarm_button.Enabled = false;
                StopSequenceButton.Visible = true;
                TestAudioButton.Enabled = false;
                StartSequenceButton.Visible = false;
                StartSequenceButton.Enabled = false;
                StopSequenceButton.Enabled = true;
                log.AppendText(DateTime.Now.ToString("hh:mm:ss tt") + "  Firing Sequence...\r");
                startTime = DateTime.Now.Ticks;
                minutes = 0;
                wplayer.controls.play();
                StartShowClock();
            
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F1))
            {
                MessageBox.Show("F1 Pressed.");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Cancel running sequence?", "Are you sure?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                disarm_button.Enabled = true;
                TestAudioButton.Enabled = true;
                StopSequenceButton.Visible = false;
                StartSequenceButton.Visible = true;
                StartSequenceButton.Enabled = true;
                t.Stop();
                ShowTimeLabel.Text = "00:00:00";
                currentCueinList = 0;
                wplayer.controls.stop();
            }
            else
            {
                return;
            } 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            StartShowClock();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            wplayer.settings.volume = trackBar1.Value;

        }

        private void arm_system_button_Click(object sender, EventArgs e)
        {
            if (NumberofModulesConnected > 0)
            {
                string ArmPassword = Interaction.InputBox("Enter password to Arm System ", "Enter Password");

                if (ArmPassword == Password)
                {
                    if (ArmSound == true)
                    {
                        Console.Beep(1200, 1000);
                        Thread.Sleep(300);
                        Console.Beep(1200, 1000);
                        Thread.Sleep(300);
                        Console.Beep(1200, 1000);
                    }
                    panel1.BackColor = Color.Red;
                    StartSequenceButton.Enabled = true;
                    SystemArmed = true;
                    toolStripStatusLabel1.Text = ("System Status: ARMED");
                    StatusStrip.BackColor = Color.Red;
                    arm_system_button.Visible = false;
                    disarm_button.Visible = true;
                    
                }
                else
                {
                    MessageBox.Show("Password Incorrect.  Please try again.", "Cannot Arm System!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            
        }

        private void disarm_button_Click(object sender, EventArgs e)
        {
            SystemArmed = false;
            disarm_button.Visible = false;
            arm_system_button.Visible = true;
            StartSequenceButton.Enabled = false;
            panel1.BackColor = Control.DefaultBackColor;
            toolStripStatusLabel1.Text = "System Status: SAFE";
            StatusStrip.BackColor = Color.Lime;


        }

    

        private void TestAudioButton_Click(object sender, EventArgs e)
        {
            TestAudioButton.Visible = false;
            stopTestbutton.Visible = true;
            wplayer.controls.play();
        }

        private void stopTestbutton_Click_1(object sender, EventArgs e)
        {
            stopTestbutton.Visible = false;
            TestAudioButton.Visible = true;
            wplayer.controls.stop();
        }

        private void loadAudioFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void changeBackgroundImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileImage.ShowDialog();
        }

        private void openFileImage_FileOk(object sender, CancelEventArgs e)
        {
            Image myimage = new Bitmap(openFileImage.FileName);
            this.BackgroundImage = myimage;
        }

        private void connectingToAModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Interaction.MsgBox("Visit www.piedpyro.tech for information on connecting to a module.");
        }

        private void a1_Click(object sender, EventArgs e)
        {
            if (ANotSelected[0])
            {
                a1.Image = OneSelected;
                ANotSelected[0] = false;
            }
            else
            {
                a1.Image = One;
                ANotSelected[0] = true;
            }
        }

        private void noBordersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            startTime = DateTime.Now.Ticks;
            minutes = 0;
            StartShowClock();
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            startTime = DateTime.Now.Ticks;
            minutes = 0;
            StartShowClock();
        }

        private void SecondsLabel_Click_1(object sender, EventArgs e)
        {

        }

        private void ShowTimeLabel_Click(object sender, EventArgs e)
        {

        }

        private void toolStripDropDownButton3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }


        private void log_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox32_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox18_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox19_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox20_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox21_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox22_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox23_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox24_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox25_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox26_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox27_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox28_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox29_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox30_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox31_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox17_Click_1(object sender, EventArgs e)
        {

        }

        private void label3_Click_2(object sender, EventArgs e)
        {

        }

        private void label6_Click_1(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click_3(object sender, EventArgs e)
        {

        }

        private void label6_Click_2(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }


        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_3(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {


        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripDropDownButton2_Click(object sender, EventArgs e)
        {
           
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripLabel1_Click_1(object sender, EventArgs e)
        {

        }

        private void maximizeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (ASelected == false)
            {
                
                a1.Image = OneSelected;
                ANotSelected[0] = false;
                a2.Image = TwoSelected;
                ANotSelected[1] = false;
                a3.Image = ThreeSelected;
                ANotSelected[2] = false;
                a4.Image = FourSelected;
                ANotSelected[3] = false;
                a5.Image = FiveSelected;
                ANotSelected[4] = false;
                a6.Image = SixSelected;
                ANotSelected[5] = false;
                a7.Image = SevenSelected;
                ANotSelected[6] = false;
                a8.Image = EightSelected;
                ANotSelected[7] = false;
                a9.Image = NineSelected;
                ANotSelected[8] = false;
                a10.Image = TenSelected;
                ANotSelected[9] = false;
                a11.Image = ElevenSelected;
                ANotSelected[10] = false;
                a12.Image = TwelveSelected;
                ANotSelected[11] = false;
                a13.Image = ThirteenSelected;
                ANotSelected[12] = false;
                a14.Image = FourteenSelected;
                ANotSelected[13] = false;
                a15.Image = FifteenSelected;
                ANotSelected[14] = false;
                a16.Image = SixteenSelected;
                ANotSelected[15] = false;
                ASelected = true;
            }

            else
            {
                ASelected = false;

                a1.Image = One;
                ANotSelected[0] = true;
                a2.Image = Two;
                ANotSelected[1] = true;
                a3.Image = Three;
                ANotSelected[2] = true;
                a4.Image = Four;
                ANotSelected[3] = true;
                a5.Image = Five;
                ANotSelected[4] = true;
                a6.Image = Six;
                ANotSelected[5] = true;
                a7.Image = Seven;
                ANotSelected[6] = true;
                a8.Image = Eight;
                ANotSelected[7] = true;
                a9.Image = Nine;
                ANotSelected[8] = true;
                a10.Image = Ten;
                ANotSelected[9] = true;
                a11.Image = Eleven;
                ANotSelected[10] = true;
                a12.Image = Twelve;
                ANotSelected[11] = true;
                a13.Image = Thirteen;
                ANotSelected[12] = true;
                a14.Image = Fourteen;
                ANotSelected[13] = true;
                a15.Image = Fifteen;
                ANotSelected[14] = true;
                a16.Image = Sixteen;
                ANotSelected[15] = true;

            }


        }

        private void a2_Click(object sender, EventArgs e)
        {
            if (ANotSelected[1])
            {
                a2.Image = TwoSelected;
                ANotSelected[1] = false;
            }
            else
            {
                a2.Image = Two;
                ANotSelected[1] = true;
            }
        }

        private void a3_Click(object sender, EventArgs e)
        {
            if (ANotSelected[2])
            {
                a3.Image = ThreeSelected;
                ANotSelected[2] = false;
            }
            else
            {
                a3.Image = Three;
                ANotSelected[2] = true;
            }
        }

        private void a4_Click(object sender, EventArgs e)
        {
            if (ANotSelected[3])
            {
                a4.Image = FourSelected;
                ANotSelected[3] = false;
            }
            else
            {
                a4.Image = Four;
                ANotSelected[3] = true;
            }
        }

        private void a5_Click(object sender, EventArgs e)
        {
            if (ANotSelected[4])
            {
                a5.Image = FiveSelected;
                ANotSelected[4] = false;
            }
            else
            {
                a5.Image = Five;
                ANotSelected[4] = true;
            }
        }

        private void a6_Click(object sender, EventArgs e)
        {
            if (ANotSelected[5])
            {
                ANotSelected[5] = false;
                a6.Image = SixSelected;
            }
            else
            {
                a6.Image = Six;
                ANotSelected[5] = true;
            }
        }

        private void a7_Click(object sender, EventArgs e)
        {
            if (ANotSelected[6])
            {
                ANotSelected[6] = false;
                a7.Image = SevenSelected;
            }
            else
            {
                a7.Image = Seven;
                ANotSelected[6] = true;
            }
        }

        private void a8_Click(object sender, EventArgs e)
        {
            if (ANotSelected[7])
            {
                ANotSelected[7] = false;
                a8.Image = EightSelected;
            }
            else
            {
                a8.Image = Eight;
                ANotSelected[7] = true;
            }
        }

        private void a9_Click(object sender, EventArgs e)
        {
            if (ANotSelected[8])
            {
                ANotSelected[8] = false;
                a9.Image = NineSelected;
            }
            else
            {
                a9.Image = Nine;
                ANotSelected[8] = true;
            }
        }

        private void a10_Click(object sender, EventArgs e)
        {
            if (ANotSelected[9])
            {
                ANotSelected[9] = false;
                a10.Image = TenSelected;
            }
            else
            {
                a10.Image = Ten;
                ANotSelected[9] = true;
            }
        }

        private void a11_Click(object sender, EventArgs e)
        {
            if (ANotSelected[10])
            {
                ANotSelected[10] = false;
                a11.Image = ElevenSelected;
            }
            else
            {
                a11.Image = Eleven;
                ANotSelected[10] = true;
            }
        }

        private void a12_Click(object sender, EventArgs e)
        {
            if (ANotSelected[11])
            {
                ANotSelected[11] = false;
                a12.Image = TwelveSelected;
            }
            else
            {
                a12.Image = Twelve;
                ANotSelected[11] = true;
            }
        }

        private void a13_Click(object sender, EventArgs e)
        {
            if (ANotSelected[12])
            {
                ANotSelected[12] = false;
                a13.Image = ThirteenSelected;
            }
            else
            {
                a13.Image = Thirteen;
                ANotSelected[12] = true;
            }
        }

        private void a14_Click(object sender, EventArgs e)
        {
            if (ANotSelected[13])
            {
                ANotSelected[13] = false;
                a14.Image = FourteenSelected;
            }
            else
            {
                a14.Image = Fourteen;
                ANotSelected[13] = true;
            }
        }

        private void a15_Click(object sender, EventArgs e)
        {
            if (ANotSelected[14])
            {
                ANotSelected[14] = false;
                a15.Image = FifteenSelected;
            }
            else
            {
                a15.Image = Fifteen;
                ANotSelected[14] = true;
            }
        }

        private void a16_Click(object sender, EventArgs e)
        {
            if (ANotSelected[15])
            {
                ANotSelected[15] = false;
                a16.Image = SixteenSelected;
            }
            else
            {
                a16.Image = Sixteen;
                ANotSelected[15] = true;
            }
        }

        private void BChecked_CheckedChanged(object sender, EventArgs e)
        {
            if (BSelected == false)
            {

                b1.Image = OneSelected;
                BNotSelected[0] = false;
                b2.Image = TwoSelected;
                BNotSelected[1] = false;
                b3.Image = ThreeSelected;
                BNotSelected[2] = false;
                b4.Image = FourSelected;
                BNotSelected[3] = false;
                b5.Image = FiveSelected;
                BNotSelected[4] = false;
                b6.Image = SixSelected;
                BNotSelected[5] = false;
                b7.Image = SevenSelected;
                BNotSelected[6] = false;
                b8.Image = EightSelected;
                BNotSelected[7] = false;
                b9.Image = NineSelected;
                BNotSelected[8] = false;
                b10.Image = TenSelected;
                BNotSelected[9] = false;
                b11.Image = ElevenSelected;
                BNotSelected[10] = false;
                b12.Image = TwelveSelected;
                BNotSelected[11] = false;
                b13.Image = ThirteenSelected;
                BNotSelected[12] = false;
                b14.Image = FourteenSelected;
                BNotSelected[13] = false;
                b15.Image = FifteenSelected;
                BNotSelected[14] = false;
                b16.Image = SixteenSelected;
                BNotSelected[15] = false;
                BSelected = true;
            }

            else
            {
                BSelected = false;

                b1.Image = One;
                BNotSelected[0] = true;
                b2.Image = Two;
                BNotSelected[1] = true;
                b3.Image = Three;
                BNotSelected[2] = true;
                b4.Image = Four;
                BNotSelected[3] = true;
                b5.Image = Five;
                BNotSelected[4] = true;
                b6.Image = Six;
                BNotSelected[5] = true;
                b7.Image = Seven;
                BNotSelected[6] = true;
                b8.Image = Eight;
                BNotSelected[7] = true;
                b9.Image = Nine;
                BNotSelected[8] = true;
                b10.Image = Ten;
                BNotSelected[9] = true;
                b11.Image = Eleven;
                BNotSelected[10] = true;
                b12.Image = Twelve;
                BNotSelected[11] = true;
                b13.Image = Thirteen;
                BNotSelected[12] = true;
                b14.Image = Fourteen;
                BNotSelected[13] = true;
                b15.Image = Fifteen;
                BNotSelected[14] = true;
                b16.Image = Sixteen;
                BNotSelected[15] = true;

            }
        }

        private void CChecked_CheckedChanged(object sender, EventArgs e)
        {
            if (CSelected == false)
            {

                c1.Image = OneSelected;
                CNotSelected[0] = false;
                c2.Image = TwoSelected;
                CNotSelected[1] = false;
                c3.Image = ThreeSelected;
                CNotSelected[2] = false;
                c4.Image = FourSelected;
                CNotSelected[3] = false;
                c5.Image = FiveSelected;
                CNotSelected[4] = false;
                c6.Image = SixSelected;
                CNotSelected[5] = false;
                c7.Image = SevenSelected;
                CNotSelected[6] = false;
                c8.Image = EightSelected;
                CNotSelected[7] = false;
                c9.Image = NineSelected;
                CNotSelected[8] = false;
                c10.Image = TenSelected;
                CNotSelected[9] = false;
                c11.Image = ElevenSelected;
                CNotSelected[10] = false;
                c12.Image = TwelveSelected;
                CNotSelected[11] = false;
                c13.Image = ThirteenSelected;
                CNotSelected[12] = false;
                c14.Image = FourteenSelected;
                CNotSelected[13] = false;
                c15.Image = FifteenSelected;
                CNotSelected[14] = false;
                c16.Image = SixteenSelected;
                CNotSelected[15] = false;
                CSelected = true;
            }

            else
            {
                CSelected = false;

                c1.Image = One;
                CNotSelected[0] = true;
                c2.Image = Two;
                CNotSelected[1] = true;
                c3.Image = Three;
                CNotSelected[2] = true;
                c4.Image = Four;
                CNotSelected[3] = true;
                c5.Image = Five;
                CNotSelected[4] = true;
                c6.Image = Six;
                CNotSelected[5] = true;
                c7.Image = Seven;
                CNotSelected[6] = true;
                c8.Image = Eight;
                CNotSelected[7] = true;
                c9.Image = Nine;
                CNotSelected[8] = true;
                c10.Image = Ten;
                CNotSelected[9] = true;
                c11.Image = Eleven;
                CNotSelected[10] = true;
                c12.Image = Twelve;
                CNotSelected[11] = true;
                c13.Image = Thirteen;
                CNotSelected[12] = true;
                c14.Image = Fourteen;
                CNotSelected[13] = true;
                c15.Image = Fifteen;
                CNotSelected[14] = true;
                c16.Image = Sixteen;
                BNotSelected[15] = true;

            }
        }

        private void DChecked_CheckedChanged(object sender, EventArgs e)
        {
            if (DSelected == false)
            {

                d1.Image = OneSelected;
                DNotSelected[0] = false;
                d2.Image = TwoSelected;
                DNotSelected[1] = false;
                d3.Image = ThreeSelected;
                DNotSelected[2] = false;
                d4.Image = FourSelected;
                DNotSelected[3] = false;
                d5.Image = FiveSelected;
                DNotSelected[4] = false;
                d6.Image = SixSelected;
                DNotSelected[5] = false;
                d7.Image = SevenSelected;
                DNotSelected[6] = false;
                d8.Image = EightSelected;
                DNotSelected[7] = false;
                d9.Image = NineSelected;
                DNotSelected[8] = false;
                d10.Image = TenSelected;
                DNotSelected[9] = false;
                d11.Image = ElevenSelected;
                DNotSelected[10] = false;
                d12.Image = TwelveSelected;
                DNotSelected[11] = false;
                d13.Image = ThirteenSelected;
                DNotSelected[12] = false;
                d14.Image = FourteenSelected;
                DNotSelected[13] = false;
                d15.Image = FifteenSelected;
                DNotSelected[14] = false;
                d16.Image = SixteenSelected;
                DNotSelected[15] = false;
                DSelected = true;
            }

            else
            {
                DSelected = false;

                d1.Image = One;
                DNotSelected[0] = true;
                d2.Image = Two;
                DNotSelected[1] = true;
                d3.Image = Three;
                DNotSelected[2] = true;
                d4.Image = Four;
                DNotSelected[3] = true;
                d5.Image = Five;
                DNotSelected[4] = true;
                d6.Image = Six;
                DNotSelected[5] = true;
                d7.Image = Seven;
                DNotSelected[6] = true;
                d8.Image = Eight;
                DNotSelected[7] = true;
                d9.Image = Nine;
                DNotSelected[8] = true;
                d10.Image = Ten;
                DNotSelected[9] = true;
                d11.Image = Eleven;
                DNotSelected[10] = true;
                d12.Image = Twelve;
                DNotSelected[11] = true;
                d13.Image = Thirteen;
                DNotSelected[12] = true;
                d14.Image = Fourteen;
                DNotSelected[13] = true;
                d15.Image = Fifteen;
                DNotSelected[14] = true;
                d16.Image = Sixteen;
                DNotSelected[15] = true;

            }
        }

        private void EChecked_CheckedChanged(object sender, EventArgs e)
        {
            if (ESelected == false)
            {
            
                e1.Image = OneSelected;
                ENotSelected[0] = false;
                e2.Image = TwoSelected;
                ENotSelected[1] = false;
                e3.Image = ThreeSelected;
                ENotSelected[2] = false;
                e4.Image = FourSelected;
                ENotSelected[3] = false;
                e5.Image = FiveSelected;
                ENotSelected[4] = false;
                e6.Image = SixSelected;
                ENotSelected[5] = false;
                e7.Image = SevenSelected;
                ENotSelected[6] = false;
                e8.Image = EightSelected;
                ENotSelected[7] = false;
                e9.Image = NineSelected;
                ENotSelected[8] = false;
                e10.Image = TenSelected;
                ENotSelected[9] = false;
                e11.Image = ElevenSelected;
                ENotSelected[10] = false;
                e12.Image = TwelveSelected;
                ENotSelected[11] = false;
                e13.Image = ThirteenSelected;
                ENotSelected[12] = false;
                e14.Image = FourteenSelected;
                ENotSelected[13] = false;
                e15.Image = FifteenSelected;
                ENotSelected[14] = false;
                e16.Image = SixteenSelected;
                ENotSelected[15] = false;
                ESelected = true;
            }

            else
            {
                ESelected = false;
            
                e1.Image = One;
                ENotSelected[0] = true;
                e2.Image = Two;
                ENotSelected[1] = true;
                e3.Image = Three;
                ENotSelected[2] = true;
                e4.Image = Four;
                ENotSelected[3] = true;
                e5.Image = Five;
                ENotSelected[4] = true;
                e6.Image = Six;
                ENotSelected[5] = true;
                e7.Image = Seven;
                ENotSelected[6] = true;
                e8.Image = Eight;
                ENotSelected[7] = true;
                e9.Image = Nine;
                ENotSelected[8] = true;
                e10.Image = Ten;
                ENotSelected[9] = true;
                e11.Image = Eleven;
                ENotSelected[10] = true;
                e12.Image = Twelve;
                ENotSelected[11] = true;
                e13.Image = Thirteen;
                ENotSelected[12] = true;
                e14.Image = Fourteen;
                ENotSelected[13] = true;
                e15.Image = Fifteen;
                ENotSelected[14] = true;
                e16.Image = Sixteen;
                ENotSelected[15] = true;

            }
        }

        private void b1_Click(object sender, EventArgs e)
        {
            if (BNotSelected[0])
            {
                b1.Image = OneSelected;
                BNotSelected[0] = false;
            }
            else
            {
                b1.Image = One;
                BNotSelected[0] = true;
            }
        }

        private void b2_Click(object sender, EventArgs e)
        {
            if (BNotSelected[1])
            {
                b2.Image = TwoSelected;
                BNotSelected[1] = false;
            }
            else
            {
                b2.Image = Two;
                BNotSelected[1] = true;
            }
        }

        private void b3_Click(object sender, EventArgs e)
        {
            if (BNotSelected[2])
            {
                b3.Image = ThreeSelected;
                BNotSelected[2] = false;
            }
            else
            {
                b3.Image = Three;
                BNotSelected[2] = true;
            }
        }

        private void b4_Click(object sender, EventArgs e)
        {
            if (BNotSelected[3])
            {
                b4.Image = FourSelected;
                BNotSelected[3] = false;
            }
            else
            {
                b4.Image = Four;
                BNotSelected[3] = true;
            }
        }

        private void b5_Click(object sender, EventArgs e)
        {
            if (BNotSelected[4])
            {
                b5.Image = FiveSelected;
                BNotSelected[4] = false;
            }
            else
            {
                b5.Image = Five;
                BNotSelected[4] = true;
            }
        }

        private void b6_Click(object sender, EventArgs e)
        {
            if (BNotSelected[5])
            {
                b6.Image = SixSelected;
                BNotSelected[5] = false;
            }
            else
            {
                b6.Image = Six;
                BNotSelected[5] = true;
            }
        }

        private void b7_Click(object sender, EventArgs e)
        {
            if (BNotSelected[6])
            {
                b7.Image = SevenSelected;
                BNotSelected[6] = false;
            }
            else
            {
                b7.Image = Seven;
                BNotSelected[6] = true;
            }
        }

        private void b8_Click(object sender, EventArgs e)
        {
            if (BNotSelected[7])
            {
                b8.Image = EightSelected;
                BNotSelected[7] = false;
            }
            else
            {
                b8.Image = Eight;
                BNotSelected[7] = true;
            }
        }

        private void b9_Click(object sender, EventArgs e)
        {
            if (BNotSelected[8])
            {
                b9.Image = NineSelected;
                BNotSelected[8] = false;
            }
            else
            {
                b9.Image = Nine;
                BNotSelected[8] = true;
            }
        }

        private void b10_Click(object sender, EventArgs e)
        {
            if (BNotSelected[9])
            {
                b10.Image = TenSelected;
                BNotSelected[9] = false;
            }
            else
            {
                b10.Image = Ten;
                BNotSelected[9] = true;
            }
        }

        private void b11_Click(object sender, EventArgs e)
        {
            if (BNotSelected[10])
            {
                b11.Image = ElevenSelected;
                BNotSelected[10] = false;
            }
            else
            {
                b11.Image = Eleven;
                BNotSelected[10] = true;
            }
        }

        private void b12_Click(object sender, EventArgs e)
        {
            if (BNotSelected[11])
            {
                b12.Image = TwelveSelected;
                BNotSelected[11] = false;
            }
            else
            {
                b12.Image = Twelve;
                BNotSelected[11] = true;
            }
        }

        private void b13_Click(object sender, EventArgs e)
        {
            if (BNotSelected[12])
            {
                b13.Image = ThirteenSelected;
                BNotSelected[12] = false;
            }
            else
            {
                b13.Image = Thirteen;
                BNotSelected[12] = true;
            }
        }

        private void b14_Click(object sender, EventArgs e)
        {
            if (BNotSelected[13])
            {
                b14.Image = FourteenSelected;
                BNotSelected[13] = false;
            }
            else
            {
                b14.Image = Fourteen;
                BNotSelected[13] = true;
            }
        }

        private void b15_Click(object sender, EventArgs e)
        {
            if (BNotSelected[14])
            {
                b15.Image = FifteenSelected;
                BNotSelected[14] = false;
            }
            else
            {
                b15.Image = Fifteen;
                BNotSelected[14] = true;
            }
        }

        private void b16_Click(object sender, EventArgs e)
        {
            if (BNotSelected[15])
            {
                b16.Image = SixteenSelected;
                BNotSelected[15] = false;
            }
            else
            {
                b16.Image = Sixteen;
                BNotSelected[15] = true;
            }
        }

        private void c1_Click(object sender, EventArgs e)
        {
            if (CNotSelected[0])
            {
                c1.Image = OneSelected;
                CNotSelected[0] = false;
            }
            else
            {
                c1.Image = One;
                CNotSelected[0] = true;
            }
        }

        private void c2_Click(object sender, EventArgs e)
        {
            if (CNotSelected[1])
            {
                c2.Image = TwoSelected;
                CNotSelected[1] = false;
            }
            else
            {
                c2.Image = Two;
                CNotSelected[1] = true;
            }
        }

        private void c3_Click(object sender, EventArgs e)
        {
            if (CNotSelected[2])
            {
                c3.Image = ThreeSelected;
                CNotSelected[2] = false;
            }
            else
            {
                c3.Image = Three;
                CNotSelected[2] = true;
            }
        }

        private void c4_Click(object sender, EventArgs e)
        {
            if (CNotSelected[3])
            {
                c4.Image = FourSelected;
                CNotSelected[3] = false;
            }
            else
            {
                c4.Image = Four;
                CNotSelected[3] = true;
            }
        }

        private void c5_Click(object sender, EventArgs e)
        {
            if (CNotSelected[4])
            {
                c5.Image = FiveSelected;
                CNotSelected[4] = false;
            }
            else
            {
                c5.Image = Five;
                CNotSelected[4] = true;
            }
        }

        private void c6_Click(object sender, EventArgs e)
        {
            if (CNotSelected[5])
            {
                c6.Image = SixSelected;
                CNotSelected[5] = false;
            }
            else
            {
                c6.Image = Six;
                CNotSelected[5] = true;
            }
        }

        private void c7_Click(object sender, EventArgs e)
        {
            if (CNotSelected[6])
            {
                c7.Image = SevenSelected;
                CNotSelected[6] = false;
            }
            else
            {
                c7.Image = Seven;
                CNotSelected[6] = true;
            }
        }

        private void c8_Click(object sender, EventArgs e)
        {
            if (CNotSelected[7])
            {
                c8.Image = EightSelected;
                CNotSelected[7] = false;
            }
            else
            {
                c8.Image = Eight;
                CNotSelected[7] = true;
            }
        }

        private void c9_Click(object sender, EventArgs e)
        {
            if (CNotSelected[8])
            {
                c9.Image = NineSelected;
                CNotSelected[8] = false;
            }
            else
            {
                c9.Image = Nine;
                CNotSelected[8] = true;
            }
        }

        private void c10_Click(object sender, EventArgs e)
        {
            if (CNotSelected[9])
            {
                c10.Image = TenSelected;
                CNotSelected[9] = false;
            }
            else
            {
                c10.Image = Ten;
                CNotSelected[9] = true;
            }
        }

        private void c11_Click(object sender, EventArgs e)
        {
            if (CNotSelected[10])
            {
                c11.Image = ElevenSelected;
                CNotSelected[10] = false;
            }
            else
            {
                c11.Image = Eleven;
                CNotSelected[10] = true;
            }
        }

        private void c12_Click(object sender, EventArgs e)
        {
            if (CNotSelected[11])
            {
                c12.Image = TwelveSelected;
                CNotSelected[11] = false;
            }
            else
            {
                c12.Image = Twelve;
                CNotSelected[11] = true;
            }
        }

        private void c13_Click(object sender, EventArgs e)
        {
            if (CNotSelected[12])
            {
                c13.Image = ThirteenSelected;
                CNotSelected[12] = false;
            }
            else
            {
                c13.Image = Thirteen;
                CNotSelected[12] = true;
            }
        }

        private void c14_Click(object sender, EventArgs e)
        {
            if (CNotSelected[13])
            {
                c14.Image = FourteenSelected;
                CNotSelected[13] = false;
            }
            else
            {
                c14.Image = Fourteen;
                CNotSelected[13] = true;
            }
        }

        private void c15_Click(object sender, EventArgs e)
        {
            if (CNotSelected[14])
            {
                c15.Image = FifteenSelected;
                CNotSelected[14] = false;
            }
            else
            {
                c15.Image = Fifteen;
                CNotSelected[14] = true;
            }
        }

        private void c16_Click(object sender, EventArgs e)
        {
            if (CNotSelected[15])
            {
                c16.Image = SixteenSelected;
                CNotSelected[15] = false;
            }
            else
            {
                c16.Image = Sixteen;
                CNotSelected[15] = true;
            }
        }

        private void d1_Click(object sender, EventArgs e)
        {
            if (DNotSelected[0])
            {
                d1.Image = OneSelected;
                DNotSelected[0] = false;
            }
            else
            {
                d1.Image = One;
                DNotSelected[0] = true;
            }
        }

        private void d2_Click(object sender, EventArgs e)
        {
            if (DNotSelected[1])
            {
                d2.Image = TwoSelected;
                DNotSelected[1] = false;
            }
            else
            {
                d2.Image = Two;
                DNotSelected[1] = true;
            }
        }

        private void d3_Click(object sender, EventArgs e)
        {
            if (DNotSelected[2])
            {
                d3.Image = ThreeSelected;
                DNotSelected[2] = false;
            }
            else
            {
                d3.Image = Three;
                DNotSelected[2] = true;
            }
        }

        private void d4_Click(object sender, EventArgs e)
        {
            if (DNotSelected[3])
            {
                d4.Image = FourSelected;
                DNotSelected[3] = false;
            }
            else
            {
                d4.Image = Four;
                DNotSelected[3] = true;
            }
        }

        private void d5_Click(object sender, EventArgs e)
        {
            if (DNotSelected[4])
            {
                d5.Image = FiveSelected;
                DNotSelected[4] = false;
            }
            else
            {
                d5.Image = Five;
                DNotSelected[4] = true;
            }
        }

        private void d6_Click(object sender, EventArgs e)
        {
            if (DNotSelected[5])
            {
                d6.Image = SixSelected;
                DNotSelected[5] = false;
            }
            else
            {
                d6.Image = Six;
                DNotSelected[5] = true;
            }
        }

        private void d7_Click(object sender, EventArgs e)
        {
            if (DNotSelected[6])
            {
                d7.Image = SevenSelected;
                DNotSelected[6] = false;
            }
            else
            {
                d7.Image = Seven;
                DNotSelected[6] = true;
            }
        }

        private void d8_Click(object sender, EventArgs e)
        {
            if (DNotSelected[7])
            {
                d8.Image = EightSelected;
                DNotSelected[7] = false;
            }
            else
            {
                d8.Image = Eight;
                DNotSelected[7] = true;
            }
        }

        private void d9_Click(object sender, EventArgs e)
        {
            if (DNotSelected[8])
            {
                d9.Image = NineSelected;
                DNotSelected[8] = false;
            }
            else
            {
                d9.Image = Nine;
                DNotSelected[8] = true;
            }
        }

        private void d10_Click(object sender, EventArgs e)
        {
            if (DNotSelected[9])
            {
                d10.Image = TenSelected;
                DNotSelected[9] = false;
            }
            else
            {
                d10.Image = Ten;
                DNotSelected[9] = true;
            }
        }

        private void d11_Click(object sender, EventArgs e)
        {
            if (DNotSelected[10])
            {
                d11.Image = ElevenSelected;
                DNotSelected[10] = false;
            }
            else
            {
                d11.Image = Eleven;
                DNotSelected[10] = true;
            }
        }

        private void d12_Click(object sender, EventArgs e)
        {
            if (DNotSelected[11])
            {
                d12.Image = TwelveSelected;
                DNotSelected[11] = false;
            }
            else
            {
                d12.Image = Twelve;
                DNotSelected[11] = true;
            }
        }

        private void d13_Click(object sender, EventArgs e)
        {
            if (DNotSelected[12])
            {
                d13.Image = ThirteenSelected;
                DNotSelected[12] = false;
            }
            else
            {
                d13.Image = Thirteen;
                DNotSelected[12] = true;
            }
        }

        private void d14_Click(object sender, EventArgs e)
        {
            if (DNotSelected[13])
            {
                d14.Image = FourteenSelected;
                DNotSelected[13] = false;
            }
            else
            {
                d14.Image = Fourteen;
                DNotSelected[13] = true;
            }
        }

        private void d15_Click(object sender, EventArgs e)
        {
            if (DNotSelected[14])
            {
                d15.Image = FifteenSelected;
                DNotSelected[14] = false;
            }
            else
            {
                d15.Image = Fifteen;
                DNotSelected[14] = true;
            }
        }

        private void d16_Click(object sender, EventArgs e)
        {
            if (DNotSelected[15])
            {
                d16.Image = SixteenSelected;
                DNotSelected[15] = false;
            }
            else
            {
                d16.Image = Sixteen;
                DNotSelected[15] = true;
            }
        }

        private void e1_Click(object sender, EventArgs e)
        {
            if (ENotSelected[0])
            {
                e1.Image = OneSelected;
                ENotSelected[0] = false;
            }
            else
            {
                e1.Image = One;
                ENotSelected[0] = true;
            }

        }

        private void e2_Click(object sender, EventArgs e)
        {
            if (ENotSelected[1])
            {
                e2.Image = TwoSelected;
                ENotSelected[1] = false;
            }
            else
            {
                e2.Image = Two;
                ENotSelected[1] = true;
            }
        }

        private void e3_Click(object sender, EventArgs e)
        {
            if (ENotSelected[2])
            {
                e3.Image = ThreeSelected;
                ENotSelected[2] = false;
            }
            else
            {
                e3.Image = Three;
                ENotSelected[2] = true;
            }
        }

        private void e4_Click(object sender, EventArgs e)
        {
            if (ENotSelected[3])
            {
                e4.Image = FourSelected;
                ENotSelected[3] = false;
            }
            else
            {
                e4.Image = Four;
                ENotSelected[3] = true;
            }
        }

        private void e5_Click(object sender, EventArgs e)
        {
            if (ENotSelected[4])
            {
                e5.Image = FiveSelected;
                ENotSelected[4] = false;
            }
            else
            {
                e5.Image = Five;
                ENotSelected[4] = true;
            }
        }

        private void e6_Click(object sender, EventArgs e)
        {
            if (ENotSelected[5])
            {
                e6.Image = SixSelected;
                ENotSelected[5] = false;
            }
            else
            {
                e6.Image = Six;
                ENotSelected[5] = true;
            }
        }

        private void e7_Click(object sender, EventArgs e)
        {
            if (ENotSelected[6])
            {
                e7.Image = SevenSelected;
                ENotSelected[6] = false;
            }
            else
            {
                e7.Image = Seven;
                ENotSelected[6] = true;
            }
        }

        private void e8_Click(object sender, EventArgs e)
        {
            if (ENotSelected[7])
            {
                e8.Image = EightSelected;
                ENotSelected[7] = false;
            }
            else
            {
                e8.Image = Eight;
                ENotSelected[7] = true;
            }
        }

        private void e9_Click(object sender, EventArgs e)
        {
            if (ENotSelected[8])
            {
                e9.Image = NineSelected;
                ENotSelected[8] = false;
            }
            else
            {
                e9.Image = Nine;
                ENotSelected[8] = true;
            }
        }

        private void e10_Click(object sender, EventArgs e)
        {
            if (ENotSelected[9])
            {
                e10.Image = TenSelected;
                ENotSelected[9] = false;
            }
            else
            {
                e10.Image = Ten;
                ENotSelected[9] = true;
            }
        }

        private void e11_Click(object sender, EventArgs e)
        {
            if (ENotSelected[10])
            {
                e11.Image = ElevenSelected;
                ENotSelected[10] = false;
            }
            else
            {
                e11.Image = Eleven;
                ENotSelected[10] = true;
            }
        }

        private void e12_Click(object sender, EventArgs e)
        {
            if (ENotSelected[11])
            {
                e12.Image = TwelveSelected;
                ENotSelected[11] = false;
            }
            else
            {
                e12.Image = Twelve;
                ENotSelected[11] = true;
            }
        }

        private void e13_Click(object sender, EventArgs e)
        {
            if (ENotSelected[12])
            {
                e13.Image = ThirteenSelected;
                ENotSelected[12] = false;
            }
            else
            {
                e13.Image = Thirteen;
                ENotSelected[12] = true;
            }
        }

        private void e14_Click(object sender, EventArgs e)
        {
            if (ENotSelected[13])
            {
                e14.Image = FourteenSelected;
                ENotSelected[13] = false;
            }
            else
            {
                e14.Image = Fourteen;
                ENotSelected[13] = true;
            }
        }

        private void e15_Click(object sender, EventArgs e)
        {
            if (ENotSelected[14])
            {
                e15.Image = FifteenSelected;
                ENotSelected[14] = false;
            }
            else
            {
                e15.Image = Fifteen;
                ENotSelected[14] = true;
            }
        }

        private void e16_Click(object sender, EventArgs e)
        {
            if (ENotSelected[15])
            {
                e16.Image = SixteenSelected;
                ENotSelected[15] = false;
            }
            else
            {
                e16.Image = Sixteen;
                ENotSelected[15] = true;
            }
        }

        private void button1_Click_4(object sender, EventArgs e)
        {
        
        }

        private void button1_Click_5(object sender, EventArgs e)
        {
            
            if (ANotSelected [0] == false)
            {
                Task.Run(() => {
                    a1Test();
                });

            }
            if (ANotSelected[1] == false)
            {
                Task.Run(() => {
                    a2Test();
                });

            }
            if (ANotSelected[2] == false)
            {
                Task.Run(() => {
                    a3Test();
                });
                
            }
            if (ANotSelected[3] == false)
            {
                Task.Run(() => {
                    a4Test();
                });
                
            }
            if (ANotSelected[4] == false)
            {
                Task.Run(() => {
                    a5Test();
                });
                
            }
            if (ANotSelected[5] == false)
            {
                Task.Run(() => {
                    a6Test();
                });
                
            }
            if (ANotSelected[6] == false)
            {
                Task.Run(() => {
                    a7Test();
                });
                
            }
            if (ANotSelected[7] == false)
            {
                Task.Run(() => {
                    a8Test();
                });
                
            }
            if (ANotSelected[8] == false)
            {
                Task.Run(() => {
                    a9Test();
                });
                
            }
            if (ANotSelected[9] == false)
            {
                Task.Run(() => {
                    a10Test();
                });
                
            }
            if (ANotSelected[10] == false)
            {
                Task.Run(() => {
                    a11Test();
                });
                
            }
            if (ANotSelected[11] == false)
            {
                Task.Run(() => {
                    a12Test();
                });
                
            }
            if (ANotSelected[12] == false)
            {
                Task.Run(() => {
                    a13Test();
                });
                
            }
            if (ANotSelected[13] == false)
            {
                Task.Run(() => {
                    a14Test();
                });
                
            }
            if (ANotSelected[14] == false)
            {
                Task.Run(() => {
                    a15Test();
                });
                
            }
            if (ANotSelected[15] == false)
            {
                Task.Run(() => {
                    a16Test();
                });
                
                AChecked.Checked = false;
            }
            if (BNotSelected[0] == false)
            {
                b1Test();
            }
            if (BNotSelected[1] == false)
            {
                b2Test();
            }
            if (BNotSelected[2] == false)
            {
                b3Test();
            }
            if (BNotSelected[3] == false)
            {
                b4Test();
            }
            if (BNotSelected[4] == false)
            {
                b5Test();
            }
            if (BNotSelected[5] == false)
            {
                b6Test();
            }
            if (BNotSelected[6] == false)
            {
                b7Test();
            }
            if (BNotSelected[7] == false)
            {
                b8Test();
            }
            if (BNotSelected[8] == false)
            {
                b9Test();
            }
            if (BNotSelected[9] == false)
            {
                b10Test();
            }
            if (BNotSelected[10] == false)
            {
                b11Test();
            }
            if (BNotSelected[11] == false)
            {
                b12Test();
            }
            if (BNotSelected[12] == false)
            {
                b13Test();
            }
            if (BNotSelected[13] == false)
            {
                b14Test();
            }
            if (BNotSelected[14] == false)
            {
                b15Test();
            }
            if (BNotSelected[15] == false)
            {
                b16Test();
                BChecked.Checked = false;
            }
            if (CNotSelected[0] == false)
            {
                c1Test();
            }
            if (CNotSelected[1] == false)
            {
                c2Test();
            }
            if (CNotSelected[2] == false)
            {
                c3Test();
            }
            if (CNotSelected[3] == false)
            {
                c4Test();
            }
            if (CNotSelected[4] == false)
            {
                c5Test();
            }
            if (CNotSelected[5] == false)
            {
                c6Test();
            }
            if (CNotSelected[6] == false)
            {
                c7Test();
            }
            if (CNotSelected[7] == false)
            {
                c8Test();
            }
            if (CNotSelected[8] == false)
            {
                c9Test();
            }
            if (CNotSelected[9] == false)
            {
                c10Test();
            }
            if (CNotSelected[10] == false)
            {
                c11Test();
            }
            if (CNotSelected[11] == false)
            {
                c12Test();
            }
            if (CNotSelected[12] == false)
            {
                c13Test();
            }
            if (CNotSelected[13] == false)
            {
                c14Test();
            }
            if (CNotSelected[14] == false)
            {
                c15Test();
            }
            if (CNotSelected[15] == false)
            {
                c16Test();
                CChecked.Checked = false;
            }
            if (DNotSelected[0] == false)
            {
                d1Test();
            }
            if (DNotSelected[1] == false)
            {
                d2Test();
            }
            if (DNotSelected[2] == false)
            {
                d3Test();
            }
            if (DNotSelected[3] == false)
            {
                d4Test();
            }
            if (DNotSelected[4] == false)
            {
                d5Test();
            }
            if (DNotSelected[5] == false)
            {
                d6Test();
            }
            if (DNotSelected[6] == false)
            {
                d7Test();
            }
            if (DNotSelected[7] == false)
            {
                d8Test();
            }
            if (DNotSelected[8] == false)
            {
                d9Test();
            }
            if (DNotSelected[9] == false)
            {
                d10Test();
            }
            if (DNotSelected[10] == false)
            {
                d11Test();
            }
            if (DNotSelected[11] == false)
            {
                d12Test();
            }
            if (DNotSelected[12] == false)
            {
                d13Test();
            }
            if (DNotSelected[13] == false)
            {
                d14Test();
            }
            if (DNotSelected[14] == false)
            {
                d15Test();
            }
            if (DNotSelected[15] == false)
            {
                d16Test();
                DChecked.Checked = false;
            }
            if (ENotSelected[0] == false)
            {
                e1Test();
            }
            if (ENotSelected[1] == false)
            {
                e2Test();
            }
            if (ENotSelected[2] == false)
            {
                e3Test();
            }
            if (ENotSelected[3] == false)
            {
                e4Test();
            }
            if (ENotSelected[4] == false)
            {
                e5Test();
            }
            if (ENotSelected[5] == false)
            {
                e6Test();
            }
            if (ENotSelected[6] == false)
            {
                e7Test();
            }
            if (ENotSelected[7] == false)
            {
                e8Test();
            }
            if (ENotSelected[8] == false)
            {
                e9Test();
            }
            if (ENotSelected[9] == false)
            {
                e10Test();
            }
            if (ENotSelected[10] == false)
            {
                e11Test();
            }
            if (ENotSelected[11] == false)
            {
                e12Test();
            }
            if (ENotSelected[12] == false)
            {
                e13Test();
            }
            if (ENotSelected[13] == false)
            {
                e14Test();
            }
            if (ENotSelected[14] == false)
            {
                e15Test();
            }
            if (ENotSelected[15] == false)
            {
                e16Test();
                EChecked.Checked = false;
            }
        }

        private void a1Test()
        {
            String output = SendData("a1test");
            a1.Image = One;
            ANotSelected[0] = true;
            if (output.Contains("A1G"))
            {
                a1.BackColor = Color.Lime;
            }
            if (output.Contains("A1B"))
            {
                a1.BackColor = Color.Red;
            }
        }

        private void a2Test()
        {
            String output = SendData("a2test");
            a2.Image = Two;
            ANotSelected[1] = true;
            if (output.Contains("A2G"))
            {
                a2.BackColor = Color.Lime;
            }
            if (output.Contains("A2B"))
            {
                a2.BackColor = Color.Red;
            }
        }

        private void a3Test()
        {
            String output = SendData("a3test");
            a3.Image = Three;
            ANotSelected[2] = true;
            if (output.Contains("A3G"))
            {
                a3.BackColor = Color.Lime;
            }
            if (output.Contains("A3B"))
            {
                a3.BackColor = Color.Red;
            }
        }

        private void a4Test()
        {
            String output = SendData("a4test");
            a4.Image = Four;
            ANotSelected[3] = true;
            if (output.Contains("A4G"))
            {
                a4.BackColor = Color.Lime;
            }
            if (output.Contains("A4B"))
            {
                a4.BackColor = Color.Red;
            }
        }

        private void a5Test()
        {
            String output = SendData("a5test");
            a5.Image = Five;
            ANotSelected[4] = true;
            if (output.Contains("A5G"))
            {
                a5.BackColor = Color.Lime;
            }
            if (output.Contains("A5B"))
            {
                a5.BackColor = Color.Red;
            }
        }

        private void a6Test()
        {
            String output = SendData("a6test");
            a6.Image = Six;
            ANotSelected[5] = true;
            if (output.Contains("A6G"))
            {
                a6.BackColor = Color.Lime;
            }
            if (output.Contains("A6B"))
            {
                a6.BackColor = Color.Red;
            }
        }

        private void a7Test()
        {
            String output = SendData("a7test");
            a7.Image = Seven;
            ANotSelected[6] = true;
            if (output.Contains("A7G"))
            {
                a7.BackColor = Color.Lime;
            }
            if (output.Contains("A7B"))
            {
                a7.BackColor = Color.Red;
            }
        }

        private void a8Test()
        {
            String output = SendData("a8test");
            a8.Image = Eight;
            ANotSelected[7] = true;
            if (output.Contains("A8G"))
            {
                a8.BackColor = Color.Lime;
            }
            if (output.Contains("A8B"))
            {
                a8.BackColor = Color.Red;
            }
        }

        private void a9Test()
        {
            String output = SendData("a9test");
            a9.Image = Nine;
            ANotSelected[8] = true;
            if (output.Contains("A9G"))
            {
                a9.BackColor = Color.Lime;
            }
            if (output.Contains("A9B"))
            {
                a9.BackColor = Color.Red;
            }
        }

        private void a10Test()
        {
            String output = SendData("a10test");
            a10.Image = Ten;
            ANotSelected[9] = true;
            if (output.Contains("A10G"))
            {
                a10.BackColor = Color.Lime;
            }
            if (output.Contains("A10B"))
            {
                a10.BackColor = Color.Red;
            }
        }

        private void a11Test()
        {
            String output = SendData("a11test");
            a11.Image = Eleven;
            ANotSelected[10] = true;
            if (output.Contains("A11G"))
            {
                a11.BackColor = Color.Lime;
            }
            if (output.Contains("A11B"))
            {
                a11.BackColor = Color.Red;
            }
        }

        private void a12Test()
        {
            String output = SendData("a12test");
            a12.Image = Twelve;
            ANotSelected[11] = true;
            if (output.Contains("A12G"))
            {
                a12.BackColor = Color.Lime;
            }
            if (output.Contains("A12B"))
            {
                a12.BackColor = Color.Red;
            }
        }

        private void a13Test()
        {
            String output = SendData("a13test");
            a13.Image = Thirteen;
            ANotSelected[12] = true;
            if (output.Contains("A13G"))
            {
                a13.BackColor = Color.Lime;
            }
            if (output.Contains("A13B"))
            {
                a13.BackColor = Color.Red;
            }
        }

        private void a14Test()
        {
            String output = SendData("a14test");
            a14.Image = Fourteen;
            ANotSelected[13] = true;
            if (output.Contains("A14G"))
            {
                a14.BackColor = Color.Lime;
            }
            if (output.Contains("A14B"))
            {
                a14.BackColor = Color.Red;
            }
        }

        private void a15Test()
        {
            String output = SendData("a15test");
            a15.Image = Fifteen;
            ANotSelected[14] = true;
            if (output.Contains("A15G"))
            {
                a15.BackColor = Color.Lime;
            }
            if (output.Contains("A15B"))
            {
                a15.BackColor = Color.Red;
            }
        }

        private void a16Test()
        {
            String output = SendData("a16test");
            a16.Image = Sixteen;
            ANotSelected[15] = true;
            if (output.Contains("A16G"))
            {
                a16.BackColor = Color.Lime;
            }
            if (output.Contains("A16B"))
            {
                a16.BackColor = Color.Red;
            }
        }

        private void b1Test()
        {
            String output = SendData("b1test");
            b1.Image = One;
            BNotSelected[0] = true;
            if (output.Contains("B1G"))
            {
                b1.BackColor = Color.Lime;
            }
            if (output.Contains("B1B"))
            {
                b1.BackColor = Color.Red;
            }
        }

        private void b2Test()
        {
            String output = ("b2test");
            b2.Image = Two;
            BNotSelected[1] = true;
            if (output.Contains("B2G"))
            {
                b2.BackColor = Color.Lime;
            }
            if (output.Contains("B2B"))
            {
                b2.BackColor = Color.Red;
            }
        }

        private void b3Test()
        {
            String output = SendData("b3test");
            b3.Image = Three;
            BNotSelected[2] = true;
            if (output.Contains("B3G"))
            {
                b3.BackColor = Color.Lime;
            }
            if (output.Contains("B3B"))
            {
                b3.BackColor = Color.Red;
            }
        }

        private void b4Test()
        {
            String output = SendData("b4test");
           b4.Image = Four;
            BNotSelected[3] = true;
            if (output.Contains("B4G"))
            {
                b4.BackColor = Color.Lime;
            }
            if (output.Contains("B4B"))
            {
                b4.BackColor = Color.Red;
            }
        }

        private void b5Test()
        {
            String output = SendData("b5test");
            b5.Image = Five;
            BNotSelected[4] = true;
            if (output.Contains("B5G"))
            {
                b5.BackColor = Color.Lime;
            }
            if (output.Contains("B5B"))
            {
                b5.BackColor = Color.Red;
            }
        }

        private void b6Test()
        {
            String output = SendData("b6test");
            b6.Image = Six;
            BNotSelected[5] = true;
            if (output.Contains("B6G"))
            {
                b6.BackColor = Color.Lime;
            }
            if (output.Contains("B6B"))
            {
                b6.BackColor = Color.Red;
            }
        }

        private void b7Test()
        {
            String output = SendData("b7test");
            b7.Image = Seven;
            BNotSelected[6] = true;
            if (output.Contains("B7G"))
            {
                b7.BackColor = Color.Lime;
            }
            if (output.Contains("B7B"))
            {
                b7.BackColor = Color.Red;
            }
        }

        private void b8Test()
        {
            String output = SendData("b8test");
            b8.Image = Eight;
            BNotSelected[7] = true;
            if (output.Contains("B8G"))
            {
                b8.BackColor = Color.Lime;
            }
            if (output.Contains("B8B"))
            {
                b8.BackColor = Color.Red;
            }
        }

        private void b9Test()
        {
            String output = SendData("b9test");
            b9.Image = Nine;
            BNotSelected[8] = true;
            if(output.Contains("B9G"))
            {
                b9.BackColor = Color.Lime;
            }
            if (output.Contains("B9B"))
            {
                b9.BackColor = Color.Red;
            }
        }

        private void b10Test()
        {
            String output = SendData("b10test");
            b10.Image = Ten;
            BNotSelected[9] = true;
            if (output.Contains("B10G"))
            {
                b10.BackColor = Color.Lime;
            }
            if (output.Contains("A10B"))
            {
                b10.BackColor = Color.Red;
            }
        }

        private void b11Test()
        {
            String output = SendData("b11test");
            b11.Image = Eleven;
            BNotSelected[10] = true;
            if (output.Contains("B11G"))
            {
                b11.BackColor = Color.Lime;
            }
            if (output.Contains("B11B"))
            {
                b11.BackColor = Color.Red;
            }
        }

        private void b12Test()
        {
            String output = SendData("b12test");
            b12.Image = Twelve;
            BNotSelected[11] = true;
            if (output.Contains("B12G"))
            {
                b12.BackColor = Color.Lime;
            }
            if (output.Contains("B12B"))
            {
                b12.BackColor = Color.Red;
            }
        }

        private void b13Test()
        {
            String output = SendData("b13test");
            b13.Image = Thirteen;
            BNotSelected[12] = true;
            if (output.Contains("B13G"))
            {
                b13.BackColor = Color.Lime;
            }
            if (output.Contains("B13B"))
            {
                b13.BackColor = Color.Red;
            }
        }

        private void b14Test()
        {
            String output = SendData("b14test");
            b14.Image = Fourteen;
            BNotSelected[13] = true;
            if (output.Contains("B14G"))
            {
                b14.BackColor = Color.Lime;
            }
            if (output.Contains("B14B"))
            {
                b14.BackColor = Color.Red;
            }
        }

        private void b15Test()
        {
            String output = SendData("b15test");
            b15.Image = Fifteen;
            BNotSelected[14] = true;
            if (output.Contains("B15G"))
            {
                b15.BackColor = Color.Lime;
            }
            if (output.Contains("B15B"))
            {
                b15.BackColor = Color.Red;
            }
        }

        private void b16Test()
        {
            String output = SendData("b16test");
            b16.Image = Sixteen;
            BNotSelected[15] = true;
            if (output.Contains("B16G"))
            {
                b16.BackColor = Color.Lime;
            }
            if (output.Contains("B16B"))
            {
                b16.BackColor = Color.Red;
            }
        }

        private void c1Test()
        {
            String output = SendData("c1test");
            c1.Image = One;
            CNotSelected[0] = true;
            if (output.Contains("C1G"))
            {
                c1.BackColor = Color.Lime;
            }
            if (output.Contains("C1B"))
            {
                c1.BackColor = Color.Red;
            }
        }

        private void c2Test()
        {
            String output = SendData("c2test");
            c2.Image = Two;
            CNotSelected[1] = true;
            if (output.Contains("C2G"))
            {
                c2.BackColor = Color.Lime;
            }
            if (output.Contains("C2B"))
            {
                c2.BackColor = Color.Red;
            }
        }

        private void c3Test()
        {
            String output = SendData("c3test");
            c3.Image = Three;
            CNotSelected[2] = true;
            if (output.Contains("C3G"))
            {
                c3.BackColor = Color.Lime;
            }
            if (output.Contains("C3B"))
            {
                c3.BackColor = Color.Red;
            }
        }

        private void c4Test()
        {
            String output = SendData("c4test");
            c4.Image = Four;
            CNotSelected[3] = true;
            if (output.Contains("C4G"))
            {
                c4.BackColor = Color.Lime;
            }
            if (output.Contains("C4B"))
            {
                c4.BackColor = Color.Red;
            }
        }

        private void c5Test()
        {
            String output = SendData("c5test");
            c5.Image = Five;
            CNotSelected[4] = true;
            if (output.Contains("C5G"))
            {
                c5.BackColor = Color.Lime;
            }
            if (output.Contains("C5B"))
            {
                c5.BackColor = Color.Red;
            }
        }

        private void c6Test()
        {
            String output = SendData("c6test");
            c6.Image = Six;
            CNotSelected[5] = true;
            if (output.Contains("C6G"))
            {
                c6.BackColor = Color.Lime;
            }
            if (output.Contains("C6B"))
            {
                c6.BackColor = Color.Red;
            }
        }

        private void c7Test()
        {
            String output = SendData("c7test");
            c7.Image = Seven;
            CNotSelected[6] = true;
            if (output.Contains("C7G"))
            {
                c7.BackColor = Color.Lime;
            }
            if (output.Contains("C7B"))
            {
                c7.BackColor = Color.Red;
            }
        }

        private void c8Test()
        {
            String output = SendData("c8test");
            c8.Image = Eight;
            CNotSelected[7] = true;
            if (output.Contains("C8G"))
            {
                c8.BackColor = Color.Lime;
            }
            if (output.Contains("C8B"))
            {
                c8.BackColor = Color.Red;
            }
        }

        private void c9Test()
        {
            String output = SendData("c9test");
            c9.Image = Nine;
            CNotSelected[8] = true;
            if (output.Contains("C9G"))
            {
                c9.BackColor = Color.Lime;
            }
            if (output.Contains("C9B"))
            {
                c9.BackColor = Color.Red;
            }
        }

        private void c10Test()
        {
            String output = SendData("c10test");
            c10.Image = Ten;
            ANotSelected[9] = true;
            if (output.Contains("C10G"))
            {
                c10.BackColor = Color.Lime;
            }
            if (output.Contains("C10B"))
            {
                c10.BackColor = Color.Red;
            }
        }

        private void c11Test()
        {
            String output = SendData("c11test");
            c11.Image = Eleven;
            CNotSelected[10] = true;
            if (output.Contains("C11G"))
            {
                c11.BackColor = Color.Lime;
            }
            if (output.Contains("C11B"))
            {
                c11.BackColor = Color.Red;
            }
        }

        private void c12Test()
        {
            String output = SendData("c12test");
            c12.Image = Twelve;
            CNotSelected[11] = true;
            if (output.Contains("C12G"))
            {
                c12.BackColor = Color.Lime;
            }
            if (output.Contains("C12B"))
            {
                c12.BackColor = Color.Red;
            }
        }

        private void c13Test()
        {
            String output = SendData("c13test");
            c13.Image = Thirteen;
            CNotSelected[12] = true;
            if (output.Contains("C13G"))
            {
                c13.BackColor = Color.Lime;
            }
            if (output.Contains("A13B"))
            {
                c13.BackColor = Color.Red;
            }
        }

        private void c14Test()
        {
            String output = SendData("c14test");
            c14.Image = Fourteen;
            CNotSelected[13] = true;
            if (output.Contains("C14G"))
            {
                c14.BackColor = Color.Lime;
            }
            if (output.Contains("C14B"))
            {
                c14.BackColor = Color.Red;
            }
        }

        private void c15Test()
        {
            String output = SendData("c15test");
            c15.Image = Fifteen;
            CNotSelected[14] = true;
            if (output.Contains("C15G"))
            {
                c15.BackColor = Color.Lime;
            }
            if (output.Contains("C15B"))
            {
                c15.BackColor = Color.Red;
            }
        }

        private void c16Test()
        {
            String output = SendData("c16test");
            c16.Image = Sixteen;
            CNotSelected[15] = true;
            if (output.Contains("C16G"))
            {
                c16.BackColor = Color.Lime;
            }
            if (output.Contains("C16B"))
            {
                c16.BackColor = Color.Red;
            }
        }

        private void d1Test()
        {
            String output = SendData("d1test");
            d1.Image = One;
            DNotSelected[0] = true;
            if (output.Contains("D1G"))
            {
                d1.BackColor = Color.Lime;
            }
            if (output.Contains("D1B"))
            {
                d1.BackColor = Color.Red;
            }
        }

        private void d2Test()
        {
            String output = SendData("d2test");
            d2.Image = Two;
            DNotSelected[1] = true;
            if (output.Contains("D2G"))
            {
                d2.BackColor = Color.Lime;
            }
            if (output.Contains("D2B"))
            {
                d2.BackColor = Color.Red;
            }
        }

        private void d3Test()
        {
            String output = SendData("d3test");
            d3.Image = Three;
            DNotSelected[2] = true;
            if (output.Contains("D3G"))
            {
                d3.BackColor = Color.Lime;
            }
            if (output.Contains("D3B"))
            {
                d3.BackColor = Color.Red;
            }
        }

        private void d4Test()
        {
            String output = SendData("d4test");
            d4.Image = Four;
            DNotSelected[3] = true;
            if (output.Contains("D4G"))
            {
                d4.BackColor = Color.Lime;
            }
            if (output.Contains("D4B"))
            {
                d4.BackColor = Color.Red;
            }
        }

        private void d5Test()
        {
            String output = SendData("d5test");
            d5.Image = Five;
            DNotSelected[4] = true;
            if (output.Contains("D5G"))
            {
                d5.BackColor = Color.Lime;
            }
            if (output.Contains("D5B"))
            {
                d5.BackColor = Color.Red;
            }
        }

        private void d6Test()
        {
            String output = SendData("d6test");
            d6.Image = Six;
            DNotSelected[5] = true;
            if (output.Contains("D6G"))
            {
                d6.BackColor = Color.Lime;
            }
            if (output.Contains("D6B"))
            {
                d6.BackColor = Color.Red;
            }
        }

        private void d7Test()
        {
            String output = SendData("d7test");
            d7.Image = Seven;
            DNotSelected[6] = true;
            if (output.Contains("D7G"))
            {
                d7.BackColor = Color.Lime;
            }
            if (output.Contains("D7B"))
            {
                d7.BackColor = Color.Red;
            }
        }

        private void d8Test()
        {
            String output = SendData("d8test");
            d8.Image = Eight;
            DNotSelected[7] = true;
            if (output.Contains("D8G"))
            {
                d8.BackColor = Color.Lime;
            }
            if (output.Contains("D8B"))
            {
                d8.BackColor = Color.Red;
            }
        }

        private void d9Test()
        {
            String output = SendData("d9test");
            d9.Image = Nine;
            DNotSelected[8] = true;
            if (output.Contains("D9G"))
            {
                d9.BackColor = Color.Lime;
            }
            if (output.Contains("D9B"))
            {
                d9.BackColor = Color.Red;
            }
        }

        private void d10Test()
        {
            String output = SendData("d10test");
            d10.Image = Ten;
            DNotSelected[9] = true;
            if (output.Contains("D10G"))
            {
                d10.BackColor = Color.Lime;
            }
            if (output.Contains("D10B"))
            {
                a10.BackColor = Color.Red;
            }
        }

        private void d11Test()
        {
            String output = SendData("d11test");
            d11.Image = Eleven;
            DNotSelected[10] = true;
            if (output.Contains("D11G"))
            {
                d11.BackColor = Color.Lime;
            }
            if (output.Contains("D11B"))
            {
                d11.BackColor = Color.Red;
            }
        }

        private void d12Test()
        {
            String output = SendData("d12test");
            d12.Image = Twelve;
            DNotSelected[11] = true;
            if (output.Contains("D12G"))
            {
                d12.BackColor = Color.Lime;
            }
            if (output.Contains("D12B"))
            {
                d12.BackColor = Color.Red;
            }
        }

        private void d13Test()
        {
            String output = SendData("d13test");
            d13.Image = Thirteen;
            DNotSelected[12] = true;
            if (output.Contains("D13G"))
            {
                d13.BackColor = Color.Lime;
            }
            if (output.Contains("D13B"))
            {
                d13.BackColor = Color.Red;
            }
        }

        private void d14Test()
        {
            String output = SendData("d14test");
            d14.Image = Fourteen;
            DNotSelected[13] = true;
            if (output.Contains("D14G"))
            {
                d14.BackColor = Color.Lime;
            }
            if (output.Contains("D14B"))
            {
                d14.BackColor = Color.Red;
            }
        }

        private void d15Test()
        {
            String output = SendData("d15test");
            d15.Image = Fifteen;
            DNotSelected[14] = true;
            if (output.Contains("D15G"))
            {
                d15.BackColor = Color.Lime;
            }
            if (output.Contains("D15B"))
            {
                d15.BackColor = Color.Red;
            }
        }

        private void d16Test()
        {
            String output = SendData("d16test");
            a16.Image = Sixteen;
            DNotSelected[15] = true;
            if (output.Contains("D16G"))
            {
                d16.BackColor = Color.Lime;
            }
            if (output.Contains("D16B"))
            {
                d16.BackColor = Color.Red;
            }
        }

        private void e1Test()
        {
            String output = SendData("e1test");
            e1.Image = One;
            ENotSelected[0] = true;
            if (output.Contains("E1G"))
            {
                e1.BackColor = Color.Lime;
            }
            if (output.Contains("E1B"))
            {
                e1.BackColor = Color.Red;
            }
        }

        private void e2Test()
        {
            String output = SendData("e2test");
            e2.Image = Two;
            ENotSelected[1] = true;
            if (output.Contains("E2G"))
            {
                e2.BackColor = Color.Lime;
            }
            if (output.Contains("E2B"))
            {
                e2.BackColor = Color.Red;
            }
        }

        private void e3Test()
        {
            String output = SendData("e3test");
            e3.Image = Three;
            ENotSelected[2] = true;
            if (output.Contains("E3G"))
            {
                e3.BackColor = Color.Lime;
            }
            if (output.Contains("E3B"))
            {
                e3.BackColor = Color.Red;
            }
        }

        private void e4Test()
        {
            String output = SendData("e4test");
            e4.Image = Four;
            ENotSelected[3] = true;
            if (output.Contains("E4G"))
            {
                e4.BackColor = Color.Lime;
            }
            if (output.Contains("E4B"))
            {
                e4.BackColor = Color.Red;
            }
        }

        private void e5Test()
        {
            String output = SendData("e5test");
            e5.Image = Five;
            ENotSelected[4] = true;
            if (output.Contains("E5G"))
            {
                e5.BackColor = Color.Lime;
            }
            if (output.Contains("E5B"))
            {
                e5.BackColor = Color.Red;
            }
        }

        private void e6Test()
        {
            String output = SendData("e6test");
            e6.Image = Six;
            ENotSelected[5] = true;
            if (output.Contains("E6G"))
            {
                e6.BackColor = Color.Lime;
            }
            if (output.Contains("E6B"))
            {
                e6.BackColor = Color.Red;
            }
        }

        private void e7Test()
        {
            String output = SendData("e7test");
            e7.Image = Seven;
            ENotSelected[6] = true;
            if (output.Contains("E7G"))
            {
                e7.BackColor = Color.Lime;
            }
            if (output.Contains("E7B"))
            {
                e7.BackColor = Color.Red;
            }
        }

        private void e8Test()
        {
            String output = SendData("e8test");
            e8.Image = Eight;
            ENotSelected[7] = true;
            if (output.Contains("E8G"))
            {
                e8.BackColor = Color.Lime;
            }
            if (output.Contains("E8B"))
            {
                e8.BackColor = Color.Red;
            }
        }

        private void e9Test()
        {
            String output = SendData("e9test");
            e9.Image = Nine;
            ENotSelected[8] = true;
            if (output.Contains("E9G"))
            {
                e9.BackColor = Color.Lime;
            }
            if (output.Contains("E9B"))
            {
                e9.BackColor = Color.Red;
            }
        }

        private void e10Test()
        {
            String output = SendData("e10test");
            e10.Image = Ten;
            ENotSelected[9] = true;
            if (output.Contains("E10G"))
            {
                e10.BackColor = Color.Lime;
            }
            if (output.Contains("E10B"))
            {
                e10.BackColor = Color.Red;
            }
        }

        private void e11Test()
        {
            String output = SendData("e11test");
            e11.Image = Eleven;
            ENotSelected[10] = true;
            if (output.Contains("E11G"))
            {
                e11.BackColor = Color.Lime;
            }
            if (output.Contains("E11B"))
            {
                e11.BackColor = Color.Red;
            }
        }

        private void e12Test()
        {
            String output = SendData("e12test");
            e12.Image = Twelve;
            ENotSelected[11] = true;
            if (output.Contains("E12G"))
            {
                e12.BackColor = Color.Lime;
            }
            if (output.Contains("E12B"))
            {
                e12.BackColor = Color.Red;
            }
        }

        private void e13Test()
        {
            String output = SendData("e13test");
            e13.Image = Thirteen;
            ENotSelected[12] = true;
            if (output.Contains("E13G"))
            {
                e13.BackColor = Color.Lime;
            }
            if (output.Contains("E13B"))
            {
                e13.BackColor = Color.Red;
            }
        }

        private void e14Test()
        {
            String output = SendData("e14test");
            e14.Image = Fourteen;
            ENotSelected[13] = true;
            if (output.Contains("E14G"))
            {
                e14.BackColor = Color.Lime;
            }
            if (output.Contains("E14B"))
            {
                e14.BackColor = Color.Red;
            }
        }

        private void e15Test()
        {
            String output = SendData("e15test");
            e15.Image = Fifteen;
            ENotSelected[14] = true;
            if (output.Contains("E15G"))
            {
                e15.BackColor = Color.Lime;
            }
            if (output.Contains("E15B"))
            {
                e15.BackColor = Color.Red;
            }
        }

        private void e16Test()
        {
            String output = SendData("e16test");
            e16.Image = Sixteen;
            ENotSelected[15] = true;
            if (output.Contains("E16G"))
            {
                e16.BackColor = Color.Lime;
            }
            if (output.Contains("E16B"))
            {
                e16.BackColor = Color.Red;
            }
        }

        private String SendData(String message)
        {
            byte[] bytes = new byte[1024];

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 50000 on the local computer.
                IPAddress localAdd = IPAddress.Parse(IP);
                IPEndPoint remoteEP = new IPEndPoint(localAdd, 50000);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes(message);

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Recieved Data: {0}",
                    Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    return Encoding.ASCII.GetString(bytes, 0, bytesRec);

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    return null;
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    return null;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        
    }
        
        private void button2_Click_2(object sender, EventArgs e)
        {
            

        }

        private void toolStripDropDownButton2_Click_1(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            String p = Interaction.InputBox("Please Enter New Password.", "Enter Arming Password", "");
            if (IP == "")
            {
                MessageBox.Show("No Password Entered!", "No Password Entered", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            else
            {
                MessageBox.Show("Password successfully changed.", "Password Changed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Password = p; 
            }
        }

        private void disableArmSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ArmSound == true)
            {
                ArmSound = false;
                disableArmSoundToolStripMenuItem.Text = ("Enable Arm Sound");
                MessageBox.Show("Arm Sound Disabled", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ArmSound = true;
                disableArmSoundToolStripMenuItem.Text = ("Disable Arm Sound");
                MessageBox.Show("Arm Sound Enabled", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileImage.ShowDialog();
        }
    }
}
