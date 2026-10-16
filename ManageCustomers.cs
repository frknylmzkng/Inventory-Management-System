using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageCustomers : Form
    {
        public ManageCustomers()
        {
            InitializeComponent();
        }

        private void ManageCustomers_Load(object sender, EventArgs e)
        {
            FormatGrid();
            PopulateCustomers();
            ClearInputs();
        }

        private void FormatGrid()
        {
            dataGridViewCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCustomers.MultiSelect = false;
            dataGridViewCustomers.ReadOnly = true;
            dataGridViewCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCustomers.AllowUserToAddRows = false;
        }

        private void PopulateCustomers()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT CustId AS [Müşteri ID], CustName AS [Müşteri Adı], CustPhone AS [Telefon] FROM CustomerTable ORDER BY CustId ASC");
                dataGridViewCustomers.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteriler listelenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txt_custid.Text = string.Empty;
            txt_custname.Text = string.Empty;
            txt_custphone.Text = string.Empty;
            dataGridViewCustomers.ClearSelection();
            txt_custname.Focus();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.Show();
            this.Close();
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
        }

        private void btn_home_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_custname.Text) || string.IsNullOrWhiteSpace(txt_custphone.Text))
            {
                MessageBox.Show("Lütfen müşteri adı ve telefon alanlarını doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO CustomerTable (CustName, CustPhone) VALUES (@name, @phone)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@name", txt_custname.Text.Trim()),
                    new SqlParameter("@phone", txt_custphone.Text.Trim())
                };

                DbHelper.ExecuteNonQuery(query, parameters);
                MessageBox.Show("Müşteri başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateCustomers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_custid.Text))
            {
                MessageBox.Show("Lütfen güncellenecek müşteriyi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_custname.Text) || string.IsNullOrWhiteSpace(txt_custphone.Text))
            {
                MessageBox.Show("Müşteri adı ve telefon alanları boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "UPDATE CustomerTable SET CustName = @name, CustPhone = @phone WHERE CustId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@name", txt_custname.Text.Trim()),
                    new SqlParameter("@phone", txt_custphone.Text.Trim()),
                    new SqlParameter("@id", Convert.ToInt32(txt_custid.Text))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    MessageBox.Show("Müşteri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateCustomers();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri güncellenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_custid.Text))
            {
                MessageBox.Show("Lütfen silinecek müşteriyi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"'{txt_custname.Text}' müşterisini silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM CustomerTable WHERE CustId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@id", Convert.ToInt32(txt_custid.Text))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    MessageBox.Show("Müşteri başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateCustomers();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri silinirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewCustomers.Rows.Count)
            {
                DataGridViewRow row = dataGridViewCustomers.Rows[e.RowIndex];
                txt_custid.Text = row.Cells[0].Value?.ToString() ?? "";
                txt_custname.Text = row.Cells[1].Value?.ToString() ?? "";
                txt_custphone.Text = row.Cells[2].Value?.ToString() ?? "";
            }
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            string fileName = $"Musteri_Listesi_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            DbHelper.ExportToCsv(dataGridViewCustomers, fileName);
        }
    }
}
