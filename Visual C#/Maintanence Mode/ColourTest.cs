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
    public partial class ColourTest : Form
    {
        private SerialPort serial;
        public ColourTest(SerialPort port)
        {
            InitializeComponent();
            //access com port
            serial = port;
        }
        /*---------------------------------------------------
         * Data from Colour Sensor Expected Fromat
         * [Red<space>Green<space>Blue<space>Clear<Newline>]
         --------------------------------------------------*/
        private void BTN_Read_Click(object sender, EventArgs e)
        {
            //string for data
            string Data_In = null;
            
            try
            {
                //send Command
                serial.WriteLine("C");
                BTN_Read.Enabled = false;
                //discard buffer
                serial.DiscardInBuffer();
                //Read data up to newline
                Data_In = serial.ReadLine();
            }
            //Error Timeout
            catch (TimeoutException)
            {
                MessageBox.Show("ERROR: Timeout");
                BTN_Read.Enabled = true;
            }
            //Error Com port not open
            catch (InvalidOperationException)
            {
                MessageBox.Show("ERROR: Open Serial Port");
                BTN_Read.Enabled = true;
            }

            //If data sting has been updated
            if (Data_In != null)
            {
                //split data using <space>
                string[] in_Data = Data_In.Split(' ');

                //clear vaule
                int W_col = int.Parse(in_Data[3]);

                //Convert raw value to a byte (0-255) using clear value
                float R_col = (float.Parse(in_Data[0]) * 255) / W_col;
                float G_col = (float.Parse(in_Data[1]) * 255) / W_col;
                float B_col = (float.Parse(in_Data[2]) * 255) / W_col;

                //display raw data
                LBL_Red.Text = in_Data[0];
                LBL_Green.Text = in_Data[1];
                LBL_Blue.Text = in_Data[2];
                LBL_Clear.Text = in_Data[3];

                //convert float to integers for RGB Values
                int R = (int)R_col;
                int G = (int)G_col;
                int B = (int)B_col;

                //Display RGB values
                LBL_Col_red.Text = R.ToString();
                LBL_Col_Green.Text = G.ToString();
                LBL_Col_Blue.Text = B.ToString();

                //Create new colour from RGB Values
                Color sample_Colour = new Color();
                sample_Colour = Color.FromArgb(R, G, B);
                //Display Colour using textbox
                TXT_Colour.BackColor = sample_Colour;
                BTN_Read.Enabled = true;
            }
            else
            {
                TXT_Colour.BackColor = Color.Black;
                BTN_Read.Enabled = true;
            }
            
        }
    }
}
