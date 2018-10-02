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
    public partial class Sort : Form
    {
        SerialPort serial;
        public Sort(SerialPort sp)
        {
            InitializeComponent();
			//Access Serial Port
            serial = sp;
        }

        private void BTN_Sort_Click(object sender, EventArgs e)
        {
            string G_Return = null;
            try
            {
				//Clear Radio Buttons
                RBTN_Tow1.Checked = false;
                RBTN_Tow2.Checked = false;
				//Send Sort Command
                serial.WriteLine("G");
				//Hide Buttons
                BTN_Sort.Enabled = false;

                G_Return = serial.ReadLine();

                switch (int.Parse(G_Return))
                {
					//Towers Full
                    case 0: RBTN_Tow1.Checked = false; RBTN_Tow2.Checked = false; MessageBox.Show("Towers Full"); break;
					//Sorted into Tower 1
                    case 1: RBTN_Tow1.Checked = true; RBTN_Tow2.Checked = false; BTN_Rst(); break;
					//Sorted into Tower 2
                    case 2: RBTN_Tow1.Checked = false; RBTN_Tow2.Checked = true; BTN_Rst(); break;
					//Error
                    default: RBTN_Tow1.Checked = false; RBTN_Tow2.Checked = false; BTN_Sort.Enabled = true; throw new System.ArgumentException("Colour Read Error"); break;
                }
            }
            catch (TimeoutException)
            {
                MessageBox.Show("ERROR TIMEOUT");
                BTN_Rst();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please Open Serial Port");
                BTN_Rst();


            }
            catch (ArgumentException)
            {
                MessageBox.Show("Colour Error");
                BTN_Rst();
            }
			catch
			{
                BTN_Rst();

            }
        }

		//reset Form
        private void BTN_Rst()
        {
            Thread.Sleep(500);
            BTN_Sort.Enabled = true;
        }

        private void Sort_Load(object sender, EventArgs e)
        {
			//Radio buttons cleared on form load
            RBTN_Tow1.Checked = false;
            RBTN_Tow2.Checked = false;
        }
    }
}
