namespace Inventory_Management_System
{
    partial class HomeForm
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
            this.btnExit = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnUsers = new System.Windows.Forms.Button();
            this.btnCategories = new System.Windows.Forms.Button();
            this.btnProducts = new System.Windows.Forms.Button();
            this.btnCustomers = new System.Windows.Forms.Button();
            this.btnOrders = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.panelCardUsers = new System.Windows.Forms.Panel();
            this.lblCountUsers = new System.Windows.Forms.Label();
            this.lblCardUsersTitle = new System.Windows.Forms.Label();
            this.panelCardProducts = new System.Windows.Forms.Panel();
            this.lblCountProducts = new System.Windows.Forms.Label();
            this.lblCardProductsTitle = new System.Windows.Forms.Label();
            this.panelCardCategories = new System.Windows.Forms.Panel();
            this.lblCountCategories = new System.Windows.Forms.Label();
            this.lblCardCategoriesTitle = new System.Windows.Forms.Label();
            this.panelCardOrders = new System.Windows.Forms.Panel();
            this.lblCountOrders = new System.Windows.Forms.Label();
            this.lblCardOrdersTitle = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            this.panelCardUsers.SuspendLayout();
            this.panelCardProducts.SuspendLayout();
            this.panelCardCategories.SuspendLayout();
            this.panelCardOrders.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panelTop.Controls.Add(this.btnExit);
            this.panelTop.Controls.Add(this.lblSubtitle);
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(880, 100);
            this.panelTop.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(818, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(50, 50);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "X";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(180, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(520, 33);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "INVENTORY MANAGEMENT SYSTEM";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.White;
            this.lblSubtitle.Location = new System.Drawing.Point(340, 60);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(183, 24);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Ana Kontrol Paneli";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 535);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(880, 35);
            this.panelBottom.TabIndex = 1;
            // 
            // panelCardProducts
            // 
            this.panelCardProducts.BackColor = System.Drawing.Color.Teal;
            this.panelCardProducts.Controls.Add(this.lblCountProducts);
            this.panelCardProducts.Controls.Add(this.lblCardProductsTitle);
            this.panelCardProducts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelCardProducts.Location = new System.Drawing.Point(60, 130);
            this.panelCardProducts.Name = "panelCardProducts";
            this.panelCardProducts.Size = new System.Drawing.Size(160, 100);
            this.panelCardProducts.TabIndex = 2;
            this.panelCardProducts.Click += new System.EventHandler(this.panelCardProducts_Click);
            // 
            // lblCountProducts
            // 
            this.lblCountProducts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCountProducts.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCountProducts.ForeColor = System.Drawing.Color.White;
            this.lblCountProducts.Location = new System.Drawing.Point(10, 45);
            this.lblCountProducts.Name = "lblCountProducts";
            this.lblCountProducts.Size = new System.Drawing.Size(140, 40);
            this.lblCountProducts.TabIndex = 1;
            this.lblCountProducts.Text = "0";
            this.lblCountProducts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCountProducts.Click += new System.EventHandler(this.panelCardProducts_Click);
            // 
            // lblCardProductsTitle
            // 
            this.lblCardProductsTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCardProductsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCardProductsTitle.ForeColor = System.Drawing.Color.White;
            this.lblCardProductsTitle.Location = new System.Drawing.Point(10, 12);
            this.lblCardProductsTitle.Name = "lblCardProductsTitle";
            this.lblCardProductsTitle.Size = new System.Drawing.Size(140, 25);
            this.lblCardProductsTitle.TabIndex = 0;
            this.lblCardProductsTitle.Text = "TOPLAM ÜRÜN";
            this.lblCardProductsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCardProductsTitle.Click += new System.EventHandler(this.panelCardProducts_Click);
            // 
            // panelCardCategories
            // 
            this.panelCardCategories.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.panelCardCategories.Controls.Add(this.lblCountCategories);
            this.panelCardCategories.Controls.Add(this.lblCardCategoriesTitle);
            this.panelCardCategories.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelCardCategories.Location = new System.Drawing.Point(260, 130);
            this.panelCardCategories.Name = "panelCardCategories";
            this.panelCardCategories.Size = new System.Drawing.Size(160, 100);
            this.panelCardCategories.TabIndex = 3;
            this.panelCardCategories.Click += new System.EventHandler(this.panelCardCategories_Click);
            // 
            // lblCountCategories
            // 
            this.lblCountCategories.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCountCategories.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCountCategories.ForeColor = System.Drawing.Color.White;
            this.lblCountCategories.Location = new System.Drawing.Point(10, 45);
            this.lblCountCategories.Name = "lblCountCategories";
            this.lblCountCategories.Size = new System.Drawing.Size(140, 40);
            this.lblCountCategories.TabIndex = 1;
            this.lblCountCategories.Text = "0";
            this.lblCountCategories.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCountCategories.Click += new System.EventHandler(this.panelCardCategories_Click);
            // 
            // lblCardCategoriesTitle
            // 
            this.lblCardCategoriesTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCardCategoriesTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCardCategoriesTitle.ForeColor = System.Drawing.Color.White;
            this.lblCardCategoriesTitle.Location = new System.Drawing.Point(10, 12);
            this.lblCardCategoriesTitle.Name = "lblCardCategoriesTitle";
            this.lblCardCategoriesTitle.Size = new System.Drawing.Size(140, 25);
            this.lblCardCategoriesTitle.TabIndex = 0;
            this.lblCardCategoriesTitle.Text = "KATEGORİLER";
            this.lblCardCategoriesTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCardCategoriesTitle.Click += new System.EventHandler(this.panelCardCategories_Click);
            // 
            // panelCardUsers
            // 
            this.panelCardUsers.BackColor = System.Drawing.Color.SteelBlue;
            this.panelCardUsers.Controls.Add(this.lblCountUsers);
            this.panelCardUsers.Controls.Add(this.lblCardUsersTitle);
            this.panelCardUsers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelCardUsers.Location = new System.Drawing.Point(460, 130);
            this.panelCardUsers.Name = "panelCardUsers";
            this.panelCardUsers.Size = new System.Drawing.Size(160, 100);
            this.panelCardUsers.TabIndex = 4;
            this.panelCardUsers.Click += new System.EventHandler(this.panelCardUsers_Click);
            // 
            // lblCountUsers
            // 
            this.lblCountUsers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCountUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCountUsers.ForeColor = System.Drawing.Color.White;
            this.lblCountUsers.Location = new System.Drawing.Point(10, 45);
            this.lblCountUsers.Name = "lblCountUsers";
            this.lblCountUsers.Size = new System.Drawing.Size(140, 40);
            this.lblCountUsers.TabIndex = 1;
            this.lblCountUsers.Text = "0";
            this.lblCountUsers.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCountUsers.Click += new System.EventHandler(this.panelCardUsers_Click);
            // 
            // lblCardUsersTitle
            // 
            this.lblCardUsersTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCardUsersTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCardUsersTitle.ForeColor = System.Drawing.Color.White;
            this.lblCardUsersTitle.Location = new System.Drawing.Point(10, 12);
            this.lblCardUsersTitle.Name = "lblCardUsersTitle";
            this.lblCardUsersTitle.Size = new System.Drawing.Size(140, 25);
            this.lblCardUsersTitle.TabIndex = 0;
            this.lblCardUsersTitle.Text = "KULLANICILAR";
            this.lblCardUsersTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCardUsersTitle.Click += new System.EventHandler(this.panelCardUsers_Click);
            // 
            // panelCardOrders
            // 
            this.panelCardOrders.BackColor = System.Drawing.Color.OrangeRed;
            this.panelCardOrders.Controls.Add(this.lblCountOrders);
            this.panelCardOrders.Controls.Add(this.lblCardOrdersTitle);
            this.panelCardOrders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelCardOrders.Location = new System.Drawing.Point(660, 130);
            this.panelCardOrders.Name = "panelCardOrders";
            this.panelCardOrders.Size = new System.Drawing.Size(160, 100);
            this.panelCardOrders.TabIndex = 5;
            this.panelCardOrders.Click += new System.EventHandler(this.panelCardOrders_Click);
            // 
            // lblCountOrders
            // 
            this.lblCountOrders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCountOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCountOrders.ForeColor = System.Drawing.Color.White;
            this.lblCountOrders.Location = new System.Drawing.Point(10, 45);
            this.lblCountOrders.Name = "lblCountOrders";
            this.lblCountOrders.Size = new System.Drawing.Size(140, 40);
            this.lblCountOrders.TabIndex = 1;
            this.lblCountOrders.Text = "0";
            this.lblCountOrders.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCountOrders.Click += new System.EventHandler(this.panelCardOrders_Click);
            // 
            // lblCardOrdersTitle
            // 
            this.lblCardOrdersTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCardOrdersTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCardOrdersTitle.ForeColor = System.Drawing.Color.White;
            this.lblCardOrdersTitle.Location = new System.Drawing.Point(10, 12);
            this.lblCardOrdersTitle.Name = "lblCardOrdersTitle";
            this.lblCardOrdersTitle.Size = new System.Drawing.Size(140, 25);
            this.lblCardOrdersTitle.TabIndex = 0;
            this.lblCardOrdersTitle.Text = "SİPARİŞLER";
            this.lblCardOrdersTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCardOrdersTitle.Click += new System.EventHandler(this.panelCardOrders_Click);
            // 
            // btnProducts
            // 
            this.btnProducts.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnProducts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProducts.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnProducts.ForeColor = System.Drawing.Color.White;
            this.btnProducts.Location = new System.Drawing.Point(100, 270);
            this.btnProducts.Name = "btnProducts";
            this.btnProducts.Size = new System.Drawing.Size(200, 60);
            this.btnProducts.TabIndex = 6;
            this.btnProducts.Text = "ÜRÜNLER";
            this.btnProducts.UseVisualStyleBackColor = false;
            this.btnProducts.Click += new System.EventHandler(this.btnProducts_Click);
            // 
            // btnCategories
            // 
            this.btnCategories.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnCategories.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCategories.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnCategories.ForeColor = System.Drawing.Color.White;
            this.btnCategories.Location = new System.Drawing.Point(340, 270);
            this.btnCategories.Name = "btnCategories";
            this.btnCategories.Size = new System.Drawing.Size(200, 60);
            this.btnCategories.TabIndex = 7;
            this.btnCategories.Text = "KATEGORİLER";
            this.btnCategories.UseVisualStyleBackColor = false;
            this.btnCategories.Click += new System.EventHandler(this.btnCategories_Click);
            // 
            // btnUsers
            // 
            this.btnUsers.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnUsers.ForeColor = System.Drawing.Color.White;
            this.btnUsers.Location = new System.Drawing.Point(580, 270);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Size = new System.Drawing.Size(200, 60);
            this.btnUsers.TabIndex = 8;
            this.btnUsers.Text = "KULLANICILAR";
            this.btnUsers.UseVisualStyleBackColor = false;
            this.btnUsers.Click += new System.EventHandler(this.btnUsers_Click);
            // 
            // btnCustomers
            // 
            this.btnCustomers.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnCustomers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCustomers.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnCustomers.ForeColor = System.Drawing.Color.White;
            this.btnCustomers.Location = new System.Drawing.Point(100, 360);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Size = new System.Drawing.Size(200, 60);
            this.btnCustomers.TabIndex = 9;
            this.btnCustomers.Text = "MÜŞTERİLER";
            this.btnCustomers.UseVisualStyleBackColor = false;
            this.btnCustomers.Click += new System.EventHandler(this.btnCustomers_Click);
            // 
            // btnOrders
            // 
            this.btnOrders.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnOrders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnOrders.ForeColor = System.Drawing.Color.White;
            this.btnOrders.Location = new System.Drawing.Point(340, 360);
            this.btnOrders.Name = "btnOrders";
            this.btnOrders.Size = new System.Drawing.Size(200, 60);
            this.btnOrders.TabIndex = 10;
            this.btnOrders.Text = "SİPARİŞLER";
            this.btnOrders.UseVisualStyleBackColor = false;
            this.btnOrders.Click += new System.EventHandler(this.btnOrders_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.Crimson;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(580, 360);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(200, 60);
            this.btnLogout.TabIndex = 11;
            this.btnLogout.Text = "ÇIKIŞ YAP";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(880, 570);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnOrders);
            this.Controls.Add(this.btnCustomers);
            this.Controls.Add(this.btnUsers);
            this.Controls.Add(this.btnCategories);
            this.Controls.Add(this.btnProducts);
            this.Controls.Add(this.panelCardOrders);
            this.Controls.Add(this.panelCardUsers);
            this.Controls.Add(this.panelCardCategories);
            this.Controls.Add(this.panelCardProducts);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HomeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HomeForm";
            this.Activated += new System.EventHandler(this.HomeForm_Activated);
            this.Load += new System.EventHandler(this.HomeForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelCardUsers.ResumeLayout(false);
            this.panelCardProducts.ResumeLayout(false);
            this.panelCardCategories.ResumeLayout(false);
            this.panelCardOrders.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnProducts;
        private System.Windows.Forms.Button btnCategories;
        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.Button btnCustomers;
        private System.Windows.Forms.Button btnOrders;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel panelCardProducts;
        private System.Windows.Forms.Label lblCardProductsTitle;
        private System.Windows.Forms.Label lblCountProducts;
        private System.Windows.Forms.Panel panelCardCategories;
        private System.Windows.Forms.Label lblCountCategories;
        private System.Windows.Forms.Label lblCardCategoriesTitle;
        private System.Windows.Forms.Panel panelCardUsers;
        private System.Windows.Forms.Label lblCountUsers;
        private System.Windows.Forms.Label lblCardUsersTitle;
        private System.Windows.Forms.Panel panelCardOrders;
        private System.Windows.Forms.Label lblCountOrders;
        private System.Windows.Forms.Label lblCardOrdersTitle;
    }
}
