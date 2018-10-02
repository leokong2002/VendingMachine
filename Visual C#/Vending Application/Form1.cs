using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.IO.Ports;
using System.Speech;
using System.Speech.Synthesis;

namespace AVSSoftware
{
    public partial class Form1 : Form
    {
        //Speech functionallity
        SpeechSynthesizer speechSynthesizerObj;

        //serial port
        SerialPort serial;

        //Language Changer
        lang Language = new lang();

        private bool card_read;
        private string start = "Please Select Language. Seleccione idioma";


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            card_read = false;

            serial = new SerialPort();
			//speech Sythesizer
            speechSynthesizerObj = new SpeechSynthesizer();
            
            LBL_Allergen.Text = start;
            //Auto Select Serial Port
			//Tries every availbe serial port on pc
            foreach(string p in SerialPort.GetPortNames())
            {
				//Set read timeout low to speed up auto conenct
                serial.ReadTimeout = 50;
                try
                {
					//Mbed Returns 0 on ?
                    serial.PortName = p;
                    serial.Open();
                    Thread.Sleep(20);
                    
                    Console.WriteLine("Trying Port " + p);
                    serial.WriteLine("?");

                    string test_return = serial.ReadLine();
                    int t_return = int.Parse(test_return);
                    if (t_return == 0)
                    {
                        Console.WriteLine("MBed Connected To " + p);
						Messagebox.showdialog("MBed Connected To " + p);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Mbed Not Connected to " + p);
						//if not connected close port before trying to connect
                        serial.Close();
                    }
                }
                catch (TimeoutException)
                {
					//no return from ?
                    Console.WriteLine("Mbed Not Connected to " + p);
                }
                catch(InvalidOperationException)
                {
					//port unavailbe
                    Console.WriteLine("Unable to open " + p);
                }
                catch (UnauthorizedAccessException)
                {
					//Port already open
                    Console.WriteLine("Port " + p + " is Already Open");
                }
                catch (FormatException)
                {
					//wrong value returned	
                    Console.WriteLine("Parse Error");
                }
            }
            serial.ReadTimeout = 5000;

        }

        private void BTN_Dispense_Click(object sender, EventArgs e)
        {
            try
            {
				//dispensing snacks selected
                serial.WriteLine("D&" + NUD_Disp.Value);
                int D_return = int.Parse(serial.ReadLine());

                Console.WriteLine("dispensing: " + NUD_Disp.Value);
                speechSynthesizerObj.SpeakAsync(Language.Disp(D_return));

				//return to start screen
				//hide all except flag buttons
                LBL_Allergen.Visible = false;
                LBL_Allergen.Text = "";
                BTN_Dispense.Visible = false;
                NUD_Disp.Visible = false;
                BTN_up.Visible = false;
                BTN_Down.Visible = false;
                BTN_ENG.Visible = true;
                BTN_Esp.Visible = true;
                NUD_Disp.Value = 0;
                TXT_colour.BackColor = Color.Gray;
            }
            catch
            {
                speechSynthesizerObj.SpeakAsync(Language.Error());
            }
        }
       
        //team Picture
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            Form2 shame = new Form2();

            shame.Show();
        }


        private void Start()
        {
            
            try
            {
				//request number of snacks
                serial.WriteLine("X");
                int QD_return = int.Parse(serial.ReadLine());
                NUD_Disp.Maximum = QD_return;

                speechSynthesizerObj.SpeakAsync(Language.Greeting());
                LBL_Allergen.Text = Language.Greeting();
                

                Barcode();

            }
            catch
            {
                LBL_Allergen.Visible = false;
                NUD_Disp.Visible = false;
                BTN_Dispense.Visible = false;
                BTN_up.Visible = false;
                BTN_Down.Visible = false;

                BTN_ENG.Visible = true;
                BTN_Esp.Visible = true;
                speechSynthesizerObj.SpeakAsync(Language.Error());
            }
        }

        //Barcode Decoder and Serial Send
        public void Barcode()
        {
            card_read = !card_read;
			//Hide All except logo and label
            NUD_Disp.Visible = false;
            BTN_Dispense.Visible = false;
            BTN_ENG.Visible = false;
            BTN_Esp.Visible = false;

            BTN_up.Visible = false;
            BTN_Down.Visible = false;
			//set button icons to thumbs up/down
            BTN_Down.BackgroundImage = Properties.Resources.Unlike_icon;
            BTN_up.BackgroundImage = Properties.Resources.Hand_thumbs_up_3_icon;

            string card_return = null;
            try
            {
				//request Barcode
                serial.WriteLine("B");
                card_return = serial.ReadLine();
                
                if (card_return != null)
                {
                    int B_return = int.Parse(card_return);
                    Console.WriteLine("Barcode: " + B_return);
					//Barcde Decoder
                    switch (B_return)
                    {
                        case 0:
                            speechSynthesizerObj.SpeakAsync(Language.Allergen(B_return));
                            LBL_Allergen.Text = Language.Allergen(B_return);
                            TXT_colour.BackColor = this.BackColor;
                            break;  //nun
                        case 1:
                            speechSynthesizerObj.SpeakAsync(Language.Allergen(B_return));
                            LBL_Allergen.Text = Language.Allergen(B_return);
                            TXT_colour.BackColor = Color.Red;
                            break;  //red
                        case 10:
                            speechSynthesizerObj.SpeakAsync(Language.Allergen(B_return));
                            LBL_Allergen.Text = Language.Allergen(B_return);
                            TXT_colour.BackColor = Color.Green;
                            break;  //green
                        case 100:
                            speechSynthesizerObj.SpeakAsync(Language.Allergen(B_return));
                            LBL_Allergen.Text = Language.Allergen(B_return);
                            TXT_colour.BackColor = Color.Blue;
                            break;  //blue
                        case 1000:
                            speechSynthesizerObj.SpeakAsync(Language.Allergen(B_return));
                            LBL_Allergen.Text = Language.Allergen(B_return);
                            TXT_colour.BackColor = Color.Yellow;
                            break;  //yellow
                        case 11:
                            speechSynthesizerObj.SpeakAsync(Language.Allergen(B_return));
                            LBL_Allergen.Text = Language.Allergen(B_return);
                            TXT_colour.BackColor = Color.Orange;
                            break;  //orange
                        case 101:
                            speechSynthesizerObj.SpeakAsync(Language.Allergen(B_return));
                            LBL_Allergen.Text = Language.Allergen(B_return);
                            TXT_colour.BackColor = Color.White;
                            break;  //white
                        case 1001:
                            speechSynthesizerObj.SpeakAsync(Language.Allergen(B_return));
                            LBL_Allergen.Text = Language.Allergen(B_return);
                            TXT_colour.BackColor = this.BackColor;
                            break;  //black
                                    //default: speechSynthesizerObj.SpeakAsync(Language.Error()); break; //not working
                    }

					//Show Accept/Decline Buttons
                    BTN_Down.Visible = true;
                    BTN_up.Visible = true;  
                }
            }
            catch (TimeoutException)
            {
                //loop if barcode times out
                Barcode();
            }
            catch
            {
                speechSynthesizerObj.SpeakAsync(Language.Error());
            }
        }

        private void Dispence()
        {
            Console.WriteLine("In Dispence");
			//Hide Flag Buttons
            BTN_Esp.Visible = false;
            BTN_ENG.Visible = false;


			//show Num-up-down box and Dispense button
            NUD_Disp.Visible = true;
            BTN_Dispense.Visible = true;

			//Change button icons to Up/down Buttons
            BTN_Down.BackgroundImage = Properties.Resources.Arrow_down_icon;
            BTN_up.BackgroundImage = Properties.Resources.Arrow_up_icon;

            LBL_Allergen.Text = Language.Sel_disp();
            speechSynthesizerObj.SpeakAsync(Language.Sel_disp());
        }

        //language selection buttons
        private void BTN_Esp_Click(object sender, EventArgs e)
        {
			//Change Lanugage and Culture to Spanish
            CultureInfo esp = new CultureInfo("es-ES");
            speechSynthesizerObj.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, esp);
            Language.Set_lang = 1;
            Console.WriteLine("Language Spanish");
            Start();
        }


        private void BTN_ENG_Click(object sender, EventArgs e)
        {
			//Change Language and Culture to English	
            CultureInfo eng = new CultureInfo("en-GB");
            speechSynthesizerObj.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, eng);
            Language.Set_lang = 0;
            Console.WriteLine("language English");
            Start();

        }
        //Updown Buttons
        private void BTN_up_Click(object sender, EventArgs e)
        {
            if (card_read == false)
            {
				//if in dispence operate num-up-down box
                NUD_Disp.UpButton();
            }
            else
            {
				//if in barcode accept
                Console.WriteLine("Barcode Accepted");
				//Barcode Accepted, go to Dispense mode
                Dispence();

                card_read = !card_read;
            }
        }

        private void BTN_Down_Click(object sender, EventArgs e)
        {
            if (card_read == false)
            {
				//if in dispence operate num-up-down box
                NUD_Disp.DownButton();
            }
            else
            {
                Console.WriteLine("Barcode Declined");
				//Barcode Decline, retry barcode
                Barcode();

                card_read = !card_read;
            }
        }

     
    }
}
