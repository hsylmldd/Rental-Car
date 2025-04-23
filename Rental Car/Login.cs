using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Rental_Car
{
    public partial class Login: Form
    {
        public Login()
        {
            InitializeComponent();
        }

        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False");
        private void label8_Click(object sender, EventArgs e)
        {
            Uname.Text = "";
            PassTb.Text = "";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query = "select count(*) from UserTbl where Uname= '" + Uname.Text + "' and Upass='" + PassTb.Text + "'";
            Con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(query, Con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows[0][0].ToString() == "1")
            {
                MainForm mainform = new MainForm();
                mainform.Show();
                this.Hide();
            }

            else
            {
                MessageBox.Show("Wrong Username or Password");
            }
            Con.Close();
        }
    }
}
        