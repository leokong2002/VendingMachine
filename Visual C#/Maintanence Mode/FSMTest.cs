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
    public partial class FSMTest : Form
    {
        private SerialPort serial;

        const int ERROR = 1;
        
        public FSMTest(SerialPort sp)
        {
            InitializeComponent();
			//access to serial port
            serial = sp;
        }

        private void BTN_Test_Click(object sender, EventArgs e)
        {
            try
            {
				//Dispense Mbed Command
                serial.WriteLine("D&" + NUD_Num.Value);
				//print to txt box
                TXT_Debug.AppendText(">> D&" + NUD_Num.Value + Environment.NewLine);

                string D_return_String = serial.ReadLine();

                int D_return = int.Parse(D_return_String);
				//error handling
                if (D_return == ERROR)
                {
                    MessageBox.Show("RETURNED: Error");
                }
                else
                {
                    TXT_Debug.AppendText("<< " + D_return_String + Environment.NewLine);
                }
            }
            catch (TimeoutException)
            {
                MessageBox.Show("ERROR TIMEOUT");
                TXT_Debug.AppendText("E> Timeout" + Environment.NewLine);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please Open Serial Port");
            }
            catch
            {
                MessageBox.Show("ERROR");
            }
               
        }

        private void BTN_Start_Click(object sender, EventArgs e)
        {
            string B_return = null;
            string S_Com = null;
            TXT_Debug.Text = "";
            try
            {
				//Query Mbed Availible
                serial.WriteLine("?");

				//Print to txt box
                TXT_Debug.AppendText(">> ?" + Environment.NewLine);

                S_Com = serial.ReadLine();
                TXT_Debug.AppendText("<< " + S_Com + Environment.NewLine);
                int S_return = int.Parse(S_Com);

				//error handling
                if(S_return == ERROR && S_Com != null)
                {
                    MessageBox.Show("RETURNED: ERROR");
                }
                else
                {
					//Request Barcode
                    serial.WriteLine("B");
                    TXT_Debug.AppendText(">> B" + Environment.NewLine);

                    B_return = serial.ReadLine();

                    TXT_Debug.AppendText("<< " + B_return + Environment.NewLine);
					//if barcode returned
                    if (B_return != null)
                    {
						//decode Barcode
                        CodeDecode(B_return);                       
						TXT_Debug.AppendText(">> X" + Environment.NewLine);
                        //Request Number of Snacks
						serial.WriteLine("X");
                    }
                     
                    string D_return = serial.ReadLine();
                    TXT_Debug.AppendText("<< " + D_return + Environment.NewLine);
                    
					//Set Maximum number of snacks
                    NUD_Num.Maximum = int.Parse(D_return);
                }
            }
            catch (TimeoutException)
            {
                MessageBox.Show("ERROR TIMEOUT");
                TXT_Debug.AppendText("E> Timeout" + Environment.NewLine);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please Open Serial Port");
            }
        }

		//Barcode Decoder
        private void CodeDecode(string value)
        {
            int code;
            try
            {
				//uses binary value of barcode converted to integer
                code = int.Parse(value);

                switch (code)
                {
                    case 1: TXT_Debug.AppendText("<< Allergen Red" + Environment.NewLine); break;
                    case 10: TXT_Debug.AppendText("<< Allergen Green" + Environment.NewLine); break;
                    case 100: TXT_Debug.AppendText("<< Allergen Blue" + Environment.NewLine); break;
                    case 1000: TXT_Debug.AppendText("<< Allergen Yellow" + Environment.NewLine); break;
                    case 11: TXT_Debug.AppendText("<< Allergen Orange" + Environment.NewLine); break;
                    case 101: TXT_Debug.AppendText("<< Allergen White" + Environment.NewLine); break;
                    case 1001: TXT_Debug.AppendText("<< Allergen Black" + Environment.NewLine); break;
                    case 0: TXT_Debug.AppendText("<< Allergen None" + Environment.NewLine); break;
                    default: TXT_Debug.AppendText("<< Decode Error" + Environment.NewLine); break;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Decode Error");
            }

        }
    }
}
