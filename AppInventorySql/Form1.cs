using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using MySqlX.XDevAPI.Relational;

namespace AppInventorySql
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ADD_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.Kode.Text))
            {
                MessageBox.Show("Kode Barang harus diisi terlebih dahulu.");
                return;
            }
            else
            {
                string connection = "server=localhost;user id=root;password=;database=oopcrud";
                MySqlConnection conn = new MySqlConnection(connection);

                string query = "INSERT INTO barang(kode_barang,nama_barang,stok_barang,satuan_barang,harga_barang,image)VALUES(@kode, @nama, @stok, @satuan, @harga, @image)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kode", this.Kode.Text);
                cmd.Parameters.AddWithValue("@nama", this.Nama.Text);
                cmd.Parameters.AddWithValue("@stok", this.Stok.Text);
                cmd.Parameters.AddWithValue("@satuan", this.comboBox1.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@harga", this.Harga.Text);
                cmd.Parameters.AddWithValue("@image", Path.GetFileName(pictureBox1.ImageLocation));

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                // Copy image to destination folder
                File.Copy(Imagetext.Text, Application.StartupPath + @"\Image\" + Path.GetFileName(pictureBox1.ImageLocation), true);
                conn.Close();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Data berhasil disimpan.");

                    // Refresh data grid view
                    string selectQuery = "SELECT * FROM barang";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(selectQuery, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dt.Columns.Add("Gambar_Barang", Type.GetType("System.Byte[]"));
                    foreach (DataRow row in dt.Rows)
                    {
                        row["Gambar_Barang"] = File.ReadAllBytes(Application.StartupPath + @"\Image\" + Path.GetFileName(row["image"].ToString()));
                    }
                    dataGridView1.DataSource = dt;

                    // Clear input fields
                    this.Kode.Text = "";
                    this.Nama.Text = "";
                    this.Stok.Text = "";
                    this.comboBox1.SelectedIndex = -1;
                    this.Harga.Text = "";
                    this.pictureBox1.Image = null;

                }
            }
        }

        private void EDIT_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.Kode.Text))
            {
                MessageBox.Show("Kode Barang harus diisi terlebih dahulu.");
                return;
            }
            string connection = "server=localhost;user id=root;password=;database=oopcrud";
            string query = "UPDATE barang SET nama_barang=@nama, harga_barang=@harga, stok_barang=@stok, satuan_barang=@satuan WHERE kode_barang=@kode";
            MySqlConnection conn = new MySqlConnection(connection);
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nama", this.Nama.Text);
            cmd.Parameters.AddWithValue("@harga", this.Harga.Text);
            cmd.Parameters.AddWithValue("@stok", this.Stok.Text);
            cmd.Parameters.AddWithValue("@satuan", this.comboBox1.Text);
            cmd.Parameters.AddWithValue("@kode", this.Kode.Text);
            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            MessageBox.Show("Data berhasil diupdate.");
            conn.Close();

            // Refresh data grid view
            string selectQuery = "SELECT * FROM barang";
            MySqlDataAdapter adapter = new MySqlDataAdapter(selectQuery, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dt.Columns.Add("Gambar_Barang", Type.GetType("System.Byte[]"));
            foreach (DataRow row in dt.Rows)
            {
                row["Gambar_Barang"] = File.ReadAllBytes(Application.StartupPath + @"\Image\" + Path.GetFileName(row["image"].ToString()));
            }
            dataGridView1.DataSource = dt;
            // Clear input fields
            this.Kode.Text = "";
            this.Nama.Text = "";
            this.Stok.Text = "";
            this.comboBox1.SelectedIndex = -1;
            this.Harga.Text = "";
            this.pictureBox1.Image = null;

        }

        private void DELETE_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.Kode.Text))
            {
                MessageBox.Show("Kode barang tidak boleh kosong.");
                return;
            }
            else
            {
                string connection = "server=localhost;user id=root;password=;database=oopcrud";
                string query = "DELETE FROM barang WHERE kode_barang='" + this.Kode.Text + "'";
                string imagePath = Application.StartupPath + @"\Image\" + Path.GetFileName(pictureBox1.ImageLocation);

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                MySqlConnection conn = new MySqlConnection(connection);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader dr;

                conn.Open();
                dr = cmd.ExecuteReader();
                MessageBox.Show("Berhasil Dihapus");
                conn.Close();

                // Refresh data grid view
                string selectQuery = "SELECT * FROM barang";
                MySqlDataAdapter adapter = new MySqlDataAdapter(selectQuery, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dt.Columns.Add("Gambar_Barang", Type.GetType("System.Byte[]"));
                foreach (DataRow row in dt.Rows)
                {
                    row["Gambar_Barang"] = File.ReadAllBytes(Application.StartupPath + @"\Image\" + Path.GetFileName(row["image"].ToString()));
                }
                dataGridView1.DataSource = dt;

                // Kosongkan semua kolom input
                this.Kode.Text = "";
                this.Nama.Text = "";
                this.Stok.Text = "";
                this.comboBox1.SelectedIndex = -1;
                this.Harga.Text = "";
                this.pictureBox1.Image = null;

            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connection = "server=localhost;user id=root;password=;database=oopcrud";
            string query = "SELECT * FROM barang";
            MySqlConnection conn = new MySqlConnection(connection);
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dt.Columns.Add("Gambar_Barang", Type.GetType("System.Byte[]"));
            foreach (DataRow row in dt.Rows)
            {
                row["Gambar_Barang"] = File.ReadAllBytes(Application.StartupPath + @"\Image\" + Path.GetFileName(row["image"].ToString()));
            }
            dataGridView1.DataSource = dt;

            // Sembunyikan kolom image
            dataGridView1.Columns["image"].Visible = false;


        }

        private void Cari_TextChanged(object sender, EventArgs e)
        {
            string connection = "server=localhost;user id=root;password=;database=oopcrud";
            MySqlConnection con = new MySqlConnection(connection);
            MySqlDataAdapter da;
            DataTable dt;
            con.Open();
            da = new MySqlDataAdapter("SELECT * FROM barang WHERE kode_barang LIKE'" + this.Cari.Text + "%'", con);
            dt = new DataTable();
            da.Fill(dt);
            dt.Columns.Add("Gambar_Barang", Type.GetType("System.Byte[]"));
            foreach (DataRow row in dt.Rows)
            {
                row["Gambar_Barang"] = File.ReadAllBytes(Application.StartupPath + @"\Image\" + Path.GetFileName(row["image"].ToString()));
            }
            dataGridView1.DataSource = dt;
            con.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfd = new OpenFileDialog();
            openfd.Filter = "Image Files(*.jpg;*.jpeg;*.gif;) | *.jpg;*.jpeg;*.gif; ";
            if (openfd.ShowDialog() == DialogResult.OK)
            {
                Imagetext.Text = openfd.FileName;
                pictureBox1.Image = new Bitmap(openfd.FileName);
                pictureBox1.ImageLocation = openfd.FileName;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
