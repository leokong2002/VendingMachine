using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//About Page
namespace AVS_Maintanence
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

		//Close Form on Button Click
        private void BTN_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void About_Load(object sender, EventArgs e)
        {
            
        }
    }
}
