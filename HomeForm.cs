using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {
            ApplyRolePermissions();
            LoadStats();
        }

        private void HomeForm_Activated(object sender, EventArgs e)
        {
            ApplyRolePermissions();
            LoadStats();
        }

        private void ApplyRolePermissions()
        {
            if (CurrentUser.IsLoggedIn)
            {
                lblSubtitle.Text = $"Giriş Yapan: {CurrentUser.FullName}   |   Rol: {CurrentUser.Role}";
            }

            if (CurrentUser.IsCashier)
            {
                // Kasiyer: Hide user management, users card, audit logs, and financial overview
                btnUsers.Visible = false;
                panelCardUsers.Visible = false;
                lblFinanceSummary.Visible = false;
                btnAuditLogs.Visible = false;
                btnOrders.Visible = true;
                panelCardOrders.Visible = true;
            }
            else if (CurrentUser.IsWarehouse)
            {
                // Depo Görevlisi: Hide user management, users card, orders, audit logs, and financial overview
                btnUsers.Visible = false;
                panelCardUsers.Visible = false;
                btnOrders.Visible = false;
                panelCardOrders.Visible = false;
                lblFinanceSummary.Visible = false;
                btnAuditLogs.Visible = false;
            }
            else
            {
                // Admin: Full access
                btnUsers.Visible = true;
                panelCardUsers.Visible = true;
                btnOrders.Visible = true;
                panelCardOrders.Visible = true;
                lblFinanceSummary.Visible = true;
                btnAuditLogs.Visible = true;
            }
        }

        public void LoadStats()
        {
            try
            {
                object prodCount = DbHelper.ExecuteScalar("SELECT COUNT(*) FROM ProductTable");
                lblCountProducts.Text = prodCount?.ToString() ?? "0";

                object catCount = DbHelper.ExecuteScalar("SELECT COUNT(*) FROM CategoryTable");
                lblCountCategories.Text = catCount?.ToString() ?? "0";

                object userCount = DbHelper.ExecuteScalar("SELECT COUNT(*) FROM UserTable");
                lblCountUsers.Text = userCount?.ToString() ?? "0";

                object orderCount = DbHelper.ExecuteScalar("SELECT COUNT(*) FROM OrderTable");
                lblCountOrders.Text = orderCount?.ToString() ?? "0";

                object critCount = DbHelper.ExecuteScalar("SELECT COUNT(*) FROM ProductTable WHERE ProdQty <= ProdMinQty");
                lblCountCritical.Text = critCount?.ToString() ?? "0";

                // Financial Overview: Revenue, Cost, Net Profit (Only computed for Admin)
                if (CurrentUser.IsAdmin)
                {
                    object revObj = DbHelper.ExecuteScalar("SELECT ISNULL(SUM(TotalAmt), 0) FROM OrderTable WHERE OrderStatus != 'İptal / İade Edildi'");
                    decimal totalRev = (revObj != null && revObj != DBNull.Value) ? Convert.ToDecimal(revObj) : 0m;

                    object costObj = DbHelper.ExecuteScalar("SELECT ISNULL(SUM(od.CostPrice * od.Qty), 0) FROM OrderDetailTable od INNER JOIN OrderTable o ON od.OrderId = o.OrderId WHERE o.OrderStatus != 'İptal / İade Edildi'");
                    decimal totalCost = (costObj != null && costObj != DBNull.Value) ? Convert.ToDecimal(costObj) : 0m;

                    decimal netProfit = totalRev - totalCost;
                    decimal profitMargin = totalRev > 0 ? (netProfit / totalRev * 100m) : 0m;

                    if (netProfit >= 0)
                    {
                        lblFinanceSummary.ForeColor = Color.DarkGreen;
                        lblFinanceSummary.Text = $"💰 Finansal Durum: Toplam Ciro: {totalRev:N2} TL   |   Toplam Maliyet: {totalCost:N2} TL   |   Net Kâr: +{netProfit:N2} TL (%{profitMargin:F1} Marj)";
                    }
                    else
                    {
                        lblFinanceSummary.ForeColor = Color.Crimson;
                        lblFinanceSummary.Text = $"💰 Finansal Durum: Toplam Ciro: {totalRev:N2} TL   |   Toplam Maliyet: {totalCost:N2} TL   |   Net Zarar: {netProfit:N2} TL (%{profitMargin:F1})";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DbHelper.LogAction("Çıkış Yapıldı", "Oturum / Kimlik", $"'{CurrentUser.Username}' ({CurrentUser.Role}) oturumu sonlandırdı.");
            CurrentUser.Clear();
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void btnAuditLogs_Click(object sender, EventArgs e)
        {
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Bu alana yalnızca Yönetici (Admin) yetkisine sahip kullanıcılar erişebilir.", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ViewAuditLogs logs = new ViewAuditLogs();
            logs.Show();
            this.Hide();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Bu alana yalnızca Yönetici (Admin) yetkisine sahip kullanıcılar erişebilir.", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ManageUsers users = new ManageUsers();
            users.Show();
            this.Hide();
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            ManageCategories categories = new ManageCategories();
            categories.Show();
            this.Hide();
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            ManageProducts products = new ManageProducts();
            products.Show();
            this.Hide();
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            ManageCustomers customers = new ManageCustomers();
            customers.Show();
            this.Hide();
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            if (CurrentUser.IsWarehouse)
            {
                MessageBox.Show("Depo görevlileri sipariş yönetimi ekranına erişemez.", "Yetki Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ViewOrders orders = new ViewOrders();
            orders.Show();
            this.Hide();
        }

        private void panelCardProducts_Click(object sender, EventArgs e)
        {
            btnProducts_Click(sender, e);
        }

        private void panelCardCategories_Click(object sender, EventArgs e)
        {
            btnCategories_Click(sender, e);
        }

        private void panelCardUsers_Click(object sender, EventArgs e)
        {
            btnUsers_Click(sender, e);
        }

        private void panelCardOrders_Click(object sender, EventArgs e)
        {
            btnOrders_Click(sender, e);
        }

        private void panelCardCritical_Click(object sender, EventArgs e)
        {
            ManageProducts products = new ManageProducts(true);
            products.Show();
            this.Hide();
        }
    }
}
