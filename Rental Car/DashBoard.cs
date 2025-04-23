using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Rental_Car
{
    public partial class DashBoard : Form
    {
        public DashBoard()
        {
            InitializeComponent();
        }

        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False");

        private void DashBoard_Load(object sender, EventArgs e)
        {
            
            
            
            
            
            
            
            
            
            
            try
            {
                Con.Open();

                // Menghitung jumlah mobil
                string querycar = "SELECT COUNT(*) FROM CarTbl";
                SqlDataAdapter sda = new SqlDataAdapter(querycar, Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                Carlbl.Text = dt.Rows[0][0].ToString();

                // Menghitung jumlah pelanggan
                string querycust = "SELECT COUNT(*) FROM CustomerTbl";
                SqlDataAdapter sda1 = new SqlDataAdapter(querycust, Con);
                DataTable dt1 = new DataTable();
                sda1.Fill(dt1);
                Custlbl.Text = dt1.Rows[0][0].ToString();

                // Menghitung jumlah pengguna
                string queryuser = "SELECT COUNT(*) FROM UserTbl";
                SqlDataAdapter sda2 = new SqlDataAdapter(queryuser, Con);
                DataTable dt2 = new DataTable();
                sda2.Fill(dt2);
                Userlbl.Text = dt2.Rows[0][0].ToString();

                // Menghitung total pendapatan hanya dari TotalRentFee di ReturnTbl
                string queryRevenue = "SELECT SUM(TotalRentFee) FROM ReturnTbl";
                SqlCommand cmdRevenue = new SqlCommand(queryRevenue, Con);
                object result = cmdRevenue.ExecuteScalar();

                if (result != DBNull.Value)
                {
                    decimal totalRevenue = Convert.ToDecimal(result);
                    Revenuelbl.Text = "Rp " + totalRevenue.ToString("N0"); // Format sebagai mata uang
                }
                else
                {
                    Revenuelbl.Text = "Rp 0"; // Jika tidak ada data, tampilkan Rp 0
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
            finally
            {
                Con.Close();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm main = new MainForm();
            main.Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            MainForm main = new MainForm();
            main.Show();
        }
    }
}
