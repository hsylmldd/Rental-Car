using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Rental_Car
{
    public partial class Customer : Form
    {
        public Customer()
        {
            InitializeComponent();
            populate(); 
            IdTb.ReadOnly = true; 
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e) // bt kembali ke menu utama
        {
            this.Hide();
            MainForm main = new MainForm();
            main.Show();
        }

        // 🔹 Method untuk menampilkan data ke DataGridView
        private void populate()
        {
            using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
            {
                Con.Open();
                string query = "SELECT CustId, CustName, CustAdd, Phone FROM CustomerTbl";
                SqlDataAdapter da = new SqlDataAdapter(query, Con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                CustomerDGV.DataSource = dt;
                Con.Close();
            }
        }

        // 🔹 INSERT (Tambah Customer)
        private void button3_Click(object sender, EventArgs e) // Button Add
        {
            if (NameTb.Text == "" || AddressTb.Text == "" || PhoneTb.Text == "")
            {
                MessageBox.Show("Missing information");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "INSERT INTO CustomerTbl (CustName, CustAdd, Phone) VALUES (@Name, @Address, @Phone); SELECT SCOPE_IDENTITY();"; // Mengambil ID baru yang dihasilkan
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Name", NameTb.Text);
                        cmd.Parameters.AddWithValue("@Address", AddressTb.Text);
                        cmd.Parameters.AddWithValue("@Phone", PhoneTb.Text);

                        int newCustId = Convert.ToInt32(cmd.ExecuteScalar()); // Mendapatkan ID baru dari database
                        MessageBox.Show($"Customer Successfully Added with ID: {newCustId}"); // Menampilkan ID baru
                    }

                    populate(); // Refresh DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // 🔹 UPDATE (Edit Customer)
        private void button4_Click(object sender, EventArgs e) // Button Edit
        {
            if (string.IsNullOrWhiteSpace(IdTb.Text) || string.IsNullOrWhiteSpace(NameTb.Text) || string.IsNullOrWhiteSpace(AddressTb.Text) || string.IsNullOrWhiteSpace(PhoneTb.Text))
            {
                MessageBox.Show("Missing information");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "UPDATE CustomerTbl SET CustName=@Name, CustAdd=@Address, Phone=@Phone WHERE CustId=@Id";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Id", IdTb.Text); // Pastikan IdTb tidak diisi oleh pengguna
                        cmd.Parameters.AddWithValue("@Name", NameTb.Text);
                        cmd.Parameters.AddWithValue("@Address", AddressTb.Text);
                        cmd.Parameters.AddWithValue("@Phone", PhoneTb.Text);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Customer Successfully Updated");
                    populate(); // Refresh DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // 🔹 DELETE (Hapus Customer)
        private void button5_Click(object sender, EventArgs e) // Button Delete
        {
            if (string.IsNullOrWhiteSpace(IdTb.Text))
            {
                MessageBox.Show("Enter the Customer ID to delete");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "DELETE FROM CustomerTbl WHERE CustId=@Id";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Id", IdTb.Text);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Customer Successfully Deleted");
                    populate(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // 🔹 Event untuk mengambil data dari DataGridView ke TextBox
        private void CustomerDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow row = CustomerDGV.Rows[e.RowIndex];

                // Ambil data berdasarkan kolom
                IdTb.Text = row.Cells["CustId"].Value?.ToString(); 
                NameTb.Text = row.Cells["CustName"].Value?.ToString();
                AddressTb.Text = row.Cells["CustAdd"].Value?.ToString();
                PhoneTb.Text = row.Cells["Phone"].Value?.ToString();
            }
        }

        private void Customer_Load(object sender, EventArgs e)
        {
            populate(); 
        }
    }
}
