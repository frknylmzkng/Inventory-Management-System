using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageProducts : Form
    {
        private static readonly Random rnd = new Random();
        private bool initialFilterCritical = false;

        public ManageProducts(bool filterCritical = false)
        {
            InitializeComponent();
            this.initialFilterCritical = filterCritical;
        }

        private void ManageProducts_Load(object sender, EventArgs e)
        {
            ApplyRoleRestrictions();
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

        private void ApplyRoleRestrictions()
        {
            if (!CurrentUser.IsAdmin)
            {
                txt_prodcost.Visible = false;
                lbl_prodcost.Visible = false;
            }

            if (CurrentUser.IsCashier)
            {
                btn_delete.Enabled = false;
            }
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
                    query = "SELECT ProdId AS [Ürün ID], ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable WHERE ProdCat = @cat AND ProdQty <= ProdMinQty ORDER BY ProdQty ASC";
                    parameters = new SqlParameter[] { new SqlParameter("@cat", categoryFilter) };
                }
                else if (onlyCritical)
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable WHERE ProdQty <= ProdMinQty ORDER BY ProdQty ASC";
                }
                else if (hasCategory)
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable WHERE ProdCat = @cat ORDER BY ProdId ASC";
                    parameters = new SqlParameter[] { new SqlParameter("@cat", categoryFilter) };
                }
                else
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable ORDER BY ProdId ASC";
                }

                DataTable dt = DbHelper.ExecuteQuery(query, parameters);
                dataGridViewProducts.DataSource = dt;

                if (!CurrentUser.IsAdmin)
                {
                    if (dataGridViewProducts.Columns["Maliyet (TL)"] != null)
                    {
                        dataGridViewProducts.Columns["Maliyet (TL)"].Visible = false;
                    }
                    if (dataGridViewProducts.Columns["Birim Kâr (TL)"] != null)
                    {
                        dataGridViewProducts.Columns["Birim Kâr (TL)"].Visible = false;
                    }
                }

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

        private string GenerateBarcode()
        {
            // 12 haneli veri basamakları: "869" + 9 rastgele rakam
            string prefix12 = "869" + rnd.Next(100000000, 999999999).ToString();
            while (prefix12.Length < 12)
            {
                prefix12 += rnd.Next(0, 10).ToString();
            }

            // EAN-13 Checksum algoritması:
            // 1-tabanlı indekste tek haneler (0, 2, 4...) * 1, çift haneler (1, 3, 5...) * 3
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = prefix12[i] - '0';
                sum += (i % 2 == 0) ? digit * 1 : digit * 3;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            return prefix12 + checkDigit.ToString();
        }

        private void btn_gen_barcode_Click(object sender, EventArgs e)
        {
            txt_prodbarcode.Text = GenerateBarcode();
        }

        private void ClearInputs()
        {
            txt_prodid.Text = string.Empty;
            txt_prodbarcode.Text = string.Empty;
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

            string barcode = txt_prodbarcode.Text.Trim();
            if (string.IsNullOrEmpty(barcode))
            {
                barcode = GenerateBarcode();
                txt_prodbarcode.Text = barcode;
            }

            try
            {
                string query = "INSERT INTO ProductTable (ProdBarcode, ProdName, ProdQty, ProdMinQty, ProdCostPrice, ProdPrice, ProdDesc, ProdCat) VALUES (@barcode, @name, @qty, @minqty, @cost, @price, @desc, @cat)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@barcode", barcode),
                    new SqlParameter("@name", txt_prodname.Text.Trim()),
                    new SqlParameter("@qty", qty),
                    new SqlParameter("@minqty", minQty),
                    new SqlParameter("@cost", costPrice),
                    new SqlParameter("@price", price),
                    new SqlParameter("@desc", txt_proddesc.Text.Trim()),
                    new SqlParameter("@cat", cmb_prodcat.SelectedItem.ToString())
                };

                DbHelper.ExecuteNonQuery(query, parameters);
                DbHelper.LogAction("Ürün Eklendi", "Ürün Yönetimi", $"Barkod: {barcode}, Ürün Adı: '{txt_prodname.Text.Trim()}', Miktar: {qty}, Kritik Sınır: {minQty}, Satış: {price:N2} TL, Kategori: '{cmb_prodcat.SelectedItem}'");
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

            int prodId = Convert.ToInt32(txt_prodid.Text);
            decimal costPrice = 0;
            if (!CurrentUser.IsAdmin)
            {
                // Admin değilse txt_prodcost gizlidir; veritabanındaki mevcut maliyet değerini koru
                object existingCostObj = DbHelper.ExecuteScalar("SELECT ProdCostPrice FROM ProductTable WHERE ProdId = @id", new SqlParameter[] { new SqlParameter("@id", prodId) });
                if (existingCostObj != null && existingCostObj != DBNull.Value)
                {
                    costPrice = Convert.ToDecimal(existingCostObj);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(txt_prodcost.Text))
                {
                    decimal.TryParse(txt_prodcost.Text.Trim(), out costPrice);
                }
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

            string barcode = txt_prodbarcode.Text.Trim();
            if (string.IsNullOrEmpty(barcode))
            {
                barcode = GenerateBarcode();
                txt_prodbarcode.Text = barcode;
            }

            try
            {
                string query = "UPDATE ProductTable SET ProdBarcode = @barcode, ProdName = @name, ProdQty = @qty, ProdMinQty = @minqty, ProdCostPrice = @cost, ProdPrice = @price, ProdDesc = @desc, ProdCat = @cat WHERE ProdId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@barcode", barcode),
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
                    DbHelper.LogAction("Ürün Güncellendi", "Ürün Yönetimi", $"ID: {txt_prodid.Text}, Barkod: {barcode}, Ürün Adı: '{txt_prodname.Text.Trim()}', Miktar: {qty}, Kritik Sınır: {minQty}, Satış: {price:N2} TL, Kategori: '{cmb_prodcat.SelectedItem}'");
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
            if (CurrentUser.IsCashier)
            {
                MessageBox.Show("Kasiyer yetkisi ile ürün silinemez!", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_prodid.Text))
            {
                MessageBox.Show("Lütfen silinecek ürünü listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string deletedName = txt_prodname.Text.Trim();
            string deletedId = txt_prodid.Text.Trim();

            DialogResult confirm = MessageBox.Show($"'{deletedName}' ürününü silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM ProductTable WHERE ProdId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@id", Convert.ToInt32(deletedId))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    DbHelper.LogAction("Ürün Silindi", "Ürün Yönetimi", $"ID: {deletedId}, Ürün Adı: '{deletedName}' kalıcı olarak silindi.");
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
                txt_prodid.Text = (row.Cells["Ürün ID"] != null ? row.Cells["Ürün ID"].Value?.ToString() : row.Cells[0].Value?.ToString()) ?? "";
                txt_prodbarcode.Text = (row.Cells["Barkod"] != null ? row.Cells["Barkod"].Value?.ToString() : row.Cells[1].Value?.ToString()) ?? "";
                txt_prodname.Text = (row.Cells["Ürün Adı"] != null ? row.Cells["Ürün Adı"].Value?.ToString() : row.Cells[2].Value?.ToString()) ?? "";
                txt_prodqty.Text = (row.Cells["Miktar"] != null ? row.Cells["Miktar"].Value?.ToString() : row.Cells[3].Value?.ToString()) ?? "";
                txt_prodminqty.Text = (row.Cells["Kritik Sınır"] != null ? row.Cells["Kritik Sınır"].Value?.ToString() : row.Cells[4].Value?.ToString()) ?? "5";
                
                txt_prodcost.Text = (row.Cells["Maliyet (TL)"] != null && row.Cells["Maliyet (TL)"].Value != null && row.Cells["Maliyet (TL)"].Value != DBNull.Value)
                    ? Convert.ToDecimal(row.Cells["Maliyet (TL)"].Value).ToString("N2")
                    : "0,00";

                txt_prodprice.Text = (row.Cells["Satış Fiyatı (TL)"] != null && row.Cells["Satış Fiyatı (TL)"].Value != null && row.Cells["Satış Fiyatı (TL)"].Value != DBNull.Value)
                    ? Convert.ToDecimal(row.Cells["Satış Fiyatı (TL)"].Value).ToString("N2")
                    : "";

                txt_proddesc.Text = (row.Cells["Açıklama"] != null ? row.Cells["Açıklama"].Value?.ToString() : row.Cells[8].Value?.ToString()) ?? "";
                string cat = (row.Cells["Kategori"] != null ? row.Cells["Kategori"].Value?.ToString() : row.Cells[9].Value?.ToString()) ?? "";
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
