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

namespace AVS_Maintanence
{
    public partial class Servo : Form
    {
        private SerialPort serial;
        public Servo(SerialPort port)
        {
            InitializeComponent();
			//access to serial port
            serial = port;
        }

        private void TB_Val_ValueChanged(object sender, EventArgs e)
        {
			//update labels
            int Pos = TB_Val.Value;
            LBL_Pos.Text = Pos.ToString();
			//label with relative angle	
            float RelAngle = (Pos / 16.000f) * 90.000f;
            LBL_RelAngle.Text = RelAngle.ToString();
            
        }

        byte[] S_com = { 0xEE, 0x00 };
        
		//All max
        private void BTN_SA_Click(object sender, EventArgs e)
        {
             
            try
            {
				//FPGA Command
                if (CMB_Dev.SelectedIndex == 0)
                {
                    serial.Write(S_com, 0, 1);
                }
                else
				//MBed Command
                {
                    serial.WriteLine("S:14&14");
                }
            }
            catch
            {

                MessageBox.Show("Please Open Serial Port");
            }
            
        }
		//All Min
        private void BTN_CA_Click(object sender, EventArgs e)
        {
            try
            {
				//FPGA Command
                if (CMB_Dev.SelectedIndex == 0)
                {
                    serial.Write(S_com, 1, 1);
                }
                else
				//MBed Command
                {
                    serial.WriteLine("S:0&0");
                }
            }
            catch
            {

                MessageBox.Show("Please Open Serial Port");
            }
            
        }

		//Set Selected
        private void BTN_SS_Click(object sender, EventArgs e)
        {
            int SV_no = new int();
            byte[] SS_com = new byte[1]; 

			//Servo 1 Command
            if(RB_S1.Checked == true)
            {
                SV_no = 1;
                switch (TB_Val.Value)
                {
                    case 0:  SS_com[0] = 0x10; break;
                    case 1:  SS_com[0] = 0x11; break;
                    case 2:  SS_com[0] = 0x12; break;
                    case 3:  SS_com[0] = 0x13; break;
                    case 4:  SS_com[0] = 0x14; break;
                    case 5:  SS_com[0] = 0x15; break;
                    case 6:  SS_com[0] = 0x16; break;
                    case 7:  SS_com[0] = 0x17; break;
                    case 8:  SS_com[0] = 0x18; break;
                    case 9:  SS_com[0] = 0x19; break;
                    case 10: SS_com[0] = 0x1a; break;
                    case 11: SS_com[0] = 0x1b; break;
                    case 12: SS_com[0] = 0x1c; break;
                    case 13: SS_com[0] = 0x1d; break;
                    case 14: SS_com[0] = 0x1e; break;
                    case 15: SS_com[0] = 0x1f; break;
                }

            }
			//Servo 2 Command
            if(RB_S2.Checked == true)
            {
                SV_no = 2;
                switch (TB_Val.Value)
                {
                    case 0:  SS_com[0] = 0x20; break;
                    case 1:  SS_com[0] = 0x21; break;
                    case 2:  SS_com[0] = 0x22; break;
                    case 3:  SS_com[0] = 0x23; break;
                    case 4:  SS_com[0] = 0x24; break;
                    case 5:  SS_com[0] = 0x25; break;
                    case 6:  SS_com[0] = 0x26; break;
                    case 7:  SS_com[0] = 0x27; break;
                    case 8:  SS_com[0] = 0x28; break;
                    case 9:  SS_com[0] = 0x29; break;
                    case 10: SS_com[0] = 0x2a; break;
                    case 11: SS_com[0] = 0x2b; break;
                    case 12: SS_com[0] = 0x2c; break;
                    case 13: SS_com[0] = 0x2d; break;
                    case 14: SS_com[0] = 0x2e; break;
                    case 15: SS_com[0] = 0x2f; break;
                }
            }
			//Servo 3 Command
            if(RB_S3.Checked == true)
            {
                SV_no = 3;
                switch (TB_Val.Value)
                {
                    case 0: SS_com[0] = 0x30; break;
                    case 1: SS_com[0] = 0x31; break;
                    case 2: SS_com[0] = 0x32; break;
                    case 3: SS_com[0] = 0x33; break;
                    case 4: SS_com[0] = 0x34; break;
                    case 5: SS_com[0] = 0x35; break;
                    case 6: SS_com[0] = 0x36; break;
                    case 7: SS_com[0] = 0x37; break;
                    case 8: SS_com[0] = 0x38; break;
                    case 9: SS_com[0] = 0x39; break;
                    case 10: SS_com[0] = 0x3a; break;
                    case 11: SS_com[0] = 0x3b; break;
                    case 12: SS_com[0] = 0x3c; break;
                    case 13: SS_com[0] = 0x3d; break;
                    case 14: SS_com[0] = 0x3e; break;
                    case 15: SS_com[0] = 0x3f; break;
                }
            }
			//Servo 4 Command
            if(RB_S4.Checked == true)
            {
                SV_no = 4;
                switch (TB_Val.Value)
                {
                    case 0: SS_com[0] = 0x40; break;
                    case 1: SS_com[0] = 0x41; break;
                    case 2: SS_com[0] = 0x42; break;
                    case 3: SS_com[0] = 0x43; break;
                    case 4: SS_com[0] = 0x44; break;
                    case 5: SS_com[0] = 0x45; break;
                    case 6: SS_com[0] = 0x46; break;
                    case 7: SS_com[0] = 0x47; break;
                    case 8: SS_com[0] = 0x48; break;
                    case 9: SS_com[0] = 0x49; break;
                    case 10: SS_com[0] = 0x4a; break;
                    case 11: SS_com[0] = 0x4b; break;
                    case 12: SS_com[0] = 0x4c; break;
                    case 13: SS_com[0] = 0x4d; break;
                    case 14: SS_com[0] = 0x4e; break;
                    case 15: SS_com[0] = 0x4f; break;
                }
            }
			//Servo 5 Command
            if(RB_S5.Checked == true)
            {
                SV_no = 5;
                switch (TB_Val.Value)
                {
                    case 0: SS_com[0] = 0x50; break;
                    case 1: SS_com[0] = 0x51; break;
                    case 2: SS_com[0] = 0x52; break;
                    case 3: SS_com[0] = 0x53; break;
                    case 4: SS_com[0] = 0x54; break;
                    case 5: SS_com[0] = 0x55; break;
                    case 6: SS_com[0] = 0x56; break;
                    case 7: SS_com[0] = 0x57; break;
                    case 8: SS_com[0] = 0x58; break;
                    case 9: SS_com[0] = 0x59; break;
                    case 10: SS_com[0] = 0x5a; break;
                    case 11: SS_com[0] = 0x5b; break;
                    case 12: SS_com[0] = 0x5c; break;
                    case 13: SS_com[0] = 0x5d; break;
                    case 14: SS_com[0] = 0x5e; break;
                    case 15: SS_com[0] = 0x5f; break;
                }
            }
			//Servo 6 Command
            if(RB_S6.Checked == true)
            {
                SV_no = 6;

                switch (TB_Val.Value)
                {
                    case 0: SS_com[0] = 0x60; break;
                    case 1: SS_com[0] = 0x61; break;
                    case 2: SS_com[0] = 0x62; break;
                    case 3: SS_com[0] = 0x63; break;
                    case 4: SS_com[0] = 0x64; break;
                    case 5: SS_com[0] = 0x65; break;
                    case 6: SS_com[0] = 0x66; break;
                    case 7: SS_com[0] = 0x67; break;
                    case 8: SS_com[0] = 0x68; break;
                    case 9: SS_com[0] = 0x69; break;
                    case 10: SS_com[0] = 0x6a; break;
                    case 11: SS_com[0] = 0x6b; break;
                    case 12: SS_com[0] = 0x6c; break;
                    case 13: SS_com[0] = 0x6d; break;
                    case 14: SS_com[0] = 0x6e; break;
                    case 15: SS_com[0] = 0x6f; break;
                }
            }

            int SV_pos = TB_Val.Value;

            try
            {
                //command syntax fpga 0xNP
                // N = Servo number 1-E
                // P = Servo Position 0-F
				//FPGA Command
                if(CMB_Dev.SelectedIndex == 0)
                {
                    serial.Write(SS_com, 0, 1);
                }
                else
				//MBed Command
                {
                    serial.WriteLine("S:" + SV_no + "&" + SV_pos);
                }
                
            }
            catch 
            {

                MessageBox.Show("Please Open Serial Port");
            }

            
        }

        private void Servo_Load(object sender, EventArgs e)
        {
			//MBed Default
            CMB_Dev.SelectedIndex = 1;
        }
    }
}
