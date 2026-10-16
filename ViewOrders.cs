using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ViewOrders : Form
    {
        private int selectedOrderId = 0;
        private string selectedOrderStatus = string.Empty;

        public ViewOrders()
        {
            InitializeComponent();
        }

        private void ViewOrders_Load(object sender, EventArgs e)
        {
            FormatGrids();
            PopulateOrders();
        }

        private void FormatGrids()
        {
            // Orders Grid
            dataGridViewOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOrders.MultiSelect = false;
            dataGridViewOrders.ReadOnly = true;
            dataGridViewOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewOrders.AllowUserToAddRows = false;

            // Details Grid
            dataGridViewOrderDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOrderDetails.MultiSelect = false;
            dataGridViewOrderDetails.ReadOnly = true;
            dataGridViewOrderDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewOrderDetails.AllowUserToAddRows = false;
        }

        private void PopulateOrders()
        {
            try
            {
                string query = "SELECT OrderId AS [Sipariş No], CustId AS [Müşteri ID], CustName AS [Müşteri Adı], OrderDate AS [Sipariş Tarihi], TotalAmt AS [Toplam Tutar (TL)], OrderStatus AS [Durum] FROM OrderTable ORDER BY OrderId ASC";
                DataTable dt = DbHelper.ExecuteQuery(query);
                dataGridViewOrders.DataSource = dt;

                if (dataGridViewOrders.Columns["Toplam Tutar (TL)"] != null)
                {
                    dataGridViewOrders.Columns["Toplam Tutar (TL)"].DefaultCellStyle.Format = "N2";
                }

                ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Siparişler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOrderDetails(int orderId)
        {
            try
            {
                string query = "SELECT ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], Qty AS [Adet], CostPrice AS [Birim Maliyet (TL)], Price AS [Satış Fiyatı (TL)], Total AS [Toplam Tutar (TL)], ((Price - CostPrice) * Qty) AS [Net Kâr (TL)] FROM OrderDetailTable WHERE OrderId = @orderid ORDER BY DetailId ASC";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@orderid", orderId)
                };

                DataTable dt = DbHelper.ExecuteQuery(query, parameters);
                dataGridViewOrderDetails.DataSource = dt;

                if (dataGridViewOrderDetails.Columns["Birim Maliyet (TL)"] != null)
                {
                    dataGridViewOrderDetails.Columns["Birim Maliyet (TL)"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridViewOrderDetails.Columns["Satış Fiyatı (TL)"] != null)
                {
                    dataGridViewOrderDetails.Columns["Satış Fiyatı (TL)"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridViewOrderDetails.Columns["Toplam Tutar (TL)"] != null)
                {
                    dataGridViewOrderDetails.Columns["Toplam Tutar (TL)"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridViewOrderDetails.Columns["Net Kâr (TL)"] != null)
                {
                    dataGridViewOrderDetails.Columns["Net Kâr (TL)"].DefaultCellStyle.Format = "N2";
                }

                dataGridViewOrderDetails.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sipariş detayları yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearSelection()
        {
            selectedOrderId = 0;
            selectedOrderStatus = string.Empty;
            dataGridViewOrders.ClearSelection();
            dataGridViewOrderDetails.DataSource = null;
            lbl_summary.Text = "Seçili Sipariş: Yok (Listeden bir sipariş seçiniz)";
        }

        private void dataGridViewOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewOrders.Rows.Count)
            {
                DataGridViewRow row = dataGridViewOrders.Rows[e.RowIndex];
                selectedOrderId = Convert.ToInt32(row.Cells[0].Value);
                string custName = row.Cells[2].Value?.ToString() ?? "";
                string orderDate = Convert.ToDateTime(row.Cells[3].Value).ToString("dd.MM.yyyy HH:mm");
                decimal total = Convert.ToDecimal(row.Cells[4].Value);
                selectedOrderStatus = row.Cells[5].Value?.ToString() ?? "Tamamlandı";

                decimal cost = 0;
                try
                {
                    object costObj = DbHelper.ExecuteScalar("SELECT ISNULL(SUM(CostPrice * Qty), 0) FROM OrderDetailTable WHERE OrderId = @id", new SqlParameter[] { new SqlParameter("@id", selectedOrderId) });
                    if (costObj != null && costObj != DBNull.Value)
                        cost = Convert.ToDecimal(costObj);
                }
                catch { }

                decimal netProfit = total - cost;
                decimal margin = total > 0 ? (netProfit / total * 100m) : 0m;

                lbl_summary.Text = $"Seçili Sipariş: #{selectedOrderId} | Müşteri: {custName} | Tarih: {orderDate} | Tutar: {total:N2} TL | Maliyet: {cost:N2} TL | Net Kâr: {netProfit:N2} TL (%{margin:F1}) | Durum: {selectedOrderStatus}";

                LoadOrderDetails(selectedOrderId);
            }
        }

        private void dataGridViewOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewOrders.Rows.Count)
            {
                DataGridViewRow row = dataGridViewOrders.Rows[e.RowIndex];
                string status = row.Cells["Durum"]?.Value?.ToString();

                if (status == "İptal / İade Edildi")
                {
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    row.DefaultCellStyle.SelectionBackColor = Color.IndianRed;
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
            }
        }

        private void btn_return_Click(object sender, EventArgs e)
        {
            if (selectedOrderId == 0)
            {
                MessageBox.Show("Lütfen iade edilecek siparişi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedOrderStatus == "İptal / İade Edildi")
            {
                MessageBox.Show("Bu sipariş zaten iade/iptal edilmiştir. Tekrar iade işlemi yapılamaz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"#{selectedOrderId} numaralı siparişi İADE / İPTAL etmek istediğinize emin misiniz?\n\n" +
                "Bu işlem sonucunda:\n" +
                "• Siparişteki tüm ürünler otomatik olarak depoya/stoğa geri yüklenecektir.\n" +
                "• Sipariş durumu 'İptal / İade Edildi' olarak güncellenecektir.",
                "Sipariş İade Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(DbHelper.ConnectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Get ordered items
                        string getItemsQuery = "SELECT ProdId, Qty FROM OrderDetailTable WHERE OrderId = @orderid";
                        DataTable dtItems = new DataTable();
                        using (SqlCommand cmd = new SqlCommand(getItemsQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@orderid", selectedOrderId);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dtItems);
                            }
                        }

                        // 2. Restore stock in ProductTable
                        foreach (DataRow row in dtItems.Rows)
                        {
                            int prodId = Convert.ToInt32(row["ProdId"]);
                            int qty = Convert.ToInt32(row["Qty"]);

                            string restoreStockQuery = "UPDATE ProductTable SET ProdQty = ProdQty + @qty WHERE ProdId = @id";
                            using (SqlCommand cmd = new SqlCommand(restoreStockQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@qty", qty);
                                cmd.Parameters.AddWithValue("@id", prodId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3. Update OrderStatus in OrderTable
                        string updateOrderQuery = "UPDATE OrderTable SET OrderStatus = @status WHERE OrderId = @orderid";
                        using (SqlCommand cmd = new SqlCommand(updateOrderQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@status", "İptal / İade Edildi");
                            cmd.Parameters.AddWithValue("@orderid", selectedOrderId);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();

                        MessageBox.Show($"#{selectedOrderId} numaralı sipariş başarıyla iade edildi!\n{dtItems.Rows.Count} kalem ürün stoğa geri eklendi.", "İade Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PopulateOrders();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("İade işlemi sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (selectedOrderId == 0)
            {
                MessageBox.Show("Lütfen silinecek siparişi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string prompt = $"#{selectedOrderId} numaralı siparişi kalıcı olarak silmek istediğinize emin misiniz?";
            if (selectedOrderStatus != "İptal / İade Edildi")
            {
                prompt += "\n\n⚠️ Dikkat: Bu sipariş henüz iade edilmemiştir. Ürünleri stoğa geri kazandırmak istiyorsanız lütfen önce 'Siparişi İade Et' butonunu kullanınız.\n\nYine de kaydı tamamen silmek istiyor musunuz?";
            }

            DialogResult confirm = MessageBox.Show(prompt, "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                // Delete detail rows then order row
                DbHelper.ExecuteNonQuery("DELETE FROM OrderDetailTable WHERE OrderId = @id", new SqlParameter[] { new SqlParameter("@id", selectedOrderId) });
                DbHelper.ExecuteNonQuery("DELETE FROM OrderTable WHERE OrderId = @id", new SqlParameter[] { new SqlParameter("@id", selectedOrderId) });

                MessageBox.Show("Sipariş başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sipariş silinirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_new_order_Click(object sender, EventArgs e)
        {
            ManageOrders mo = new ManageOrders();
            mo.Show();
            this.Close();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearSelection();
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            PopulateOrders();
        }

        private void btn_export_orders_Click(object sender, EventArgs e)
        {
            string fileName = $"Siparis_Listesi_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            DbHelper.ExportToCsv(dataGridViewOrders, fileName);
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            if (selectedOrderId == 0)
            {
                MessageBox.Show("Lütfen fişini yazdırmak istediğiniz siparişi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable dtItems = dataGridViewOrderDetails.DataSource as DataTable;
            if (dtItems == null || dtItems.Rows.Count == 0)
            {
                string query = "SELECT ProdName AS [Ürün Adı], Qty AS [Adet], Price AS [Birim Fiyat (TL)], Total AS [Toplam (TL)] FROM OrderDetailTable WHERE OrderId = @orderid ORDER BY DetailId ASC";
                dtItems = DbHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@orderid", selectedOrderId) });
            }

            // Find selected row
            DataGridViewRow selectedRow = null;
            foreach (DataGridViewRow r in dataGridViewOrders.Rows)
            {
                if (Convert.ToInt32(r.Cells[0].Value) == selectedOrderId)
                {
                    selectedRow = r;
                    break;
                }
            }

            if (selectedRow == null) return;

            string custName = selectedRow.Cells[2].Value?.ToString() ?? "";
            DateTime orderDate = Convert.ToDateTime(selectedRow.Cells[3].Value);
            decimal total = Convert.ToDecimal(selectedRow.Cells[4].Value);
            string status = selectedRow.Cells[5].Value?.ToString() ?? "Tamamlandı";

            ReceiptPrinter.ShowReceipt(selectedOrderId, custName, orderDate, status, total, dtItems);
        }

        private void btn_home_Click(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.Show();
            this.Close();
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            btn_home_Click(sender, e);
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            btn_home_Click(sender, e);
        }
    }
}
