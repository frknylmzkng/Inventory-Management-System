namespace Inventory_Management_System
{
    partial class ManageBackupRestore
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.btn_back = new System.Windows.Forms.Button();
            this.btn_exit = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.grpBackup = new System.Windows.Forms.GroupBox();
            this.lbl_backup_status = new System.Windows.Forms.Label();
            this.btn_start_backup = new System.Windows.Forms.Button();
            this.btn_browse_backup = new System.Windows.Forms.Button();
            this.txt_backup_path = new System.Windows.Forms.TextBox();
            this.lbl_backup_path = new System.Windows.Forms.Label();
            this.lbl_backup_desc = new System.Windows.Forms.Label();
            this.grpRestore = new System.Windows.Forms.GroupBox();
            this.lbl_restore_status = new System.Windows.Forms.Label();
            this.btn_start_restore = new System.Windows.Forms.Button();
            this.chc_safety_backup = new System.Windows.Forms.CheckBox();
            this.btn_browse_restore = new System.Windows.Forms.Button();
            this.txt_restore_path = new System.Windows.Forms.TextBox();
            this.lbl_restore_path = new System.Windows.Forms.Label();
            this.lbl_warning = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            this.grpBackup.SuspendLayout();
            this.grpRestore.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panelTop.Controls.Add(this.btn_back);
            this.panelTop.Controls.Add(this.btn_exit);
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Controls.Add(this.lblSubtitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 85);
            this.panelTop.TabIndex = 0;
            // 
            // btn_back
            // 
            this.btn_back.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_back.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_back.ForeColor = System.Drawing.Color.White;
            this.btn_back.Location = new System.Drawing.Point(15, 20);
            this.btn_back.Name = "btn_back";
            this.btn_back.Size = new System.Drawing.Size(95, 45);
            this.btn_back.TabIndex = 16;
            this.btn_back.Text = "◀ GERİ";
            this.btn_back.UseVisualStyleBackColor = true;
            this.btn_back.Click += new System.EventHandler(this.btn_back_Click);
            // 
            // btn_exit
            // 
            this.btn_exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_exit.ForeColor = System.Drawing.Color.White;
            this.btn_exit.Location = new System.Drawing.Point(835, 15);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(50, 50);
            this.btn_exit.TabIndex = 15;
            this.btn_exit.Text = "X";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(145, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(635, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "VERİTABANI YEDEKLEME VE GERİ YÜKLEME";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.White;
            this.lblSubtitle.Location = new System.Drawing.Point(260, 52);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(405, 16);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Disaster Recovery (Felaket Kurtarma) ve Tam Veri Güvenliği Yönetimi";
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 535);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(900, 25);
            this.panelBottom.TabIndex = 1;
            // 
            // grpBackup
            // 
            this.grpBackup.BackColor = System.Drawing.Color.White;
            this.grpBackup.Controls.Add(this.lbl_backup_status);
            this.grpBackup.Controls.Add(this.btn_start_backup);
            this.grpBackup.Controls.Add(this.btn_browse_backup);
            this.grpBackup.Controls.Add(this.txt_backup_path);
            this.grpBackup.Controls.Add(this.lbl_backup_path);
            this.grpBackup.Controls.Add(this.lbl_backup_desc);
            this.grpBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.grpBackup.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.grpBackup.Location = new System.Drawing.Point(35, 100);
            this.grpBackup.Name = "grpBackup";
            this.grpBackup.Size = new System.Drawing.Size(830, 185);
            this.grpBackup.TabIndex = 2;
            this.grpBackup.TabStop = false;
            this.grpBackup.Text = "📦 1. Veritabanı Yedeği Al (Full Backup)";
            // 
            // lbl_backup_status
            // 
            this.lbl_backup_status.AutoSize = true;
            this.lbl_backup_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_backup_status.ForeColor = System.Drawing.Color.Green;
            this.lbl_backup_status.Location = new System.Drawing.Point(255, 145);
            this.lbl_backup_status.Name = "lbl_backup_status";
            this.lbl_backup_status.Size = new System.Drawing.Size(125, 13);
            this.lbl_backup_status.TabIndex = 5;
            this.lbl_backup_status.Text = "Durum: Yedekleme hazır.";
            // 
            // btn_start_backup
            // 
            this.btn_start_backup.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btn_start_backup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_start_backup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_start_backup.ForeColor = System.Drawing.Color.White;
            this.btn_start_backup.Location = new System.Drawing.Point(25, 130);
            this.btn_start_backup.Name = "btn_start_backup";
            this.btn_start_backup.Size = new System.Drawing.Size(210, 40);
            this.btn_start_backup.TabIndex = 4;
            this.btn_start_backup.Text = "💾 ŞİMDİ YEDEK AL";
            this.btn_start_backup.UseVisualStyleBackColor = false;
            this.btn_start_backup.Click += new System.EventHandler(this.btn_start_backup_Click);
            // 
            // btn_browse_backup
            // 
            this.btn_browse_backup.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_browse_backup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_browse_backup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_browse_backup.ForeColor = System.Drawing.Color.White;
            this.btn_browse_backup.Location = new System.Drawing.Point(685, 87);
            this.btn_browse_backup.Name = "btn_browse_backup";
            this.btn_browse_backup.Size = new System.Drawing.Size(125, 26);
            this.btn_browse_backup.TabIndex = 3;
            this.btn_browse_backup.Text = "📁 Hedef Konum...";
            this.btn_browse_backup.UseVisualStyleBackColor = false;
            this.btn_browse_backup.Click += new System.EventHandler(this.btn_browse_backup_Click);
            // 
            // txt_backup_path
            // 
            this.txt_backup_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txt_backup_path.Location = new System.Drawing.Point(25, 90);
            this.txt_backup_path.Name = "txt_backup_path";
            this.txt_backup_path.Size = new System.Drawing.Size(650, 21);
            this.txt_backup_path.TabIndex = 2;
            // 
            // lbl_backup_path
            // 
            this.lbl_backup_path.AutoSize = true;
            this.lbl_backup_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_backup_path.Location = new System.Drawing.Point(22, 68);
            this.lbl_backup_path.Name = "lbl_backup_path";
            this.lbl_backup_path.Size = new System.Drawing.Size(183, 13);
            this.lbl_backup_path.TabIndex = 1;
            this.lbl_backup_path.Text = "Yedek Dosyasının Kayıt Yolu:";
            // 
            // lbl_backup_desc
            // 
            this.lbl_backup_desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_backup_desc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lbl_backup_desc.Location = new System.Drawing.Point(22, 28);
            this.lbl_backup_desc.Name = "lbl_backup_desc";
            this.lbl_backup_desc.Size = new System.Drawing.Size(785, 30);
            this.lbl_backup_desc.TabIndex = 0;
            this.lbl_backup_desc.Text = "Sistemdeki tüm tablolar (ürünler, siparişler, müşteriler, kullanıcılar, maliyetle" +
    "r ve denetim logları) tek bir .bak dosyası halinde eksiksiz arşivlenir.";
            // 
            // grpRestore
            // 
            this.grpRestore.BackColor = System.Drawing.Color.White;
            this.grpRestore.Controls.Add(this.lbl_restore_status);
            this.grpRestore.Controls.Add(this.btn_start_restore);
            this.grpRestore.Controls.Add(this.chc_safety_backup);
            this.grpRestore.Controls.Add(this.btn_browse_restore);
            this.grpRestore.Controls.Add(this.txt_restore_path);
            this.grpRestore.Controls.Add(this.lbl_restore_path);
            this.grpRestore.Controls.Add(this.lbl_warning);
            this.grpRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.grpRestore.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.grpRestore.Location = new System.Drawing.Point(35, 300);
            this.grpRestore.Name = "grpRestore";
            this.grpRestore.Size = new System.Drawing.Size(830, 215);
            this.grpRestore.TabIndex = 3;
            this.grpRestore.TabStop = false;
            this.grpRestore.Text = "♻️ 2. Yedekten Geri Yükle (Disaster Recovery)";
            // 
            // lbl_restore_status
            // 
            this.lbl_restore_status.AutoSize = true;
            this.lbl_restore_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_restore_status.ForeColor = System.Drawing.Color.Crimson;
            this.lbl_restore_status.Location = new System.Drawing.Point(300, 180);
            this.lbl_restore_status.Name = "lbl_restore_status";
            this.lbl_restore_status.Size = new System.Drawing.Size(200, 13);
            this.lbl_restore_status.TabIndex = 6;
            this.lbl_restore_status.Text = "Durum: Geri yükleme dosyası seçilmedi.";
            // 
            // btn_start_restore
            // 
            this.btn_start_restore.BackColor = System.Drawing.Color.Crimson;
            this.btn_start_restore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_start_restore.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_start_restore.ForeColor = System.Drawing.Color.White;
            this.btn_start_restore.Location = new System.Drawing.Point(25, 165);
            this.btn_start_restore.Name = "btn_start_restore";
            this.btn_start_restore.Size = new System.Drawing.Size(260, 40);
            this.btn_start_restore.TabIndex = 5;
            this.btn_start_restore.Text = "🔄 VERİTABANINI GERİ YÜKLE";
            this.btn_start_restore.UseVisualStyleBackColor = false;
            this.btn_start_restore.Click += new System.EventHandler(this.btn_start_restore_Click);
            // 
            // chc_safety_backup
            // 
            this.chc_safety_backup.AutoSize = true;
            this.chc_safety_backup.Checked = true;
            this.chc_safety_backup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chc_safety_backup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.chc_safety_backup.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.chc_safety_backup.Location = new System.Drawing.Point(25, 133);
            this.chc_safety_backup.Name = "chc_safety_backup";
            this.chc_safety_backup.Size = new System.Drawing.Size(430, 19);
            this.chc_safety_backup.TabIndex = 4;
            this.chc_safety_backup.Text = "Geri yükleme öncesinde otomatik güvenlik yedeği al (Önerilen)";
            this.chc_safety_backup.UseVisualStyleBackColor = true;
            // 
            // btn_browse_restore
            // 
            this.btn_browse_restore.BackColor = System.Drawing.Color.Orange;
            this.btn_browse_restore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_browse_restore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_browse_restore.ForeColor = System.Drawing.Color.White;
            this.btn_browse_restore.Location = new System.Drawing.Point(685, 95);
            this.btn_browse_restore.Name = "btn_browse_restore";
            this.btn_browse_restore.Size = new System.Drawing.Size(125, 26);
            this.btn_browse_restore.TabIndex = 3;
            this.btn_browse_restore.Text = "📁 Dosya Seç...";
            this.btn_browse_restore.UseVisualStyleBackColor = false;
            this.btn_browse_restore.Click += new System.EventHandler(this.btn_browse_restore_Click);
            // 
            // txt_restore_path
            // 
            this.txt_restore_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txt_restore_path.Location = new System.Drawing.Point(25, 98);
            this.txt_restore_path.Name = "txt_restore_path";
            this.txt_restore_path.Size = new System.Drawing.Size(650, 21);
            this.txt_restore_path.TabIndex = 2;
            // 
            // lbl_restore_path
            // 
            this.lbl_restore_path.AutoSize = true;
            this.lbl_restore_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_restore_path.Location = new System.Drawing.Point(22, 78);
            this.lbl_restore_path.Name = "lbl_restore_path";
            this.lbl_restore_path.Size = new System.Drawing.Size(225, 13);
            this.lbl_restore_path.TabIndex = 1;
            this.lbl_restore_path.Text = "Geri Yüklenecek .bak Dosyası Konumu:";
            // 
            // lbl_warning
            // 
            this.lbl_warning.BackColor = System.Drawing.Color.MistyRose;
            this.lbl_warning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_warning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_warning.ForeColor = System.Drawing.Color.DarkRed;
            this.lbl_warning.Location = new System.Drawing.Point(25, 28);
            this.lbl_warning.Name = "lbl_warning";
            this.lbl_warning.Padding = new System.Windows.Forms.Padding(5);
            this.lbl_warning.Size = new System.Drawing.Size(785, 42);
            this.lbl_warning.TabIndex = 0;
            this.lbl_warning.Text = "⚠️ UYARI: Geri yükleme işlemi, aktif veritabanının üzerine seçilen yedek dosyasın" +
    "daki verileri yazacaktır. Bu işlem geri alınamaz. İşlem süresince aktif oturumla" +
    "r geçici olarak bekletilecektir.";
            // 
            // ManageBackupRestore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(900, 560);
            this.Controls.Add(this.grpRestore);
            this.Controls.Add(this.grpBackup);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ManageBackupRestore";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ManageBackupRestore";
            this.Load += new System.EventHandler(this.ManageBackupRestore_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.grpBackup.ResumeLayout(false);
            this.grpBackup.PerformLayout();
            this.grpRestore.ResumeLayout(false);
            this.grpRestore.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btn_back;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.GroupBox grpBackup;
        private System.Windows.Forms.Label lbl_backup_desc;
        private System.Windows.Forms.Label lbl_backup_path;
        private System.Windows.Forms.TextBox txt_backup_path;
        private System.Windows.Forms.Button btn_browse_backup;
        private System.Windows.Forms.Button btn_start_backup;
        private System.Windows.Forms.Label lbl_backup_status;
        private System.Windows.Forms.GroupBox grpRestore;
        private System.Windows.Forms.Label lbl_warning;
        private System.Windows.Forms.Label lbl_restore_path;
        private System.Windows.Forms.TextBox txt_restore_path;
        private System.Windows.Forms.Button btn_browse_restore;
        private System.Windows.Forms.CheckBox chc_safety_backup;
        private System.Windows.Forms.Button btn_start_restore;
        private System.Windows.Forms.Label lbl_restore_status;
    }
}
