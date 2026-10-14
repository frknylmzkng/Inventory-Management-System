using System;
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
                string query = "SELECT COUNT(*) FROM UserTable WHERE UsernameDB = @user AND PasswordDB = @pass";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@user", username),
                    new SqlParameter("@pass", password)
                };

                object result = DbHelper.ExecuteScalar(query, parameters);
                int count = Convert.ToInt32(result);

                if (count > 0)
                {
                    HomeForm home = new HomeForm();
                    home.Show();
                    this.Hide();
                }
                else
                {
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
