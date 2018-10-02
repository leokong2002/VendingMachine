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
    public partial class tof : Form
    {
        SerialPort serial = new SerialPort();
        public tof(SerialPort sp)
        {
            InitializeComponent();
            //access com port
            serial = sp;
        }

        private void BTN_Read_Click(object sender, EventArgs e)
        {
            //Maximum return value for time of flight sensor
            const int Max_read = 255;
            //String for read data
            string Data_in = null;

            PB_Value.Maximum = Max_read;

            try
            {
                //send command
                serial.WriteLine("T");
                BTN_Read.Enabled = false;
                //clear input buffer and read serial
                serial.DiscardInBuffer();
                Data_in = serial.ReadLine();
            }
            //Error read timeout 
            catch (TimeoutException)
            {
                MessageBox.Show("ERROR: Timeout");
            }
            //error com port closed 
            catch (InvalidOperationException)
            {
                MessageBox.Show("ERROR: Open Serial Port");
            }
            //if input data has been updated
            if (Data_in != null)
            {
                //display raw data 
                LBL_Return.Text = Data_in.ToString();
                
                PB_Value.Value = int.Parse(Data_in);

                BTN_Read.Enabled = true;
            }

        }
    }
}
