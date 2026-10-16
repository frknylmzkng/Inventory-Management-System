using System;
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
            LoadStats();
        }

        private void HomeForm_Activated(object sender, EventArgs e)
        {
            LoadStats();
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
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
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
            ViewOrders orders = new ViewOrders();
            orders.Show();
            this.Hide();
        }
    }
}
