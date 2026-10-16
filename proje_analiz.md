# Proje Analizi ve Birleşik Kaynak Kod Dokümanı

## Proje: Inventory Management System (Stok ve Sipariş Yönetim Sistemi)

- **Teknoloji:** C# / .NET Framework 4.8 / Windows Forms
- **Veritabanı:** Microsoft SQL Server LocalDB (`InventoryDB_03052026.mdf`)
- **Mimari Bileşenler:** RBAC Rol Yetkilendirme (Admin, Kasiyer, Depocu), Audit Log (İşlem Geçmişi), Barkod Desteği, Maliyet/Kâr Analizi, Disaster Recovery (Yedekleme & Geri Yükleme)

---

## 1. Proje Dizin Ağacı (Folder Tree)

```
Inventory Management System/
├── Properties/
│   ├── AssemblyInfo.cs
│   ├── Resources.Designer.cs
│   ├── Resources.resx
│   ├── Settings.Designer.cs
│   └── Settings.settings
├── .gitignore
├── App.config
├── charlvera-meadows-8582365.png [Binary Medya - 5.0 MB]
├── CurrentUser.cs
├── DbHelper.cs
├── HomeForm.cs
├── HomeForm.Designer.cs
├── Inventory Management System.csproj
├── Inventory Management System.csproj.user
├── Inventory Management System.sln
├── Inventory Management System_TemporaryKey.pfx [Sertifika]
├── Login.cs
├── Login.Designer.cs
├── Login.resx
├── ManageBackupRestore.cs
├── ManageBackupRestore.Designer.cs
├── ManageCategories.cs
├── ManageCategories.Designer.cs
├── ManageCustomers.cs
├── ManageCustomers.Designer.cs
├── ManageOrders.cs
├── ManageOrders.Designer.cs
├── ManageProducts.cs
├── ManageProducts.Designer.cs
├── ManageUsers.cs
├── ManageUsers.Designer.cs
├── ManageUsers.resx
├── Program.cs
├── README.md
├── ReceiptPrinter.cs
├── shutterstock_1930996376-scaled.jpg [Binary Medya - 222 KB]
├── ViewAuditLogs.cs
├── ViewAuditLogs.Designer.cs
├── ViewOrders.cs
└── ViewOrders.Designer.cs
```

---

## 2. Kaynak Kod Dosyaları

### Dosya: `Program.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Program.cs`
- **Dosya Boyutu:** 0.54 KB
- **Satır Sayısı:** 22

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }
    }
}
```

---

### Dosya: `CurrentUser.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\CurrentUser.cs`
- **Dosya Boyutu:** 1.08 KB
- **Satır Sayısı:** 28

```csharp
using System;

namespace Inventory_Management_System
{
    public static class CurrentUser
    {
        public static string Username { get; set; } = string.Empty;
        public static string FullName { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;

        public static bool IsLoggedIn => !string.IsNullOrEmpty(Username);

        public static bool IsAdmin => string.Equals(Role, "Yönetici (Admin)", StringComparison.OrdinalIgnoreCase) ||
                                      string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);

        public static bool IsCashier => string.Equals(Role, "Kasiyer", StringComparison.OrdinalIgnoreCase);

        public static bool IsWarehouse => string.Equals(Role, "Depo Görevlisi", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(Role, "Depocu", StringComparison.OrdinalIgnoreCase);

        public static void Clear()
        {
            Username = string.Empty;
            FullName = string.Empty;
            Role = string.Empty;
        }
    }
}
```

---

### Dosya: `DbHelper.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\DbHelper.cs`
- **Dosya Boyutu:** 9.03 KB
- **Satır Sayısı:** 222

```csharp
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

        public static string MasterConnectionString => @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30";
        public static string DatabaseFilePath => @"C:\Users\FURKAN YILMAZ\Documents\InventoryDB_03052026.mdf";

        public static void BackupDatabase(string destinationBakPath)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string dbName = conn.Database;
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 120;
                    cmd.CommandText = "BACKUP DATABASE [" + dbName + "] TO DISK = @path WITH FORMAT, INIT, NAME = 'Inventory Management System Full Backup'";
                    cmd.Parameters.AddWithValue("@path", destinationBakPath);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void RestoreDatabase(string sourceBakPath)
        {
            SqlConnection.ClearAllPools();

            using (SqlConnection conn = new SqlConnection(MasterConnectionString))
            {
                conn.Open();
                string dbName = DatabaseFilePath;
                try
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 180;
                        cmd.CommandText = $@"
                            ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                            RESTORE DATABASE [{dbName}] FROM DISK = @path WITH REPLACE;
                            ALTER DATABASE [{dbName}] SET MULTI_USER;
                        ";
                        cmd.Parameters.AddWithValue("@path", sourceBakPath);
                        cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    try
                    {
                        using (SqlCommand resetCmd = conn.CreateCommand())
                        {
                            resetCmd.CommandText = $"IF DB_ID('{dbName}') IS NOT NULL ALTER DATABASE [{dbName}] SET MULTI_USER;";
                            resetCmd.ExecuteNonQuery();
                        }
                    }
                    catch { }
                }
            }

            SqlConnection.ClearAllPools();
        }
    }
}
```

---

### Dosya: `ReceiptPrinter.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ReceiptPrinter.cs`
- **Dosya Boyutu:** 7.54 KB
- **Satır Sayısı:** 146

```csharp
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public static class ReceiptPrinter
    {
        public static void ShowReceipt(int orderId, string custName, DateTime orderDate, string orderStatus, decimal total, DataTable items)
        {
            PrintDocument doc = new PrintDocument();
            doc.DocumentName = $"Siparis_Fisi_#{orderId}";

            // Calculate dynamic height based on item count
            int itemCount = items != null ? items.Rows.Count : 0;
            int pageHeight = Math.Max(520, 300 + (itemCount * 24));
            doc.DefaultPageSettings.PaperSize = new PaperSize("CustomReceipt", 340, pageHeight);
            doc.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

            doc.PrintPage += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.Clear(Color.White);

                Font fontTitle = new Font("Arial", 12, FontStyle.Bold);
                Font fontSubtitle = new Font("Arial", 8.5F, FontStyle.Bold);
                Font fontRegular = new Font("Courier New", 8.5F, FontStyle.Regular);
                Font fontBold = new Font("Courier New", 8.5F, FontStyle.Bold);
                Font fontTotal = new Font("Arial", 11, FontStyle.Bold);
                Font fontFooter = new Font("Arial", 7.5F, FontStyle.Italic);

                float left = 15;
                float width = 310;
                float y = 15;

                StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
                StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far };

                // Header
                g.DrawString("INVENTORY MANAGEMENT SYSTEM", fontTitle, Brushes.DarkTurquoise, new RectangleF(left, y, width, 20), centerFormat);
                y += 22;
                g.DrawString("SATIŞ FİŞİ / SİPARİŞ MAKBUZU", fontSubtitle, Brushes.DarkSlateGray, new RectangleF(left, y, width, 16), centerFormat);
                y += 20;

                // Separator
                g.DrawLine(Pens.DarkGray, left, y, left + width, y);
                y += 8;

                // Order metadata
                g.DrawString($"Sipariş No : #{orderId}", fontBold, Brushes.Black, left, y);
                y += 16;
                g.DrawString($"Tarih      : {orderDate:dd.MM.yyyy HH:mm}", fontRegular, Brushes.Black, left, y);
                y += 16;
                g.DrawString($"Müşteri    : {custName}", fontRegular, Brushes.Black, left, y);
                y += 16;

                Brush statusBrush = (orderStatus == "İptal / İade Edildi") ? Brushes.Crimson : Brushes.DarkGreen;
                g.DrawString($"Durum      : {orderStatus}", fontBold, statusBrush, left, y);
                y += 20;

                // Table Header
                g.DrawLine(Pens.Black, left, y, left + width, y);
                y += 5;
                g.DrawString("ÜRÜN", fontBold, Brushes.Black, left, y);
                g.DrawString("AD.", fontBold, Brushes.Black, left + 140, y);
                g.DrawString("FİYAT", fontBold, Brushes.Black, left + 175, y);
                g.DrawString("TUTAR", fontBold, Brushes.Black, new RectangleF(left + 230, y, 80, 16), rightFormat);
                y += 16;
                g.DrawLine(Pens.Black, left, y, left + width, y);
                y += 7;

                // Items
                if (items != null && items.Rows.Count > 0)
                {
                    foreach (DataRow row in items.Rows)
                    {
                        string name = "";
                        if (row.Table.Columns.Contains("Ürün Adı")) name = row["Ürün Adı"]?.ToString() ?? "";
                        else if (row.Table.Columns.Contains("ProdName")) name = row["ProdName"]?.ToString() ?? "";

                        if (name.Length > 16) name = name.Substring(0, 14) + "..";

                        string qty = "";
                        if (row.Table.Columns.Contains("Adet")) qty = row["Adet"]?.ToString() ?? "";
                        else if (row.Table.Columns.Contains("Miktar")) qty = row["Miktar"]?.ToString() ?? "";
                        else if (row.Table.Columns.Contains("Qty")) qty = row["Qty"]?.ToString() ?? "";

                        decimal price = 0;
                        if (row.Table.Columns.Contains("Birim Fiyat (TL)")) decimal.TryParse(row["Birim Fiyat (TL)"].ToString(), out price);
                        else if (row.Table.Columns.Contains("Satış Fiyatı (TL)")) decimal.TryParse(row["Satış Fiyatı (TL)"].ToString(), out price);
                        else if (row.Table.Columns.Contains("Birim Fiyat")) decimal.TryParse(row["Birim Fiyat"].ToString(), out price);
                        else if (row.Table.Columns.Contains("Satış Fiyatı")) decimal.TryParse(row["Satış Fiyatı"].ToString(), out price);
                        else if (row.Table.Columns.Contains("Fiyat")) decimal.TryParse(row["Fiyat"].ToString(), out price);
                        else if (row.Table.Columns.Contains("Price")) decimal.TryParse(row["Price"].ToString(), out price);

                        decimal lineTotal = 0;
                        if (row.Table.Columns.Contains("Toplam (TL)")) decimal.TryParse(row["Toplam (TL)"].ToString(), out lineTotal);
                        else if (row.Table.Columns.Contains("Toplam Tutar")) decimal.TryParse(row["Toplam Tutar"].ToString(), out lineTotal);
                        else if (row.Table.Columns.Contains("Toplam")) decimal.TryParse(row["Toplam"].ToString(), out lineTotal);
                        else if (row.Table.Columns.Contains("Total")) decimal.TryParse(row["Total"].ToString(), out lineTotal);

                        g.DrawString(name, fontRegular, Brushes.Black, left, y);
                        g.DrawString(qty, fontRegular, Brushes.Black, left + 140, y);
                        g.DrawString(price.ToString("N0"), fontRegular, Brushes.Black, left + 175, y);
                        g.DrawString(lineTotal.ToString("N2") + " TL", fontRegular, Brushes.Black, new RectangleF(left + 220, y, 90, 16), rightFormat);
                        y += 18;
                    }
                }
                else
                {
                    g.DrawString("(Ürün detayı bulunmuyor)", fontFooter, Brushes.Gray, left, y);
                    y += 18;
                }

                // Total line
                y += 4;
                g.DrawLine(Pens.Black, left, y, left + width, y);
                y += 8;

                g.DrawString("TOPLAM TUTAR:", fontBold, Brushes.Black, left + 60, y);
                g.DrawString($"{total:N2} TL", fontTotal, Brushes.Black, new RectangleF(left + 170, y - 2, 140, 20), rightFormat);
                y += 30;

                // Footer
                g.DrawLine(Pens.LightGray, left, y, left + width, y);
                y += 8;
                g.DrawString("Bizi tercih ettiğiniz için teşekkür ederiz!", fontFooter, Brushes.DimGray, new RectangleF(left, y, width, 16), centerFormat);
                y += 16;
                g.DrawString("* Bu belge bilgi amaçlı satış fişidir *", fontFooter, Brushes.DimGray, new RectangleF(left, y, width, 16), centerFormat);
            };

            PrintPreviewDialog preview = new PrintPreviewDialog
            {
                Document = doc,
                Width = 450,
                Height = 650,
                StartPosition = FormStartPosition.CenterScreen,
                Text = $"Sipariş Fişi Önizleme - #{orderId}"
            };

            preview.ShowDialog();
        }
    }
}
```

---

### Dosya: `App.config`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\App.config`
- **Dosya Boyutu:** 0.18 KB
- **Satır Sayısı:** 6

```xml
﻿<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8" />
    </startup>
</configuration>
```

---

### Dosya: `Inventory Management System.csproj`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Inventory Management System.csproj`
- **Dosya Boyutu:** 7.52 KB
- **Satır Sayısı:** 188

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{A983206F-6C2A-4EC4-8DB1-6C32FA8695A9}</ProjectGuid>
    <OutputType>WinExe</OutputType>
    <RootNamespace>Inventory_Management_System</RootNamespace>
    <AssemblyName>Inventory Management System</AssemblyName>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <Deterministic>true</Deterministic>
    <IsWebBootstrapper>false</IsWebBootstrapper>
    <PublishUrl>C:\Users\FURKAN YILMAZ\Yeni klasör %282%29\</PublishUrl>
    <Install>true</Install>
    <InstallFrom>Disk</InstallFrom>
    <UpdateEnabled>false</UpdateEnabled>
    <UpdateMode>Foreground</UpdateMode>
    <UpdateInterval>7</UpdateInterval>
    <UpdateIntervalUnits>Days</UpdateIntervalUnits>
    <UpdatePeriodically>false</UpdatePeriodically>
    <UpdateRequired>false</UpdateRequired>
    <MapFileExtensions>true</MapFileExtensions>
    <ApplicationRevision>3</ApplicationRevision>
    <ApplicationVersion>1.0.0.%2a</ApplicationVersion>
    <UseApplicationTrust>false</UseApplicationTrust>
    <PublishWizardCompleted>true</PublishWizardCompleted>
    <BootstrapperEnabled>true</BootstrapperEnabled>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup>
    <ManifestCertificateThumbprint>E392C80289C1D13CEC3785E513CF42568473981C</ManifestCertificateThumbprint>
  </PropertyGroup>
  <PropertyGroup>
    <ManifestKeyFile>Inventory Management System_TemporaryKey.pfx</ManifestKeyFile>
  </PropertyGroup>
  <PropertyGroup>
    <GenerateManifests>true</GenerateManifests>
  </PropertyGroup>
  <PropertyGroup>
    <SignManifests>true</SignManifests>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="System.Xml.Linq" />
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System.Data" />
    <Reference Include="System.Deployment" />
    <Reference Include="System.Drawing" />
    <Reference Include="System.Net.Http" />
    <Reference Include="System.Windows.Forms" />
    <Reference Include="System.Xml" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include="Login.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="Login.Designer.cs">
      <DependentUpon>Login.cs</DependentUpon>
    </Compile>
    <Compile Include="ManageUsers.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="ManageUsers.Designer.cs">
      <DependentUpon>ManageUsers.cs</DependentUpon>
    </Compile>
    <Compile Include="CurrentUser.cs" />
    <Compile Include="DbHelper.cs" />
    <Compile Include="ReceiptPrinter.cs" />
    <Compile Include="HomeForm.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="HomeForm.Designer.cs">
      <DependentUpon>HomeForm.cs</DependentUpon>
    </Compile>
    <Compile Include="ManageCategories.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="ManageCategories.Designer.cs">
      <DependentUpon>ManageCategories.cs</DependentUpon>
    </Compile>
    <Compile Include="ManageProducts.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="ManageProducts.Designer.cs">
      <DependentUpon>ManageProducts.cs</DependentUpon>
    </Compile>
    <Compile Include="ManageCustomers.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="ManageCustomers.Designer.cs">
      <DependentUpon>ManageCustomers.cs</DependentUpon>
    </Compile>
    <Compile Include="ManageOrders.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="ManageOrders.Designer.cs">
      <DependentUpon>ManageOrders.cs</DependentUpon>
    </Compile>
    <Compile Include="ViewOrders.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="ViewOrders.Designer.cs">
      <DependentUpon>ViewOrders.cs</DependentUpon>
    </Compile>
    <Compile Include="ViewAuditLogs.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="ViewAuditLogs.Designer.cs">
      <DependentUpon>ViewAuditLogs.cs</DependentUpon>
    </Compile>
    <Compile Include="ManageBackupRestore.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="ManageBackupRestore.Designer.cs">
      <DependentUpon>ManageBackupRestore.cs</DependentUpon>
    </Compile>
    <Compile Include="Program.cs" />
    <Compile Include="Properties\AssemblyInfo.cs" />
    <EmbeddedResource Include="Login.resx">
      <DependentUpon>Login.cs</DependentUpon>
    </EmbeddedResource>
    <EmbeddedResource Include="ManageUsers.resx">
      <DependentUpon>ManageUsers.cs</DependentUpon>
    </EmbeddedResource>
    <EmbeddedResource Include="Properties\Resources.resx">
      <Generator>ResXFileCodeGenerator</Generator>
      <LastGenOutput>Resources.Designer.cs</LastGenOutput>
      <SubType>Designer</SubType>
    </EmbeddedResource>
    <Compile Include="Properties\Resources.Designer.cs">
      <AutoGen>True</AutoGen>
      <DependentUpon>Resources.resx</DependentUpon>
      <DesignTime>True</DesignTime>
    </Compile>
    <None Include="Inventory Management System_TemporaryKey.pfx" />
    <None Include="Properties\Settings.settings">
      <Generator>SettingsSingleFileGenerator</Generator>
      <LastGenOutput>Settings.Designer.cs</LastGenOutput>
    </None>
    <Compile Include="Properties\Settings.Designer.cs">
      <AutoGen>True</AutoGen>
      <DependentUpon>Settings.settings</DependentUpon>
      <DesignTimeSharedInput>True</DesignTimeSharedInput>
    </Compile>
  </ItemGroup>
  <ItemGroup>
    <None Include="App.config" />
  </ItemGroup>
  <ItemGroup>
    <None Include="shutterstock_1930996376-scaled.jpg" />
  </ItemGroup>
  <ItemGroup>
    <BootstrapperPackage Include=".NETFramework,Version=v4.8">
      <Visible>False</Visible>
      <ProductName>Microsoft .NET Framework 4.8 %28x86 and x64%29</ProductName>
      <Install>true</Install>
    </BootstrapperPackage>
    <BootstrapperPackage Include="Microsoft.Net.Framework.3.5.SP1">
      <Visible>False</Visible>
      <ProductName>.NET Framework 3.5 SP1</ProductName>
      <Install>false</Install>
    </BootstrapperPackage>
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
</Project>
```

---

### Dosya: `Inventory Management System.csproj.user`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Inventory Management System.csproj.user`
- **Dosya Boyutu:** 0.45 KB
- **Satır Sayısı:** 13

```xml
﻿<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <PublishUrlHistory />
    <InstallUrlHistory />
    <SupportUrlHistory />
    <UpdateUrlHistory />
    <BootstrapperUrlHistory />
    <ErrorReportUrlHistory />
    <FallbackCulture>en-US</FallbackCulture>
    <VerifyUploadedFiles>false</VerifyUploadedFiles>
  </PropertyGroup>
</Project>
```

---

### Dosya: `Inventory Management System.sln`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Inventory Management System.sln`
- **Dosya Boyutu:** 1.13 KB
- **Satır Sayısı:** 25

```text
﻿
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.5.33530.505
MinimumVisualStudioVersion = 10.0.40219.1
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Inventory Management System", "Inventory Management System.csproj", "{A983206F-6C2A-4EC4-8DB1-6C32FA8695A9}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{A983206F-6C2A-4EC4-8DB1-6C32FA8695A9}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{A983206F-6C2A-4EC4-8DB1-6C32FA8695A9}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{A983206F-6C2A-4EC4-8DB1-6C32FA8695A9}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{A983206F-6C2A-4EC4-8DB1-6C32FA8695A9}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {AF327F63-EA40-4845-A319-7F400236193E}
	EndGlobalSection
EndGlobal
```

---

### Dosya: `.gitignore`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\.gitignore`
- **Dosya Boyutu:** 0.34 KB
- **Satır Sayısı:** 37

```text
## Visual Studio / .NET .gitignore
.vs/
bin/
obj/
*.user
*.suo
*.userprefs
*.pidb
*.booproj
*.svclog
*.mdf
*.ldf
*.ndf

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Aa]rchive/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio temporary files
*.tmp
*.bak
*.swp
*~
thumbs.db
ehthumbs.db
Desktop.ini
$RECYCLE.BIN/
```

---

### Dosya: `README.md`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\README.md`
- **Dosya Boyutu:** 2.88 KB
- **Satır Sayısı:** 68

```markdown
# 📦 Inventory Management System (Envanter ve Stok Yönetim Sistemi)

C# Windows Forms ve Microsoft SQL Server (LocalDB) kullanılarak geliştirilmiş kapsamlı bir masaüstü Stok ve Sipariş Yönetim Sistemi.

---

## 🚀 Özellikler

* 🔐 **Kullanıcı Doğrulama & Giriş (Login):** Veritabanı destekli güvenli giriş ve yetkili kullanıcı oturumu.
* 📊 **Ana Kontrol Paneli (Dashboard):** Toplam ürün, kategori, kullanıcı ve sipariş sayılarını gösteren canlı istatistik kartları.
* 📦 **Ürün Yönetimi (Products):**
  * Ürün ekleme, düzenleme, silme ve listeleme.
  * Kategori bazlı filtreleme ve arama.
  * **Kritik Stok Uyarısı:** Stoğu 5 ve altında kalan ürünleri otomatik olarak kırmızı tonuyla vurgulama.
* 🗂️ **Kategori Yönetimi (Categories):** Ürünleri gruplamak için kategori CRUD işlemleri.
* 👥 **Kullanıcı Yönetimi (Users):** Sistem kullanıcılarını yönetme (Ekle/Düzenle/Sil).
* 👤 **Müşteri Yönetimi (Customers):** Müşteri kayıt ve iletişim bilgilerini yönetme.
* 🛒 **Sipariş & Satış Yönetimi (Orders):**
  * Müşteri ve ürün seçimi ile sipariş oluşturma.
  * Sepete ürün ekleme, adet belirleme ve dinamik toplam tutar hesaplama.
  * Sipariş onaylandığında satılan ürünlerin miktarını **stoktan otomatik düşme**.

---

## 🛠️ Teknolojiler

* **Dil:** C# (.NET Framework 4.8)
* **Arayüz:** Windows Forms (WinForms)
* **Veritabanı:** Microsoft SQL Server LocalDB (`MSSQLLocalDB`)
* **Veri Erişimi:** ADO.NET (Parametreli SQL sorguları ile SQL Injection korumalı)

---

## 📁 Proje Yapısı

```
Inventory Management System/
├── DbHelper.cs                   # Merkezi veritabanı bağlantı ve sorgu yöneticisi
├── Login.cs                      # Giriş ekranı ve kimlik doğrulama
├── HomeForm.cs                   # Ana kontrol paneli (Dashboard)
├── ManageProducts.cs             # Ürün ve stok yönetimi
├── ManageCategories.cs           # Kategori yönetimi
├── ManageCustomers.cs            # Müşteri yönetimi
├── ManageOrders.cs               # Sipariş sepeti ve satış tamamlama
├── ManageUsers.cs                # Sistem kullanıcı yönetimi
├── Program.cs                    # Uygulama başlangıç noktası
├── App.config                    # Yapılandırma dosyası
└── Inventory Management System.sln
```

---

## 💻 Kurulum ve Çalıştırma

1. Projeyi klonlayın:
   ```bash
   git clone https://github.com/KULLANICI_ADINIZ/Inventory-Management-System.git
   ```
2. Visual Studio ile `Inventory Management System.sln` çözümünü açın.
3. `DbHelper.cs` dosyasındaki SQL Server bağlantı dizesinin ortamınıza uygun olduğunu doğrulayın.
4. Çözümü derleyin ve `F5` tuşuna basarak çalıştırın.

---

## 👤 Geliştirici

* **Furkan Yılmaz**
* E-posta: [furkanylmaz0@outlook.com](mailto:furkanylmaz0@outlook.com)
```

---

### Dosya: `Login.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Login.cs`
- **Dosya Boyutu:** 3.16 KB
- **Satır Sayısı:** 84

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void chc_ShowName_CheckedChanged(object sender, EventArgs e)
        {
            if (chc_ShowName.Checked)
                txt_password.PasswordChar = '\0';
            else
                txt_password.PasswordChar = '*';
        }

        private void lbl_clear_Click(object sender, EventArgs e)
        {
            txt_login.Text = string.Empty;
            txt_password.Text = string.Empty;
            txt_login.Focus();
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txt_login.Text.Trim();
            string password = txt_password.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifrenizi giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "SELECT UsernameDB, FullnameDB, RoleDB FROM UserTable WHERE UsernameDB = @user AND PasswordDB = @pass";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@user", username),
                    new SqlParameter("@pass", password)
                };

                DataTable dt = DbHelper.ExecuteQuery(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    CurrentUser.Username = row["UsernameDB"].ToString();
                    CurrentUser.FullName = row["FullnameDB"]?.ToString() ?? CurrentUser.Username;
                    CurrentUser.Role = (row.Table.Columns.Contains("RoleDB") && row["RoleDB"] != DBNull.Value) ? row["RoleDB"].ToString() : "Yönetici (Admin)";

                    DbHelper.LogAction("Giriş Başarılı", "Oturum / Kimlik", $"'{CurrentUser.Username}' ({CurrentUser.Role}) sisteme başarıyla giriş yaptı.", CurrentUser.Username, CurrentUser.Role);

                    HomeForm home = new HomeForm();
                    home.Show();
                    this.Hide();
                }
                else
                {
                    DbHelper.LogAction("Başarısız Giriş", "Oturum / Kimlik", $"'{username}' kullanıcı adı ile hatalı şifre girişi denendi.", username, "Bilinmiyor");
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_password.Text = string.Empty;
                    txt_password.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Giriş yapılırken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
```

---

### Dosya: `Login.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Login.Designer.cs`
- **Dosya Boyutu:** 9.89 KB
- **Satır Sayısı:** 214

```csharp
namespace Inventory_Management_System
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbl_clear = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txt_password = new System.Windows.Forms.MaskedTextBox();
            this.txt_login = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chc_ShowName = new System.Windows.Forms.CheckBox();
            this.btn_exit = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.AntiqueWhite;
            this.panel1.Controls.Add(this.chc_ShowName);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.lbl_clear);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.txt_password);
            this.panel1.Controls.Add(this.txt_login);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(125, 67);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(242, 332);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(68, 62);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // lbl_clear
            // 
            this.lbl_clear.AutoSize = true;
            this.lbl_clear.Location = new System.Drawing.Point(28, 211);
            this.lbl_clear.Name = "lbl_clear";
            this.lbl_clear.Size = new System.Drawing.Size(42, 13);
            this.lbl_clear.TabIndex = 4;
            this.lbl_clear.Text = "CLEAR";
            this.lbl_clear.Click += new System.EventHandler(this.lbl_clear_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(112, 206);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "LOGIN";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txt_password
            // 
            this.txt_password.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txt_password.Location = new System.Drawing.Point(24, 180);
            this.txt_password.Name = "txt_password";
            this.txt_password.PasswordChar = '*';
            this.txt_password.Size = new System.Drawing.Size(163, 20);
            this.txt_password.TabIndex = 2;
            // 
            // txt_login
            // 
            this.txt_login.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txt_login.Location = new System.Drawing.Point(24, 140);
            this.txt_login.Name = "txt_login";
            this.txt_login.Size = new System.Drawing.Size(163, 20);
            this.txt_login.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(93, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "LOGIN";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(150, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(197, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "INVENTORY MANAGEMENT SYSTEM";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Kullanıcı Adı";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Şifre";
            // 
            // chc_ShowName
            // 
            this.chc_ShowName.AutoSize = true;
            this.chc_ShowName.Location = new System.Drawing.Point(112, 244);
            this.chc_ShowName.Name = "chc_ShowName";
            this.chc_ShowName.Size = new System.Drawing.Size(102, 17);
            this.chc_ShowName.TabIndex = 9;
            this.chc_ShowName.Text = "Show Password";
            this.chc_ShowName.UseVisualStyleBackColor = true;
            this.chc_ShowName.CheckedChanged += new System.EventHandler(this.chc_ShowName_CheckedChanged);
            // 
            // btn_exit
            // 
            this.btn_exit.BackColor = System.Drawing.Color.Transparent;
            this.btn_exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_exit.ForeColor = System.Drawing.Color.White;
            this.btn_exit.Location = new System.Drawing.Point(468, 12);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(34, 34);
            this.btn_exit.TabIndex = 10;
            this.btn_exit.Text = "X";
            this.btn_exit.UseVisualStyleBackColor = false;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Inventory_Management_System.Properties.Resources.shutterstock_1930996376_scaled;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(514, 471);
            this.Controls.Add(this.btn_exit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbl_clear;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MaskedTextBox txt_password;
        private System.Windows.Forms.MaskedTextBox txt_login;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chc_ShowName;
        private System.Windows.Forms.Button btn_exit;
    }
}
```

---

### Dosya: `Login.resx`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Login.resx`
- **Dosya Boyutu:** 8099.08 KB
- **Satır Sayısı:** 92207

```xml
﻿<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- 
    Microsoft ResX Schema 
    
    Version 2.0
    
    The primary goals of this format is to allow a simple XML format 
    that is mostly human readable. The generation and parsing of the 
    various data types are done through the TypeConverter classes 
    associated with the data types.
    
    Example:
    
    ... ado.net/XML headers & schema ...
    <resheader name="resmimetype">text/microsoft-resx</resheader>
    <resheader name="version">2.0</resheader>
    <resheader name="reader">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name="writer">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name="Name1"><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name="Color1" type="System.Drawing.Color, System.Drawing">Blue</data>
    <data name="Bitmap1" mimetype="application/x-microsoft.net.object.binary.base64">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name="Icon1" type="System.Drawing.Icon, System.Drawing" mimetype="application/x-microsoft.net.object.bytearray.base64">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>
                
    There are any number of "resheader" rows that contain simple 
    name/value pairs.
    
    Each data row contains a name, and value. The row also contains a 
    type or mimetype. Type corresponds to a .NET class that support 
    text/value conversion through the TypeConverter architecture. 
    Classes that don't support this are serialized and stored with the 
    mimetype set.
    
    The mimetype is used for serialized objects, and tells the 
    ResXResourceReader how to depersist the object. This is currently not 
    extensible. For a given mimetype the value must be set accordingly:
    
    Note - application/x-microsoft.net.object.binary.base64 is the format 
    that the ResXResourceWriter will generate, however the reader can 
    read any of the formats listed below.
    
    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.
    
    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array 
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <assembly alias="System.Drawing" name="System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
  <data name="pictureBox1.Image" type="System.Drawing.Bitmap, System.Drawing" mimetype="application/x-microsoft.net.object.bytearray.base64">
    <value>
      <!-- [8.2 MB Base64 Boyutundaki Binary Görsel Verisi Doküman Okunabilirliği ve Performansı İçin Kısaltıldı] -->
    </value>
  </data>
</root>
```

---

### Dosya: `HomeForm.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\HomeForm.cs`
- **Dosya Boyutu:** 8.49 KB
- **Satır Sayısı:** 227

```csharp
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
                // Kasiyer: Hide user management, users card, audit logs, backup, and financial overview
                btnUsers.Visible = false;
                panelCardUsers.Visible = false;
                lblFinanceSummary.Visible = false;
                btnAuditLogs.Visible = false;
                btnBackup.Visible = false;
                btnOrders.Visible = true;
                panelCardOrders.Visible = true;
            }
            else if (CurrentUser.IsWarehouse)
            {
                // Depo Görevlisi: Hide user management, users card, orders, audit logs, backup, and financial overview
                btnUsers.Visible = false;
                panelCardUsers.Visible = false;
                btnOrders.Visible = false;
                panelCardOrders.Visible = false;
                lblFinanceSummary.Visible = false;
                btnAuditLogs.Visible = false;
                btnBackup.Visible = false;
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
                btnBackup.Visible = true;
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

        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Bu alana yalnızca Yönetici (Admin) yetkisine sahip kullanıcılar erişebilir.", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ManageBackupRestore backupForm = new ManageBackupRestore();
            backupForm.Show();
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
```

---

### Dosya: `HomeForm.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\HomeForm.Designer.cs`
- **Dosya Boyutu:** 29.92 KB
- **Satır Sayısı:** 501

```csharp
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
            this.btnAuditLogs = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
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
            this.panelCardCritical = new System.Windows.Forms.Panel();
            this.lblCountCritical = new System.Windows.Forms.Label();
            this.lblCardCriticalTitle = new System.Windows.Forms.Label();
            this.lblFinanceSummary = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            this.panelCardUsers.SuspendLayout();
            this.panelCardProducts.SuspendLayout();
            this.panelCardCategories.SuspendLayout();
            this.panelCardOrders.SuspendLayout();
            this.panelCardCritical.SuspendLayout();
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
            this.panelCardProducts.Location = new System.Drawing.Point(25, 130);
            this.panelCardProducts.Name = "panelCardProducts";
            this.panelCardProducts.Size = new System.Drawing.Size(150, 100);
            this.panelCardProducts.TabIndex = 2;
            this.panelCardProducts.Click += new System.EventHandler(this.panelCardProducts_Click);
            // 
            // lblCountProducts
            // 
            this.lblCountProducts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCountProducts.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCountProducts.ForeColor = System.Drawing.Color.White;
            this.lblCountProducts.Location = new System.Drawing.Point(5, 45);
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
            this.lblCardProductsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCardProductsTitle.ForeColor = System.Drawing.Color.White;
            this.lblCardProductsTitle.Location = new System.Drawing.Point(5, 12);
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
            this.panelCardCategories.Location = new System.Drawing.Point(195, 130);
            this.panelCardCategories.Name = "panelCardCategories";
            this.panelCardCategories.Size = new System.Drawing.Size(150, 100);
            this.panelCardCategories.TabIndex = 3;
            this.panelCardCategories.Click += new System.EventHandler(this.panelCardCategories_Click);
            // 
            // lblCountCategories
            // 
            this.lblCountCategories.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCountCategories.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCountCategories.ForeColor = System.Drawing.Color.White;
            this.lblCountCategories.Location = new System.Drawing.Point(5, 45);
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
            this.lblCardCategoriesTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCardCategoriesTitle.ForeColor = System.Drawing.Color.White;
            this.lblCardCategoriesTitle.Location = new System.Drawing.Point(5, 12);
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
            this.panelCardUsers.Location = new System.Drawing.Point(365, 130);
            this.panelCardUsers.Name = "panelCardUsers";
            this.panelCardUsers.Size = new System.Drawing.Size(150, 100);
            this.panelCardUsers.TabIndex = 4;
            this.panelCardUsers.Click += new System.EventHandler(this.panelCardUsers_Click);
            // 
            // lblCountUsers
            // 
            this.lblCountUsers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCountUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCountUsers.ForeColor = System.Drawing.Color.White;
            this.lblCountUsers.Location = new System.Drawing.Point(5, 45);
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
            this.lblCardUsersTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCardUsersTitle.ForeColor = System.Drawing.Color.White;
            this.lblCardUsersTitle.Location = new System.Drawing.Point(5, 12);
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
            this.panelCardOrders.Location = new System.Drawing.Point(535, 130);
            this.panelCardOrders.Name = "panelCardOrders";
            this.panelCardOrders.Size = new System.Drawing.Size(150, 100);
            this.panelCardOrders.TabIndex = 5;
            this.panelCardOrders.Click += new System.EventHandler(this.panelCardOrders_Click);
            // 
            // lblCountOrders
            // 
            this.lblCountOrders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCountOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCountOrders.ForeColor = System.Drawing.Color.White;
            this.lblCountOrders.Location = new System.Drawing.Point(5, 45);
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
            this.lblCardOrdersTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCardOrdersTitle.ForeColor = System.Drawing.Color.White;
            this.lblCardOrdersTitle.Location = new System.Drawing.Point(5, 12);
            this.lblCardOrdersTitle.Name = "lblCardOrdersTitle";
            this.lblCardOrdersTitle.Size = new System.Drawing.Size(140, 25);
            this.lblCardOrdersTitle.TabIndex = 0;
            this.lblCardOrdersTitle.Text = "SİPARİŞLER";
            this.lblCardOrdersTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCardOrdersTitle.Click += new System.EventHandler(this.panelCardOrders_Click);
            // 
            // panelCardCritical
            // 
            this.panelCardCritical.BackColor = System.Drawing.Color.Crimson;
            this.panelCardCritical.Controls.Add(this.lblCountCritical);
            this.panelCardCritical.Controls.Add(this.lblCardCriticalTitle);
            this.panelCardCritical.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelCardCritical.Location = new System.Drawing.Point(705, 130);
            this.panelCardCritical.Name = "panelCardCritical";
            this.panelCardCritical.Size = new System.Drawing.Size(150, 100);
            this.panelCardCritical.TabIndex = 6;
            this.panelCardCritical.Click += new System.EventHandler(this.panelCardCritical_Click);
            // 
            // lblCountCritical
            // 
            this.lblCountCritical.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCountCritical.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCountCritical.ForeColor = System.Drawing.Color.White;
            this.lblCountCritical.Location = new System.Drawing.Point(5, 45);
            this.lblCountCritical.Name = "lblCountCritical";
            this.lblCountCritical.Size = new System.Drawing.Size(140, 40);
            this.lblCountCritical.TabIndex = 1;
            this.lblCountCritical.Text = "0";
            this.lblCountCritical.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCountCritical.Click += new System.EventHandler(this.panelCardCritical_Click);
            // 
            // lblCardCriticalTitle
            // 
            this.lblCardCriticalTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCardCriticalTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCardCriticalTitle.ForeColor = System.Drawing.Color.White;
            this.lblCardCriticalTitle.Location = new System.Drawing.Point(5, 12);
            this.lblCardCriticalTitle.Name = "lblCardCriticalTitle";
            this.lblCardCriticalTitle.Size = new System.Drawing.Size(140, 25);
            this.lblCardCriticalTitle.TabIndex = 0;
            this.lblCardCriticalTitle.Text = "KRİTİK STOK";
            this.lblCardCriticalTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCardCriticalTitle.Click += new System.EventHandler(this.panelCardCritical_Click);
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
            // lblFinanceSummary
            // 
            this.lblFinanceSummary.BackColor = System.Drawing.Color.White;
            this.lblFinanceSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFinanceSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblFinanceSummary.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblFinanceSummary.Location = new System.Drawing.Point(25, 238);
            this.lblFinanceSummary.Name = "lblFinanceSummary";
            this.lblFinanceSummary.Size = new System.Drawing.Size(830, 24);
            this.lblFinanceSummary.TabIndex = 12;
            this.lblFinanceSummary.Text = "💰 Finansal Özet: Toplam Ciro: 0,00 TL  |  Toplam Maliyet: 0,00 TL  |  Net Kâr: +0,00 TL";
            this.lblFinanceSummary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAuditLogs
            // 
            this.btnAuditLogs.BackColor = System.Drawing.Color.DarkSlateGray;
            this.btnAuditLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuditLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnAuditLogs.ForeColor = System.Drawing.Color.White;
            this.btnAuditLogs.Location = new System.Drawing.Point(100, 435);
            this.btnAuditLogs.Name = "btnAuditLogs";
            this.btnAuditLogs.Size = new System.Drawing.Size(320, 50);
            this.btnAuditLogs.TabIndex = 13;
            this.btnAuditLogs.Text = "📋 İŞLEM GEÇMİŞİ (AUDIT LOG)";
            this.btnAuditLogs.UseVisualStyleBackColor = false;
            this.btnAuditLogs.Click += new System.EventHandler(this.btnAuditLogs_Click);
            // 
            // btnBackup
            // 
            this.btnBackup.BackColor = System.Drawing.Color.SteelBlue;
            this.btnBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnBackup.ForeColor = System.Drawing.Color.White;
            this.btnBackup.Location = new System.Drawing.Point(460, 435);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(320, 50);
            this.btnBackup.TabIndex = 14;
            this.btnBackup.Text = "💾 YEDEKLEME VE GERİ YÜKLEME";
            this.btnBackup.UseVisualStyleBackColor = false;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(880, 570);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.btnAuditLogs);
            this.Controls.Add(this.lblFinanceSummary);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnOrders);
            this.Controls.Add(this.btnCustomers);
            this.Controls.Add(this.btnUsers);
            this.Controls.Add(this.btnCategories);
            this.Controls.Add(this.btnProducts);
            this.Controls.Add(this.panelCardCritical);
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
            this.panelCardCritical.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnAuditLogs;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Label lblFinanceSummary;
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
        private System.Windows.Forms.Panel panelCardCritical;
        private System.Windows.Forms.Label lblCountCritical;
        private System.Windows.Forms.Label lblCardCriticalTitle;
    }
}
```

---

### Dosya: `ManageProducts.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageProducts.cs`
- **Dosya Boyutu:** 20.00 KB
- **Satır Sayısı:** 462

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageProducts : Form
    {
        private bool initialFilterCritical = false;

        public ManageProducts(bool filterCritical = false)
        {
            InitializeComponent();
            this.initialFilterCritical = filterCritical;
        }

        private void ManageProducts_Load(object sender, EventArgs e)
        {
            ApplyRoleRestrictions();
            FormatGrid();
            LoadCategories();
            if (initialFilterCritical)
            {
                chc_critical.Checked = true;
            }
            else
            {
                PopulateProducts();
            }
            ClearInputs();
        }

        private void ApplyRoleRestrictions()
        {
            if (!CurrentUser.IsAdmin)
            {
                txt_prodcost.Visible = false;
                lbl_prodcost.Visible = false;
            }

            if (CurrentUser.IsCashier)
            {
                btn_delete.Enabled = false;
            }
        }

        private void FormatGrid()
        {
            dataGridViewProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewProducts.MultiSelect = false;
            dataGridViewProducts.ReadOnly = true;
            dataGridViewProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewProducts.AllowUserToAddRows = false;
        }

        private void LoadCategories()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT CatName FROM CategoryTable ORDER BY CatName");

                cmb_prodcat.Items.Clear();
                cmb_searchcat.Items.Clear();
                cmb_searchcat.Items.Add("-- Tüm Kategoriler --");

                foreach (DataRow row in dt.Rows)
                {
                    string cat = row["CatName"].ToString();
                    cmb_prodcat.Items.Add(cat);
                    cmb_searchcat.Items.Add(cat);
                }

                cmb_prodcat.SelectedIndex = -1;
                cmb_searchcat.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategoriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateProducts(string categoryFilter = null)
        {
            try
            {
                string query;
                SqlParameter[] parameters = null;

                bool onlyCritical = chc_critical.Checked;
                bool hasCategory = !string.IsNullOrEmpty(categoryFilter) && categoryFilter != "-- Tüm Kategoriler --";

                if (onlyCritical && hasCategory)
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable WHERE ProdCat = @cat AND ProdQty <= ProdMinQty ORDER BY ProdQty ASC";
                    parameters = new SqlParameter[] { new SqlParameter("@cat", categoryFilter) };
                }
                else if (onlyCritical)
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable WHERE ProdQty <= ProdMinQty ORDER BY ProdQty ASC";
                }
                else if (hasCategory)
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable WHERE ProdCat = @cat ORDER BY ProdId ASC";
                    parameters = new SqlParameter[] { new SqlParameter("@cat", categoryFilter) };
                }
                else
                {
                    query = "SELECT ProdId AS [Ürün ID], ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], ProdQty AS [Miktar], ProdMinQty AS [Kritik Sınır], ProdCostPrice AS [Maliyet (TL)], ProdPrice AS [Satış Fiyatı (TL)], (ProdPrice - ProdCostPrice) AS [Birim Kâr (TL)], ProdDesc AS [Açıklama], ProdCat AS [Kategori] FROM ProductTable ORDER BY ProdId ASC";
                }

                DataTable dt = DbHelper.ExecuteQuery(query, parameters);
                dataGridViewProducts.DataSource = dt;

                if (!CurrentUser.IsAdmin)
                {
                    if (dataGridViewProducts.Columns["Maliyet (TL)"] != null)
                    {
                        dataGridViewProducts.Columns["Maliyet (TL)"].Visible = false;
                    }
                    if (dataGridViewProducts.Columns["Birim Kâr (TL)"] != null)
                    {
                        dataGridViewProducts.Columns["Birim Kâr (TL)"].Visible = false;
                    }
                }

                if (dataGridViewProducts.Columns["Maliyet (TL)"] != null)
                {
                    dataGridViewProducts.Columns["Maliyet (TL)"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridViewProducts.Columns["Satış Fiyatı (TL)"] != null)
                {
                    dataGridViewProducts.Columns["Satış Fiyatı (TL)"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridViewProducts.Columns["Birim Kâr (TL)"] != null)
                {
                    dataGridViewProducts.Columns["Birim Kâr (TL)"].DefaultCellStyle.Format = "N2";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürünler listelenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chc_critical_CheckedChanged(object sender, EventArgs e)
        {
            string selectedCat = cmb_searchcat.SelectedItem?.ToString();
            PopulateProducts(selectedCat);
        }

        private void dataGridViewProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewProducts.Rows.Count)
            {
                DataGridViewRow row = dataGridViewProducts.Rows[e.RowIndex];
                if (row.Cells["Miktar"]?.Value != null && row.Cells["Kritik Sınır"]?.Value != null)
                {
                    if (int.TryParse(row.Cells["Miktar"].Value.ToString(), out int qty) &&
                        int.TryParse(row.Cells["Kritik Sınır"].Value.ToString(), out int minQty))
                    {
                        if (qty == 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.MistyRose;
                            row.DefaultCellStyle.ForeColor = Color.DarkRed;
                            row.DefaultCellStyle.SelectionBackColor = Color.IndianRed;
                            row.DefaultCellStyle.SelectionForeColor = Color.White;
                        }
                        else if (qty <= minQty)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                            row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                            row.DefaultCellStyle.SelectionBackColor = Color.Orange;
                            row.DefaultCellStyle.SelectionForeColor = Color.White;
                        }
                    }
                }
            }
        }

        private string GenerateBarcode()
        {
            Random rnd = new Random();
            return "869" + rnd.Next(100000000, 999999999).ToString() + rnd.Next(0, 9).ToString();
        }

        private void btn_gen_barcode_Click(object sender, EventArgs e)
        {
            txt_prodbarcode.Text = GenerateBarcode();
        }

        private void ClearInputs()
        {
            txt_prodid.Text = string.Empty;
            txt_prodbarcode.Text = string.Empty;
            txt_prodname.Text = string.Empty;
            txt_prodqty.Text = string.Empty;
            txt_prodminqty.Text = "5";
            txt_prodcost.Text = string.Empty;
            txt_prodprice.Text = string.Empty;
            txt_proddesc.Text = string.Empty;
            cmb_prodcat.SelectedIndex = -1;
            dataGridViewProducts.ClearSelection();
            txt_prodname.Focus();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearInputs();
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

        private void btn_home_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_prodname.Text) || string.IsNullOrWhiteSpace(txt_prodqty.Text) || string.IsNullOrWhiteSpace(txt_prodprice.Text))
            {
                MessageBox.Show("Lütfen ürün adı, stok miktarı ve fiyat alanlarını doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txt_prodqty.Text.Trim(), out int qty) || qty < 0)
            {
                MessageBox.Show("Geçerli bir stok miktarı giriniz (0 veya daha büyük bir tam sayı).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txt_prodprice.Text.Trim(), out decimal price) || price < 0)
            {
                MessageBox.Show("Geçerli bir ürün fiyatı giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal costPrice = 0;
            if (!string.IsNullOrWhiteSpace(txt_prodcost.Text))
            {
                decimal.TryParse(txt_prodcost.Text.Trim(), out costPrice);
            }

            if (cmb_prodcat.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir kategori seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int minQty = 5;
            if (!string.IsNullOrWhiteSpace(txt_prodminqty.Text))
            {
                int.TryParse(txt_prodminqty.Text.Trim(), out minQty);
            }

            string barcode = txt_prodbarcode.Text.Trim();
            if (string.IsNullOrEmpty(barcode))
            {
                barcode = GenerateBarcode();
                txt_prodbarcode.Text = barcode;
            }

            try
            {
                string query = "INSERT INTO ProductTable (ProdBarcode, ProdName, ProdQty, ProdMinQty, ProdCostPrice, ProdPrice, ProdDesc, ProdCat) VALUES (@barcode, @name, @qty, @minqty, @cost, @price, @desc, @cat)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@barcode", barcode),
                    new SqlParameter("@name", txt_prodname.Text.Trim()),
                    new SqlParameter("@qty", qty),
                    new SqlParameter("@minqty", minQty),
                    new SqlParameter("@cost", costPrice),
                    new SqlParameter("@price", price),
                    new SqlParameter("@desc", txt_proddesc.Text.Trim()),
                    new SqlParameter("@cat", cmb_prodcat.SelectedItem.ToString())
                };

                DbHelper.ExecuteNonQuery(query, parameters);
                DbHelper.LogAction("Ürün Eklendi", "Ürün Yönetimi", $"Barkod: {barcode}, Ürün Adı: '{txt_prodname.Text.Trim()}', Miktar: {qty}, Kritik Sınır: {minQty}, Satış: {price:N2} TL, Kategori: '{cmb_prodcat.SelectedItem}'");
                MessageBox.Show("Ürün başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateProducts();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_prodid.Text))
            {
                MessageBox.Show("Lütfen güncellenecek ürünü listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txt_prodqty.Text.Trim(), out int qty) || qty < 0)
            {
                MessageBox.Show("Geçerli bir stok miktarı giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txt_prodprice.Text.Trim(), out decimal price) || price < 0)
            {
                MessageBox.Show("Geçerli bir fiyat giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal costPrice = 0;
            if (!string.IsNullOrWhiteSpace(txt_prodcost.Text))
            {
                decimal.TryParse(txt_prodcost.Text.Trim(), out costPrice);
            }

            if (cmb_prodcat.SelectedItem == null || cmb_prodcat.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen bir kategori seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int minQty = 5;
            if (!string.IsNullOrWhiteSpace(txt_prodminqty.Text))
            {
                int.TryParse(txt_prodminqty.Text.Trim(), out minQty);
            }

            string barcode = txt_prodbarcode.Text.Trim();
            if (string.IsNullOrEmpty(barcode))
            {
                barcode = GenerateBarcode();
                txt_prodbarcode.Text = barcode;
            }

            try
            {
                string query = "UPDATE ProductTable SET ProdBarcode = @barcode, ProdName = @name, ProdQty = @qty, ProdMinQty = @minqty, ProdCostPrice = @cost, ProdPrice = @price, ProdDesc = @desc, ProdCat = @cat WHERE ProdId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@barcode", barcode),
                    new SqlParameter("@name", txt_prodname.Text.Trim()),
                    new SqlParameter("@qty", qty),
                    new SqlParameter("@minqty", minQty),
                    new SqlParameter("@cost", costPrice),
                    new SqlParameter("@price", price),
                    new SqlParameter("@desc", txt_proddesc.Text.Trim()),
                    new SqlParameter("@cat", cmb_prodcat.SelectedItem.ToString()),
                    new SqlParameter("@id", Convert.ToInt32(txt_prodid.Text))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    DbHelper.LogAction("Ürün Güncellendi", "Ürün Yönetimi", $"ID: {txt_prodid.Text}, Barkod: {barcode}, Ürün Adı: '{txt_prodname.Text.Trim()}', Miktar: {qty}, Kritik Sınır: {minQty}, Satış: {price:N2} TL, Kategori: '{cmb_prodcat.SelectedItem}'");
                    MessageBox.Show("Ürün başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateProducts();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün güncellenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (CurrentUser.IsCashier)
            {
                MessageBox.Show("Kasiyer yetkisi ile ürün silinemez!", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_prodid.Text))
            {
                MessageBox.Show("Lütfen silinecek ürünü listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string deletedName = txt_prodname.Text.Trim();
            string deletedId = txt_prodid.Text.Trim();

            DialogResult confirm = MessageBox.Show($"'{deletedName}' ürününü silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM ProductTable WHERE ProdId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@id", Convert.ToInt32(deletedId))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    DbHelper.LogAction("Ürün Silindi", "Ürün Yönetimi", $"ID: {deletedId}, Ürün Adı: '{deletedName}' kalıcı olarak silindi.");
                    MessageBox.Show("Ürün başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateProducts();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün silinirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewProducts.Rows.Count)
            {
                DataGridViewRow row = dataGridViewProducts.Rows[e.RowIndex];
                txt_prodid.Text = row.Cells[0].Value?.ToString() ?? "";
                txt_prodbarcode.Text = row.Cells[1].Value?.ToString() ?? "";
                txt_prodname.Text = row.Cells[2].Value?.ToString() ?? "";
                txt_prodqty.Text = row.Cells[3].Value?.ToString() ?? "";
                txt_prodminqty.Text = row.Cells[4].Value?.ToString() ?? "5";
                txt_prodcost.Text = row.Cells[5].Value != null ? Convert.ToDecimal(row.Cells[5].Value).ToString("N2") : "0,00";
                txt_prodprice.Text = row.Cells[6].Value != null ? Convert.ToDecimal(row.Cells[6].Value).ToString("N2") : "";
                txt_proddesc.Text = row.Cells[8].Value?.ToString() ?? "";
                string cat = row.Cells[9].Value?.ToString() ?? "";
                if (cmb_prodcat.Items.Contains(cat))
                    cmb_prodcat.SelectedItem = cat;
                else
                    cmb_prodcat.SelectedIndex = -1;
            }
        }

        private void cmb_searchcat_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedCat = cmb_searchcat.SelectedItem?.ToString();
            PopulateProducts(selectedCat);
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            chc_critical.Checked = false;
            cmb_searchcat.SelectedIndex = 0;
            PopulateProducts();
            ClearInputs();
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            string fileName = $"Urun_Stok_Listesi_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            DbHelper.ExportToCsv(dataGridViewProducts, fileName);
        }
    }
}
```

---

### Dosya: `ManageProducts.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageProducts.Designer.cs`
- **Dosya Boyutu:** 26.92 KB
- **Satır Sayısı:** 507

```csharp
namespace Inventory_Management_System
{
    partial class ManageProducts
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
            this.txt_prodid = new System.Windows.Forms.TextBox();
            this.txt_prodname = new System.Windows.Forms.TextBox();
            this.txt_prodqty = new System.Windows.Forms.TextBox();
            this.txt_prodprice = new System.Windows.Forms.TextBox();
            this.txt_proddesc = new System.Windows.Forms.TextBox();
            this.cmb_prodcat = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_add = new System.Windows.Forms.Button();
            this.btn_edit = new System.Windows.Forms.Button();
            this.btn_delete = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_home = new System.Windows.Forms.Button();
            this.dataGridViewProducts = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmb_searchcat = new System.Windows.Forms.ComboBox();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.lbl_prodminqty = new System.Windows.Forms.Label();
            this.txt_prodminqty = new System.Windows.Forms.TextBox();
            this.chc_critical = new System.Windows.Forms.CheckBox();
            this.btn_export = new System.Windows.Forms.Button();
            this.lbl_prodcost = new System.Windows.Forms.Label();
            this.txt_prodcost = new System.Windows.Forms.TextBox();
            this.lbl_prodbarcode = new System.Windows.Forms.Label();
            this.txt_prodbarcode = new System.Windows.Forms.TextBox();
            this.btn_gen_barcode = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProducts)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel1.Controls.Add(this.btn_back);
            this.panel1.Controls.Add(this.btn_exit);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(920, 100);
            this.panel1.TabIndex = 0;
            // 
            // btn_back
            // 
            this.btn_back.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_back.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_back.ForeColor = System.Drawing.Color.White;
            this.btn_back.Location = new System.Drawing.Point(15, 25);
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
            this.btn_exit.Location = new System.Drawing.Point(858, 12);
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
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(300, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(332, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "MANAGE PRODUCTS";
            // 
            // txt_prodid
            // 
            this.txt_prodid.Location = new System.Drawing.Point(95, 120);
            this.txt_prodid.Name = "txt_prodid";
            this.txt_prodid.ReadOnly = true;
            this.txt_prodid.Size = new System.Drawing.Size(40, 20);
            this.txt_prodid.TabIndex = 1;
            // 
            // lbl_prodbarcode
            // 
            this.lbl_prodbarcode.AutoSize = true;
            this.lbl_prodbarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_prodbarcode.Location = new System.Drawing.Point(140, 123);
            this.lbl_prodbarcode.Name = "lbl_prodbarcode";
            this.lbl_prodbarcode.Size = new System.Drawing.Size(51, 13);
            this.lbl_prodbarcode.TabIndex = 26;
            this.lbl_prodbarcode.Text = "Barkod:";
            // 
            // txt_prodbarcode
            // 
            this.txt_prodbarcode.Location = new System.Drawing.Point(195, 120);
            this.txt_prodbarcode.Name = "txt_prodbarcode";
            this.txt_prodbarcode.Size = new System.Drawing.Size(85, 20);
            this.txt_prodbarcode.TabIndex = 2;
            // 
            // btn_gen_barcode
            // 
            this.btn_gen_barcode.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_gen_barcode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_gen_barcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_gen_barcode.ForeColor = System.Drawing.Color.White;
            this.btn_gen_barcode.Location = new System.Drawing.Point(283, 118);
            this.btn_gen_barcode.Name = "btn_gen_barcode";
            this.btn_gen_barcode.Size = new System.Drawing.Size(32, 23);
            this.btn_gen_barcode.TabIndex = 27;
            this.btn_gen_barcode.Text = "🎲";
            this.btn_gen_barcode.UseVisualStyleBackColor = false;
            this.btn_gen_barcode.Click += new System.EventHandler(this.btn_gen_barcode_Click);
            // 
            // txt_prodname
            // 
            this.txt_prodname.Location = new System.Drawing.Point(140, 155);
            this.txt_prodname.Name = "txt_prodname";
            this.txt_prodname.Size = new System.Drawing.Size(150, 20);
            this.txt_prodname.TabIndex = 2;
            // 
            // txt_prodqty
            // 
            this.txt_prodqty.Location = new System.Drawing.Point(140, 190);
            this.txt_prodqty.Name = "txt_prodqty";
            this.txt_prodqty.Size = new System.Drawing.Size(55, 20);
            this.txt_prodqty.TabIndex = 3;
            // 
            // lbl_prodminqty
            // 
            this.lbl_prodminqty.AutoSize = true;
            this.lbl_prodminqty.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_prodminqty.Location = new System.Drawing.Point(202, 193);
            this.lbl_prodminqty.Name = "lbl_prodminqty";
            this.lbl_prodminqty.Size = new System.Drawing.Size(74, 13);
            this.lbl_prodminqty.TabIndex = 22;
            this.lbl_prodminqty.Text = "Kritik Sınır:";
            // 
            // txt_prodminqty
            // 
            this.txt_prodminqty.Location = new System.Drawing.Point(280, 190);
            this.txt_prodminqty.Name = "txt_prodminqty";
            this.txt_prodminqty.Size = new System.Drawing.Size(40, 20);
            this.txt_prodminqty.TabIndex = 4;
            this.txt_prodminqty.Text = "5";
            // 
            // txt_prodprice
            // 
            this.txt_prodprice.Location = new System.Drawing.Point(125, 225);
            this.txt_prodprice.Name = "txt_prodprice";
            this.txt_prodprice.Size = new System.Drawing.Size(65, 20);
            this.txt_prodprice.TabIndex = 5;
            // 
            // lbl_prodcost
            // 
            this.lbl_prodcost.AutoSize = true;
            this.lbl_prodcost.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_prodcost.Location = new System.Drawing.Point(195, 228);
            this.lbl_prodcost.Name = "lbl_prodcost";
            this.lbl_prodcost.Size = new System.Drawing.Size(51, 13);
            this.lbl_prodcost.TabIndex = 25;
            this.lbl_prodcost.Text = "Maliyet:";
            // 
            // txt_prodcost
            // 
            this.txt_prodcost.Location = new System.Drawing.Point(250, 225);
            this.txt_prodcost.Name = "txt_prodcost";
            this.txt_prodcost.Size = new System.Drawing.Size(55, 20);
            this.txt_prodcost.TabIndex = 6;
            // 
            // txt_proddesc
            // 
            this.txt_proddesc.Location = new System.Drawing.Point(140, 260);
            this.txt_proddesc.Name = "txt_proddesc";
            this.txt_proddesc.Size = new System.Drawing.Size(150, 20);
            this.txt_proddesc.TabIndex = 5;
            // 
            // cmb_prodcat
            // 
            this.cmb_prodcat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_prodcat.FormattingEnabled = true;
            this.cmb_prodcat.Location = new System.Drawing.Point(140, 295);
            this.cmb_prodcat.Name = "cmb_prodcat";
            this.cmb_prodcat.Size = new System.Drawing.Size(150, 21);
            this.cmb_prodcat.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(30, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Ürün ID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(30, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Ürün Adı:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(30, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Stok Miktarı:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.Location = new System.Drawing.Point(30, 227);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "Satış Fiyatı:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label6.Location = new System.Drawing.Point(30, 262);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "Açıklama:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label7.Location = new System.Drawing.Point(30, 297);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 15);
            this.label7.TabIndex = 12;
            this.label7.Text = "Kategori:";
            // 
            // btn_add
            // 
            this.btn_add.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_add.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_add.ForeColor = System.Drawing.Color.White;
            this.btn_add.Location = new System.Drawing.Point(33, 340);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(75, 30);
            this.btn_add.TabIndex = 13;
            this.btn_add.Text = "ADD";
            this.btn_add.UseVisualStyleBackColor = false;
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // btn_edit
            // 
            this.btn_edit.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_edit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_edit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_edit.ForeColor = System.Drawing.Color.White;
            this.btn_edit.Location = new System.Drawing.Point(120, 340);
            this.btn_edit.Name = "btn_edit";
            this.btn_edit.Size = new System.Drawing.Size(75, 30);
            this.btn_edit.TabIndex = 14;
            this.btn_edit.Text = "EDIT";
            this.btn_edit.UseVisualStyleBackColor = false;
            this.btn_edit.Click += new System.EventHandler(this.btn_edit_Click);
            // 
            // btn_delete
            // 
            this.btn_delete.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_delete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_delete.ForeColor = System.Drawing.Color.White;
            this.btn_delete.Location = new System.Drawing.Point(207, 340);
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.Size = new System.Drawing.Size(75, 30);
            this.btn_delete.TabIndex = 15;
            this.btn_delete.Text = "DELETE";
            this.btn_delete.UseVisualStyleBackColor = false;
            this.btn_delete.Click += new System.EventHandler(this.btn_delete_Click);
            // 
            // 
            // btn_clear
            // 
            this.btn_clear.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_clear.ForeColor = System.Drawing.Color.White;
            this.btn_clear.Location = new System.Drawing.Point(33, 385);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(249, 34);
            this.btn_clear.TabIndex = 21;
            this.btn_clear.Text = "SEÇİMİ TEMİZLE";
            this.btn_clear.UseVisualStyleBackColor = false;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_home
            // 
            this.btn_home.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_home.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_home.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_home.ForeColor = System.Drawing.Color.White;
            this.btn_home.Location = new System.Drawing.Point(33, 428);
            this.btn_home.Name = "btn_home";
            this.btn_home.Size = new System.Drawing.Size(249, 34);
            this.btn_home.TabIndex = 16;
            this.btn_home.Text = "◀ ANA MENÜ";
            this.btn_home.UseVisualStyleBackColor = false;
            this.btn_home.Click += new System.EventHandler(this.btn_home_Click);
            // 
            // dataGridViewProducts
            // 
            this.dataGridViewProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewProducts.Location = new System.Drawing.Point(320, 155);
            this.dataGridViewProducts.Name = "dataGridViewProducts";
            this.dataGridViewProducts.Size = new System.Drawing.Size(575, 360);
            this.dataGridViewProducts.TabIndex = 17;
            this.dataGridViewProducts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewProducts_CellClick);
            this.dataGridViewProducts.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewProducts_CellFormatting);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 533);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(920, 37);
            this.panel2.TabIndex = 18;
            // 
            // cmb_searchcat
            // 
            this.cmb_searchcat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_searchcat.FormattingEnabled = true;
            this.cmb_searchcat.Location = new System.Drawing.Point(320, 119);
            this.cmb_searchcat.Name = "cmb_searchcat";
            this.cmb_searchcat.Size = new System.Drawing.Size(150, 21);
            this.cmb_searchcat.TabIndex = 19;
            this.cmb_searchcat.SelectionChangeCommitted += new System.EventHandler(this.cmb_searchcat_SelectionChangeCommitted);
            // 
            // btn_refresh
            // 
            this.btn_refresh.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_refresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_refresh.ForeColor = System.Drawing.Color.White;
            this.btn_refresh.Location = new System.Drawing.Point(478, 118);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(70, 24);
            this.btn_refresh.TabIndex = 20;
            this.btn_refresh.Text = "Yenile";
            this.btn_refresh.UseVisualStyleBackColor = false;
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // chc_critical
            // 
            this.chc_critical.AutoSize = true;
            this.chc_critical.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chc_critical.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.chc_critical.ForeColor = System.Drawing.Color.Crimson;
            this.chc_critical.Location = new System.Drawing.Point(558, 121);
            this.chc_critical.Name = "chc_critical";
            this.chc_critical.Size = new System.Drawing.Size(210, 17);
            this.chc_critical.TabIndex = 23;
            this.chc_critical.Text = "⚠️ Sadece Kritik Stokları Göster";
            this.chc_critical.UseVisualStyleBackColor = true;
            this.chc_critical.CheckedChanged += new System.EventHandler(this.chc_critical_CheckedChanged);
            // 
            // btn_export
            // 
            this.btn_export.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btn_export.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_export.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_export.ForeColor = System.Drawing.Color.White;
            this.btn_export.Location = new System.Drawing.Point(778, 117);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(117, 25);
            this.btn_export.TabIndex = 24;
            this.btn_export.Text = "📊 EXCEL\'E AKTAR";
            this.btn_export.UseVisualStyleBackColor = false;
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            // 
            // ManageProducts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(920, 570);
            this.Controls.Add(this.btn_gen_barcode);
            this.Controls.Add(this.txt_prodbarcode);
            this.Controls.Add(this.lbl_prodbarcode);
            this.Controls.Add(this.btn_export);
            this.Controls.Add(this.chc_critical);
            this.Controls.Add(this.lbl_prodminqty);
            this.Controls.Add(this.txt_prodminqty);
            this.Controls.Add(this.btn_refresh);
            this.Controls.Add(this.cmb_searchcat);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dataGridViewProducts);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.btn_home);
            this.Controls.Add(this.btn_delete);
            this.Controls.Add(this.btn_edit);
            this.Controls.Add(this.btn_add);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmb_prodcat);
            this.Controls.Add(this.txt_proddesc);
            this.Controls.Add(this.txt_prodprice);
            this.Controls.Add(this.lbl_prodcost);
            this.Controls.Add(this.txt_prodcost);
            this.Controls.Add(this.txt_prodqty);
            this.Controls.Add(this.txt_prodname);
            this.Controls.Add(this.txt_prodid);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ManageProducts";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ManageProducts";
            this.Load += new System.EventHandler(this.ManageProducts_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProducts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_prodid;
        private System.Windows.Forms.Label lbl_prodbarcode;
        private System.Windows.Forms.TextBox txt_prodbarcode;
        private System.Windows.Forms.Button btn_gen_barcode;
        private System.Windows.Forms.TextBox txt_prodname;
        private System.Windows.Forms.TextBox txt_prodqty;
        private System.Windows.Forms.TextBox txt_prodprice;
        private System.Windows.Forms.Label lbl_prodcost;
        private System.Windows.Forms.TextBox txt_prodcost;
        private System.Windows.Forms.TextBox txt_proddesc;
        private System.Windows.Forms.ComboBox cmb_prodcat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Button btn_edit;
        private System.Windows.Forms.Button btn_delete;
        private System.Windows.Forms.Button btn_home;
        private System.Windows.Forms.DataGridView dataGridViewProducts;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.ComboBox cmb_searchcat;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.Button btn_back;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.TextBox txt_prodminqty;
        private System.Windows.Forms.Label lbl_prodminqty;
        private System.Windows.Forms.CheckBox chc_critical;
        private System.Windows.Forms.Button btn_export;
    }
}
```

---

### Dosya: `ManageCategories.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageCategories.cs`
- **Dosya Boyutu:** 7.38 KB
- **Satır Sayısı:** 198

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageCategories : Form
    {
        public ManageCategories()
        {
            InitializeComponent();
        }

        private void ManageCategories_Load(object sender, EventArgs e)
        {
            if (CurrentUser.IsCashier)
            {
                btn_delete.Enabled = false;
            }

            FormatGrid();
            PopulateCategories();
            ClearInputs();
        }

        private void FormatGrid()
        {
            dataGridViewCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCategories.MultiSelect = false;
            dataGridViewCategories.ReadOnly = true;
            dataGridViewCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCategories.AllowUserToAddRows = false;
        }

        private void PopulateCategories()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT CatId AS [Kategori ID], CatName AS [Kategori Adı] FROM CategoryTable ORDER BY CatId ASC");
                dataGridViewCategories.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategoriler listelenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txt_catid.Text = string.Empty;
            txt_catname.Text = string.Empty;
            dataGridViewCategories.ClearSelection();
            txt_catname.Focus();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearInputs();
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

        private void btn_home_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_catname.Text))
            {
                MessageBox.Show("Lütfen kategori adını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO CategoryTable (CatName) VALUES (@name)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@name", txt_catname.Text.Trim())
                };

                DbHelper.ExecuteNonQuery(query, parameters);
                DbHelper.LogAction("Kategori Eklendi", "Kategori Yönetimi", $"'{txt_catname.Text.Trim()}' kategorisi eklendi.");
                MessageBox.Show("Kategori başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateCategories();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategori eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_catid.Text))
            {
                MessageBox.Show("Lütfen güncellenecek kategoriyi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_catname.Text))
            {
                MessageBox.Show("Kategori adı boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "UPDATE CategoryTable SET CatName = @name WHERE CatId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@name", txt_catname.Text.Trim()),
                    new SqlParameter("@id", Convert.ToInt32(txt_catid.Text))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    DbHelper.LogAction("Kategori Güncellendi", "Kategori Yönetimi", $"ID: {txt_catid.Text}, Yeni Kategori Adı: '{txt_catname.Text.Trim()}'");
                    MessageBox.Show("Kategori başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateCategories();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategori güncellenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (CurrentUser.IsCashier)
            {
                MessageBox.Show("Kasiyer yetkisi ile kategori silinemez!", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_catid.Text))
            {
                MessageBox.Show("Lütfen silinecek kategoriyi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string deletedName = txt_catname.Text.Trim();
            string deletedId = txt_catid.Text.Trim();

            DialogResult confirm = MessageBox.Show($"'{deletedName}' kategorisini silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM CategoryTable WHERE CatId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@id", Convert.ToInt32(deletedId))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    DbHelper.LogAction("Kategori Silindi", "Kategori Yönetimi", $"ID: {deletedId}, Kategori Adı: '{deletedName}' silindi.");
                    MessageBox.Show("Kategori başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateCategories();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategori silinirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewCategories.Rows.Count)
            {
                DataGridViewRow row = dataGridViewCategories.Rows[e.RowIndex];
                txt_catid.Text = row.Cells[0].Value?.ToString() ?? "";
                txt_catname.Text = row.Cells[1].Value?.ToString() ?? "";
            }
        }
    }
}
```

---

### Dosya: `ManageCategories.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageCategories.Designer.cs`
- **Dosya Boyutu:** 13.41 KB
- **Satır Sayısı:** 263

```csharp
namespace Inventory_Management_System
{
    partial class ManageCategories
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
            this.txt_catid = new System.Windows.Forms.TextBox();
            this.txt_catname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_add = new System.Windows.Forms.Button();
            this.btn_edit = new System.Windows.Forms.Button();
            this.btn_delete = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_home = new System.Windows.Forms.Button();
            this.dataGridViewCategories = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCategories)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel1.Controls.Add(this.btn_back);
            this.panel1.Controls.Add(this.btn_exit);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(880, 100);
            this.panel1.TabIndex = 0;
            // 
            // btn_back
            // 
            this.btn_back.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_back.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_back.ForeColor = System.Drawing.Color.White;
            this.btn_back.Location = new System.Drawing.Point(15, 25);
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
            this.btn_exit.Location = new System.Drawing.Point(818, 12);
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
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(260, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "MANAGE CATEGORIES";
            // 
            // txt_catid
            // 
            this.txt_catid.Location = new System.Drawing.Point(167, 180);
            this.txt_catid.Name = "txt_catid";
            this.txt_catid.ReadOnly = true;
            this.txt_catid.Size = new System.Drawing.Size(120, 20);
            this.txt_catid.TabIndex = 1;
            // 
            // txt_catname
            // 
            this.txt_catname.Location = new System.Drawing.Point(167, 220);
            this.txt_catname.Name = "txt_catname";
            this.txt_catname.Size = new System.Drawing.Size(120, 20);
            this.txt_catname.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(40, 181);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Kategori ID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(40, 221);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Kategori Adı:";
            // 
            // btn_add
            // 
            this.btn_add.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_add.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_add.ForeColor = System.Drawing.Color.White;
            this.btn_add.Location = new System.Drawing.Point(43, 290);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(75, 30);
            this.btn_add.TabIndex = 9;
            this.btn_add.Text = "ADD";
            this.btn_add.UseVisualStyleBackColor = false;
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // btn_edit
            // 
            this.btn_edit.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_edit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_edit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_edit.ForeColor = System.Drawing.Color.White;
            this.btn_edit.Location = new System.Drawing.Point(125, 290);
            this.btn_edit.Name = "btn_edit";
            this.btn_edit.Size = new System.Drawing.Size(75, 30);
            this.btn_edit.TabIndex = 10;
            this.btn_edit.Text = "EDIT";
            this.btn_edit.UseVisualStyleBackColor = false;
            this.btn_edit.Click += new System.EventHandler(this.btn_edit_Click);
            // 
            // btn_delete
            // 
            this.btn_delete.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_delete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_delete.ForeColor = System.Drawing.Color.White;
            this.btn_delete.Location = new System.Drawing.Point(207, 290);
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.Size = new System.Drawing.Size(75, 30);
            this.btn_delete.TabIndex = 11;
            this.btn_delete.Text = "DELETE";
            this.btn_delete.UseVisualStyleBackColor = false;
            this.btn_delete.Click += new System.EventHandler(this.btn_delete_Click);
            // 
            // 
            // 
            // 
            // btn_clear
            // 
            this.btn_clear.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_clear.ForeColor = System.Drawing.Color.White;
            this.btn_clear.Location = new System.Drawing.Point(40, 335);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(235, 34);
            this.btn_clear.TabIndex = 13;
            this.btn_clear.Text = "SEÇİMİ TEMİZLE";
            this.btn_clear.UseVisualStyleBackColor = false;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_home
            // 
            this.btn_home.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_home.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_home.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_home.ForeColor = System.Drawing.Color.White;
            this.btn_home.Location = new System.Drawing.Point(40, 380);
            this.btn_home.Name = "btn_home";
            this.btn_home.Size = new System.Drawing.Size(235, 34);
            this.btn_home.TabIndex = 12;
            this.btn_home.Text = "◀ ANA MENÜ";
            this.btn_home.UseVisualStyleBackColor = false;
            this.btn_home.Click += new System.EventHandler(this.btn_home_Click);
            // 
            // dataGridViewCategories
            // 
            this.dataGridViewCategories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCategories.Location = new System.Drawing.Point(340, 120);
            this.dataGridViewCategories.Name = "dataGridViewCategories";
            this.dataGridViewCategories.Size = new System.Drawing.Size(514, 395);
            this.dataGridViewCategories.TabIndex = 14;
            this.dataGridViewCategories.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCategories_CellClick);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 533);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(880, 37);
            this.panel2.TabIndex = 15;
            // 
            // ManageCategories
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(880, 570);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dataGridViewCategories);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.btn_home);
            this.Controls.Add(this.btn_delete);
            this.Controls.Add(this.btn_edit);
            this.Controls.Add(this.btn_add);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_catname);
            this.Controls.Add(this.txt_catid);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ManageCategories";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ManageCategories";
            this.Load += new System.EventHandler(this.ManageCategories_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCategories)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_catid;
        private System.Windows.Forms.TextBox txt_catname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Button btn_edit;
        private System.Windows.Forms.Button btn_delete;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Button btn_home;
        private System.Windows.Forms.DataGridView dataGridViewCategories;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Button btn_back;
    }
}
```

---

### Dosya: `ManageCustomers.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageCustomers.cs`
- **Dosya Boyutu:** 7.83 KB
- **Satır Sayısı:** 197

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageCustomers : Form
    {
        public ManageCustomers()
        {
            InitializeComponent();
        }

        private void ManageCustomers_Load(object sender, EventArgs e)
        {
            FormatGrid();
            PopulateCustomers();
            ClearInputs();
        }

        private void FormatGrid()
        {
            dataGridViewCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCustomers.MultiSelect = false;
            dataGridViewCustomers.ReadOnly = true;
            dataGridViewCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCustomers.AllowUserToAddRows = false;
        }

        private void PopulateCustomers()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT CustId AS [Müşteri ID], CustName AS [Müşteri Adı], CustPhone AS [Telefon] FROM CustomerTable ORDER BY CustId ASC");
                dataGridViewCustomers.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteriler listelenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txt_custid.Text = string.Empty;
            txt_custname.Text = string.Empty;
            txt_custphone.Text = string.Empty;
            dataGridViewCustomers.ClearSelection();
            txt_custname.Focus();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearInputs();
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

        private void btn_home_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_custname.Text) || string.IsNullOrWhiteSpace(txt_custphone.Text))
            {
                MessageBox.Show("Lütfen müşteri adı ve telefon alanlarını doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO CustomerTable (CustName, CustPhone) VALUES (@name, @phone)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@name", txt_custname.Text.Trim()),
                    new SqlParameter("@phone", txt_custphone.Text.Trim())
                };

                DbHelper.ExecuteNonQuery(query, parameters);
                DbHelper.LogAction("Müşteri Eklendi", "Müşteri Yönetimi", $"Müşteri Adı: '{txt_custname.Text.Trim()}', Telefon: '{txt_custphone.Text.Trim()}'");
                MessageBox.Show("Müşteri başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateCustomers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_custid.Text))
            {
                MessageBox.Show("Lütfen güncellenecek müşteriyi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_custname.Text) || string.IsNullOrWhiteSpace(txt_custphone.Text))
            {
                MessageBox.Show("Müşteri adı ve telefon alanları boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "UPDATE CustomerTable SET CustName = @name, CustPhone = @phone WHERE CustId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@name", txt_custname.Text.Trim()),
                    new SqlParameter("@phone", txt_custphone.Text.Trim()),
                    new SqlParameter("@id", Convert.ToInt32(txt_custid.Text))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    DbHelper.LogAction("Müşteri Güncellendi", "Müşteri Yönetimi", $"ID: {txt_custid.Text}, Yeni Ad: '{txt_custname.Text.Trim()}', Yeni Tel: '{txt_custphone.Text.Trim()}'");
                    MessageBox.Show("Müşteri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateCustomers();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri güncellenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_custid.Text))
            {
                MessageBox.Show("Lütfen silinecek müşteriyi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string deletedName = txt_custname.Text.Trim();
            string deletedId = txt_custid.Text.Trim();

            DialogResult confirm = MessageBox.Show($"'{deletedName}' müşterisini silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM CustomerTable WHERE CustId = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@id", Convert.ToInt32(deletedId))
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    DbHelper.LogAction("Müşteri Silindi", "Müşteri Yönetimi", $"ID: {deletedId}, Müşteri: '{deletedName}' silindi.");
                    MessageBox.Show("Müşteri başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateCustomers();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri silinirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewCustomers.Rows.Count)
            {
                DataGridViewRow row = dataGridViewCustomers.Rows[e.RowIndex];
                txt_custid.Text = row.Cells[0].Value?.ToString() ?? "";
                txt_custname.Text = row.Cells[1].Value?.ToString() ?? "";
                txt_custphone.Text = row.Cells[2].Value?.ToString() ?? "";
            }
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            string fileName = $"Musteri_Listesi_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            DbHelper.ExportToCsv(dataGridViewCustomers, fileName);
        }
    }
}
```

---

### Dosya: `ManageCustomers.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageCustomers.Designer.cs`
- **Dosya Boyutu:** 15.53 KB
- **Satır Sayısı:** 300

```csharp
namespace Inventory_Management_System
{
    partial class ManageCustomers
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
            this.txt_custid = new System.Windows.Forms.TextBox();
            this.txt_custname = new System.Windows.Forms.TextBox();
            this.txt_custphone = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_add = new System.Windows.Forms.Button();
            this.btn_edit = new System.Windows.Forms.Button();
            this.btn_delete = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_home = new System.Windows.Forms.Button();
            this.btn_export = new System.Windows.Forms.Button();
            this.dataGridViewCustomers = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCustomers)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel1.Controls.Add(this.btn_back);
            this.panel1.Controls.Add(this.btn_exit);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(880, 100);
            this.panel1.TabIndex = 0;
            // 
            // btn_back
            // 
            this.btn_back.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_back.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_back.ForeColor = System.Drawing.Color.White;
            this.btn_back.Location = new System.Drawing.Point(15, 25);
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
            this.btn_exit.Location = new System.Drawing.Point(818, 12);
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
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(260, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(359, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "MANAGE CUSTOMERS";
            // 
            // txt_custid
            // 
            this.txt_custid.Location = new System.Drawing.Point(167, 160);
            this.txt_custid.Name = "txt_custid";
            this.txt_custid.ReadOnly = true;
            this.txt_custid.Size = new System.Drawing.Size(120, 20);
            this.txt_custid.TabIndex = 1;
            // 
            // txt_custname
            // 
            this.txt_custname.Location = new System.Drawing.Point(167, 200);
            this.txt_custname.Name = "txt_custname";
            this.txt_custname.Size = new System.Drawing.Size(120, 20);
            this.txt_custname.TabIndex = 2;
            // 
            // txt_custphone
            // 
            this.txt_custphone.Location = new System.Drawing.Point(167, 240);
            this.txt_custphone.Name = "txt_custphone";
            this.txt_custphone.Size = new System.Drawing.Size(120, 20);
            this.txt_custphone.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(40, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Müşteri ID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(40, 201);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Müşteri Adı:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(40, 241);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Telefon:";
            // 
            // btn_add
            // 
            this.btn_add.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_add.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_add.ForeColor = System.Drawing.Color.White;
            this.btn_add.Location = new System.Drawing.Point(43, 290);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(75, 30);
            this.btn_add.TabIndex = 9;
            this.btn_add.Text = "ADD";
            this.btn_add.UseVisualStyleBackColor = false;
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // btn_edit
            // 
            this.btn_edit.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_edit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_edit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_edit.ForeColor = System.Drawing.Color.White;
            this.btn_edit.Location = new System.Drawing.Point(125, 290);
            this.btn_edit.Name = "btn_edit";
            this.btn_edit.Size = new System.Drawing.Size(75, 30);
            this.btn_edit.TabIndex = 10;
            this.btn_edit.Text = "EDIT";
            this.btn_edit.UseVisualStyleBackColor = false;
            this.btn_edit.Click += new System.EventHandler(this.btn_edit_Click);
            // 
            // btn_delete
            // 
            this.btn_delete.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_delete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_delete.ForeColor = System.Drawing.Color.White;
            this.btn_delete.Location = new System.Drawing.Point(207, 290);
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.Size = new System.Drawing.Size(75, 30);
            this.btn_delete.TabIndex = 11;
            this.btn_delete.Text = "DELETE";
            this.btn_delete.UseVisualStyleBackColor = false;
            this.btn_delete.Click += new System.EventHandler(this.btn_delete_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_clear.ForeColor = System.Drawing.Color.White;
            this.btn_clear.Location = new System.Drawing.Point(43, 335);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(239, 34);
            this.btn_clear.TabIndex = 13;
            this.btn_clear.Text = "SEÇİMİ TEMİZLE";
            this.btn_clear.UseVisualStyleBackColor = false;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_home
            // 
            this.btn_home.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_home.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_home.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_home.ForeColor = System.Drawing.Color.White;
            this.btn_home.Location = new System.Drawing.Point(43, 380);
            this.btn_home.Name = "btn_home";
            this.btn_home.Size = new System.Drawing.Size(239, 34);
            this.btn_home.TabIndex = 12;
            this.btn_home.Text = "◀ ANA MENÜ";
            this.btn_home.UseVisualStyleBackColor = false;
            this.btn_home.Click += new System.EventHandler(this.btn_home_Click);
            // 
            // btn_export
            // 
            this.btn_export.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btn_export.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_export.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_export.ForeColor = System.Drawing.Color.White;
            this.btn_export.Location = new System.Drawing.Point(43, 425);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(239, 34);
            this.btn_export.TabIndex = 16;
            this.btn_export.Text = "📊 EXCEL\'E AKTAR";
            this.btn_export.UseVisualStyleBackColor = false;
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            // 
            // dataGridViewCustomers
            // 
            this.dataGridViewCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCustomers.Location = new System.Drawing.Point(340, 120);
            this.dataGridViewCustomers.Name = "dataGridViewCustomers";
            this.dataGridViewCustomers.Size = new System.Drawing.Size(514, 395);
            this.dataGridViewCustomers.TabIndex = 14;
            this.dataGridViewCustomers.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCustomers_CellClick);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 533);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(880, 37);
            this.panel2.TabIndex = 15;
            // 
            // ManageCustomers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(880, 570);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dataGridViewCustomers);
            this.Controls.Add(this.btn_export);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.btn_home);
            this.Controls.Add(this.btn_delete);
            this.Controls.Add(this.btn_edit);
            this.Controls.Add(this.btn_add);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_custphone);
            this.Controls.Add(this.txt_custname);
            this.Controls.Add(this.txt_custid);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ManageCustomers";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ManageCustomers";
            this.Load += new System.EventHandler(this.ManageCustomers_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCustomers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_custid;
        private System.Windows.Forms.TextBox txt_custname;
        private System.Windows.Forms.TextBox txt_custphone;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Button btn_edit;
        private System.Windows.Forms.Button btn_delete;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Button btn_home;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.DataGridView dataGridViewCustomers;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Button btn_back;
    }
}
```

---

### Dosya: `ManageOrders.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageOrders.cs`
- **Dosya Boyutu:** 19.67 KB
- **Satır Sayısı:** 450

```csharp
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
        private string selectedProdBarcode = "";
        private string selectedProdName = "";
        private decimal selectedProdPrice = 0;
        private int selectedProdStock = 0;
        private decimal selectedProdCost = 0;
        private decimal grandTotal = 0;

        public ManageOrders()
        {
            InitializeComponent();
        }

        private void ManageOrders_Load(object sender, EventArgs e)
        {
            if (CurrentUser.IsWarehouse)
            {
                MessageBox.Show("Depo görevlileri sipariş oluşturma ekranına erişemez.", "Yetki Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                HomeForm home = new HomeForm();
                home.Show();
                this.Close();
                return;
            }

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
            cartTable.Columns.Add("Barkod", typeof(string));
            cartTable.Columns.Add("Ürün Adı", typeof(string));
            cartTable.Columns.Add("Miktar", typeof(int));
            cartTable.Columns.Add("Birim Fiyat", typeof(decimal));
            cartTable.Columns.Add("Toplam", typeof(decimal));
            cartTable.Columns.Add("Maliyet", typeof(decimal));

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

            if (dataGridViewCart.Columns.Contains("Maliyet"))
                dataGridViewCart.Columns["Maliyet"].Visible = false;
            if (dataGridViewCart.Columns.Contains("Birim Fiyat"))
                dataGridViewCart.Columns["Birim Fiyat"].DefaultCellStyle.Format = "N2";
            if (dataGridViewCart.Columns.Contains("Toplam"))
                dataGridViewCart.Columns["Toplam"].DefaultCellStyle.Format = "N2";
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
                DataTable dt = DbHelper.ExecuteQuery("SELECT ProdId AS [ID], ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], ProdQty AS [Stok], ProdPrice AS [Fiyat], ProdCostPrice AS [Maliyet], ProdCat AS [Kategori] FROM ProductTable ORDER BY ProdId ASC");
                dataGridViewProducts.DataSource = dt;
                if (dataGridViewProducts.Columns.Contains("Maliyet"))
                    dataGridViewProducts.Columns["Maliyet"].Visible = false;
                if (dataGridViewProducts.Columns.Contains("Fiyat"))
                    dataGridViewProducts.Columns["Fiyat"].DefaultCellStyle.Format = "N2";
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
            selectedProdBarcode = "";
            selectedProdName = "";
            selectedProdPrice = 0;
            selectedProdStock = 0;
            selectedProdCost = 0;
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
                selectedProdBarcode = row.Cells[1].Value?.ToString() ?? "";
                selectedProdName = row.Cells[2].Value?.ToString() ?? "";
                selectedProdStock = Convert.ToInt32(row.Cells[3].Value);
                selectedProdPrice = Convert.ToDecimal(row.Cells[4].Value);
                selectedProdCost = row.Cells[5].Value != DBNull.Value ? Convert.ToDecimal(row.Cells[5].Value) : 0m;
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
                cartTable.Rows.Add(selectedProdId, selectedProdBarcode, selectedProdName, qty, selectedProdPrice, total, selectedProdCost);
            }

            CalculateTotal();
            txt_qty.Text = string.Empty;
        }

        private void AddProductByBarcode(string barcodeText)
        {
            if (string.IsNullOrWhiteSpace(barcodeText))
                return;

            barcodeText = barcodeText.Trim();
            try
            {
                string query = "SELECT ProdId, ProdBarcode, ProdName, ProdQty, ProdPrice, ProdCostPrice FROM ProductTable WHERE ProdBarcode = @code OR CAST(ProdId AS NVARCHAR) = @code";
                DataTable dt = DbHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@code", barcodeText) });

                if (dt == null || dt.Rows.Count == 0)
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show($"'{barcodeText}' barkodlu/kodlu ürün sistemde bulunamadı!", "Ürün Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txt_barcode.SelectAll();
                    txt_barcode.Focus();
                    return;
                }

                DataRow row = dt.Rows[0];
                int prodId = Convert.ToInt32(row["ProdId"]);
                string barcode = row["ProdBarcode"]?.ToString() ?? "";
                string prodName = row["ProdName"]?.ToString() ?? "";
                int stock = Convert.ToInt32(row["ProdQty"]);
                decimal price = Convert.ToDecimal(row["ProdPrice"]);
                decimal cost = (row["ProdCostPrice"] != DBNull.Value) ? Convert.ToDecimal(row["ProdCostPrice"]) : 0m;

                if (stock <= 0)
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show($"'{prodName}' ürününün stoğu tükenmiştir! (Mevcut Stok: 0)", "Stok Yetersiz", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txt_barcode.SelectAll();
                    txt_barcode.Focus();
                    return;
                }

                // Check if already in cart
                bool alreadyInCart = false;
                foreach (DataRow cartRow in cartTable.Rows)
                {
                    if (Convert.ToInt32(cartRow["Ürün ID"]) == prodId)
                    {
                        int currentQty = Convert.ToInt32(cartRow["Miktar"]);
                        if (currentQty + 1 > stock)
                        {
                            System.Media.SystemSounds.Beep.Play();
                            MessageBox.Show($"'{prodName}' için sepetteki adet mevcut stoku ({stock}) aşıyor!", "Stok Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txt_barcode.SelectAll();
                            txt_barcode.Focus();
                            return;
                        }
                        cartRow["Miktar"] = currentQty + 1;
                        cartRow["Toplam"] = (currentQty + 1) * price;
                        alreadyInCart = true;
                        break;
                    }
                }

                if (!alreadyInCart)
                {
                    cartTable.Rows.Add(prodId, barcode, prodName, 1, price, price, cost);
                }

                CalculateTotal();
                System.Media.SystemSounds.Asterisk.Play();

                // Select row in dataGridViewProducts if visible
                foreach (DataGridViewRow dgvRow in dataGridViewProducts.Rows)
                {
                    if (Convert.ToInt32(dgvRow.Cells[0].Value) == prodId)
                    {
                        dgvRow.Selected = true;
                        dataGridViewProducts.FirstDisplayedScrollingRowIndex = dgvRow.Index;
                        break;
                    }
                }

                txt_barcode.Clear();
                txt_barcode.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Barkod ile ürün eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txt_barcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddProductByBarcode(txt_barcode.Text);
            }
        }

        private void btn_barcode_add_Click(object sender, EventArgs e)
        {
            AddProductByBarcode(txt_barcode.Text);
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
                            string barcode = (row.Table.Columns.Contains("Barkod") && row["Barkod"] != DBNull.Value) ? row["Barkod"].ToString() : "";
                            string prodName = row["Ürün Adı"].ToString();
                            int qty = Convert.ToInt32(row["Miktar"]);
                            decimal price = Convert.ToDecimal(row["Birim Fiyat"]);
                            decimal total = Convert.ToDecimal(row["Toplam"]);
                            decimal cost = (row.Table.Columns.Contains("Maliyet") && row["Maliyet"] != DBNull.Value) ? Convert.ToDecimal(row["Maliyet"]) : 0m;

                            string detailQuery = "INSERT INTO OrderDetailTable (OrderId, ProdId, ProdBarcode, ProdName, Qty, Price, Total, CostPrice) VALUES (@orderid, @prodid, @barcode, @prodname, @qty, @price, @total, @cost)";
                            using (SqlCommand cmd = new SqlCommand(detailQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@orderid", orderId);
                                cmd.Parameters.AddWithValue("@prodid", prodId);
                                cmd.Parameters.AddWithValue("@barcode", barcode);
                                cmd.Parameters.AddWithValue("@prodname", prodName);
                                cmd.Parameters.AddWithValue("@qty", qty);
                                cmd.Parameters.AddWithValue("@price", price);
                                cmd.Parameters.AddWithValue("@total", total);
                                cmd.Parameters.AddWithValue("@cost", cost);
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

                        DbHelper.LogAction("Sipariş Oluşturuldu", "Sipariş Yönetimi", $"Sipariş No: #{orderId}, Müşteri: '{txt_custname.Text.Trim()}', Kalem Sayısı: {cartTable.Rows.Count}, Toplam Tutar: {grandTotal:N2} TL");

                        DialogResult printChoice = MessageBox.Show(
                            $"#{orderId} numaralı sipariş başarıyla tamamlandı!\nToplam Tutar: {grandTotal:N2} TL\n\nSatış fişini görüntülemek ve yazdırmak ister misiniz?",
                            "Sipariş Tamamlandı",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information
                        );

                        if (printChoice == DialogResult.Yes)
                        {
                            DataTable printItems = cartTable.Copy();
                            string custName = txt_custname.Text.Trim();
                            ReceiptPrinter.ShowReceipt(orderId, custName, DateTime.Now, "Tamamlandı", grandTotal, printItems);
                        }

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
```

---

### Dosya: `ManageOrders.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageOrders.Designer.cs`
- **Dosya Boyutu:** 23.85 KB
- **Satır Sayısı:** 436

```csharp
namespace Inventory_Management_System
{
    partial class ManageOrders
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
            this.dataGridViewCustomers = new System.Windows.Forms.DataGridView();
            this.dataGridViewProducts = new System.Windows.Forms.DataGridView();
            this.dataGridViewCart = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_custid = new System.Windows.Forms.TextBox();
            this.txt_custname = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_qty = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_add_to_cart = new System.Windows.Forms.Button();
            this.lbl_total = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_insert_order = new System.Windows.Forms.Button();
            this.btn_home = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_clear_cart = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_view_orders = new System.Windows.Forms.Button();
            this.lbl_barcode = new System.Windows.Forms.Label();
            this.txt_barcode = new System.Windows.Forms.TextBox();
            this.btn_barcode_add = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCustomers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProducts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCart)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel1.Controls.Add(this.btn_view_orders);
            this.panel1.Controls.Add(this.btn_back);
            this.panel1.Controls.Add(this.btn_exit);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(980, 80);
            this.panel1.TabIndex = 0;
            // 
            // btn_back
            // 
            this.btn_back.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_back.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_back.ForeColor = System.Drawing.Color.White;
            this.btn_back.Location = new System.Drawing.Point(15, 18);
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
            this.btn_exit.Location = new System.Drawing.Point(920, 12);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(45, 45);
            this.btn_exit.TabIndex = 15;
            this.btn_exit.Text = "X";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // btn_view_orders
            // 
            this.btn_view_orders.BackColor = System.Drawing.Color.White;
            this.btn_view_orders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_view_orders.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_view_orders.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.btn_view_orders.Location = new System.Drawing.Point(730, 18);
            this.btn_view_orders.Name = "btn_view_orders";
            this.btn_view_orders.Size = new System.Drawing.Size(170, 45);
            this.btn_view_orders.TabIndex = 17;
            this.btn_view_orders.Text = "📋 SİPARİŞ LİSTESİ";
            this.btn_view_orders.UseVisualStyleBackColor = false;
            this.btn_view_orders.Click += new System.EventHandler(this.btn_view_orders_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(340, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(306, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "MANAGE ORDERS";
            // 
            // dataGridViewCustomers
            // 
            this.dataGridViewCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCustomers.Location = new System.Drawing.Point(20, 115);
            this.dataGridViewCustomers.Name = "dataGridViewCustomers";
            this.dataGridViewCustomers.Size = new System.Drawing.Size(440, 150);
            this.dataGridViewCustomers.TabIndex = 1;
            this.dataGridViewCustomers.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCustomers_CellClick);
            // 
            // dataGridViewProducts
            // 
            this.dataGridViewProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewProducts.Location = new System.Drawing.Point(490, 115);
            this.dataGridViewProducts.Name = "dataGridViewProducts";
            this.dataGridViewProducts.Size = new System.Drawing.Size(470, 150);
            this.dataGridViewProducts.TabIndex = 2;
            this.dataGridViewProducts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewProducts_CellClick);
            // 
            // dataGridViewCart
            // 
            this.dataGridViewCart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCart.Location = new System.Drawing.Point(20, 380);
            this.dataGridViewCart.Name = "dataGridViewCart";
            this.dataGridViewCart.Size = new System.Drawing.Size(650, 180);
            this.dataGridViewCart.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(20, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Müşteri Seçiniz";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(490, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Ürün Seçiniz";
            // 
            // lbl_barcode
            // 
            this.lbl_barcode.AutoSize = true;
            this.lbl_barcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_barcode.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lbl_barcode.Location = new System.Drawing.Point(615, 93);
            this.lbl_barcode.Name = "lbl_barcode";
            this.lbl_barcode.Size = new System.Drawing.Size(107, 13);
            this.lbl_barcode.TabIndex = 22;
            this.lbl_barcode.Text = "⚡ Barkod (Enter):";
            // 
            // txt_barcode
            // 
            this.txt_barcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txt_barcode.Location = new System.Drawing.Point(728, 89);
            this.txt_barcode.Name = "txt_barcode";
            this.txt_barcode.Size = new System.Drawing.Size(135, 21);
            this.txt_barcode.TabIndex = 23;
            this.txt_barcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_barcode_KeyDown);
            // 
            // btn_barcode_add
            // 
            this.btn_barcode_add.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btn_barcode_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_barcode_add.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_barcode_add.ForeColor = System.Drawing.Color.White;
            this.btn_barcode_add.Location = new System.Drawing.Point(868, 87);
            this.btn_barcode_add.Name = "btn_barcode_add";
            this.btn_barcode_add.Size = new System.Drawing.Size(92, 25);
            this.btn_barcode_add.TabIndex = 24;
            this.btn_barcode_add.Text = "+ Sepete Ekle";
            this.btn_barcode_add.UseVisualStyleBackColor = false;
            this.btn_barcode_add.Click += new System.EventHandler(this.btn_barcode_add_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(20, 355);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 18);
            this.label4.TabIndex = 6;
            this.label4.Text = "Sipariş Sepeti";
            // 
            // txt_custid
            // 
            this.txt_custid.Location = new System.Drawing.Point(120, 275);
            this.txt_custid.Name = "txt_custid";
            this.txt_custid.ReadOnly = true;
            this.txt_custid.Size = new System.Drawing.Size(100, 20);
            this.txt_custid.TabIndex = 7;
            // 
            // txt_custname
            // 
            this.txt_custname.Location = new System.Drawing.Point(120, 305);
            this.txt_custname.Name = "txt_custname";
            this.txt_custname.ReadOnly = true;
            this.txt_custname.Size = new System.Drawing.Size(150, 20);
            this.txt_custname.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.Location = new System.Drawing.Point(20, 278);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Müşteri ID:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label6.Location = new System.Drawing.Point(20, 308);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Müşteri Adı:";
            // 
            // txt_qty
            // 
            this.txt_qty.Location = new System.Drawing.Point(590, 275);
            this.txt_qty.Name = "txt_qty";
            this.txt_qty.Size = new System.Drawing.Size(80, 20);
            this.txt_qty.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label7.Location = new System.Drawing.Point(490, 278);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Sipariş Adedi:";
            // 
            // btn_add_to_cart
            // 
            this.btn_add_to_cart.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_add_to_cart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_add_to_cart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_add_to_cart.ForeColor = System.Drawing.Color.White;
            this.btn_add_to_cart.Location = new System.Drawing.Point(690, 271);
            this.btn_add_to_cart.Name = "btn_add_to_cart";
            this.btn_add_to_cart.Size = new System.Drawing.Size(120, 28);
            this.btn_add_to_cart.TabIndex = 13;
            this.btn_add_to_cart.Text = "Sepete Ekle";
            this.btn_add_to_cart.UseVisualStyleBackColor = false;
            this.btn_add_to_cart.Click += new System.EventHandler(this.btn_add_to_cart_Click);
            // 
            // lbl_total
            // 
            this.lbl_total.AutoSize = true;
            this.lbl_total.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_total.ForeColor = System.Drawing.Color.DarkRed;
            this.lbl_total.Location = new System.Drawing.Point(700, 420);
            this.lbl_total.Name = "lbl_total";
            this.lbl_total.Size = new System.Drawing.Size(73, 31);
            this.lbl_total.TabIndex = 14;
            this.lbl_total.Text = "0 TL";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label9.Location = new System.Drawing.Point(700, 390);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 18);
            this.label9.TabIndex = 15;
            this.label9.Text = "Toplam Tutar:";
            // 
            // btn_insert_order
            // 
            this.btn_insert_order.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btn_insert_order.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_insert_order.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_insert_order.ForeColor = System.Drawing.Color.White;
            this.btn_insert_order.Location = new System.Drawing.Point(700, 470);
            this.btn_insert_order.Name = "btn_insert_order";
            this.btn_insert_order.Size = new System.Drawing.Size(240, 40);
            this.btn_insert_order.TabIndex = 16;
            this.btn_insert_order.Text = "Siparişi Tamamla";
            this.btn_insert_order.UseVisualStyleBackColor = false;
            this.btn_insert_order.Click += new System.EventHandler(this.btn_insert_order_Click);
            // 
            // btn_home
            // 
            this.btn_home.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_home.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_home.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_home.ForeColor = System.Drawing.Color.White;
            this.btn_home.Location = new System.Drawing.Point(700, 520);
            this.btn_home.Name = "btn_home";
            this.btn_home.Size = new System.Drawing.Size(240, 35);
            this.btn_home.TabIndex = 17;
            this.btn_home.Text = "◀ GERİ (ANA MENÜ)";
            this.btn_home.UseVisualStyleBackColor = false;
            this.btn_home.Click += new System.EventHandler(this.btn_home_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 580);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(980, 30);
            this.panel2.TabIndex = 18;
            // 
            // btn_clear_cart
            // 
            this.btn_clear_cart.BackColor = System.Drawing.Color.Crimson;
            this.btn_clear_cart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_clear_cart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_clear_cart.ForeColor = System.Drawing.Color.White;
            this.btn_clear_cart.Location = new System.Drawing.Point(550, 350);
            this.btn_clear_cart.Name = "btn_clear_cart";
            this.btn_clear_cart.Size = new System.Drawing.Size(120, 25);
            this.btn_clear_cart.TabIndex = 19;
            this.btn_clear_cart.Text = "Sepeti Temizle";
            this.btn_clear_cart.UseVisualStyleBackColor = false;
            this.btn_clear_cart.Click += new System.EventHandler(this.btn_clear_cart_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_clear.ForeColor = System.Drawing.Color.White;
            this.btn_clear.Location = new System.Drawing.Point(285, 275);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(185, 45);
            this.btn_clear.TabIndex = 20;
            this.btn_clear.Text = "SEÇİMİ TEMİZLE";
            this.btn_clear.UseVisualStyleBackColor = false;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // ManageOrders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.Controls.Add(this.btn_barcode_add);
            this.Controls.Add(this.txt_barcode);
            this.Controls.Add(this.lbl_barcode);
            this.Controls.Add(this.btn_clear_cart);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btn_home);
            this.Controls.Add(this.btn_insert_order);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lbl_total);
            this.Controls.Add(this.btn_add_to_cart);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txt_qty);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt_custname);
            this.Controls.Add(this.txt_custid);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridViewCart);
            this.Controls.Add(this.dataGridViewProducts);
            this.Controls.Add(this.dataGridViewCustomers);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ManageOrders";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ManageOrders";
            this.Load += new System.EventHandler(this.ManageOrders_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCustomers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProducts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.DataGridView dataGridViewCustomers;
        private System.Windows.Forms.DataGridView dataGridViewProducts;
        private System.Windows.Forms.DataGridView dataGridViewCart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_barcode;
        private System.Windows.Forms.TextBox txt_barcode;
        private System.Windows.Forms.Button btn_barcode_add;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_custid;
        private System.Windows.Forms.TextBox txt_custname;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_qty;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_add_to_cart;
        private System.Windows.Forms.Label lbl_total;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_insert_order;
        private System.Windows.Forms.Button btn_home;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_clear_cart;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Button btn_view_orders;
        private System.Windows.Forms.Button btn_back;
    }
}
```

---

### Dosya: `ViewOrders.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ViewOrders.cs`
- **Dosya Boyutu:** 16.37 KB
- **Satır Sayısı:** 375

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ViewOrders : Form
    {
        private int selectedOrderId = 0;
        private string selectedOrderStatus = string.Empty;

        public ViewOrders()
        {
            InitializeComponent();
        }

        private void ViewOrders_Load(object sender, EventArgs e)
        {
            if (CurrentUser.IsWarehouse)
            {
                MessageBox.Show("Depo görevlileri sipariş görüntüleme ekranına erişemez.", "Yetki Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                HomeForm home = new HomeForm();
                home.Show();
                this.Close();
                return;
            }

            if (!CurrentUser.IsAdmin)
            {
                btn_return.Enabled = false;
            }

            FormatGrids();
            PopulateOrders();
        }

        private void FormatGrids()
        {
            // Orders Grid
            dataGridViewOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOrders.MultiSelect = false;
            dataGridViewOrders.ReadOnly = true;
            dataGridViewOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewOrders.AllowUserToAddRows = false;

            // Details Grid
            dataGridViewOrderDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOrderDetails.MultiSelect = false;
            dataGridViewOrderDetails.ReadOnly = true;
            dataGridViewOrderDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewOrderDetails.AllowUserToAddRows = false;
        }

        private void PopulateOrders()
        {
            try
            {
                string query = "SELECT OrderId AS [Sipariş No], CustId AS [Müşteri ID], CustName AS [Müşteri Adı], OrderDate AS [Sipariş Tarihi], TotalAmt AS [Toplam Tutar (TL)], OrderStatus AS [Durum] FROM OrderTable ORDER BY OrderId ASC";
                DataTable dt = DbHelper.ExecuteQuery(query);
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

        private void LoadOrderDetails(int orderId)
        {
            try
            {
                string query = "SELECT ProdBarcode AS [Barkod], ProdName AS [Ürün Adı], Qty AS [Adet], CostPrice AS [Birim Maliyet (TL)], Price AS [Satış Fiyatı (TL)], Total AS [Toplam Tutar (TL)], ((Price - CostPrice) * Qty) AS [Net Kâr (TL)] FROM OrderDetailTable WHERE OrderId = @orderid ORDER BY DetailId ASC";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@orderid", orderId)
                };

                DataTable dt = DbHelper.ExecuteQuery(query, parameters);
                dataGridViewOrderDetails.DataSource = dt;

                if (dataGridViewOrderDetails.Columns["Birim Maliyet (TL)"] != null)
                {
                    dataGridViewOrderDetails.Columns["Birim Maliyet (TL)"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridViewOrderDetails.Columns["Satış Fiyatı (TL)"] != null)
                {
                    dataGridViewOrderDetails.Columns["Satış Fiyatı (TL)"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridViewOrderDetails.Columns["Toplam Tutar (TL)"] != null)
                {
                    dataGridViewOrderDetails.Columns["Toplam Tutar (TL)"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridViewOrderDetails.Columns["Net Kâr (TL)"] != null)
                {
                    dataGridViewOrderDetails.Columns["Net Kâr (TL)"].DefaultCellStyle.Format = "N2";
                }

                dataGridViewOrderDetails.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sipariş detayları yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearSelection()
        {
            selectedOrderId = 0;
            selectedOrderStatus = string.Empty;
            dataGridViewOrders.ClearSelection();
            dataGridViewOrderDetails.DataSource = null;
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
                decimal total = Convert.ToDecimal(row.Cells[4].Value);
                selectedOrderStatus = row.Cells[5].Value?.ToString() ?? "Tamamlandı";

                decimal cost = 0;
                try
                {
                    object costObj = DbHelper.ExecuteScalar("SELECT ISNULL(SUM(CostPrice * Qty), 0) FROM OrderDetailTable WHERE OrderId = @id", new SqlParameter[] { new SqlParameter("@id", selectedOrderId) });
                    if (costObj != null && costObj != DBNull.Value)
                        cost = Convert.ToDecimal(costObj);
                }
                catch { }

                if (CurrentUser.IsAdmin)
                {
                    decimal netProfit = total - cost;
                    decimal margin = total > 0 ? (netProfit / total * 100m) : 0m;
                    lbl_summary.Text = $"Seçili Sipariş: #{selectedOrderId} | Müşteri: {custName} | Tarih: {orderDate} | Tutar: {total:N2} TL | Maliyet: {cost:N2} TL | Net Kâr: {netProfit:N2} TL (%{margin:F1}) | Durum: {selectedOrderStatus}";
                }
                else
                {
                    lbl_summary.Text = $"Seçili Sipariş: #{selectedOrderId} | Müşteri: {custName} | Tarih: {orderDate} | Tutar: {total:N2} TL | Durum: {selectedOrderStatus}";
                }

                LoadOrderDetails(selectedOrderId);
            }
        }

        private void dataGridViewOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewOrders.Rows.Count)
            {
                DataGridViewRow row = dataGridViewOrders.Rows[e.RowIndex];
                string status = row.Cells["Durum"]?.Value?.ToString();

                if (status == "İptal / İade Edildi")
                {
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    row.DefaultCellStyle.SelectionBackColor = Color.IndianRed;
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
            }
        }

        private void btn_return_Click(object sender, EventArgs e)
        {
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Sipariş iptal ve iade işlemleri yalnızca Yönetici (Admin) yetkisiyle gerçekleştirilebilir.", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedOrderId == 0)
            {
                MessageBox.Show("Lütfen iade edilecek siparişi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedOrderStatus == "İptal / İade Edildi")
            {
                MessageBox.Show("Bu sipariş zaten iade/iptal edilmiştir. Tekrar iade işlemi yapılamaz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"#{selectedOrderId} numaralı siparişi İADE / İPTAL etmek istediğinize emin misiniz?\n\n" +
                "Bu işlem sonucunda:\n" +
                "• Siparişteki tüm ürünler otomatik olarak depoya/stoğa geri yüklenecektir.\n" +
                "• Sipariş durumu 'İptal / İade Edildi' olarak güncellenecektir.",
                "Sipariş İade Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(DbHelper.ConnectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Get ordered items
                        string getItemsQuery = "SELECT ProdId, Qty FROM OrderDetailTable WHERE OrderId = @orderid";
                        DataTable dtItems = new DataTable();
                        using (SqlCommand cmd = new SqlCommand(getItemsQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@orderid", selectedOrderId);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dtItems);
                            }
                        }

                        // 2. Restore stock in ProductTable
                        foreach (DataRow row in dtItems.Rows)
                        {
                            int prodId = Convert.ToInt32(row["ProdId"]);
                            int qty = Convert.ToInt32(row["Qty"]);

                            string restoreStockQuery = "UPDATE ProductTable SET ProdQty = ProdQty + @qty WHERE ProdId = @id";
                            using (SqlCommand cmd = new SqlCommand(restoreStockQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@qty", qty);
                                cmd.Parameters.AddWithValue("@id", prodId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3. Update OrderStatus in OrderTable
                        string updateOrderQuery = "UPDATE OrderTable SET OrderStatus = @status WHERE OrderId = @orderid";
                        using (SqlCommand cmd = new SqlCommand(updateOrderQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@status", "İptal / İade Edildi");
                            cmd.Parameters.AddWithValue("@orderid", selectedOrderId);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();

                        DbHelper.LogAction("Sipariş İptal / İade", "Sipariş Yönetimi", $"Sipariş No: #{selectedOrderId} iptal ve iade edildi. {dtItems.Rows.Count} kalem ürün stoğa geri yüklendi.");

                        MessageBox.Show($"#{selectedOrderId} numaralı sipariş başarıyla iade edildi!\n{dtItems.Rows.Count} kalem ürün stoğa geri eklendi.", "İade Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PopulateOrders();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("İade işlemi sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (selectedOrderId == 0)
            {
                MessageBox.Show("Lütfen silinecek siparişi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string prompt = $"#{selectedOrderId} numaralı siparişi kalıcı olarak silmek istediğinize emin misiniz?";
            if (selectedOrderStatus != "İptal / İade Edildi")
            {
                prompt += "\n\n⚠️ Dikkat: Bu sipariş henüz iade edilmemiştir. Ürünleri stoğa geri kazandırmak istiyorsanız lütfen önce 'Siparişi İade Et' butonunu kullanınız.\n\nYine de kaydı tamamen silmek istiyor musunuz?";
            }

            DialogResult confirm = MessageBox.Show(prompt, "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                // Delete detail rows then order row
                DbHelper.ExecuteNonQuery("DELETE FROM OrderDetailTable WHERE OrderId = @id", new SqlParameter[] { new SqlParameter("@id", selectedOrderId) });
                DbHelper.ExecuteNonQuery("DELETE FROM OrderTable WHERE OrderId = @id", new SqlParameter[] { new SqlParameter("@id", selectedOrderId) });

                MessageBox.Show("Sipariş başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateOrders();
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

        private void btn_export_orders_Click(object sender, EventArgs e)
        {
            string fileName = $"Siparis_Listesi_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            DbHelper.ExportToCsv(dataGridViewOrders, fileName);
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            if (selectedOrderId == 0)
            {
                MessageBox.Show("Lütfen fişini yazdırmak istediğiniz siparişi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable dtItems = dataGridViewOrderDetails.DataSource as DataTable;
            if (dtItems == null || dtItems.Rows.Count == 0)
            {
                string query = "SELECT ProdName AS [Ürün Adı], Qty AS [Adet], Price AS [Birim Fiyat (TL)], Total AS [Toplam (TL)] FROM OrderDetailTable WHERE OrderId = @orderid ORDER BY DetailId ASC";
                dtItems = DbHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@orderid", selectedOrderId) });
            }

            // Find selected row
            DataGridViewRow selectedRow = null;
            foreach (DataGridViewRow r in dataGridViewOrders.Rows)
            {
                if (Convert.ToInt32(r.Cells[0].Value) == selectedOrderId)
                {
                    selectedRow = r;
                    break;
                }
            }

            if (selectedRow == null) return;

            string custName = selectedRow.Cells[2].Value?.ToString() ?? "";
            DateTime orderDate = Convert.ToDateTime(selectedRow.Cells[3].Value);
            decimal total = Convert.ToDecimal(selectedRow.Cells[4].Value);
            string status = selectedRow.Cells[5].Value?.ToString() ?? "Tamamlandı";

            ReceiptPrinter.ShowReceipt(selectedOrderId, custName, orderDate, status, total, dtItems);
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
```

---

### Dosya: `ViewOrders.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ViewOrders.Designer.cs`
- **Dosya Boyutu:** 18.02 KB
- **Satır Sayısı:** 320

```csharp
namespace Inventory_Management_System
{
    partial class ViewOrders
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
            this.lblOrdersTitle = new System.Windows.Forms.Label();
            this.btn_export_orders = new System.Windows.Forms.Button();
            this.dataGridViewOrders = new System.Windows.Forms.DataGridView();
            this.lblDetailsTitle = new System.Windows.Forms.Label();
            this.btn_print = new System.Windows.Forms.Button();
            this.dataGridViewOrderDetails = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbl_summary = new System.Windows.Forms.Label();
            this.btn_new_order = new System.Windows.Forms.Button();
            this.btn_return = new System.Windows.Forms.Button();
            this.btn_delete = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.btn_home = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrderDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel1.Controls.Add(this.btn_back);
            this.panel1.Controls.Add(this.btn_exit);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(960, 100);
            this.panel1.TabIndex = 0;
            // 
            // btn_back
            // 
            this.btn_back.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_back.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_back.ForeColor = System.Drawing.Color.White;
            this.btn_back.Location = new System.Drawing.Point(15, 25);
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
            this.btn_exit.Location = new System.Drawing.Point(898, 12);
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
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(275, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(410, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "SİPARİŞ LİSTESİ (ORDERS)";
            // 
            // lblOrdersTitle
            // 
            this.lblOrdersTitle.AutoSize = true;
            this.lblOrdersTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblOrdersTitle.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblOrdersTitle.Location = new System.Drawing.Point(30, 110);
            this.lblOrdersTitle.Name = "lblOrdersTitle";
            this.lblOrdersTitle.Size = new System.Drawing.Size(124, 17);
            this.lblOrdersTitle.TabIndex = 17;
            this.lblOrdersTitle.Text = "📋 SİPARİŞLER";
            // 
            // btn_export_orders
            // 
            this.btn_export_orders.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btn_export_orders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_export_orders.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_export_orders.ForeColor = System.Drawing.Color.White;
            this.btn_export_orders.Location = new System.Drawing.Point(390, 105);
            this.btn_export_orders.Name = "btn_export_orders";
            this.btn_export_orders.Size = new System.Drawing.Size(150, 26);
            this.btn_export_orders.TabIndex = 20;
            this.btn_export_orders.Text = "📊 EXCEL\'E AKTAR";
            this.btn_export_orders.UseVisualStyleBackColor = false;
            this.btn_export_orders.Click += new System.EventHandler(this.btn_export_orders_Click);
            // 
            // dataGridViewOrders
            // 
            this.dataGridViewOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOrders.Location = new System.Drawing.Point(30, 135);
            this.dataGridViewOrders.Name = "dataGridViewOrders";
            this.dataGridViewOrders.Size = new System.Drawing.Size(510, 305);
            this.dataGridViewOrders.TabIndex = 1;
            this.dataGridViewOrders.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewOrders_CellClick);
            this.dataGridViewOrders.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewOrders_CellFormatting);
            // 
            // lblDetailsTitle
            // 
            this.lblDetailsTitle.AutoSize = true;
            this.lblDetailsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblDetailsTitle.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblDetailsTitle.Location = new System.Drawing.Point(560, 110);
            this.lblDetailsTitle.Name = "lblDetailsTitle";
            this.lblDetailsTitle.Size = new System.Drawing.Size(165, 17);
            this.lblDetailsTitle.TabIndex = 18;
            this.lblDetailsTitle.Text = "📦 SİPARİŞ KALEMLERİ";
            // 
            // btn_print
            // 
            this.btn_print.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_print.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_print.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_print.ForeColor = System.Drawing.Color.White;
            this.btn_print.Location = new System.Drawing.Point(770, 105);
            this.btn_print.Name = "btn_print";
            this.btn_print.Size = new System.Drawing.Size(160, 26);
            this.btn_print.TabIndex = 21;
            this.btn_print.Text = "🖨️ FİŞ YAZDIR / PDF";
            this.btn_print.UseVisualStyleBackColor = false;
            this.btn_print.Click += new System.EventHandler(this.btn_print_Click);
            // 
            // dataGridViewOrderDetails
            // 
            this.dataGridViewOrderDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOrderDetails.Location = new System.Drawing.Point(560, 135);
            this.dataGridViewOrderDetails.Name = "dataGridViewOrderDetails";
            this.dataGridViewOrderDetails.Size = new System.Drawing.Size(370, 305);
            this.dataGridViewOrderDetails.TabIndex = 19;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 533);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(960, 37);
            this.panel2.TabIndex = 2;
            // 
            // lbl_summary
            // 
            this.lbl_summary.AutoSize = true;
            this.lbl_summary.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_summary.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lbl_summary.Location = new System.Drawing.Point(30, 452);
            this.lbl_summary.Name = "lbl_summary";
            this.lbl_summary.Size = new System.Drawing.Size(315, 16);
            this.lbl_summary.TabIndex = 3;
            this.lbl_summary.Text = "Seçili Sipariş: Yok (Listeden bir sipariş seçiniz)";
            // 
            // btn_new_order
            // 
            this.btn_new_order.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btn_new_order.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_new_order.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_new_order.ForeColor = System.Drawing.Color.White;
            this.btn_new_order.Location = new System.Drawing.Point(30, 482);
            this.btn_new_order.Name = "btn_new_order";
            this.btn_new_order.Size = new System.Drawing.Size(155, 40);
            this.btn_new_order.TabIndex = 4;
            this.btn_new_order.Text = "+ YENİ SİPARİŞ";
            this.btn_new_order.UseVisualStyleBackColor = false;
            this.btn_new_order.Click += new System.EventHandler(this.btn_new_order_Click);
            // 
            // btn_return
            // 
            this.btn_return.BackColor = System.Drawing.Color.OrangeRed;
            this.btn_return.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_return.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_return.ForeColor = System.Drawing.Color.White;
            this.btn_return.Location = new System.Drawing.Point(195, 482);
            this.btn_return.Name = "btn_return";
            this.btn_return.Size = new System.Drawing.Size(185, 40);
            this.btn_return.TabIndex = 5;
            this.btn_return.Text = "↩️ SİPARİŞİ İADE ET";
            this.btn_return.UseVisualStyleBackColor = false;
            this.btn_return.Click += new System.EventHandler(this.btn_return_Click);
            // 
            // btn_delete
            // 
            this.btn_delete.BackColor = System.Drawing.Color.Crimson;
            this.btn_delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_delete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_delete.ForeColor = System.Drawing.Color.White;
            this.btn_delete.Location = new System.Drawing.Point(390, 482);
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.Size = new System.Drawing.Size(125, 40);
            this.btn_delete.TabIndex = 6;
            this.btn_delete.Text = "🗑️ SİL";
            this.btn_delete.UseVisualStyleBackColor = false;
            this.btn_delete.Click += new System.EventHandler(this.btn_delete_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_clear.ForeColor = System.Drawing.Color.White;
            this.btn_clear.Location = new System.Drawing.Point(525, 482);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(140, 40);
            this.btn_clear.TabIndex = 7;
            this.btn_clear.Text = "SEÇİMİ TEMİZLE";
            this.btn_clear.UseVisualStyleBackColor = false;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_refresh
            // 
            this.btn_refresh.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_refresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_refresh.ForeColor = System.Drawing.Color.White;
            this.btn_refresh.Location = new System.Drawing.Point(675, 482);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(115, 40);
            this.btn_refresh.TabIndex = 8;
            this.btn_refresh.Text = "🔄 YENİLE";
            this.btn_refresh.UseVisualStyleBackColor = false;
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // btn_home
            // 
            this.btn_home.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_home.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_home.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_home.ForeColor = System.Drawing.Color.White;
            this.btn_home.Location = new System.Drawing.Point(800, 482);
            this.btn_home.Name = "btn_home";
            this.btn_home.Size = new System.Drawing.Size(130, 40);
            this.btn_home.TabIndex = 9;
            this.btn_home.Text = "◀ ANA MENÜ";
            this.btn_home.UseVisualStyleBackColor = false;
            this.btn_home.Click += new System.EventHandler(this.btn_home_Click);
            // 
            // ViewOrders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(960, 570);
            this.Controls.Add(this.btn_home);
            this.Controls.Add(this.btn_refresh);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.btn_delete);
            this.Controls.Add(this.btn_return);
            this.Controls.Add(this.btn_new_order);
            this.Controls.Add(this.lbl_summary);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dataGridViewOrderDetails);
            this.Controls.Add(this.btn_print);
            this.Controls.Add(this.lblDetailsTitle);
            this.Controls.Add(this.dataGridViewOrders);
            this.Controls.Add(this.btn_export_orders);
            this.Controls.Add(this.lblOrdersTitle);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ViewOrders";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ViewOrders";
            this.Load += new System.EventHandler(this.ViewOrders_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrderDetails)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_back;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblOrdersTitle;
        private System.Windows.Forms.Button btn_export_orders;
        private System.Windows.Forms.DataGridView dataGridViewOrders;
        private System.Windows.Forms.Label lblDetailsTitle;
        private System.Windows.Forms.Button btn_print;
        private System.Windows.Forms.DataGridView dataGridViewOrderDetails;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbl_summary;
        private System.Windows.Forms.Button btn_new_order;
        private System.Windows.Forms.Button btn_return;
        private System.Windows.Forms.Button btn_delete;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.Button btn_home;
    }
}
```

---

### Dosya: `ManageUsers.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageUsers.cs`
- **Dosya Boyutu:** 10.17 KB
- **Satır Sayısı:** 246

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class ManageUsers : Form
    {
        public ManageUsers()
        {
            InitializeComponent();
        }

        private void ManageUsers_Load(object sender, EventArgs e)
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
            PopulateUsers();
            ClearInputs();
        }

        private void FormatGrid()
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void PopulateUsers()
        {
            try
            {
                DataTable dt = DbHelper.ExecuteQuery("SELECT UsernameDB AS [Kullanıcı Adı], FullnameDB AS [Ad Soyad], PasswordDB AS [Şifre], TelephoneDB AS [Telefon], RoleDB AS [Rol] FROM UserTable ORDER BY UsernameDB ASC");
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcılar listelenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txt_username.Text = string.Empty;
            txt_fullname.Text = string.Empty;
            txt_password2.Text = string.Empty;
            txt_phone.Text = string.Empty;
            if (cmb_role.Items.Count > 1)
            {
                cmb_role.SelectedIndex = 1; // Default "Kasiyer"
            }
            dataGridView1.ClearSelection();
            txt_username.Focus();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearInputs();
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

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_username.Text) || string.IsNullOrWhiteSpace(txt_password2.Text))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre alanlarını doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string role = cmb_role.SelectedItem?.ToString() ?? "Kasiyer";

            try
            {
                string query = "INSERT INTO UserTable (UsernameDB, FullnameDB, PasswordDB, TelephoneDB, RoleDB) VALUES (@p1, @p2, @p3, @p4, @p5)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@p1", txt_username.Text.Trim()),
                    new SqlParameter("@p2", txt_fullname.Text.Trim()),
                    new SqlParameter("@p3", txt_password2.Text.Trim()),
                    new SqlParameter("@p4", txt_phone.Text.Trim()),
                    new SqlParameter("@p5", role)
                };

                DbHelper.ExecuteNonQuery(query, parameters);
                DbHelper.LogAction("Kullanıcı Eklendi", "Kullanıcı Yönetimi", $"Kullanıcı: '{txt_username.Text.Trim()}', Ad Soyad: '{txt_fullname.Text.Trim()}', Rol: '{role}'");
                MessageBox.Show("Kullanıcı başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateUsers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_username.Text))
            {
                MessageBox.Show("Lütfen güncellenecek kullanıcıyı listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string role = cmb_role.SelectedItem?.ToString() ?? "Kasiyer";

            try
            {
                string query = "UPDATE UserTable SET FullnameDB = @p2, PasswordDB = @p3, TelephoneDB = @p4, RoleDB = @p5 WHERE UsernameDB = @p1";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@p1", txt_username.Text.Trim()),
                    new SqlParameter("@p2", txt_fullname.Text.Trim()),
                    new SqlParameter("@p3", txt_password2.Text.Trim()),
                    new SqlParameter("@p4", txt_phone.Text.Trim()),
                    new SqlParameter("@p5", role)
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    // Oturum açmış olan kullanıcı kendi profilini güncellediyse oturum bilgilerini güncelle
                    if (string.Equals(CurrentUser.Username, txt_username.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        CurrentUser.FullName = txt_fullname.Text.Trim();
                        CurrentUser.Role = role;
                    }

                    DbHelper.LogAction("Kullanıcı Güncellendi", "Kullanıcı Yönetimi", $"Kullanıcı: '{txt_username.Text.Trim()}', Yeni Ad Soyad: '{txt_fullname.Text.Trim()}', Yeni Rol: '{role}'");
                    MessageBox.Show("Kullanıcı bilgileri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateUsers();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Güncellenecek kullanıcı bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_username.Text))
            {
                MessageBox.Show("Lütfen silinecek kullanıcıyı listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string targetUser = txt_username.Text.Trim();

            // Güvenlik kuralı: Kendi hesabını ve ana admin hesabını silmeyi engelle
            if (string.Equals(targetUser, CurrentUser.Username, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Şu anda oturum açmış olduğunuz kendi hesabınızı silemezsiniz!", "İşlem Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.Equals(targetUser, "admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Sistem ana yöneticisi ('admin') hesabı silinemez!", "İşlem Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"'{targetUser}' kullanıcısını silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM UserTable WHERE UsernameDB = @p1";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@p1", targetUser)
                };

                int affected = DbHelper.ExecuteNonQuery(query, parameters);
                if (affected > 0)
                {
                    DbHelper.LogAction("Kullanıcı Silindi", "Kullanıcı Yönetimi", $"Kullanıcı Adı: '{targetUser}' kalıcı olarak silindi.");
                    MessageBox.Show("Kullanıcı başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateUsers();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Silinecek kullanıcı bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı silinirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txt_username.Text = row.Cells[0].Value?.ToString() ?? "";
                txt_fullname.Text = row.Cells[1].Value?.ToString() ?? "";
                txt_password2.Text = row.Cells[2].Value?.ToString() ?? "";
                txt_phone.Text = row.Cells[3].Value?.ToString() ?? "";

                string role = row.Cells.Count > 4 ? (row.Cells[4].Value?.ToString() ?? "") : "";
                if (!string.IsNullOrEmpty(role) && cmb_role.Items.Contains(role))
                {
                    cmb_role.SelectedItem = role;
                }
                else
                {
                    cmb_role.SelectedIndex = 1;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            btn_back_Click(sender, e);
        }
    }
}
```

---

### Dosya: `ManageUsers.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageUsers.Designer.cs`
- **Dosya Boyutu:** 15.68 KB
- **Satır Sayısı:** 328

```csharp
namespace Inventory_Management_System
{
    partial class ManageUsers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_back = new System.Windows.Forms.Button();
            this.btn_exit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_username = new System.Windows.Forms.TextBox();
            this.txt_phone = new System.Windows.Forms.TextBox();
            this.txt_password2 = new System.Windows.Forms.TextBox();
            this.txt_fullname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_role = new System.Windows.Forms.Label();
            this.cmb_role = new System.Windows.Forms.ComboBox();
            this.btn_add = new System.Windows.Forms.Button();
            this.btn_edit = new System.Windows.Forms.Button();
            this.btn_delete = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel1.Controls.Add(this.btn_back);
            this.panel1.Controls.Add(this.btn_exit);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(880, 100);
            this.panel1.TabIndex = 0;
            // 
            // btn_back
            // 
            this.btn_back.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_back.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_back.ForeColor = System.Drawing.Color.White;
            this.btn_back.Location = new System.Drawing.Point(15, 25);
            this.btn_back.Name = "btn_back";
            this.btn_back.Size = new System.Drawing.Size(95, 45);
            this.btn_back.TabIndex = 16;
            this.btn_back.Text = "◀ GERİ";
            this.btn_back.UseVisualStyleBackColor = true;
            this.btn_back.Click += new System.EventHandler(this.btn_back_Click);
            // 
            // btn_exit
            // 
            this.btn_exit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_exit.Location = new System.Drawing.Point(818, 12);
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
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(280, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(253, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "MANAGE USER";
            // 
            // txt_username
            // 
            this.txt_username.Location = new System.Drawing.Point(167, 161);
            this.txt_username.Name = "txt_username";
            this.txt_username.Size = new System.Drawing.Size(100, 20);
            this.txt_username.TabIndex = 1;
            // 
            // txt_phone
            // 
            this.txt_phone.Location = new System.Drawing.Point(167, 239);
            this.txt_phone.Name = "txt_phone";
            this.txt_phone.Size = new System.Drawing.Size(100, 20);
            this.txt_phone.TabIndex = 2;
            // 
            // txt_password2
            // 
            this.txt_password2.Location = new System.Drawing.Point(167, 213);
            this.txt_password2.Name = "txt_password2";
            this.txt_password2.Size = new System.Drawing.Size(100, 20);
            this.txt_password2.TabIndex = 3;
            // 
            // txt_fullname
            // 
            this.txt_fullname.Location = new System.Drawing.Point(167, 187);
            this.txt_fullname.Name = "txt_fullname";
            this.txt_fullname.Size = new System.Drawing.Size(100, 20);
            this.txt_fullname.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Kullanıcı Adı";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Tam Ad";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 216);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Şifre";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 242);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Telefon";
            // 
            // label_role
            // 
            this.label_role.AutoSize = true;
            this.label_role.Location = new System.Drawing.Point(41, 267);
            this.label_role.Name = "label_role";
            this.label_role.Size = new System.Drawing.Size(23, 13);
            this.label_role.TabIndex = 18;
            this.label_role.Text = "Rol";
            // 
            // cmb_role
            // 
            this.cmb_role.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_role.FormattingEnabled = true;
            this.cmb_role.Items.AddRange(new object[] {
            "Yönetici (Admin)",
            "Kasiyer",
            "Depo Görevlisi"});
            this.cmb_role.Location = new System.Drawing.Point(167, 264);
            this.cmb_role.Name = "cmb_role";
            this.cmb_role.Size = new System.Drawing.Size(100, 21);
            this.cmb_role.TabIndex = 5;
            // 
            // btn_add
            // 
            this.btn_add.Location = new System.Drawing.Point(44, 294);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(75, 23);
            this.btn_add.TabIndex = 9;
            this.btn_add.Text = "ADD";
            this.btn_add.UseVisualStyleBackColor = true;
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // btn_edit
            // 
            this.btn_edit.Location = new System.Drawing.Point(125, 294);
            this.btn_edit.Name = "btn_edit";
            this.btn_edit.Size = new System.Drawing.Size(75, 23);
            this.btn_edit.TabIndex = 10;
            this.btn_edit.Text = "EDIT";
            this.btn_edit.UseVisualStyleBackColor = true;
            this.btn_edit.Click += new System.EventHandler(this.btn_edit_Click);
            // 
            // btn_delete
            // 
            this.btn_delete.Location = new System.Drawing.Point(206, 294);
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.Size = new System.Drawing.Size(75, 23);
            this.btn_delete.TabIndex = 11;
            this.btn_delete.Text = "DELETE";
            this.btn_delete.UseVisualStyleBackColor = true;
            this.btn_delete.Click += new System.EventHandler(this.btn_delete_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btn_clear.ForeColor = System.Drawing.Color.White;
            this.btn_clear.Location = new System.Drawing.Point(44, 335);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(237, 34);
            this.btn_clear.TabIndex = 17;
            this.btn_clear.Text = "SEÇİMİ TEMİZLE";
            this.btn_clear.UseVisualStyleBackColor = false;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.DarkTurquoise;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(44, 380);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(237, 34);
            this.button4.TabIndex = 12;
            this.button4.Text = "◀ ANA MENÜ";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(354, 109);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(514, 418);
            this.dataGridView1.TabIndex = 13;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkTurquoise;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 533);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(880, 37);
            this.panel2.TabIndex = 14;
            // 
            // ManageUsers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(880, 570);
            this.Controls.Add(this.cmb_role);
            this.Controls.Add(this.label_role);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btn_delete);
            this.Controls.Add(this.btn_edit);
            this.Controls.Add(this.btn_add);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_fullname);
            this.Controls.Add(this.txt_password2);
            this.Controls.Add(this.txt_phone);
            this.Controls.Add(this.txt_username);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ManageUsers";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ManageUsers";
            this.Load += new System.EventHandler(this.ManageUsers_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_username;
        private System.Windows.Forms.TextBox txt_phone;
        private System.Windows.Forms.TextBox txt_password2;
        private System.Windows.Forms.TextBox txt_fullname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_role;
        private System.Windows.Forms.ComboBox cmb_role;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Button btn_edit;
        private System.Windows.Forms.Button btn_delete;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Button btn_back;
    }
}
```

---

### Dosya: `ManageUsers.resx`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageUsers.resx`
- **Dosya Boyutu:** 5.68 KB
- **Satır Sayısı:** 120

```xml
﻿<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- 
    Microsoft ResX Schema 
    
    Version 2.0
    
    The primary goals of this format is to allow a simple XML format 
    that is mostly human readable. The generation and parsing of the 
    various data types are done through the TypeConverter classes 
    associated with the data types.
    
    Example:
    
    ... ado.net/XML headers & schema ...
    <resheader name="resmimetype">text/microsoft-resx</resheader>
    <resheader name="version">2.0</resheader>
    <resheader name="reader">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name="writer">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name="Name1"><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name="Color1" type="System.Drawing.Color, System.Drawing">Blue</data>
    <data name="Bitmap1" mimetype="application/x-microsoft.net.object.binary.base64">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name="Icon1" type="System.Drawing.Icon, System.Drawing" mimetype="application/x-microsoft.net.object.bytearray.base64">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>
                
    There are any number of "resheader" rows that contain simple 
    name/value pairs.
    
    Each data row contains a name, and value. The row also contains a 
    type or mimetype. Type corresponds to a .NET class that support 
    text/value conversion through the TypeConverter architecture. 
    Classes that don't support this are serialized and stored with the 
    mimetype set.
    
    The mimetype is used for serialized objects, and tells the 
    ResXResourceReader how to depersist the object. This is currently not 
    extensible. For a given mimetype the value must be set accordingly:
    
    Note - application/x-microsoft.net.object.binary.base64 is the format 
    that the ResXResourceWriter will generate, however the reader can 
    read any of the formats listed below.
    
    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.
    
    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array 
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
</root>
```

---

### Dosya: `ViewAuditLogs.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ViewAuditLogs.cs`
- **Dosya Boyutu:** 8.22 KB
- **Satır Sayısı:** 191

```csharp
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
```

---

### Dosya: `ViewAuditLogs.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ViewAuditLogs.Designer.cs`
- **Dosya Boyutu:** 14.82 KB
- **Satır Sayısı:** 280

```csharp
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
```

---

### Dosya: `ManageBackupRestore.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageBackupRestore.cs`
- **Dosya Boyutu:** 8.61 KB
- **Satır Sayısı:** 206

```csharp
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
                HomeForm home = new HomeForm();
                home.Show();
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

                HomeForm home = new HomeForm();
                home.Show();
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
```

---

### Dosya: `ManageBackupRestore.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\ManageBackupRestore.Designer.cs`
- **Dosya Boyutu:** 20.88 KB
- **Satır Sayısı:** 356

```csharp
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
```

---

### Dosya: `Properties\AssemblyInfo.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Properties\AssemblyInfo.cs`
- **Dosya Boyutu:** 1.43 KB
- **Satır Sayısı:** 36

```csharp
﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Inventory Management System")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Inventory Management System")]
[assembly: AssemblyCopyright("Copyright ©  2026")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a983206f-6c2a-4ec4-8db1-6c32fa8695a9")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
```

---

### Dosya: `Properties\Resources.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Properties\Resources.Designer.cs`
- **Dosya Boyutu:** 3.23 KB
- **Satır Sayısı:** 73

```csharp
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inventory_Management_System.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Inventory_Management_System.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap shutterstock_1930996376_scaled {
            get {
                object obj = ResourceManager.GetObject("shutterstock_1930996376-scaled", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}
```

---

### Dosya: `Properties\Resources.resx`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Properties\Resources.resx`
- **Dosya Boyutu:** 6.09 KB
- **Satır Sayısı:** 124

```xml
﻿<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- 
    Microsoft ResX Schema 
    
    Version 2.0
    
    The primary goals of this format is to allow a simple XML format 
    that is mostly human readable. The generation and parsing of the 
    various data types are done through the TypeConverter classes 
    associated with the data types.
    
    Example:
    
    ... ado.net/XML headers & schema ...
    <resheader name="resmimetype">text/microsoft-resx</resheader>
    <resheader name="version">2.0</resheader>
    <resheader name="reader">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name="writer">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name="Name1"><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name="Color1" type="System.Drawing.Color, System.Drawing">Blue</data>
    <data name="Bitmap1" mimetype="application/x-microsoft.net.object.binary.base64">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name="Icon1" type="System.Drawing.Icon, System.Drawing" mimetype="application/x-microsoft.net.object.bytearray.base64">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>
                
    There are any number of "resheader" rows that contain simple 
    name/value pairs.
    
    Each data row contains a name, and value. The row also contains a 
    type or mimetype. Type corresponds to a .NET class that support 
    text/value conversion through the TypeConverter architecture. 
    Classes that don't support this are serialized and stored with the 
    mimetype set.
    
    The mimetype is used for serialized objects, and tells the 
    ResXResourceReader how to depersist the object. This is currently not 
    extensible. For a given mimetype the value must be set accordingly:
    
    Note - application/x-microsoft.net.object.binary.base64 is the format 
    that the ResXResourceWriter will generate, however the reader can 
    read any of the formats listed below.
    
    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.
    
    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array 
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <assembly alias="System.Windows.Forms" name="System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
  <data name="shutterstock_1930996376-scaled" type="System.Resources.ResXFileRef, System.Windows.Forms">
    <value>..\shutterstock_1930996376-scaled.jpg;System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a</value>
  </data>
</root>
```

---

### Dosya: `Properties\Settings.Designer.cs`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Properties\Settings.Designer.cs`
- **Dosya Boyutu:** 1.09 KB
- **Satır Sayısı:** 30

```csharp
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inventory_Management_System.Properties
{


    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {

        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }
    }
}
```

---

### Dosya: `Properties\Settings.settings`

- **Tam Dosya Yolu:** `C:\Users\FURKAN YILMAZ\Desktop\Inventory Management System\Properties\Settings.settings`
- **Dosya Boyutu:** 0.24 KB
- **Satır Sayısı:** 7

```xml
﻿<?xml version='1.0' encoding='utf-8'?>
<SettingsFile xmlns="http://schemas.microsoft.com/VisualStudio/2004/01/settings" CurrentProfile="(Default)">
  <Profiles>
    <Profile Name="(Default)" />
  </Profiles>
  <Settings />
</SettingsFile>
```

---

