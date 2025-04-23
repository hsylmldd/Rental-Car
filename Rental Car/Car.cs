using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Rental_Car
{
    public partial class Car : Form
    {
        public Car()
        {
            InitializeComponent();
        }
        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False");
        private void fillAvailable()
        {
            try
            {
                Con.Open();
                // Ambil data unik dari kolom Available
                string query = "SELECT DISTINCT Available FROM CarTbl";
                SqlDataAdapter da = new SqlDataAdapter(query, Con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    // Bersihkan item yang ada sebelumnya
                    Search.Items.Clear();

                    // Tambahkan item "Available" dan "Rented" secara manual
                    Search.Items.Add("Available");
                    Search.Items.Add("Rented");

                    // Set SelectedIndex ke -1 agar tidak ada item yang terpilih secara default
                    Search.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("Tidak ada mobil yang tersedia untuk disewa.");
                    Search.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading available cars: " + ex.Message);
            }
            finally
            {
                Con.Close();
            }
        }


        private void Car_Load(object sender, EventArgs e)
        {
            populate();
            fillAvailable();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Menutup aplikasi
        }

        private void populate()
        {
            using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
            {
                Con.Open();
                string query = "SELECT * FROM CarTbl";
                SqlDataAdapter da = new SqlDataAdapter(query, Con);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                var ds = new DataSet();
                da.Fill(ds);
                CarsDGV.DataSource = ds.Tables[0];
                Con.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e) // Button Add
        {
            if (RegNumTb.Text == "" || BrandTb.Text == "" || ModelTb.Text == "" || PriceTb.Text == "" || AvailableCb.SelectedItem == null)
            {
                MessageBox.Show("Missing information");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "INSERT INTO CarTbl (RegNum, Brand, Model, Price, Available) VALUES (@RegNum, @Brand, @Model, @Price, @Available)";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@RegNum", RegNumTb.Text);
                        cmd.Parameters.AddWithValue("@Brand", BrandTb.Text);
                        cmd.Parameters.AddWithValue("@Model", ModelTb.Text);
                        cmd.Parameters.AddWithValue("@Price", PriceTb.Text);
                        cmd.Parameters.AddWithValue("@Available", AvailableCb.SelectedItem.ToString());

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Car Successfully Added");
                    populate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void CarsDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Pastikan indeks baris valid
            {
                DataGridViewRow row = CarsDGV.Rows[e.RowIndex]; // Ambil baris yang diklik

                // Pastikan urutan kolom benar
                RegNumTb.Text = row.Cells["RegNum"].Value?.ToString();
                BrandTb.Text = row.Cells["Brand"].Value?.ToString();
                ModelTb.Text = row.Cells["Model"].Value?.ToString();
                PriceTb.Text = row.Cells["Price"].Value?.ToString(); // Ambil dari kolom Price

                // Pastikan AvailableCb diisi dengan nilai yang benar
                string availableValue = row.Cells["Available"].Value?.ToString();
                if (!string.IsNullOrEmpty(availableValue) && AvailableCb.Items.Contains(availableValue))
                {
                    AvailableCb.SelectedItem = availableValue;
                }
                else
                {
                    AvailableCb.SelectedIndex = -1; // Kosongkan jika tidak cocok
                }
            }
        }


        private void button4_Click(object sender, EventArgs e) // Button Edit
        {
            if (RegNumTb.Text == "" || BrandTb.Text == "" || ModelTb.Text == "" || PriceTb.Text == "" || AvailableCb.SelectedItem == null)
            {
                MessageBox.Show("Missing information");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "UPDATE CarTbl SET Brand = @Brand, Model = @Model, Price = @Price, Available = @Available WHERE RegNum = @RegNum";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@RegNum", RegNumTb.Text);
                        cmd.Parameters.AddWithValue("@Brand", BrandTb.Text);
                        cmd.Parameters.AddWithValue("@Model", ModelTb.Text);
                        cmd.Parameters.AddWithValue("@Price", PriceTb.Text);
                        cmd.Parameters.AddWithValue("@Available", AvailableCb.SelectedItem.ToString());

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Car Successfully Updated");
                    populate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e) // Button Delete
        {
            if (RegNumTb.Text == "")
            {
                MessageBox.Show("Select a car to delete");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "DELETE FROM CarTbl WHERE RegNum = @RegNum";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@RegNum", RegNumTb.Text);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Car Successfully Deleted");
                    populate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.Hide();
            MainForm main = new MainForm();
            main.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            populate();
        }

        private void Search_SelectionChangeCommitted(object sender, EventArgs e)
        {
            using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
            {
                string flag = "";

                // Periksa apakah item yang dipilih adalah "Available" atau "Rented"
                if (Search.SelectedItem != null)
                {
                    if (Search.SelectedItem.ToString() == "Available")
                    {
                        flag = "Yes"; // Konversi "Available" ke "Yes"
                    }
                    else if (Search.SelectedItem.ToString() == "Rented")
                    {
                        flag = "No"; // Konversi "Rented" ke "No"
                    }

                    Con.Open();
                    string query = "SELECT * FROM CarTbl WHERE Available = '" + flag + "'";

                    SqlDataAdapter da = new SqlDataAdapter(query, Con);
                    SqlCommandBuilder builder = new SqlCommandBuilder(da);
                    var ds = new DataSet();
                    da.Fill(ds);
                    CarsDGV.DataSource = ds.Tables[0];
                    Con.Close();
                }
                else
                {
                    // Jika tidak ada item yang dipilih, tampilkan semua data
                    populate();
                }
            }
        }
    }
}
