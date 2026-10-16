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
        private string selectedProdName = "";
        private decimal selectedProdPrice = 0;
        private int selectedProdStock = 0;
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
            cartTable.Columns.Add("Ürün Adı", typeof(string));
            cartTable.Columns.Add("Miktar", typeof(int));
            cartTable.Columns.Add("Birim Fiyat", typeof(decimal));
            cartTable.Columns.Add("Toplam", typeof(decimal));

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
                DataTable dt = DbHelper.ExecuteQuery("SELECT ProdId AS [ID], ProdName AS [Ürün Adı], ProdQty AS [Stok], ProdPrice AS [Fiyat], ProdCat AS [Kategori] FROM ProductTable ORDER BY ProdId ASC");
                dataGridViewProducts.DataSource = dt;
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
            selectedProdName = "";
            selectedProdPrice = 0;
            selectedProdStock = 0;
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
                selectedProdName = row.Cells[1].Value?.ToString() ?? "";
                selectedProdStock = Convert.ToInt32(row.Cells[2].Value);
                selectedProdPrice = Convert.ToDecimal(row.Cells[3].Value);
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
                cartTable.Rows.Add(selectedProdId, selectedProdName, qty, selectedProdPrice, total);
            }

            CalculateTotal();
            txt_qty.Text = string.Empty;
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
                            string prodName = row["Ürün Adı"].ToString();
                            int qty = Convert.ToInt32(row["Miktar"]);
                            decimal price = Convert.ToDecimal(row["Fiyat"]);
                            decimal total = Convert.ToDecimal(row["Toplam"]);

                            string detailQuery = "INSERT INTO OrderDetailTable (OrderId, ProdId, ProdName, Qty, Price, Total) VALUES (@orderid, @prodid, @prodname, @qty, @price, @total)";
                            using (SqlCommand cmd = new SqlCommand(detailQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@orderid", orderId);
                                cmd.Parameters.AddWithValue("@prodid", prodId);
                                cmd.Parameters.AddWithValue("@prodname", prodName);
                                cmd.Parameters.AddWithValue("@qty", qty);
                                cmd.Parameters.AddWithValue("@price", price);
                                cmd.Parameters.AddWithValue("@total", total);
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

                        MessageBox.Show($"#{orderId} numaralı sipariş başarıyla tamamlandı!\nToplam Tutar: {grandTotal:N2} TL", "Sipariş Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
