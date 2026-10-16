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
            if (CurrentUser.IsCashier)
            {
                btn_delete.Enabled = false;
            }

            FormatGrid();
            PopulateCategories();
            ClearInputs();
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
                DataTable dt = DbHelper.ExecuteQuery("SELECT CatId AS [Kategori ID], CatName AS [Kategori Adı] FROM CategoryTable ORDER BY CatId ASC");
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
            dataGridViewCategories.ClearSelection();
            txt_catname.Focus();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            Form home = Application.OpenForms["HomeForm"];
            if (home != null)
            {
                home.Show();
            }
            else
            {
                new HomeForm().Show();
            }
            this.Close();
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Uygulamadan çıkmak istediğinize emin misiniz?", "Çıkış Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btn_home_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
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
                DbHelper.LogAction("Kategori Eklendi", "Kategori Yönetimi", $"'{txt_catname.Text.Trim()}' kategorisi eklendi.");
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

            int catId = Convert.ToInt32(txt_catid.Text);
            string newCatName = txt_catname.Text.Trim();

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Fetch old category name
                        string oldCatName = "";
                        string selectOldQuery = "SELECT CatName FROM CategoryTable WHERE CatId = @id";
                        using (SqlCommand cmdOld = new SqlCommand(selectOldQuery, conn, trans))
                        {
                            cmdOld.Parameters.AddWithValue("@id", catId);
                            object oldObj = cmdOld.ExecuteScalar();
                            oldCatName = oldObj?.ToString() ?? "";
                        }

                        // 2. Update CategoryTable
                        string updateCatQuery = "UPDATE CategoryTable SET CatName = @name WHERE CatId = @id";
                        using (SqlCommand cmdCat = new SqlCommand(updateCatQuery, conn, trans))
                        {
                            cmdCat.Parameters.AddWithValue("@name", newCatName);
                            cmdCat.Parameters.AddWithValue("@id", catId);
                            cmdCat.ExecuteNonQuery();
                        }

                        // 3. Update ProductTable
                        if (!string.IsNullOrEmpty(oldCatName))
                        {
                            string updateProdQuery = "UPDATE ProductTable SET ProdCat = @newName WHERE ProdCat = @oldName";
                            using (SqlCommand cmdProd = new SqlCommand(updateProdQuery, conn, trans))
                            {
                                cmdProd.Parameters.AddWithValue("@newName", newCatName);
                                cmdProd.Parameters.AddWithValue("@oldName", oldCatName);
                                cmdProd.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();

                        DbHelper.LogAction("Kategori Güncellendi", "Kategori Yönetimi", $"ID: {catId}, Eski: '{oldCatName}', Yeni Kategori Adı: '{newCatName}'");
                        MessageBox.Show("Kategori ve bağlı ürünlerin kategorileri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PopulateCategories();
                        ClearInputs();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("Kategori güncellenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (CurrentUser.IsCashier)
            {
                MessageBox.Show("Kasiyer yetkisi ile kategori silinemez!", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_catid.Text))
            {
                MessageBox.Show("Lütfen silinecek kategoriyi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string deletedName = txt_catname.Text.Trim();
            string deletedId = txt_catid.Text.Trim();

            // İlişkisel veri bütünlüğü kontrolü: Bu kategoriye bağlı ürün var mı?
            try
            {
                object countObj = DbHelper.ExecuteScalar("SELECT COUNT(*) FROM ProductTable WHERE ProdCat = @catName", new SqlParameter[]
                {
                    new SqlParameter("@catName", deletedName)
                });
                int productCount = (countObj != null && countObj != DBNull.Value) ? Convert.ToInt32(countObj) : 0;
                if (productCount > 0)
                {
                    MessageBox.Show($"Bu kategoriye bağlı {productCount} adet ürün bulunmaktadır!\nÜrünler başka bir kategoriye taşınmadan veya silinmeden bu kategori silinemez.", "Silme Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategori kontrol edilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult confirm = MessageBox.Show($"'{deletedName}' kategorisini silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM CategoryTable WHERE CatId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@id", Convert.ToInt32(deletedId))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    DbHelper.LogAction("Kategori Silindi", "Kategori Yönetimi", $"ID: {deletedId}, Kategori Adı: '{deletedName}' silindi.");
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
