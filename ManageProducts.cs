using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageProducts : Form
    {
        private bool initialFilterCritical = false;

        public ManageProducts(bool filterCritical = false)
        {
            InitializeComponent();
            this.initialFilterCritical = filterCritical;
        }

        private void ManageProducts_Load(object sender, EventArgs e)
        {
            FormatGrid();
            LoadCategories();
            if (initialFilterCritical)
            {
                chc_critical.Checked = true;
            }
            else
            {
                PopulateProducts();
            }
            ClearInputs();
        }

        private void FormatGrid()
        {
            dataGridViewProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewProducts.MultiSelect = false;
            dataGridViewProducts.ReadOnly = true;
            dataGridViewProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewProducts.AllowUserToAddRows = false;
        }

        private void LoadCategories()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT CatName FROM CategoryTable ORDER BY CatName");

                cmb_prodcat.Items.Clear();
                cmb_searchcat.Items.Clear();
                cmb_searchcat.Items.Add("-- Tüm Kategoriler --");

                foreach (DataRow row in dt.Rows)
                {
                    string cat = row["CatName"].ToString();
                    cmb_prodcat.Items.Add(cat);
                    cmb_searchcat.Items.Add(cat);
                }

                cmb_prodcat.SelectedIndex = -1;
                cmb_searchcat.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategoriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateProducts(string categoryFilter = null)
        {
            try
            {
                string query;
                SqlParameter[] parameters = null;

                bool onlyCritical = chc_critical.Checked;
                bool hasCategory = !string.IsNullOrEmpty(categoryFilter) && categoryFilter != "-- Tüm Kategoriler --";

                if (onlyCritical && hasCategory)
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable WHERE ProdCat = @cat AND ProdQty <= ProdMinQty ORDER BY ProdQty ASC";
                    parameters = new SqlParameter[] { new SqlParameter("@cat", categoryFilter) };
                }
                else if (onlyCritical)
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable WHERE ProdQty <= ProdMinQty ORDER BY ProdQty ASC";
                }
                else if (hasCategory)
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable WHERE ProdCat = @cat ORDER BY ProdId ASC";
                    parameters = new SqlParameter[] { new SqlParameter("@cat", categoryFilter) };
                }
                else
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable ORDER BY ProdId ASC";
                }

                DataTable dt = DbHelper.ExecuteQuery(query, parameters);
                dataGridViewProducts.DataSource = dt;

                if (dataGridViewProducts.Columns["Maliyet (TL)"] != null)
                {
                    dataGridViewProducts.Columns["Maliyet (TL)"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridViewProducts.Columns["Satış Fiyatı (TL)"] != null)
                {
                    dataGridViewProducts.Columns["Satış Fiyatı (TL)"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridViewProducts.Columns["Birim Kâr (TL)"] != null)
                {
                    dataGridViewProducts.Columns["Birim Kâr (TL)"].DefaultCellStyle.Format = "N2";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürünler listelenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chc_critical_CheckedChanged(object sender, EventArgs e)
        {
            string selectedCat = cmb_searchcat.SelectedItem?.ToString();
            PopulateProducts(selectedCat);
        }

        private void dataGridViewProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewProducts.Rows.Count)
            {
                DataGridViewRow row = dataGridViewProducts.Rows[e.RowIndex];
                if (row.Cells["Miktar"]?.Value != null && row.Cells["Kritik Sınır"]?.Value != null)
                {
                    if (int.TryParse(row.Cells["Miktar"].Value.ToString(), out int qty) &&
                        int.TryParse(row.Cells["Kritik Sınır"].Value.ToString(), out int minQty))
                    {
                        if (qty == 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.MistyRose;
                            row.DefaultCellStyle.ForeColor = Color.DarkRed;
                            row.DefaultCellStyle.SelectionBackColor = Color.IndianRed;
                            row.DefaultCellStyle.SelectionForeColor = Color.White;
                        }
                        else if (qty <= minQty)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                            row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                            row.DefaultCellStyle.SelectionBackColor = Color.Orange;
                            row.DefaultCellStyle.SelectionForeColor = Color.White;
                        }
                    }
                }
            }
        }

        private void ClearInputs()
        {
            txt_prodid.Text = string.Empty;
            txt_prodname.Text = string.Empty;
            txt_prodqty.Text = string.Empty;
            txt_prodminqty.Text = "5";
            txt_prodcost.Text = string.Empty;
            txt_prodprice.Text = string.Empty;
            txt_proddesc.Text = string.Empty;
            cmb_prodcat.SelectedIndex = -1;
            dataGridViewProducts.ClearSelection();
            txt_prodname.Focus();
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
            if (string.IsNullOrWhiteSpace(txt_prodname.Text) || string.IsNullOrWhiteSpace(txt_prodqty.Text) || string.IsNullOrWhiteSpace(txt_prodprice.Text))
            {
                MessageBox.Show("Lütfen ürün adı, stok miktarı ve fiyat alanlarını doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txt_prodqty.Text.Trim(), out int qty) || qty < 0)
            {
                MessageBox.Show("Geçerli bir stok miktarı giriniz (0 veya daha büyük bir tam sayı).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txt_prodprice.Text.Trim(), out decimal price) || price < 0)
            {
                MessageBox.Show("Geçerli bir ürün fiyatı giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal costPrice = 0;
            if (!string.IsNullOrWhiteSpace(txt_prodcost.Text))
            {
                decimal.TryParse(txt_prodcost.Text.Trim(), out costPrice);
            }

            if (cmb_prodcat.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir kategori seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int minQty = 5;
            if (!string.IsNullOrWhiteSpace(txt_prodminqty.Text))
            {
                int.TryParse(txt_prodminqty.Text.Trim(), out minQty);
            }

            try
            {
                string query = "INSERT INTO ProductTable (ProdName, ProdQty, ProdMinQty, ProdCostPrice, ProdPrice, ProdDesc, ProdCat) VALUES (@name, @qty, @minqty, @cost, @price, @desc, @cat)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@name", txt_prodname.Text.Trim()),
                    new SqlParameter("@qty", qty),
                    new SqlParameter("@minqty", minQty),
                    new SqlParameter("@cost", costPrice),
                    new SqlParameter("@price", price),
                    new SqlParameter("@desc", txt_proddesc.Text.Trim()),
                    new SqlParameter("@cat", cmb_prodcat.SelectedItem.ToString())
                };

                DbHelper.ExecuteNonQuery(query, parameters);
                MessageBox.Show("Ürün başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateProducts();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_prodid.Text))
            {
                MessageBox.Show("Lütfen güncellenecek ürünü listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txt_prodqty.Text.Trim(), out int qty) || qty < 0)
            {
                MessageBox.Show("Geçerli bir stok miktarı giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txt_prodprice.Text.Trim(), out decimal price) || price < 0)
            {
                MessageBox.Show("Geçerli bir fiyat giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal costPrice = 0;
            if (!string.IsNullOrWhiteSpace(txt_prodcost.Text))
            {
                decimal.TryParse(txt_prodcost.Text.Trim(), out costPrice);
            }

            if (cmb_prodcat.SelectedItem == null || cmb_prodcat.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen bir kategori seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int minQty = 5;
            if (!string.IsNullOrWhiteSpace(txt_prodminqty.Text))
            {
                int.TryParse(txt_prodminqty.Text.Trim(), out minQty);
            }

            try
            {
                string query = "UPDATE ProductTable SET ProdName = @name, ProdQty = @qty, ProdMinQty = @minqty, ProdCostPrice = @cost, ProdPrice = @price, ProdDesc = @desc, ProdCat = @cat WHERE ProdId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@name", txt_prodname.Text.Trim()),
                    new SqlParameter("@qty", qty),
                    new SqlParameter("@minqty", minQty),
                    new SqlParameter("@cost", costPrice),
                    new SqlParameter("@price", price),
                    new SqlParameter("@desc", txt_proddesc.Text.Trim()),
                    new SqlParameter("@cat", cmb_prodcat.SelectedItem.ToString()),
                    new SqlParameter("@id", Convert.ToInt32(txt_prodid.Text))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    MessageBox.Show("Ürün başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateProducts();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün güncellenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_prodid.Text))
            {
                MessageBox.Show("Lütfen silinecek ürünü listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"'{txt_prodname.Text}' ürününü silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM ProductTable WHERE ProdId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@id", Convert.ToInt32(txt_prodid.Text))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    MessageBox.Show("Ürün başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateProducts();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün silinirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewProducts.Rows.Count)
            {
                DataGridViewRow row = dataGridViewProducts.Rows[e.RowIndex];
                txt_prodid.Text = row.Cells[0].Value?.ToString() ?? "";
                txt_prodname.Text = row.Cells[1].Value?.ToString() ?? "";
                txt_prodqty.Text = row.Cells[2].Value?.ToString() ?? "";
                txt_prodminqty.Text = row.Cells[3].Value?.ToString() ?? "5";
                txt_prodcost.Text = row.Cells[4].Value != null ? Convert.ToDecimal(row.Cells[4].Value).ToString("N2") : "0,00";
                txt_prodprice.Text = row.Cells[5].Value != null ? Convert.ToDecimal(row.Cells[5].Value).ToString("N2") : "";
                txt_proddesc.Text = row.Cells[7].Value?.ToString() ?? "";
                string cat = row.Cells[8].Value?.ToString() ?? "";
                if (cmb_prodcat.Items.Contains(cat))
                    cmb_prodcat.SelectedItem = cat;
                else
                    cmb_prodcat.SelectedIndex = -1;
            }
        }

        private void cmb_searchcat_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedCat = cmb_searchcat.SelectedItem?.ToString();
            PopulateProducts(selectedCat);
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            chc_critical.Checked = false;
            cmb_searchcat.SelectedIndex = 0;
            PopulateProducts();
            ClearInputs();
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            string fileName = $"Urun_Stok_Listesi_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            DbHelper.ExportToCsv(dataGridViewProducts, fileName);
        }
    }
}
