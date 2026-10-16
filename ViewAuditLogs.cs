using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ViewAuditLogs : Form
    {
        public ViewAuditLogs()
        {
            InitializeComponent();
        }

        private void ViewAuditLogs_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Bu alana yalnızca Yönetici (Admin) yetkisine sahip kullanıcılar erişebilir.", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                HomeForm home = new HomeForm();
                home.Show();
                this.Close();
                return;
            }

            FormatGrid();
            LoadFilterOptions();
            PopulateLogs();
        }

        private void FormatGrid()
        {
            dataGridViewLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewLogs.MultiSelect = false;
            dataGridViewLogs.ReadOnly = true;
            dataGridViewLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewLogs.AllowUserToAddRows = false;
            dataGridViewLogs.RowHeadersVisible = false;
        }

        private void LoadFilterOptions()
        {
            cmb_module.Items.Clear();
            cmb_module.Items.Add("-- Tüm Modüller --");
            cmb_module.Items.Add("Oturum / Kimlik");
            cmb_module.Items.Add("Ürün Yönetimi");
            cmb_module.Items.Add("Kategori Yönetimi");
            cmb_module.Items.Add("Sipariş Yönetimi");
            cmb_module.Items.Add("Kullanıcı Yönetimi");
            cmb_module.Items.Add("Müşteri Yönetimi");
            cmb_module.SelectedIndex = 0;

            cmb_action.Items.Clear();
            cmb_action.Items.Add("-- Tüm İşlemler --");
            cmb_action.Items.Add("Giriş Başarılı");
            cmb_action.Items.Add("Başarısız Giriş");
            cmb_action.Items.Add("Çıkış Yapıldı");
            cmb_action.Items.Add("Ürün Eklendi");
            cmb_action.Items.Add("Ürün Güncellendi");
            cmb_action.Items.Add("Ürün Silindi");
            cmb_action.Items.Add("Kategori Eklendi");
            cmb_action.Items.Add("Kategori Güncellendi");
            cmb_action.Items.Add("Kategori Silindi");
            cmb_action.Items.Add("Sipariş Oluşturuldu");
            cmb_action.Items.Add("Sipariş İptal / İade");
            cmb_action.Items.Add("Kullanıcı Eklendi");
            cmb_action.Items.Add("Kullanıcı Güncellendi");
            cmb_action.Items.Add("Kullanıcı Silindi");
            cmb_action.Items.Add("Müşteri Eklendi");
            cmb_action.Items.Add("Müşteri Güncellendi");
            cmb_action.Items.Add("Müşteri Silindi");
            cmb_action.SelectedIndex = 0;
        }

        private void PopulateLogs()
        {
            try
            {
                string query = "SELECT LogId AS [Kayıt No], LogDate AS [Tarih & Saat], Username AS [Kullanıcı], UserRole AS [Rol], ActionType AS [İşlem Türü], Module AS [Modül], Details AS [İşlem Detayları] FROM AuditLogTable WHERE 1=1";
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (cmb_module.SelectedIndex > 0)
                {
                    query += " AND Module = @module";
                    parameters.Add(new SqlParameter("@module", cmb_module.SelectedItem.ToString()));
                }

                if (cmb_action.SelectedIndex > 0)
                {
                    query += " AND ActionType = @action";
                    parameters.Add(new SqlParameter("@action", cmb_action.SelectedItem.ToString()));
                }

                string searchText = txt_search.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    query += " AND (Username LIKE @search OR Details LIKE @search OR ActionType LIKE @search)";
                    parameters.Add(new SqlParameter("@search", "%" + searchText + "%"));
                }

                query += " ORDER BY LogId DESC";

                DataTable dt = DbHelper.ExecuteQuery(query, parameters.Count > 0 ? parameters.ToArray() : null);
                dataGridViewLogs.DataSource = dt;

                if (dataGridViewLogs.Columns["Tarih & Saat"] != null)
                {
                    dataGridViewLogs.Columns["Tarih & Saat"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
                }

                if (dataGridViewLogs.Columns["Kayıt No"] != null) dataGridViewLogs.Columns["Kayıt No"].FillWeight = 40;
                if (dataGridViewLogs.Columns["Tarih & Saat"] != null) dataGridViewLogs.Columns["Tarih & Saat"].FillWeight = 85;
                if (dataGridViewLogs.Columns["Kullanıcı"] != null) dataGridViewLogs.Columns["Kullanıcı"].FillWeight = 60;
                if (dataGridViewLogs.Columns["Rol"] != null) dataGridViewLogs.Columns["Rol"].FillWeight = 70;
                if (dataGridViewLogs.Columns["İşlem Türü"] != null) dataGridViewLogs.Columns["İşlem Türü"].FillWeight = 80;
                if (dataGridViewLogs.Columns["Modül"] != null) dataGridViewLogs.Columns["Modül"].FillWeight = 75;
                if (dataGridViewLogs.Columns["İşlem Detayları"] != null) dataGridViewLogs.Columns["İşlem Detayları"].FillWeight = 190;

                lbl_status.Text = $"Toplam {dt.Rows.Count} denetim kaydı listeleniyor.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Denetim kayıtları yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            PopulateLogs();
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            cmb_module.SelectedIndex = 0;
            cmb_action.SelectedIndex = 0;
            txt_search.Text = string.Empty;
            PopulateLogs();
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            string fileName = $"Sistem_Islem_Gecmisi_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            DbHelper.ExportToCsv(dataGridViewLogs, fileName);
        }

        private void dataGridViewLogs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewLogs.Rows.Count)
            {
                DataGridViewRow row = dataGridViewLogs.Rows[e.RowIndex];
                string action = row.Cells["İşlem Türü"]?.Value?.ToString() ?? "";

                if (action.Contains("Silin") || action.Contains("İptal") || action.Contains("Başarısız"))
                {
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    row.DefaultCellStyle.SelectionBackColor = Color.IndianRed;
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
                else if (action.Contains("Eklen") || action.Contains("Başarılı") || action.Contains("Oluşturuldu"))
                {
                    row.DefaultCellStyle.BackColor = Color.Honeydew;
                    row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                    row.DefaultCellStyle.SelectionBackColor = Color.MediumSeaGreen;
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
                else if (action.Contains("Güncellen"))
                {
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                    row.DefaultCellStyle.ForeColor = Color.DarkGoldenrod;
                    row.DefaultCellStyle.SelectionBackColor = Color.Orange;
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
            }
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
    }
}
