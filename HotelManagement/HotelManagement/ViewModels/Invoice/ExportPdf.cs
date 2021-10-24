using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.ViewModels.Invoice
{
    class ExportPdf
    {
        void FormatFile(int length, string filePath)
        {
            string tahoma_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "tahoma.ttf");

            //Create a base font object making sure to specify IDENTITY-H
            BaseFont bf = BaseFont.CreateFont(tahoma_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            //Create a specific font object
            Font f12 = new Font(bf, 12, Font.NORMAL);
            Font f12B = new Font(bf, 12, Font.BOLD);
            Font f20 = new Font(bf, 20, Font.BOLD);
            Font f15 = new Font(bf, 15, Font.BOLD);

            PdfPTable pdfTable = new PdfPTable(length - 1);
            pdfTable.DefaultCell.Padding = 3;
            pdfTable.WidthPercentage = 100;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                Paragraph para3 = new Paragraph("HotelBTNQ", f15);
                Paragraph para4 = new Paragraph("Địa chỉ: số nhà 123, phường Đông Hòa, thị xã Dĩ An, tỉnh Bình Dương", f12);
                Paragraph para5 = new Paragraph("Điện thoại: 0808008008 - 0345678989", f12);
                Paragraph para6 = new Paragraph("Email: HotelBTNQ@gmail.com", f12);
                Paragraph para7 = new Paragraph("HÓA ĐƠN THANH TOÁN", f20);
                Paragraph para8 = new Paragraph("* * *", f20);
                Paragraph para9 = new Paragraph("------------------------------------------------------------", f12);

                Paragraph c1 = new Paragraph("     Mã hóa đơn: ", f12);
                Paragraph paraLienHe = new Paragraph("     Liên hệ:.", f12);

                pdfDoc.Close();
                stream.Close();
            }
        }
    }
}
