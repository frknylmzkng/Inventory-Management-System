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
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Bu alana yalnızca Yönetici (Admin) yetkisine sahip kullanıcılar erişebilir.", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                HomeForm home = new HomeForm();
                home.Show();
                this.Close();
                return;
            }

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
                DataTable dt = DbHelper.ExecuteQuery("SELECT UsernameDB AS [Kullanıcı Adı], FullnameDB AS [Ad Soyad], PasswordDB AS [Şifre], TelephoneDB AS [Telefon], RoleDB AS [Rol] FROM UserTable ORDER BY UsernameDB ASC");
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
            if (cmb_role.Items.Count > 1)
            {
                cmb_role.SelectedIndex = 1; // Default "Kasiyer"
            }
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

            string role = cmb_role.SelectedItem?.ToString() ?? "Kasiyer";

            try
            {
                string query = "INSERT INTO UserTable (UsernameDB, FullnameDB, PasswordDB, TelephoneDB, RoleDB) VALUES (@p1, @p2, @p3, @p4, @p5)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@p1", txt_username.Text.Trim()),
                    new SqlParameter("@p2", txt_fullname.Text.Trim()),
                    new SqlParameter("@p3", txt_password2.Text.Trim()),
                    new SqlParameter("@p4", txt_phone.Text.Trim()),
                    new SqlParameter("@p5", role)
                };

                DbHelper.ExecuteNonQuery(query, parameters);
                DbHelper.LogAction("Kullanıcı Eklendi", "Kullanıcı Yönetimi", $"Kullanıcı: '{txt_username.Text.Trim()}', Ad Soyad: '{txt_fullname.Text.Trim()}', Rol: '{role}'");
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

            string role = cmb_role.SelectedItem?.ToString() ?? "Kasiyer";

            try
            {
                string query = "UPDATE UserTable SET FullnameDB = @p2, PasswordDB = @p3, TelephoneDB = @p4, RoleDB = @p5 WHERE UsernameDB = @p1";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@p1", txt_username.Text.Trim()),
                    new SqlParameter("@p2", txt_fullname.Text.Trim()),
                    new SqlParameter("@p3", txt_password2.Text.Trim()),
                    new SqlParameter("@p4", txt_phone.Text.Trim()),
                    new SqlParameter("@p5", role)
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    // Oturum açmış olan kullanıcı kendi profilini güncellediyse oturum bilgilerini güncelle
                    if (string.Equals(CurrentUser.Username, txt_username.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        CurrentUser.FullName = txt_fullname.Text.Trim();
                        CurrentUser.Role = role;
                    }

                    DbHelper.LogAction("Kullanıcı Güncellendi", "Kullanıcı Yönetimi", $"Kullanıcı: '{txt_username.Text.Trim()}', Yeni Ad Soyad: '{txt_fullname.Text.Trim()}', Yeni Rol: '{role}'");
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

            string targetUser = txt_username.Text.Trim();

            // Güvenlik kuralı: Kendi hesabını ve ana admin hesabını silmeyi engelle
            if (string.Equals(targetUser, CurrentUser.Username, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Şu anda oturum açmış olduğunuz kendi hesabınızı silemezsiniz!", "İşlem Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.Equals(targetUser, "admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Sistem ana yöneticisi ('admin') hesabı silinemez!", "İşlem Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"'{targetUser}' kullanıcısını silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM UserTable WHERE UsernameDB = @p1";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@p1", targetUser)
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    DbHelper.LogAction("Kullanıcı Silindi", "Kullanıcı Yönetimi", $"Kullanıcı Adı: '{targetUser}' kalıcı olarak silindi.");
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

                string role = row.Cells.Count > 4 ? (row.Cells[4].Value?.ToString() ?? "") : "";
                if (!string.IsNullOrEmpty(role) && cmb_role.Items.Contains(role))
                {
                    cmb_role.SelectedItem = role;
                }
                else
                {
                    cmb_role.SelectedIndex = 1;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
        }
    }
}
