using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rental_Car
{
    public partial class Splash: Form
    {
        public Splash()
        {
            InitializeComponent();


        }
        int starpoint = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            starpoint += 1;
            Myprogress.Value = starpoint;
            Percentage.Text = ""+starpoint;
            if (Myprogress.Value == 100)
            {
                Myprogress.Value = 0;
                timer1.Stop();
                Login log = new Login();
                log.Show();
                this.Hide();

            }
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
