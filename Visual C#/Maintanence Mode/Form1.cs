using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace AVS_Maintanence
{
    public partial class Form1 : Form
    {
        SerialPort sp = new SerialPort();

        //Baud Rate Setting
        int Baud_rate = 9600;
        
        //Timeout 10 seconds
        const int READ_TIMEOUT = 10000;   

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //get available ports
            COM_Sel();

            //Set Connection Label
            TS_LBL_Con.Text = "Not Connected";
            TS_LBL_Con.ForeColor = Color.Black;
        }

        //Populate Combo Box with Available Serial Ports
        private void COM_Sel()
        {
            TS_CMB_ComPort.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                TS_CMB_ComPort.Items.Add(s);
            } 
            if(TS_CMB_ComPort.Items.Count != 0)
            {
                TS_CMB_ComPort.SelectedIndex = 0;
            }
        }

        /*--------------------------------------------------------------------
        * Open MDI Windows
        * Serial Port passed to each as argument
        ---------------------------------------------------------------------*/
        //Find if form open, used for creating single instances
        private bool IsOpen(String name)
        {
            foreach(Form frm in Application.OpenForms)
            {
                if(frm.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        //Servo Control Window
        private void servoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Servo SVR = new Servo(sp)
            {
                MdiParent = this,
                Name = "servo"
            };
            
            if (IsOpen("servo") == false)
            { 
                SVR.Show();
            }
            
        }
        //State Machine Test
        private void stateMachineTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FSMTest FSM = new FSMTest(sp)
            {
                MdiParent = this,
                Name = "fsm"
            };

            if (IsOpen("fsm") == false)
            {
                FSM.Show();
            }
        }
        //Colour Sensor Read
        private void colourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColourTest Ctest = new ColourTest(sp)
            {
                MdiParent = this,
                Name = "ctest"
            };

            if(IsOpen("ctest") == false)
            {
                Ctest.Show();
            }
        }
        //Time of Flight Sensor Read
        private void timeOfFlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tof Time_Flight = new tof(sp)
            {
                MdiParent = this,
                Name = "tof"
            };

            if(IsOpen("tof") == false)
            {
                Time_Flight.Show();
            }
        }
        //Sort Controler
        private void sortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sort srt = new Sort(sp)
            {
                MdiParent = this,
                Name = "srt"
            };

            if(IsOpen("srt") == false)
            {
                srt.Show();
            }
        }

        //Card Reader Sensor Read
        private void cardReaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CardCode card = new CardCode(sp)
            {
                MdiParent = this,
                Name = "card"
            };

            if (IsOpen("card") == false)
            {
                card.Show();
            }
        }

        //About Dialog
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }
        //Close Main Form
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*----------------------------------------------------------
         * Application Functionality
         ---------------------------------------------------------*/
        //Connect ot serial port
        private void TS_BTN_ComCon_Click(object sender, EventArgs e)
        {
            string COMPort = TS_CMB_ComPort.Text.ToString();
            string cur_com = sp.PortName;

            //If com port is close and new com port selected
            if (!sp.IsOpen && cur_com != COMPort)
            {
                //set the com port
                sp.PortName = COMPort;
            }
            //if teh com port is open and new com port selected
            else if (sp.IsOpen && cur_com != COMPort)
            {
                //close the port and set new port name
                sp.Close();
                sp.PortName = COMPort;
            }

            //set serial port properties
            sp.ReadTimeout = READ_TIMEOUT;
            sp.BaudRate = Baud_rate;

            //try to open serial port
            try
            {
                sp.Open();
            }
            catch
            {
                //if unable to open serial port display in status bar
                TS_LBL_Con.Text = "Unable to Open " + COMPort;
                TS_LBL_Con.ForeColor = Color.Red;
            }

            //Display open com port and baud rate in status bar
            TS_LBL_Con.Text = COMPort + " @ " + Baud_rate.ToString() + " Open";
            TS_LBL_Con.ForeColor = Color.Green;
        }        
        //Close Serial Port
        private void closePortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //only close if serial port is already open
            if (sp.IsOpen)
            {
                sp.Close();
                TS_LBL_Con.Text = "Not Connected";
                TS_LBL_Con.ForeColor = Color.Black;
            }                       
        }
        //Send Text
        private void TS_BTN_Send_Click(object sender, EventArgs e)
        {
            try
            {
                //Read string from text box and send
                string txt = TS_TXT_Send.Text.ToString();
                sp.WriteLine(txt);
                //clear text box
                TS_TXT_Send.Text = "";
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please Open Serial Port");
            }
        }
        //refresh com port list
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            COM_Sel();
        }
        /*-----------------------------------------------------
         * Set Baud Rate
         ----------------------------------------------------*/
        private void Baud_9600_Click(object sender, EventArgs e)
        {
            //set baud rate to 9600
            Baud_rate = 9600;
            //check the 9600 item and clear the 115200 item
            Baud_9600.Checked = true;
            Baud_115200.Checked = false;
        }

        private void Baud_115200_Click(object sender, EventArgs e)
        {
            //set the baud rate to 115200
            Baud_rate = 115200;
            //check the 115200 item and clear the 9600 item
            Baud_9600.Checked = false;
            Baud_115200.Checked = true;
        }
		//Home All Button
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            try
            {
                sp.WriteLine("S:1&8");
                Thread.Sleep(10);
                sp.WriteLine("S:2&8");
                Thread.Sleep(10);
                sp.WriteLine("S:3&8");
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please Open Serial Port");
            }
        }
		//Purge Button
        private void BTN_Purge_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 7; i++)
                {
                    sp.WriteLine("S:2&15");
                    Thread.Sleep(10);
                    sp.WriteLine("S:3&0");
                    Thread.Sleep(500);
                    sp.WriteLine("S:2&8");
                    Thread.Sleep(10);
                    sp.WriteLine("S:3&8");
                    Thread.Sleep(500);
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please Opne Serial Port");
            }
        }
    }
}
