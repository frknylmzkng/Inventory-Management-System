using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageUsers : Form
    {
        public ManageUsers()
        {
            InitializeComponent();
        }

        private void ManageUsers_Load(object sender, EventArgs e)
        {
            FormatGrid();
            PopulateUsers();
            ClearInputs();
        }

        private void FormatGrid()
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void PopulateUsers()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT UsernameDB AS [Kullanıcı Adı], FullnameDB AS [Ad Soyad], PasswordDB AS [Şifre], TelephoneDB AS [Telefon] FROM UserTable ORDER BY UsernameDB ASC");
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcılar listelenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txt_username.Text = string.Empty;
            txt_fullname.Text = string.Empty;
            txt_password2.Text = string.Empty;
            txt_phone.Text = string.Empty;
            dataGridView1.ClearSelection();
            txt_username.Focus();
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

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_username.Text) || string.IsNullOrWhiteSpace(txt_password2.Text))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre alanlarını doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO UserTable (UsernameDB, FullnameDB, PasswordDB, TelephoneDB) VALUES (@p1, @p2, @p3, @p4)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@p1", txt_username.Text.Trim()),
                    new SqlParameter("@p2", txt_fullname.Text.Trim()),
                    new SqlParameter("@p3", txt_password2.Text.Trim()),
                    new SqlParameter("@p4", txt_phone.Text.Trim())
                };

                DbHelper.ExecuteNonQuery(query, parameters);
                MessageBox.Show("Kullanıcı başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateUsers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_username.Text))
            {
                MessageBox.Show("Lütfen güncellenecek kullanıcıyı listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "UPDATE UserTable SET FullnameDB = @p2, PasswordDB = @p3, TelephoneDB = @p4 WHERE UsernameDB = @p1";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@p1", txt_username.Text.Trim()),
                    new SqlParameter("@p2", txt_fullname.Text.Trim()),
                    new SqlParameter("@p3", txt_password2.Text.Trim()),
                    new SqlParameter("@p4", txt_phone.Text.Trim())
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    MessageBox.Show("Kullanıcı bilgileri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateUsers();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Güncellenecek kullanıcı bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_username.Text))
            {
                MessageBox.Show("Lütfen silinecek kullanıcıyı listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"'{txt_username.Text}' kullanıcısını silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM UserTable WHERE UsernameDB = @p1";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@p1", txt_username.Text.Trim())
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    MessageBox.Show("Kullanıcı başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateUsers();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Silinecek kullanıcı bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı silinirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txt_username.Text = row.Cells[0].Value?.ToString() ?? "";
                txt_fullname.Text = row.Cells[1].Value?.ToString() ?? "";
                txt_password2.Text = row.Cells[2].Value?.ToString() ?? "";
                txt_phone.Text = row.Cells[3].Value?.ToString() ?? "";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
        }
    }
}
