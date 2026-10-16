using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ViewOrders : Form
    {
        private int selectedOrderId = 0;

        public ViewOrders()
        {
            InitializeComponent();
        }

        private void ViewOrders_Load(object sender, EventArgs e)
        {
            FormatGrid();
            PopulateOrders();
        }

        private void FormatGrid()
        {
            dataGridViewOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOrders.MultiSelect = false;
            dataGridViewOrders.ReadOnly = true;
            dataGridViewOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewOrders.AllowUserToAddRows = false;
        }

        private void PopulateOrders()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT OrderId AS [Sipariş No], CustId AS [Müşteri ID], CustName AS [Müşteri Adı], OrderDate AS [Sipariş Tarihi], TotalAmt AS [Toplam Tutar (TL)] FROM OrderTable ORDER BY OrderId ASC");
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

        private void ClearSelection()
        {
            selectedOrderId = 0;
            dataGridViewOrders.ClearSelection();
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
                string total = Convert.ToDecimal(row.Cells[4].Value).ToString("N2");

                lbl_summary.Text = $"Seçili Sipariş: #{selectedOrderId} | Müşteri: {custName} | Tarih: {orderDate} | Tutar: {total} TL";
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (selectedOrderId == 0)
            {
                MessageBox.Show("Lütfen silinecek siparişi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"#{selectedOrderId} numaralı siparişi silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM OrderTable WHERE OrderId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@id", selectedOrderId)
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    MessageBox.Show("Sipariş başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateOrders();
                }
                else
                {
                    MessageBox.Show("Silinecek sipariş bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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
