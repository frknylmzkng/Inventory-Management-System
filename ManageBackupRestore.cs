using System;
using System.IO;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageBackupRestore : Form
    {
        public ManageBackupRestore()
        {
            InitializeComponent();
        }

        private void ManageBackupRestore_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Bu alana yalnızca Yönetici (Admin) yetkisine sahip kullanıcılar erişebilir.", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                return;
            }

            string defaultDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            txt_backup_path.Text = Path.Combine(defaultDesktop, $"InventoryDB_Backup_{DateTime.Now:yyyyMMdd_HHmm}.bak");
        }

        private void btn_browse_backup_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Yedek Dosyasının Kaydedileceği Konumu Seçin";
                sfd.Filter = "SQL Server Yedek Dosyası (*.bak)|*.bak";
                sfd.FileName = Path.GetFileName(txt_backup_path.Text);
                if (!string.IsNullOrEmpty(txt_backup_path.Text) && Directory.Exists(Path.GetDirectoryName(txt_backup_path.Text)))
                {
                    sfd.InitialDirectory = Path.GetDirectoryName(txt_backup_path.Text);
                }

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    txt_backup_path.Text = sfd.FileName;
                }
            }
        }

        private void btn_start_backup_Click(object sender, EventArgs e)
        {
            string backupPath = txt_backup_path.Text.Trim();
            if (string.IsNullOrWhiteSpace(backupPath))
            {
                MessageBox.Show("Lütfen geçerli bir yedekleme dosya yolu belirtiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                lbl_backup_status.ForeColor = System.Drawing.Color.DarkOrange;
                lbl_backup_status.Text = "Yedekleme işlemi gerçekleştiriliyor, lütfen bekleyiniz...";
                Application.DoEvents();

                DbHelper.BackupDatabase(backupPath);

                FileInfo fi = new FileInfo(backupPath);
                string sizeMb = (fi.Length / (1024.0 * 1024.0)).ToString("F2") + " MB";

                DbHelper.LogAction("Yedekleme Alındı", "Sistem / Yedekleme", $"Tam veritabanı yedeği alındı: '{backupPath}' (Boyut: {sizeMb})");

                lbl_backup_status.ForeColor = System.Drawing.Color.DarkGreen;
                lbl_backup_status.Text = $"✅ Son Yedek: {DateTime.Now:dd.MM.yyyy HH:mm:ss} ({sizeMb})";

                MessageBox.Show(
                    $"Veritabanı yedeği başarıyla alındı!\n\n" +
                    $"📁 Dosya: {backupPath}\n" +
                    $"📦 Boyut: {sizeMb}\n" +
                    $"⏰ Tarih: {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
                    "Yedekleme Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                lbl_backup_status.ForeColor = System.Drawing.Color.Crimson;
                lbl_backup_status.Text = "❌ Yedekleme başarısız oldu!";
                MessageBox.Show("Yedekleme sırasında bir hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btn_browse_restore_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Geri Yüklenecek .bak Yedek Dosyasını Seçin";
                ofd.Filter = "SQL Server Yedek Dosyası (*.bak)|*.bak|Tüm Dosyalar (*.*)|*.*";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txt_restore_path.Text = ofd.FileName;
                    FileInfo fi = new FileInfo(ofd.FileName);
                    string sizeMb = (fi.Length / (1024.0 * 1024.0)).ToString("F2") + " MB";
                    lbl_restore_status.ForeColor = System.Drawing.Color.DarkGreen;
                    lbl_restore_status.Text = $"Seçilen Dosya: {fi.Name} ({sizeMb}) - Geri yüklemeye hazır.";
                }
            }
        }

        private void btn_start_restore_Click(object sender, EventArgs e)
        {
            string restorePath = txt_restore_path.Text.Trim();
            if (string.IsNullOrWhiteSpace(restorePath) || !File.Exists(restorePath))
            {
                MessageBox.Show("Lütfen geçerli ve mevcut bir .bak yedek dosyası seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "DİKKAT: Veritabanı geri yükleme işlemi, mevcut tüm verilerin (ürünler, siparişler, müşteriler vb.) " +
                "üzerine seçilen yedeği yazacaktır.\n\n" +
                $"Yedek Dosyası:\n{restorePath}\n\n" +
                "Devam etmek istediğinize emin misiniz?",
                "Kritik Geri Yükleme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // 1. Safety backup if checked
                if (chc_safety_backup.Checked)
                {
                    lbl_restore_status.ForeColor = System.Drawing.Color.DarkOrange;
                    lbl_restore_status.Text = "Önceden acil durum güvenlik yedeği alınıyor...";
                    Application.DoEvents();

                    string safetyBak = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"InventoryDB_PreRestore_Safety_{DateTime.Now:yyyyMMdd_HHmmss}.bak");
                    try
                    {
                        DbHelper.BackupDatabase(safetyBak);
                        DbHelper.LogAction("Güvenlik Yedeği", "Sistem / Yedekleme", $"Geri yükleme öncesi otomatik güvenlik yedeği alındı: '{safetyBak}'");
                    }
                    catch
                    {
                        // Proceed even if safety backup fails, but user is restoring
                    }
                }

                lbl_restore_status.ForeColor = System.Drawing.Color.Crimson;
                lbl_restore_status.Text = "Veritabanı geri yükleniyor, lütfen bekleyiniz...";
                Application.DoEvents();

                DbHelper.RestoreDatabase(restorePath);

                DbHelper.LogAction("Geri Yükleme Yapıldı", "Sistem / Yedekleme", $"Veritabanı başarıyla geri yüklendi: '{restorePath}'");

                lbl_restore_status.ForeColor = System.Drawing.Color.DarkGreen;
                lbl_restore_status.Text = "✅ Veritabanı başarıyla geri yüklendi!";

                MessageBox.Show(
                    "Veritabanı başarıyla geri yüklendi!\n\n" +
                    "Güncel verilerin tüm modüllere yansıması için Ana Menü ekranına yönlendiriliyorsunuz.",
                    "Geri Yükleme Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

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
            catch (Exception ex)
            {
                lbl_restore_status.ForeColor = System.Drawing.Color.Crimson;
                lbl_restore_status.Text = "❌ Geri yükleme sırasında hata oluştu!";
                MessageBox.Show("Geri yükleme sırasında bir hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
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
    }
}
