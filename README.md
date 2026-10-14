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
