using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageOrders : Form
    {
        private DataTable cartTable;
        private int selectedProdId = 0;
        private string selectedProdBarcode = "";
        private string selectedProdName = "";
        private decimal selectedProdPrice = 0;
        private int selectedProdStock = 0;
        private decimal selectedProdCost = 0;
        private decimal grandTotal = 0;

        public ManageOrders()
        {
            InitializeComponent();
        }

        private void ManageOrders_Load(object sender, EventArgs e)
        {
            InitCartTable();
            FormatGrids();
            LoadCustomers();
            LoadProducts();
            ClearSelections();
        }

        private void InitCartTable()
        {
            cartTable = new DataTable();
            cartTable.Columns.Add("Ürün ID", typeof(int));
            cartTable.Columns.Add("Barkod", typeof(string));
            cartTable.Columns.Add("Ürün Adı", typeof(string));
            cartTable.Columns.Add("Miktar", typeof(int));
            cartTable.Columns.Add("Birim Fiyat", typeof(decimal));
            cartTable.Columns.Add("Toplam", typeof(decimal));
            cartTable.Columns.Add("Maliyet", typeof(decimal));

            dataGridViewCart.DataSource = cartTable;
        }

        private void FormatGrids()
        {
            dataGridViewCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCustomers.MultiSelect = false;
            dataGridViewCustomers.ReadOnly = true;
            dataGridViewCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCustomers.AllowUserToAddRows = false;

            dataGridViewProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewProducts.MultiSelect = false;
            dataGridViewProducts.ReadOnly = true;
            dataGridViewProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewProducts.AllowUserToAddRows = false;

            dataGridViewCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCart.MultiSelect = false;
            dataGridViewCart.ReadOnly = true;
            dataGridViewCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCart.AllowUserToAddRows = false;

            if (dataGridViewCart.Columns.Contains("Maliyet"))
                dataGridViewCart.Columns["Maliyet"].Visible = false;
            if (dataGridViewCart.Columns.Contains("Birim Fiyat"))
                dataGridViewCart.Columns["Birim Fiyat"].DefaultCellStyle.Format = "N2";
            if (dataGridViewCart.Columns.Contains("Toplam"))
                dataGridViewCart.Columns["Toplam"].DefaultCellStyle.Format = "N2";
        }

        private void LoadCustomers()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT CustId AS [ID], CustName AS [Müşteri Adı], CustPhone AS [Telefon] FROM CustomerTable ORDER BY CustId ASC");
                dataGridViewCustomers.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT ProdId AS [ID], ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], ProdQty AS [Stok], ProdPrice AS [Fiyat], ProdCostPrice AS [Maliyet], ProdCat AS [Kategori] FROM ProductTable ORDER BY ProdId ASC");
                dataGridViewProducts.DataSource = dt;
                if (dataGridViewProducts.Columns.Contains("Maliyet"))
                    dataGridViewProducts.Columns["Maliyet"].Visible = false;
                if (dataGridViewProducts.Columns.Contains("Fiyat"))
                    dataGridViewProducts.Columns["Fiyat"].DefaultCellStyle.Format = "N2";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürünler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearSelections()
        {
            txt_custid.Text = string.Empty;
            txt_custname.Text = string.Empty;
            txt_qty.Text = string.Empty;
            selectedProdId = 0;
            selectedProdBarcode = "";
            selectedProdName = "";
            selectedProdPrice = 0;
            selectedProdStock = 0;
            selectedProdCost = 0;
            dataGridViewCustomers.ClearSelection();
            dataGridViewProducts.ClearSelection();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearSelections();
        }

        private void dataGridViewCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewCustomers.Rows.Count)
            {
                DataGridViewRow row = dataGridViewCustomers.Rows[e.RowIndex];
                txt_custid.Text = row.Cells[0].Value?.ToString() ?? "";
                txt_custname.Text = row.Cells[1].Value?.ToString() ?? "";
            }
        }

        private void dataGridViewProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewProducts.Rows.Count)
            {
                DataGridViewRow row = dataGridViewProducts.Rows[e.RowIndex];
                selectedProdId = Convert.ToInt32(row.Cells[0].Value);
                selectedProdBarcode = row.Cells[1].Value?.ToString() ?? "";
                selectedProdName = row.Cells[2].Value?.ToString() ?? "";
                selectedProdStock = Convert.ToInt32(row.Cells[3].Value);
                selectedProdPrice = Convert.ToDecimal(row.Cells[4].Value);
                selectedProdCost = row.Cells[5].Value != DBNull.Value ? Convert.ToDecimal(row.Cells[5].Value) : 0m;
                txt_qty.Focus();
            }
        }

        private void btn_add_to_cart_Click(object sender, EventArgs e)
        {
            if (selectedProdId == 0)
            {
                MessageBox.Show("Lütfen önce sağdaki listeden bir ürün seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txt_qty.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Lütfen 0'dan büyük geçerli bir miktar giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (qty > selectedProdStock)
            {
                MessageBox.Show($"Yetersiz stok! Mevcut stok: {selectedProdStock}", "Stok Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if already in cart
            bool alreadyInCart = false;
            foreach (DataRow row in cartTable.Rows)
            {
                if (Convert.ToInt32(row["Ürün ID"]) == selectedProdId)
                {
                    int currentQty = Convert.ToInt32(row["Miktar"]);
                    if (currentQty + qty > selectedProdStock)
                    {
                        MessageBox.Show($"Sepetteki adetle birlikte stok sınırı aşılıyor! Maksimum eklenebilecek: {selectedProdStock - currentQty}", "Stok Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    row["Miktar"] = currentQty + qty;
                    row["Toplam"] = (currentQty + qty) * selectedProdPrice;
                    alreadyInCart = true;
                    break;
                }
            }

            if (!alreadyInCart)
            {
                decimal total = qty * selectedProdPrice;
                cartTable.Rows.Add(selectedProdId, selectedProdBarcode, selectedProdName, qty, selectedProdPrice, total, selectedProdCost);
            }

            CalculateTotal();
            txt_qty.Text = string.Empty;
        }

        private void AddProductByBarcode(string barcodeText)
        {
            if (string.IsNullOrWhiteSpace(barcodeText))
                return;

            barcodeText = barcodeText.Trim();
            try
            {
                string query = "SELECT ProdId, ProdBarcode, ProdName, ProdQty, ProdPrice, ProdCostPrice FROM ProductTable WHERE ProdBarcode = @code OR CAST(ProdId AS NVARCHAR) = @code";
                DataTable dt = DbHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@code", barcodeText) });

                if (dt == null || dt.Rows.Count == 0)
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show($"'{barcodeText}' barkodlu/kodlu ürün sistemde bulunamadı!", "Ürün Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txt_barcode.SelectAll();
                    txt_barcode.Focus();
                    return;
                }

                DataRow row = dt.Rows[0];
                int prodId = Convert.ToInt32(row["ProdId"]);
                string barcode = row["ProdBarcode"]?.ToString() ?? "";
                string prodName = row["ProdName"]?.ToString() ?? "";
                int stock = Convert.ToInt32(row["ProdQty"]);
                decimal price = Convert.ToDecimal(row["ProdPrice"]);
                decimal cost = (row["ProdCostPrice"] != DBNull.Value) ? Convert.ToDecimal(row["ProdCostPrice"]) : 0m;

                if (stock <= 0)
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show($"'{prodName}' ürününün stoğu tükenmiştir! (Mevcut Stok: 0)", "Stok Yetersiz", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txt_barcode.SelectAll();
                    txt_barcode.Focus();
                    return;
                }

                // Check if already in cart
                bool alreadyInCart = false;
                foreach (DataRow cartRow in cartTable.Rows)
                {
                    if (Convert.ToInt32(cartRow["Ürün ID"]) == prodId)
                    {
                        int currentQty = Convert.ToInt32(cartRow["Miktar"]);
                        if (currentQty + 1 > stock)
                        {
                            System.Media.SystemSounds.Beep.Play();
                            MessageBox.Show($"'{prodName}' için sepetteki adet mevcut stoku ({stock}) aşıyor!", "Stok Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txt_barcode.SelectAll();
                            txt_barcode.Focus();
                            return;
                        }
                        cartRow["Miktar"] = currentQty + 1;
                        cartRow["Toplam"] = (currentQty + 1) * price;
                        alreadyInCart = true;
                        break;
                    }
                }

                if (!alreadyInCart)
                {
                    cartTable.Rows.Add(prodId, barcode, prodName, 1, price, price, cost);
                }

                CalculateTotal();
                System.Media.SystemSounds.Asterisk.Play();

                // Select row in dataGridViewProducts if visible
                foreach (DataGridViewRow dgvRow in dataGridViewProducts.Rows)
                {
                    if (Convert.ToInt32(dgvRow.Cells[0].Value) == prodId)
                    {
                        dgvRow.Selected = true;
                        dataGridViewProducts.FirstDisplayedScrollingRowIndex = dgvRow.Index;
                        break;
                    }
                }

                txt_barcode.Clear();
                txt_barcode.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Barkod ile ürün eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txt_barcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddProductByBarcode(txt_barcode.Text);
            }
        }

        private void btn_barcode_add_Click(object sender, EventArgs e)
        {
            AddProductByBarcode(txt_barcode.Text);
        }

        private void CalculateTotal()
        {
            grandTotal = 0;
            foreach (DataRow row in cartTable.Rows)
            {
                grandTotal += Convert.ToDecimal(row["Toplam"]);
            }
            lbl_total.Text = grandTotal.ToString("N2") + " TL";
        }

        private void btn_clear_cart_Click(object sender, EventArgs e)
        {
            cartTable.Rows.Clear();
            CalculateTotal();
        }

        private void btn_insert_order_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_custid.Text))
            {
                MessageBox.Show("Lütfen önce listeden siparişi veren müşteriyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cartTable.Rows.Count == 0)
            {
                MessageBox.Show("Sipariş sepeti boş. Lütfen en az bir ürün ekleyiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(DbHelper.ConnectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert into OrderTable and retrieve generated OrderId
                        string orderQuery = "INSERT INTO OrderTable (CustId, CustName, OrderDate, TotalAmt, OrderStatus) OUTPUT INSERTED.OrderId VALUES (@custid, @custname, @date, @total, @status)";
                        int orderId;
                        using (SqlCommand cmd = new SqlCommand(orderQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@custid", Convert.ToInt32(txt_custid.Text));
                            cmd.Parameters.AddWithValue("@custname", txt_custname.Text.Trim());
                            cmd.Parameters.AddWithValue("@date", DateTime.Now);
                            cmd.Parameters.AddWithValue("@total", grandTotal);
                            cmd.Parameters.AddWithValue("@status", "Tamamlandı");
                            orderId = (int)cmd.ExecuteScalar();
                        }

                        // 2. Insert items into OrderDetailTable and decrement stock in ProductTable
                        foreach (DataRow row in cartTable.Rows)
                        {
                            int prodId = Convert.ToInt32(row["Ürün ID"]);
                            string barcode = (row.Table.Columns.Contains("Barkod") && row["Barkod"] != DBNull.Value) ? row["Barkod"].ToString() : "";
                            string prodName = row["Ürün Adı"].ToString();
                            int qty = Convert.ToInt32(row["Miktar"]);
                            decimal price = Convert.ToDecimal(row["Birim Fiyat"]);
                            decimal total = Convert.ToDecimal(row["Toplam"]);
                            decimal cost = (row.Table.Columns.Contains("Maliyet") && row["Maliyet"] != DBNull.Value) ? Convert.ToDecimal(row["Maliyet"]) : 0m;

                            string detailQuery = "INSERT INTO OrderDetailTable (OrderId, ProdId, ProdBarcode, ProdName, Qty, Price, Total, CostPrice) VALUES (@orderid, @prodid, @barcode, @prodname, @qty, @price, @total, @cost)";
                            using (SqlCommand cmd = new SqlCommand(detailQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@orderid", orderId);
                                cmd.Parameters.AddWithValue("@prodid", prodId);
                                cmd.Parameters.AddWithValue("@barcode", barcode);
                                cmd.Parameters.AddWithValue("@prodname", prodName);
                                cmd.Parameters.AddWithValue("@qty", qty);
                                cmd.Parameters.AddWithValue("@price", price);
                                cmd.Parameters.AddWithValue("@total", total);
                                cmd.Parameters.AddWithValue("@cost", cost);
                                cmd.ExecuteNonQuery();
                            }

                            string updateStockQuery = "UPDATE ProductTable SET ProdQty = ProdQty - @qty WHERE ProdId = @id";
                            using (SqlCommand cmd = new SqlCommand(updateStockQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@qty", qty);
                                cmd.Parameters.AddWithValue("@id", prodId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();

                        DialogResult printChoice = MessageBox.Show(
                            $"#{orderId} numaralı sipariş başarıyla tamamlandı!\nToplam Tutar: {grandTotal:N2} TL\n\nSatış fişini görüntülemek ve yazdırmak ister misiniz?",
                            "Sipariş Tamamlandı",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information
                        );

                        if (printChoice == DialogResult.Yes)
                        {
                            DataTable printItems = cartTable.Copy();
                            string custName = txt_custname.Text.Trim();
                            ReceiptPrinter.ShowReceipt(orderId, custName, DateTime.Now, "Tamamlandı", grandTotal, printItems);
                        }

                        // Clear & reload
                        cartTable.Rows.Clear();
                        CalculateTotal();
                        ClearSelections();
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("Sipariş kaydedilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.Show();
            this.Close();
        }

        private void btn_home_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
        }

        private void btn_view_orders_Click(object sender, EventArgs e)
        {
            ViewOrders vo = new ViewOrders();
            vo.Show();
            this.Close();
        }
    }
}
