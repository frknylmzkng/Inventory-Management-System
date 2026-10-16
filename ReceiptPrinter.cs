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
