using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Rental_Car
{
    public partial class Return : Form
    {
        public Return()
        {
            InitializeComponent();
        }

        // Mengisi DataGridView dengan data dari tabel RentalTbl
        private void populate()
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "SELECT * FROM RentalTbl";
                    SqlDataAdapter da = new SqlDataAdapter(query, Con);
                    var ds = new DataSet();
                    da.Fill(ds);
                    RentDGV.DataSource = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rental data: " + ex.Message);
            }
        }

        // Mengisi DataGridView dengan data dari tabel ReturnTbl
        private void populateRet()
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "SELECT * FROM ReturnTbl";
                    SqlDataAdapter da = new SqlDataAdapter(query, Con);
                    var ds = new DataSet();
                    da.Fill(ds);
                    ReturnDGV.DataSource = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading return data: " + ex.Message);
            }
        }

        // Event yang dipanggil saat form Return dimuat
        private void Return_Load(object sender, EventArgs e)
        {
            populate();    // Mengisi data rental
            populateRet(); // Mengisi data pengembalian
        }

        // Mengisi TextBox berdasarkan baris yang dipilih di RentDGV
        private void RentDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = RentDGV.Rows[e.RowIndex];
                CarIdTb.Text = row.Cells[1].Value?.ToString() ?? "";
                CustNameTb.Text = row.Cells[2].Value?.ToString() ?? ""; // Nama pelanggan atau ID pelanggan tergantung desain tabel RentalTbl
                ReturnDate.Value = Convert.ToDateTime(row.Cells[4].Value);

                DateTime d1 = ReturnDate.Value.Date;
                DateTime d2 = DateTime.Now.Date;
                int NrofDays = (d2 - d1).Days;

                if (NrofDays <= 0)
                {
                    DelayTb.Text = "No Delay";
                    FineTb.Text = "0";
                }
                else
                {
                    DelayTb.Text = NrofDays.ToString();
                    FineTb.Text = (NrofDays * 25000).ToString(); // Misalnya Rp25.000 per hari keterlambatan
                }
            }
        }

        // Tombol untuk kembali ke MainForm
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm main = new MainForm();
            main.Show();
        }

        // Tombol untuk menutup form
        private void label3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Tombol untuk menyimpan pengembalian mobil ke database
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CarIdTb.Text) || string.IsNullOrWhiteSpace(CustNameTb.Text) || string.IsNullOrWhiteSpace(FineTb.Text) || string.IsNullOrWhiteSpace(DelayTb.Text))
            {
                MessageBox.Show("Missing Information");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();

                    decimal fine = Convert.ToDecimal(FineTb.Text); // Denda diambil dari input pengguna
                    decimal rentFee = 0; // Inisialisasi rentFee

                    // Ambil nilai RentFee dari tabel RentalTbl
                    string getRentFeeQuery = "SELECT RentFee FROM RentalTbl WHERE CarReg = @CarReg";
                    using (SqlCommand getRentFeeCmd = new SqlCommand(getRentFeeQuery, Con))
                    {
                        getRentFeeCmd.Parameters.AddWithValue("@CarReg", CarIdTb.Text);
                        object rentFeeResult = getRentFeeCmd.ExecuteScalar();

                        if (rentFeeResult != null && rentFeeResult != DBNull.Value)
                        {
                            rentFee = Convert.ToDecimal(rentFeeResult);
                        }
                    }

                    decimal totalRentFee = fine + rentFee; // Hitung TotalRentFee

                    // 1. Simpan data pengembalian ke tabel ReturnTbl (termasuk TotalRentFee)
                    string query = "INSERT INTO ReturnTbl (CarReg, CustName, ReturnDate, Delay, Fine, TotalRentFee) VALUES (@CarReg, @CustName, @ReturnDate, @Delay, @Fine, @TotalRentFee); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@CarReg", CarIdTb.Text);
                        cmd.Parameters.AddWithValue("@CustName", CustNameTb.Text);
                        cmd.Parameters.AddWithValue("@ReturnDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Delay", DelayTb.Text == "No Delay" ? 0 : Convert.ToInt32(DelayTb.Text));
                        cmd.Parameters.AddWithValue("@Fine", fine); // Simpan denda
                        cmd.Parameters.AddWithValue("@TotalRentFee", totalRentFee); // Simpan total sewa
                        int newReturnId = Convert.ToInt32(cmd.ExecuteScalar());
                        MessageBox.Show("Return Successfully Added with ID: " + newReturnId.ToString());
                    }

                    // 2. Update status mobil di tabel CarTbl menjadi 'YES'
                    string updateQuery = "UPDATE CarTbl SET Available = 'YES' WHERE RegNum = @CarReg";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, Con))
                    {
                        updateCmd.Parameters.AddWithValue("@CarReg", CarIdTb.Text);
                        updateCmd.ExecuteNonQuery();
                    }

                    // 3. Hapus data rental dari tabel RentalTbl
                    string deleteQuery = "DELETE FROM RentalTbl WHERE CarReg = @CarReg";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, Con))
                    {
                        deleteCmd.Parameters.AddWithValue("@CarReg", CarIdTb.Text);
                        deleteCmd.ExecuteNonQuery();
                    }

                    ClearFields(); // Bersihkan input setelah pengembalian berhasil disimpan
                    populate();    // Refresh data rental di RentDGV
                    populateRet(); // Refresh data pengembalian di ReturnDGV
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding return: " + ex.Message);
            }
        }


        // Fungsi untuk membersihkan semua input setelah pengembalian selesai
        private void ClearFields()
        {
            CarIdTb.Clear();
            CustNameTb.Clear();
            ReturnDate.Value = DateTime.Now; // Reset ke tanggal hari ini
            DelayTb.Clear();
            FineTb.Clear();
        }
    }
}
