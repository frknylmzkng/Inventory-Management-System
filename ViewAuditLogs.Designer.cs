namespace Inventory_Management_System
{
    partial class ViewAuditLogs
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_back = new System.Windows.Forms.Button();
            this.btn_exit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSubtitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbl_module = new System.Windows.Forms.Label();
            this.cmb_module = new System.Windows.Forms.ComboBox();
            this.lbl_action = new System.Windows.Forms.Label();
            this.cmb_action = new System.Windows.Forms.ComboBox();
            this.lbl_search = new System.Windows.Forms.Label();
            this.txt_search = new System.Windows.Forms.TextBox();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.btn_export = new System.Windows.Forms.Button();
            this.dataGridViewLogs = new System.Windows.Forms.DataGridView();
            this.lbl_status = new System.Windows.Forms.Label();
            this.lbl_security_info = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLogs)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel1.Controls.Add(this.labelSubtitle);
            this.panel1.Controls.Add(this.btn_back);
            this.panel1.Controls.Add(this.btn_exit);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(960, 85);
            this.panel1.TabIndex = 0;
            // 
            // labelSubtitle
            // 
            this.labelSubtitle.AutoSize = true;
            this.labelSubtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.labelSubtitle.ForeColor = System.Drawing.Color.White;
            this.labelSubtitle.Location = new System.Drawing.Point(295, 52);
            this.labelSubtitle.Name = "labelSubtitle";
            this.labelSubtitle.Size = new System.Drawing.Size(370, 16);
            this.labelSubtitle.TabIndex = 17;
            this.labelSubtitle.Text = "Sistem İşlem Geçmişi, Değişiklikler ve Denetim İzi (Audit Log)";
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
            this.btn_exit.Location = new System.Drawing.Point(898, 15);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(50, 50);
            this.btn_exit.TabIndex = 15;
            this.btn_exit.Text = "X";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(235, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(490, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "İŞLEM GEÇMİŞİ VE DENETİM İZİ";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 545);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(960, 25);
            this.panel2.TabIndex = 1;
            // 
            // lbl_module
            // 
            this.lbl_module.AutoSize = true;
            this.lbl_module.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_module.Location = new System.Drawing.Point(15, 102);
            this.lbl_module.Name = "lbl_module";
            this.lbl_module.Size = new System.Drawing.Size(45, 13);
            this.lbl_module.TabIndex = 2;
            this.lbl_module.Text = "Modül:";
            // 
            // cmb_module
            // 
            this.cmb_module.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_module.FormattingEnabled = true;
            this.cmb_module.Location = new System.Drawing.Point(65, 98);
            this.cmb_module.Name = "cmb_module";
            this.cmb_module.Size = new System.Drawing.Size(130, 21);
            this.cmb_module.TabIndex = 3;
            this.cmb_module.SelectedIndexChanged += new System.EventHandler(this.FilterChanged);
            // 
            // lbl_action
            // 
            this.lbl_action.AutoSize = true;
            this.lbl_action.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_action.Location = new System.Drawing.Point(205, 102);
            this.lbl_action.Name = "lbl_action";
            this.lbl_action.Size = new System.Drawing.Size(40, 13);
            this.lbl_action.TabIndex = 4;
            this.lbl_action.Text = "İşlem:";
            // 
            // cmb_action
            // 
            this.cmb_action.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_action.FormattingEnabled = true;
            this.cmb_action.Location = new System.Drawing.Point(250, 98);
            this.cmb_action.Name = "cmb_action";
            this.cmb_action.Size = new System.Drawing.Size(140, 21);
            this.cmb_action.TabIndex = 5;
            this.cmb_action.SelectedIndexChanged += new System.EventHandler(this.FilterChanged);
            // 
            // lbl_search
            // 
            this.lbl_search.AutoSize = true;
            this.lbl_search.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_search.Location = new System.Drawing.Point(400, 102);
            this.lbl_search.Name = "lbl_search";
            this.lbl_search.Size = new System.Drawing.Size(46, 13);
            this.lbl_search.TabIndex = 6;
            this.lbl_search.Text = "Arama:";
            // 
            // txt_search
            // 
            this.txt_search.Location = new System.Drawing.Point(450, 98);
            this.txt_search.Name = "txt_search";
            this.txt_search.Size = new System.Drawing.Size(170, 20);
            this.txt_search.TabIndex = 7;
            this.txt_search.TextChanged += new System.EventHandler(this.FilterChanged);
            // 
            // btn_refresh
            // 
            this.btn_refresh.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_refresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_refresh.ForeColor = System.Drawing.Color.White;
            this.btn_refresh.Location = new System.Drawing.Point(630, 95);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(130, 27);
            this.btn_refresh.TabIndex = 8;
            this.btn_refresh.Text = "🔄 FİLTREYİ SIFIRLA";
            this.btn_refresh.UseVisualStyleBackColor = false;
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // btn_export
            // 
            this.btn_export.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btn_export.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_export.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_export.ForeColor = System.Drawing.Color.White;
            this.btn_export.Location = new System.Drawing.Point(770, 95);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(175, 27);
            this.btn_export.TabIndex = 9;
            this.btn_export.Text = "📊 EXCEL / CSV AKTAR";
            this.btn_export.UseVisualStyleBackColor = false;
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            // 
            // dataGridViewLogs
            // 
            this.dataGridViewLogs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLogs.Location = new System.Drawing.Point(15, 130);
            this.dataGridViewLogs.Name = "dataGridViewLogs";
            this.dataGridViewLogs.Size = new System.Drawing.Size(930, 375);
            this.dataGridViewLogs.TabIndex = 10;
            this.dataGridViewLogs.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewLogs_CellFormatting);
            // 
            // lbl_status
            // 
            this.lbl_status.AutoSize = true;
            this.lbl_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_status.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lbl_status.Location = new System.Drawing.Point(15, 515);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(185, 15);
            this.lbl_status.TabIndex = 11;
            this.lbl_status.Text = "Toplam 0 kayıt listeleniyor.";
            // 
            // lbl_security_info
            // 
            this.lbl_security_info.AutoSize = true;
            this.lbl_security_info.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_security_info.ForeColor = System.Drawing.Color.Gray;
            this.lbl_security_info.Location = new System.Drawing.Point(520, 517);
            this.lbl_security_info.Name = "lbl_security_info";
            this.lbl_security_info.Size = new System.Drawing.Size(425, 13);
            this.lbl_security_info.TabIndex = 12;
            this.lbl_security_info.Text = "🛡️ Bilgi: Denetim kayıtları sistem güvenliği gereğince silinemez ve değiştirilemez (WORM).";
            // 
            // ViewAuditLogs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(960, 570);
            this.Controls.Add(this.lbl_security_info);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.dataGridViewLogs);
            this.Controls.Add(this.btn_export);
            this.Controls.Add(this.btn_refresh);
            this.Controls.Add(this.txt_search);
            this.Controls.Add(this.lbl_search);
            this.Controls.Add(this.cmb_action);
            this.Controls.Add(this.lbl_action);
            this.Controls.Add(this.cmb_module);
            this.Controls.Add(this.lbl_module);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ViewAuditLogs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ViewAuditLogs";
            this.Load += new System.EventHandler(this.ViewAuditLogs_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLogs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_back;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSubtitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbl_module;
        private System.Windows.Forms.ComboBox cmb_module;
        private System.Windows.Forms.Label lbl_action;
        private System.Windows.Forms.ComboBox cmb_action;
        private System.Windows.Forms.Label lbl_search;
        private System.Windows.Forms.TextBox txt_search;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.DataGridView dataGridViewLogs;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.Label lbl_security_info;
    }
}
