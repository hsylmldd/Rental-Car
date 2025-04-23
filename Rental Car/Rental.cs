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
    public partial class Rental : Form
    {
        public Rental()
        {
            InitializeComponent();
            populate(); // Menampilkan data rental saat form dibuka
            fillcombo(); // Mengisi ComboBox mobil
            fillCustomer(); // Mengisi ComboBox pelanggan
            IdTb.ReadOnly = true; // Atur IdTb menjadi ReadOnly saat form diinisialisasi
        }

        // Koneksi ke database
        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False");

        // Class untuk menyimpan data mobil (RegNum dan Price)
        private class CarItem
        {
            public string RegNum { get; set; }
            public decimal Price { get; set; }

            public override string ToString()
            {
                return RegNum; // Tampilkan RegNum di ComboBox
            }
        }

        // Mengisi ComboBox hanya dengan mobil yang Available = 'YES'
        private void fillcombo()
        {
            try
            {
                Con.Open();
                string query = "SELECT RegNum, Price FROM CarTbl WHERE Available = 'YES'";
                SqlDataAdapter da = new SqlDataAdapter(query, Con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    List<CarItem> carList = new List<CarItem>();
                    foreach (DataRow row in dt.Rows)
                    {
                        carList.Add(new CarItem()
                        {
                            RegNum = row["RegNum"].ToString(),
                            Price = Convert.ToDecimal(row["Price"])
                        });
                    }
                    CarRegCb.DataSource = carList;
                    CarRegCb.DisplayMember = "RegNum";
                    CarRegCb.ValueMember = "RegNum";
                }
                else
                {
                    MessageBox.Show("Tidak ada mobil yang tersedia untuk disewa.");
                    CarRegCb.DataSource = null;
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

        // Mengisi ComboBox untuk Customer ID
        private void fillCustomer()
        {
            try
            {
                Con.Open();
                string query = "SELECT CustId FROM CustomerTbl";
                SqlDataAdapter da = new SqlDataAdapter(query, Con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                CustCb.ValueMember = "CustId";
                CustCb.DataSource = dt;
                CustCb.DisplayMember = "CustId";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer list: " + ex.Message);
            }
            finally
            {
                Con.Close();
            }
        }

        // Menampilkan data rental ke DataGridView
        private void populate()
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "SELECT RentId, CarReg, CustName, RentDate, ReturnDate, RentFee FROM RentalTbl";
                    SqlDataAdapter da = new SqlDataAdapter(query, Con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    RentDGV.DataSource = dt;
                    RentDGV.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rental data: " + ex.Message);
            }
        }

        // Event Load Form
        private void Rental_Load(object sender, EventArgs e)
        {
            fillcombo();
            fillCustomer();
            populate();
        }

        // Memperbarui status mobil menjadi "NO" (Disewakan)
        private void UpdateonRent(string carReg)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "UPDATE CarTbl SET Available = 'NO' WHERE RegNum = @RegNum";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@RegNum", carReg);
                        cmd.ExecuteNonQuery();
                        //MessageBox.Show("Car Successfully Updated");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating car status: " + ex.Message);
            }
        }

        // Memperbarui status mobil menjadi "YES" (Tersedia)
        private void UpdateonRentDelete(string carReg)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "UPDATE CarTbl SET Available = 'YES' WHERE RegNum = @RegNum";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@RegNum", carReg);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            //MessageBox.Show("Car Successfully Updated");
                        }
                        else
                        {
                            MessageBox.Show("No car found with the selected registration number.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating car status: " + ex.Message);
            }
        }

        private void fetchCustName()
        {
            Con.Open();
            string query = "SELECT CustName FROM CustomerTbl WHERE CustId=" + CustCb.SelectedValue.ToString();
            SqlCommand cmd = new SqlCommand(query, Con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                CustNameTb.Text = dr["CustName"].ToString();
            }
            Con.Close();
        }

        // Tombol Add Rental
        private void button3_Click(object sender, EventArgs e)
        {
            if (CarRegCb.SelectedItem == null || string.IsNullOrWhiteSpace(CustNameTb.Text) || string.IsNullOrWhiteSpace(FeesTb.Text))
            {
                MessageBox.Show("Missing information");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();

                    string query = "INSERT INTO RentalTbl (CarReg, CustName, RentDate, ReturnDate, RentFee) " +
                                   "VALUES (@CarReg, @CustName, @RentDate, @ReturnDate, @RentFee); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@CarReg", ((CarItem)CarRegCb.SelectedItem).RegNum); // Menggunakan RegNum dari objek CarItem
                        cmd.Parameters.AddWithValue("@CustName", CustNameTb.Text);
                        cmd.Parameters.AddWithValue("@RentDate", RentDate.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@ReturnDate", ReturnDate.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@RentFee", FeesTb.Text);

                        int newRentId = Convert.ToInt32(cmd.ExecuteScalar());
                        MessageBox.Show("Car Successfully Rented with ID: " + newRentId.ToString());
                        UpdateonRent(((CarItem)CarRegCb.SelectedItem).RegNum);
                    }

                }
                fillcombo(); // Tambahkan ini untuk mengisi ulang ComboBox

                populate();
                clearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding rental: " + ex.Message);
            }
        }

        // Mengisi TextBox dari DataGridView saat diklik
        private void RentDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < RentDGV.Rows.Count)
            {
                DataGridViewRow row = RentDGV.Rows[e.RowIndex];

                //Pastikan setiap sel tidak null sebelum diakses
                IdTb.Text = row.Cells[0].Value?.ToString() ?? ""; //Masukkan data dari RentId

                //Dapatkan RegNum dari DataGridView
                string regNum = row.Cells[1].Value?.ToString() ?? "";

                //Cari CarItem yang sesuai dari CarRegCb.DataSource
                CarItem selectedCar = ((List<CarItem>)CarRegCb.DataSource).FirstOrDefault(c => c.RegNum == regNum);
                CarRegCb.SelectedItem = selectedCar;  //Pilih CarItem yang sesuai di ComboBox

                CustNameTb.Text = row.Cells[2].Value?.ToString() ?? "";
                RentDate.Text = row.Cells[3].Value?.ToString() ?? "";
                ReturnDate.Text = row.Cells[4].Value?.ToString() ?? "";
                FeesTb.Text = row.Cells[5].Value?.ToString() ?? "";
            }
        }

        private void CustCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            fetchCustName();
        }

        // Event saat pilihan mobil berubah
        private void CarRegCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateFees();
        }

        // Event saat tanggal mulai berubah
        private void RentDate_ValueChanged(object sender, EventArgs e)
        {
            UpdateFees();
        }

        // Event saat tanggal selesai berubah
        private void ReturnDate_ValueChanged(object sender, EventArgs e)
        {
            UpdateFees();
        }

        // Fungsi untuk menghitung dan menampilkan biaya rental
        private void UpdateFees()
        {
            // Pastikan CarRegCb memiliki item yang dipilih
            if (CarRegCb.SelectedItem != null && CarRegCb.SelectedItem is CarItem)
            {
                CarItem selectedCar = (CarItem)CarRegCb.SelectedItem;

                // Hitung selisih hari
                TimeSpan timeSpan = ReturnDate.Value - RentDate.Value;
                int days = timeSpan.Days + 1; // Tambah 1 agar tanggal terakhir dihitung

                // Tentukan harga mobil per hari
                decimal dailyPrice = selectedCar.Price;

                // Hitung total biaya
                decimal totalFees = dailyPrice * days;

                // Tampilkan biaya di FeesTb
                FeesTb.Text = totalFees.ToString();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            MainForm main = new MainForm();
            main.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IdTb.Text))
            {
                MessageBox.Show("Pilih data rental yang ingin dihapus!");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();

                    // Periksa apakah Rent ID ada di database
                    string checkQuery = "SELECT COUNT(*) FROM RentalTbl WHERE RentId = @RentId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, Con))
                    {
                        checkCmd.Parameters.AddWithValue("@RentId", IdTb.Text);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count == 0)
                        {
                            MessageBox.Show("Data rental tidak ditemukan.");
                            return;
                        }
                    }

                    // Dapatkan CarReg sebelum menghapus data rental
                    string getCarRegQuery = "SELECT CarReg FROM RentalTbl WHERE RentId = @RentId";
                    string carRegToDelete = "";

                    using (SqlCommand getCarRegCmd = new SqlCommand(getCarRegQuery, Con))
                    {
                        getCarRegCmd.Parameters.AddWithValue("@RentId", IdTb.Text);
                        SqlDataReader reader = getCarRegCmd.ExecuteReader();

                        if (reader.Read())
                        {
                            carRegToDelete = reader["CarReg"].ToString();
                        }
                        reader.Close();
                    }

                    // Menghapus data rental berdasarkan Rent ID
                    string query = "DELETE FROM RentalTbl WHERE RentId = @RentId";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@RentId", IdTb.Text);
                        cmd.ExecuteNonQuery();
                    }
                    if (!string.IsNullOrEmpty(carRegToDelete))
                    {
                        UpdateonRentDelete(carRegToDelete);
                    }
                    else
                    {
                        MessageBox.Show("CarReg tidak ditemukan atau sudah dihapus.");
                    }

                    MessageBox.Show("Data rental berhasil dihapus dan mobil kembali tersedia.");

                }
                fillcombo(); // Tambahkan ini untuk mengisi ulang ComboBox
                populate();
                clearFields();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat menghapus rental: " + ex.Message);
            }
        }


        // Fungsi untuk mengosongkan input setelah delete
        private void clearFields()
        {
            IdTb.Clear();
            CustNameTb.Clear();
            FeesTb.Clear();
        }
    }
}
