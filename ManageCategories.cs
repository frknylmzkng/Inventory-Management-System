using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageCategories : Form
    {
        public ManageCategories()
        {
            InitializeComponent();
        }

        private void ManageCategories_Load(object sender, EventArgs e)
        {
            FormatGrid();
            PopulateCategories();
        }

        private void FormatGrid()
        {
            dataGridViewCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCategories.MultiSelect = false;
            dataGridViewCategories.ReadOnly = true;
            dataGridViewCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCategories.AllowUserToAddRows = false;
        }

        private void PopulateCategories()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT CatId AS [Kategori ID], CatName AS [Kategori Adı] FROM CategoryTable ORDER BY CatId DESC");
                dataGridViewCategories.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategoriler listelenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txt_catid.Text = string.Empty;
            txt_catname.Text = string.Empty;
            txt_catname.Focus();
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_home_Click(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.Show();
            this.Hide();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_catname.Text))
            {
                MessageBox.Show("Lütfen kategori adını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO CategoryTable (CatName) VALUES (@name)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@name", txt_catname.Text.Trim())
                };

                DbHelper.ExecuteNonQuery(query, parameters);
                MessageBox.Show("Kategori başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateCategories();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategori eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_catid.Text))
            {
                MessageBox.Show("Lütfen güncellenecek kategoriyi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_catname.Text))
            {
                MessageBox.Show("Kategori adı boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "UPDATE CategoryTable SET CatName = @name WHERE CatId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@name", txt_catname.Text.Trim()),
                    new SqlParameter("@id", Convert.ToInt32(txt_catid.Text))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    MessageBox.Show("Kategori başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateCategories();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategori güncellenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_catid.Text))
            {
                MessageBox.Show("Lütfen silinecek kategoriyi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"'{txt_catname.Text}' kategorisini silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM CategoryTable WHERE CatId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@id", Convert.ToInt32(txt_catid.Text))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    MessageBox.Show("Kategori başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateCategories();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategori silinirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewCategories.Rows.Count)
            {
                DataGridViewRow row = dataGridViewCategories.Rows[e.RowIndex];
                txt_catid.Text = row.Cells[0].Value?.ToString() ?? "";
                txt_catname.Text = row.Cells[1].Value?.ToString() ?? "";
            }
        }
    }
}
