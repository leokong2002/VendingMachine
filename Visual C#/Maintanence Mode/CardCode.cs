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
    public partial class CardCode : Form
    {
        SerialPort serial;
        public CardCode(SerialPort sp)
        {
            InitializeComponent();
			//Access to serial port
            serial = sp;
        }

        private void BTN_Read_Click(object sender, EventArgs e)
        {
            if (serial.IsOpen)
            {
                try
                {
					//request barcode form Mbed
                    serial.WriteLine("B");
					//Hide Read Button
                    BTN_Read.Enabled = false;
                    string B_return = serial.ReadLine();
                    LBL_Return.Text = "Value Returned: " + B_return;
                    //MessageBox.Show(B_return);
					//decode returned barcode
                    BarcodeDecode(int.Parse(B_return));
                }
                catch (TimeoutException)
                {
                    MessageBox.Show("TIMEOUT ERROR");
                }
            }
            else
            {
                MessageBox.Show("Please Open Serial Port");
            }
        }
		//Barcode Decoder Binary Value of barcode coverted into integer
        private void BarcodeDecode(int cardn_no)
        {
            switch (cardn_no)
            {
                case 1: LBL_Allergen.Text = "Red"; TXT_Allergen.BackColor = Color.Red; break;
                case 10: LBL_Allergen.Text = "Green"; TXT_Allergen.BackColor = Color.Green; break;
                case 100: LBL_Allergen.Text = "Blue"; TXT_Allergen.BackColor = Color.Blue; break;
                case 1000: LBL_Allergen.Text = "Yellow"; TXT_Allergen.BackColor = Color.Yellow; break;
                case 11: LBL_Allergen.Text = "Orange"; TXT_Allergen.BackColor = Color.Orange; break;
                case 101: LBL_Allergen.Text = "White"; TXT_Allergen.BackColor = Color.White; break;
                case 1001: LBL_Allergen.Text = "Black"; TXT_Allergen.BackColor = Color.Black; break;
                case 0: LBL_Allergen.Text = "No Allergen"; TXT_Allergen.BackColor = Color.Gray; break;
                default: LBL_Allergen.Text = "ERROR"; TXT_Allergen.BackColor = Color.Gray; break;
            }

            BTN_Read.Enabled = true;
                
        }
    }
}
