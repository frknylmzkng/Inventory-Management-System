using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public static class DbHelper
    {
        public static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\FURKAN YILMAZ\Documents\InventoryDB_03052026.mdf;Integrated Security=True;Connect Timeout=30";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }

        public static void ExportToCsv(DataGridView grid, string defaultFileName)
        {
            if (grid == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("Dışa aktarılacak veri bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel CSV Dosyası (*.csv)|*.csv|Metin Dosyası (*.txt)|*.txt";
                sfd.FileName = defaultFileName;
                sfd.Title = "Verileri Excel / CSV Olarak Kaydet";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName, false, new UTF8Encoding(true)))
                        {
                            // Headers
                            System.Collections.Generic.List<string> headers = new System.Collections.Generic.List<string>();
                            foreach (DataGridViewColumn col in grid.Columns)
                            {
                                if (col.Visible)
                                {
                                    headers.Add("\"" + col.HeaderText.Replace("\"", "\"\"") + "\"");
                                }
                            }
                            sw.WriteLine(string.Join(";", headers));

                            // Rows
                            foreach (DataGridViewRow row in grid.Rows)
                            {
                                if (row.IsNewRow) continue;
                                System.Collections.Generic.List<string> cells = new System.Collections.Generic.List<string>();
                                foreach (DataGridViewColumn col in grid.Columns)
                                {
                                    if (col.Visible)
                                    {
                                        string val = row.Cells[col.Index].Value?.ToString() ?? "";
                                        cells.Add("\"" + val.Replace("\"", "\"\"") + "\"");
                                    }
                                }
                                sw.WriteLine(string.Join(";", cells));
                            }
                        }

                        DialogResult openChoice = MessageBox.Show(
                            "Veriler başarıyla Excel/CSV dosyasına aktarıldı!\n\nDosyayı şimdi açmak ister misiniz?",
                            "Dışa Aktarma Başarılı",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information
                        );

                        if (openChoice == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Dışa aktarma sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public static void LogAction(string actionType, string module, string details, string username = null, string userRole = null)
        {
            try
            {
                string user = !string.IsNullOrEmpty(username) ? username : (CurrentUser.IsLoggedIn ? CurrentUser.Username : "Sistem");
                string role = !string.IsNullOrEmpty(userRole) ? userRole : (CurrentUser.IsLoggedIn ? CurrentUser.Role : "Bilinmiyor");

                string query = "INSERT INTO AuditLogTable (LogDate, Username, UserRole, ActionType, Module, Details) VALUES (GETDATE(), @user, @role, @action, @module, @details)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@user", user),
                    new SqlParameter("@role", role),
                    new SqlParameter("@action", actionType ?? ""),
                    new SqlParameter("@module", module ?? ""),
                    new SqlParameter("@details", details ?? "")
                };

                ExecuteNonQuery(query, parameters);
            }
            catch
            {
                // Audit logging should never disrupt user transactions
            }
        }
    }
}
