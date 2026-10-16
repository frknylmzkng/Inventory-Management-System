# 📦 Inventory Management System (Envanter ve Stok Yönetim Sistemi)

C# Windows Forms ve Microsoft SQL Server (LocalDB) kullanılarak geliştirilmiş, **Rol Tabanlı Yetkilendirme (RBAC)**, **Denetim İzi (Audit Logging)**, **Satış Fişi Yazdırma**, **SHA-256 Güvenliği**, **Dinamik Veritabanı Yönetimi** ve **Finansal Kâr/Zarar Analizi** özelliklerine sahip kurumsal düzeyde masaüstü Stok ve Sipariş Yönetim Sistemi.

---

## 🚀 Öne Çıkan Özellikler

### 🔐 1. Kimlik Doğrulama, Rol Tabanlı Erişim (RBAC) ve Güvenlik
* **SHA-256 Şifre Güvenliği:** Tüm kullanıcı şifreleri veritabanında tek yönlü SHA-256 algoritmasıyla özetlenerek (hash) saklanır; arayüz ve tablolarda şifreler gizlenir.
* **Otomatik Şifre Geçişi (Migration):** Geriye dönük uyumluluk sayesinde eski düz metin şifreler ilk başarılı girişte otomatik olarak SHA-256 formatına yükseltilir.
* **3 Farklı Kullanıcı Rolü:**
  * 👑 **Yönetici (Admin):** Tüm modüllere tam erişim, finansal analizler, kullanıcı yönetimi, denetim izleri, yedekleme/geri yükleme ve sipariş iade/silme yetkisi.
  * 💼 **Kasiyer:** Müşteri yönetimi, sipariş oluşturma, fiş yazdırma ve stok görüntüleme; maliyet verileri ve silme işlemleri kısıtlıdır.
  * 📦 **Depo Görevlisi:** Ürün ekleme/düzenleme, stok güncelleme, barkod üretimi ve kategori yönetimi; sipariş ve mali verilere erişim kısıtlıdır.
* **SQL Injection Koruması:** Tüm veritabanı sorguları ADO.NET parametreli (`SqlParameter`) komutlar üzerinden yürütülür.

### 📊 2. Canlı Kontrol Paneli (Dashboard) & Finansal Analiz
* **Performans Optimize Sayaçlar:** Tek bir birleşik SQL sorgusu ile anlık toplam ürün, kategori, kullanıcı, sipariş ve kritik stok sayıları çekilir.
* **Finansal Durum Özeti (Admin):** Tamamlanan satışların cirosu, satılan ürünlerin maliyet fiyatları üzerinden net maliyet, net kâr/zarar ve kâr marjı (%) canlı olarak hesaplanır.
* **Çift Çalışma Koruması:** Form yaşam döngüsündeki gereksiz sorgu tekrarları engellenmiştir.

### 📦 3. Ürün & Stok Yönetimi
* **EAN-13 Standart Barkod Üretimi:** Gerçek kontrol basamağı (checksum digit) algoritması hesaplayan 13 haneli geçerli barkod üreteci.
* **Kritik Stok Uyarısı:** Stoğu kritik seviyenin (varsayılan: 5) altına inen ürünler tabloda turuncu, tükenenler ise kırmızı renk ile görsel olarak vurgulanır.
* **Maliyet Korunumu:** Admin olmayan kullanıcılar ürün düzenlediğinde gizli tutulan ürün maliyet fiyatı (`ProdCostPrice`) sıfırlanmaz, korunur.
* **Kategori Filtreleme & Arama:** Kategori bazlı anlık stok filtreleme.
* **Excel / CSV Dışa Aktarma:** Tek tıkla ürün ve stok listesini UTF-8 uyumlu CSV olarak kaydetme ve otomatik açma.

### 🗂️ 4. Kategori Yönetimi & İlişkisel Bütünlük
* **İlişkisel Veri Bütünlüğü (Cascading Update):** Kategori adı güncellendiğinde `SqlTransaction` ile `CategoryTable` ve bağlı tüm ürünlerin `ProductTable.ProdCat` alanları eş zamanlı güncellenir.
* **Silme Koruması:** İçinde aktif ürün bulunan kategorilerin yanlışlıkla silinmesi engellenir.

### 🛒 5. Sipariş, Satış & Fiş Yazdırma
* **Sepet Mekanizması:** Müşteri ve ürün seçimi, adet belirleme, stok kontrolü ve anlık ara toplam/genel toplam hesaplama.
* **Stok Düşümü & Transaction Güvenliği:** Sipariş onaylandığında ürünlerin stoktan düşülmesi ve detay kalemlerinin kaydedilmesi atomik transaction içinde yürütülür.
* **Termal / A4 Satış Fişi Yazdırma:** `PrintPreviewDialog` ve `PrintDocument` entegrasyonu ile sipariş tamamlandığında veya sipariş geçmişinden anında makbuz yazdırma/önizleme. Bellek sızıntısına karşı tüm GDI+ nesneleri (`Font`, `Brush`, `StringFormat`) `using` bloklarıyla imha edilir.

### 🔄 6. Sipariş Geçmişi, İptal / İade ve Silme
* **İptal & İade Yönetimi:** Sipariş iptal edildiğinde sepetteki tüm kalemler `SqlTransaction` ile ürün stoğuna otomatik olarak geri eklenir.
* **Yetkili Sipariş Silme:** Yalnızca Admin yetkisiyle, onay ve audit log kaydı alınarak sipariş ve detayları kalıcı olarak silinebilir.

### 📜 7. Sistem Denetim İzi (Audit Log)
* Oturum açma/kapama, hatalı şifre denemeleri, ürün ekleme/güncelleme/silme, kategori işlemleri, sipariş hareketleri ve yedekleme adımları tarih, kullanıcı ve rol bilgisiyle `AuditLogTable` tablosuna kaydedilir.
* Modül ve işlem türüne göre filtrelenebilir, renk kodlu (Ekleme = Yeşil, Güncelleme = Sarı, Silme/Hata = Kırmızı) denetim ekranı.

### 💾 8. Veritabanı Yedekleme ve Geri Yükleme (Backup & Restore)
* Veritabanının `.bak` dosya yedeğini alma.
* Mevcut bağlantıları otomatik sonlandırarak (`SINGLE_USER`) yedekten sorunsuz geri yükleme yapabilme ve işlem öncesi otomatik güvenlik yedeği oluşturma.

---

## 🛠️ Teknolojiler & Mimari

| Katman | Teknoloji / Kütüphane |
|---|---|
| **Platform** | .NET Framework 4.8 (C# 7.3) |
| **Arayüz (UI)** | Windows Forms (WinForms) |
| **Veritabanı** | Microsoft SQL Server LocalDB (`MSSQLLocalDB`) |
| **Veri Erişimi** | ADO.NET (`System.Data.SqlClient`) |
| **Güvenlik & Şifreleme** | `System.Security.Cryptography.SHA256` |
| **Yapılandırma** | `System.Configuration` & `App.config` |
| **Yazdırma & Raporlama** | GDI+ (`System.Drawing.Printing`, `PrintPreviewDialog`) |

---

## 📁 Proje Dizin Yapısı

```
Inventory Management System/
├── App.config                      # Dinamik veritabanı bağlantı dizesi
├── CurrentUser.cs                  # Aktif oturum ve yetki durumu yöneticisi
├── DbHelper.cs                     # Merkezi SQL, Transaction, SHA-256 ve CSV yöneticisi
├── HomeForm.cs                     # Ana kontrol paneli (Dashboard) ve canlı istatistikler
├── Login.cs                        # Güvenli giriş ve SHA-256 parola geçiş ekranı
├── ManageBackupRestore.cs          # Veritabanı yedekleme ve geri yükleme arayüzü
├── ManageCategories.cs             # İlişkisel kategori yönetimi
├── ManageCustomers.cs              # Müşteri CRUD yönetimi
├── ManageOrders.cs                 # Sipariş sepeti, satış ve stok düşüm arayüzü
├── ManageProducts.cs               # Ürün, stok, EAN-13 barkod ve kritik stok ekranı
├── ManageUsers.cs                  # Kullanıcı ve rol yönetimi (Admin)
├── Program.cs                      # Uygulama giriş noktası
├── ReceiptPrinter.cs               # GDI+ sızıntısız satış fişi yazdırma motoru
├── ViewAuditLogs.cs                # Güvenlik ve işlem denetim izi izleme ekranı
├── ViewOrders.cs                   # Sipariş geçmişi, iade ve yazdırma yönetimi
└── Inventory Management System.sln # Visual Studio Çözüm Dosyası
```

---

## 💻 Kurulum ve Çalıştırma

### Gereksinimler
- Windows 10 / 11
- Visual Studio 2019 / 2022 (.NET Masaüstü Geliştirme iş yükü)
- Microsoft SQL Server LocalDB (Visual Studio ile birlikte kurulur)

### Adımlar
1. Projeyi klonlayın:
   ```bash
   git clone https://github.com/frknylmzkng/Inventory-Management-System.git
   ```
2. Visual Studio ile `Inventory Management System.sln` çözüm dosyasını açın.
3. Proje derlendiğinde `App.config` içindeki dinamik bağlantı dizesi kullanıcının `Belgelerim` (`MyDocuments`) klasöründeki `InventoryDB_03052026.mdf` veritabanına otomatik bağlanır.
4. Çözümü derleyin (`Ctrl + Shift + B`) ve çalıştırın (`F5`).

### Varsayılan Giriş Bilgileri
* **Kullanıcı Adı:** `admin`
* **Varsayılan Şifre:** `admin123` (veya veritabanınızdaki mevcut şifreniz; ilk girişte otomatik olarak SHA-256 formatına yükseltilir)
* **Rol:** `Yönetici (Admin)`

---

## 👤 Geliştirici

* **Furkan Yılmaz**
* GitHub: [@frknylmzkng](https://github.com/frknylmzkng)
* E-posta: [furkanylmaz0@outlook.com](mailto:furkanylmaz0@outlook.com)
