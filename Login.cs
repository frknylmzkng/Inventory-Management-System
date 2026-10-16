using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void chc_ShowName_CheckedChanged(object sender, EventArgs e)
        {
            if (chc_ShowName.Checked)
                txt_password.PasswordChar = '\0';
            else
                txt_password.PasswordChar = '*';
        }

        public void ResetInputs()
        {
            txt_password.Text = string.Empty;
            txt_login.Focus();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }

        private void lbl_clear_Click(object sender, EventArgs e)
        {
            txt_login.Text = string.Empty;
            txt_password.Text = string.Empty;
            txt_login.Focus();
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txt_login.Text.Trim();
            string password = txt_password.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifrenizi giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string passHash = DbHelper.HashPassword(password);
                string query = "SELECT UsernameDB, FullnameDB, RoleDB, PasswordDB FROM UserTable WHERE UsernameDB = @user AND (PasswordDB = @passHash OR PasswordDB = @passPlain)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@user", username),
                    new SqlParameter("@passHash", passHash),
                    new SqlParameter("@passPlain", password)
                };

                DataTable dt = DbHelper.ExecuteQuery(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    CurrentUser.Username = row["UsernameDB"].ToString();
                    CurrentUser.FullName = row["FullnameDB"]?.ToString() ?? CurrentUser.Username;
                    CurrentUser.Role = (row.Table.Columns.Contains("RoleDB") && row["RoleDB"] != DBNull.Value) ? row["RoleDB"].ToString() : "Yönetici (Admin)";

                    string currentDbPass = row["PasswordDB"]?.ToString() ?? "";
                    if (currentDbPass == password && currentDbPass != passHash)
                    {
                        // Düz metin şifre tespit edildi; güvenliği sağlamak için veritabanında otomatik SHA-256 olarak güncelle
                        try
                        {
                            DbHelper.ExecuteNonQuery("UPDATE UserTable SET PasswordDB = @passHash WHERE UsernameDB = @user", new SqlParameter[]
                            {
                                new SqlParameter("@passHash", passHash),
                                new SqlParameter("@user", username)
                            });
                        }
                        catch
                        {
                            // Migration hatası olsa bile girişe izin ver
                        }
                    }

                    DbHelper.LogAction("Giriş Başarılı", "Oturum / Kimlik", $"'{CurrentUser.Username}' ({CurrentUser.Role}) sisteme başarıyla giriş yaptı.", CurrentUser.Username, CurrentUser.Role);

                    Form home = Application.OpenForms["HomeForm"];
                    if (home != null)
                    {
                        home.Show();
                    }
                    else
                    {
                        new HomeForm().Show();
                    }
                    this.Hide();
                }
                else
                {
                    DbHelper.LogAction("Başarısız Giriş", "Oturum / Kimlik", $"'{username}' kullanıcı adı ile hatalı şifre girişi denendi.", username, "Bilinmiyor");
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_password.Text = string.Empty;
                    txt_password.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Giriş yapılırken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
