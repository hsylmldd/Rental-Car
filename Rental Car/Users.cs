using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Rental_Car
{
    public partial class Users : Form
    {
        public Users()
        {
            InitializeComponent();
        }
        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False");
        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void populate()
        {
            Con.Open();
            string query = "SELECT * FROM UserTbl"; //Perbaiki query
            SqlDataAdapter da = new SqlDataAdapter(query, Con);
            SqlCommandBuilder builder = new SqlCommandBuilder(da);
            var ds = new DataSet();
            da.Fill(ds);
            UserDGV.DataSource = ds.Tables[0];
            Con.Close();

        }
        private void button3_Click(object sender, EventArgs e) // button add
        {
            // Validasi input tidak boleh kosong
            if (Uname.Text == "" || Upass.Text == "") //UId Dihapus
            {
                MessageBox.Show("Missing information");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))

                {
                    Con.Open();

                    // // Periksa apakah ID sudah ada
                    // string checkQuery = "SELECT COUNT(*) FROM UserTbl WHERE Id = @UId";  //UId Dihapus
                    // using (SqlCommand checkCmd = new SqlCommand(checkQuery, Con))
                    // {
                    //     checkCmd.Parameters.AddWithValue("@UId", UId.Text);
                    //     int count = (int)checkCmd.ExecuteScalar();

                    //     if (count > 0)
                    //     {
                    //         MessageBox.Show("ID already exists, please use a different ID.");
                    //         return;
                    //     }
                    // }

                    // Insert data jika ID belum ada
                    string query = "INSERT INTO UserTbl (Uname, Upass) VALUES (@Uname, @Upass); SELECT SCOPE_IDENTITY();"; //UId Dihapus

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        //cmd.Parameters.AddWithValue("@UId", UId.Text);  //UId Dihapus
                        cmd.Parameters.AddWithValue("@Uname", Uname.Text);
                        cmd.Parameters.AddWithValue("@Upass", Upass.Text); // Password disimpan tanpa hash
                        //cmd.ExecuteNonQuery(); //UId Dihapus
                        int newUserId = Convert.ToInt32(cmd.ExecuteScalar()); // ExecuteScalar untuk mendapatkan identitas
                        MessageBox.Show("User Successfully Added with ID: " + newUserId.ToString());
                    }

                    // MessageBox.Show("User Successfully Added");
                    Con.Close();
                    populate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Users_Load(object sender, EventArgs e)
        {
            populate();
        }

        private void button5_Click(object sender, EventArgs e) //delete button 
        {
            if (UId.Text == "")
            {
                MessageBox.Show("Missing Information");
            }
            else
            {
                try
                {
                    Con.Open();
                    string query = "delete from UserTbl where Id= " + UId.Text + ";";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User Deleted Successfully");
                    Con.Close();
                    populate();
                }
                catch (Exception Myex)
                {
                    MessageBox.Show(Myex.Message);
                }

            }
        }

        private void UserDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Pastikan indeks baris valid
            {
                DataGridViewRow row = UserDGV.Rows[e.RowIndex]; // Ambil baris yang diklik
                UId.Text = row.Cells[0].Value?.ToString();
                Uname.Text = row.Cells[1].Value?.ToString();
                Upass.Text = row.Cells[2].Value?.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e) // Button Edit
        {
            if (UId.Text == "" || Uname.Text == "" || Upass.Text == "")
            {
                MessageBox.Show("Missing information");
                return;
            }

            try
            {
                using (SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lazir\OneDrive\Dokumen\CarRentaldb.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False"))
                {
                    Con.Open();
                    string query = "UPDATE UserTbl SET Uname = @Uname, Upass = @Upass WHERE Id = @UId"; 

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@UId", UId.Text);
                        cmd.Parameters.AddWithValue("@Uname", Uname.Text);
                        cmd.Parameters.AddWithValue("@Upass", Upass.Text); 
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("User Successfully Updated");
                    populate(); // Perbarui tampilan tabel
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
    }
}
