using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Collections;
using ZedGraph;
using System.Threading;
using System.Diagnostics;

namespace SaveTextFile
{
    public partial class Form1 : Form
    {
        // Biến lưu data
        Thread Savefile;
        Thread Savefile1;
        private string file_ecg = @"D:\ecg.txt";
        private string file_ppg = @"D:\ppg.txt";
        int dt, dt2;
        private double time;
        private ArrayList data_ecg;
        private ArrayList data_ppg;
        ArrayList list1 = new ArrayList();
        ArrayList list2 = new ArrayList();
        public bool is_s1 = false;
        public bool stop_draw = true;
        public int save_length = 500000;//Chiều dài để lưu
        private int f_s = 1000;
        public int index = 0;
        public int tick = 0;
        private FileProcessing save_ecg;
        private FileProcessing save_ppg;
        public bool is_save = false;
        private bool is_read = false;
        int[] data_save;
        int[] data_save1;
        RollingPointPairList my_line1 = new RollingPointPairList(1200000);
        RollingPointPairList my_line2 = new RollingPointPairList(1200000);
        PointPairList list = new PointPairList();
        public float second;
        public int minute;
        public Form1()
        {
            {
                InitializeComponent();
                data_ecg = new ArrayList();
                save_ecg = new FileProcessing(file_ecg);
                data_save = new int[data_ecg.Count];

                data_ppg = new ArrayList();
                save_ppg = new FileProcessing(file_ppg);
                data_save1 = new int[data_ppg.Count];
            }
            string[] list_port = SerialPort.GetPortNames();
            CbdetectCom.Items.AddRange(list_port);
            //  time = 0;
            //cứ sau khoảng thời gian này thì sự kiện timer_tick được gọi
            timer1.Interval = 100;

            myzed.GraphPane.AddCurve("ECG", my_line1, Color.Red, SymbolType.None);
            myzed.GraphPane.AddCurve("PPG", my_line2, Color.Blue, SymbolType.None);
            createGraph(myzed);
            Com.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);
        }

        #region Thay đổi trục thời gian của Zedgraph
        private void SetTime_ZedGraph(ZedGraphControl zed)
        {
            try
            {
                Scale xscale = zed.GraphPane.XAxis.Scale;
               if (xscale.Max < time)

                {
                    xscale.Max = time;
                    xscale.Min = xscale.Max - 4;//Khung cửa sổ thời gian hiện tín hiệu   
                              
                }
                zed.GraphPane.AxisChange();
                zed.Invalidate();
            }
            catch  { }
        }

        private void createGraph(ZedGraphControl zg1)
        {
            //Set a reference to the GraphPane
            GraphPane myPane = zg1.GraphPane;
            //Set title
            myPane.Title.Text = "Bio-Medical ECG & PPG Signal";
            myPane.XAxis.Title.Text = "Time";
            myPane.YAxis.Title.Text = "Amplitude";          
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (is_read == true)
            {
                SetTime_ZedGraph(myzed);

            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            is_read = true;
            timer1.Start();
            if (Com.IsOpen)
            {
                Com.Close();
                btnConnect.Text = "Connect";
            }
            else
            {
                try
                {
                    Com.PortName = CbdetectCom.Text;
                    Com.Open();
                    btnConnect.Text = "Disconnect";
                    flag1 = true;
                }
                catch
                {
                    MessageBox.Show("oooo" + Com.PortName, "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        bool is_full = false;
        private void Send_Click(object sender, EventArgs e)
        {

            Savefile = new Thread(new ThreadStart(ThreadSaveFile));
            Savefile1 = new Thread(new ThreadStart(ThreadSaveFile1));
            

            if (!is_full)
            {
                is_full = true;
                timer2.Enabled = true; // bat dau thi cho timer 1 dem
                label3.Text = "Saving...";
            }
            else
            {
                Savefile.Start();
                Savefile1.Start();
                is_full = false;
                timer2.Enabled = false;
                label3.Text = "Done";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ThreadSaveFile()
        {
            while (true)
            {
                {
                    data_save = new int[data_ecg.Count];
                    data_ecg.CopyTo(0, data_save, 0, data_ecg.Count);
                    save_ecg.SaveData(data_save);
                    data_ecg.Clear();
                    is_save = true;
                    Savefile.Abort();
                }
            }
        }
        private void ThreadSaveFile1()
        {
            while (true)
            {
                {
                    data_save1 = new int[data_ppg.Count];
                    data_ppg.CopyTo(0, data_save1, 0, data_ppg.Count);
                    save_ppg.SaveData1(data_save1);
                    data_ppg.Clear();
                    is_save = true;
                    Savefile1.Abort();
                }
            }
        }
        bool flag1 = true;
        string s;
        int flagcheck_value = 0;
        int flagcheck_old = 0;
        int index1;
        string a,info;
        string old;

        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

                string k = Com.ReadLine();
                if (k != "" & k != null)
                {
                    if (k.IndexOf("@") > 0 && k.IndexOf("@") < 5 && k.IndexOf("$") < 1)
                    {
                        index1 = k.IndexOf("@");
                        info = k.Remove(index1);
                        a = info;
                        old = info;
                        flagcheck_value = 1;
                    }

                    if (k.IndexOf("$") > 0 && k.IndexOf("$") < 5 && k.IndexOf("@") < 1)
                    {
                        index1 = k.IndexOf("$");
                        info = k.Remove(index1);
                        a = info;
                        old = info;
                        flagcheck_value = 2;
                    }
                }
                else
                {
                    a = old;
                    flagcheck_old = 1;
                    old = "";
                    //Debugger.Break();

                }
                if (flagcheck_value == 1 || flagcheck_old == 1)
                {
                    {
                        dt = Convert.ToInt32(a);
                    }

                    if (is_read)
                    {
                        my_line1.Add(time, dt);
                        time += 1.0 / f_s;
                    }
                    if (is_full)
                    {
                        data_ecg.Add(dt);
                    }
                }   
                if (flagcheck_value == 2 || flagcheck_old == 1)
                {

                    {
                        dt2 = Convert.ToInt32(a);
                    }

                    if (is_read)
                    {
                        my_line2.Add(time, dt2);
                        time += 1.0 / f_s;
                    }
                    if (is_full)
                    {
                        data_ppg.Add(dt2);
                    }
                }
            }
       


        private void btnStopDraw_Click(object sender, EventArgs e)
        {
            stop_draw = false;
            is_read = false;
        }

        private void cRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Com.IsOpen)
            {
                Com.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            second++;
            label5.Text = minute.ToString() + " : " + second.ToString();
            if (second == 59)
            {
                second = -1;
                minute++;
            }
        }

        private void myzed_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}