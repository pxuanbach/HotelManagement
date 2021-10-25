using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.ViewModels
{
    class ExportInvoice
    {
        public static string tahoma_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "tahoma.ttf");

        //Create a base font object making sure to specify IDENTITY-H
        public static BaseFont bf = BaseFont.CreateFont(tahoma_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        //Create a specific font object
        public Font f11 = new Font(bf, 11, Font.NORMAL);
        public Font f11G = new Font(bf, 11, Font.NORMAL, BaseColor.GRAY);
        public Font f11W = new Font(bf, 11, Font.NORMAL, BaseColor.WHITE);
        public Font f11B = new Font(bf, 11, Font.BOLD);
        public Font f20B = new Font(bf, 20, Font.BOLD);
        public Font f15B = new Font(bf, 15, Font.BOLD);
        public Font f15W = new Font(bf, 15, Font.NORMAL, BaseColor.WHITE);

        public void Export(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                FormatHeader(pdfDoc);

                FormatMainGuestInformation(pdfDoc);

                FormatInvoiceInformation(pdfDoc);

                FormatListRoom_Folio(pdfDoc);

                pdfDoc.Close();
                stream.Close();
            }
        }

        #region Format Document
        public void FormatHeader(Document pdfDoc)
        {
            Paragraph hotelName = new Paragraph("BTNQ Hotel", f15B);
            hotelName.Alignment = Element.ALIGN_CENTER;

            Paragraph hotelAddress = new Paragraph("Address: 123, Dong Hoa, Di An, Binh Duong", f11);
            hotelAddress.Alignment = Element.ALIGN_CENTER;

            Paragraph hotelPhone = new Paragraph("Phone: 0808008008 - 0345678989", f11);
            hotelPhone.Alignment = Element.ALIGN_CENTER;

            Paragraph hotelEmail = new Paragraph("Email: BTNQHotel@gmail.com", f11);
            hotelEmail.Alignment = Element.ALIGN_CENTER;

            Paragraph title = new Paragraph("INVOICE", f20B);
            title.Alignment = Element.ALIGN_CENTER;

            pdfDoc.Add(hotelName);
            pdfDoc.Add(hotelAddress);
            pdfDoc.Add(hotelPhone);
            pdfDoc.Add(hotelEmail);
            pdfDoc.Add(new Phrase("                         ", f11));
            pdfDoc.Add(title);
            pdfDoc.Add(Saperator());
            pdfDoc.Add(new Phrase("                         ", f11));
        }

        public void FormatMainGuestInformation(Document pdfDoc)
        {
            Paragraph invoiceFor = new Paragraph();
            invoiceFor.Add(Chunk.TABBING);
            invoiceFor.Add(new Chunk("Invoice For: ", f11G));
            invoiceFor.Add(Chunk.TABBING);
            invoiceFor.Add(new Chunk("Pham Xuan Bach", f11B));
            invoiceFor.Add(Chunk.TABBING);
            invoiceFor.Add(Chunk.TABBING);
            invoiceFor.Add(Chunk.TABBING);
            invoiceFor.Add(Chunk.TABBING);
            invoiceFor.Add(new Chunk("ID Card: ", f11G));
            invoiceFor.Add(new Chunk("2345678910", f11));

            Paragraph phoneEmail = new Paragraph();
            phoneEmail.Add(Chunk.TABBING);
            phoneEmail.Add(new Chunk("Phone: ", f11G));
            phoneEmail.Add(Chunk.TABBING);
            phoneEmail.Add(new Chunk("0123456789", f11));
            phoneEmail.Add(Chunk.TABBING);
            phoneEmail.Add(Chunk.TABBING);
            phoneEmail.Add(Chunk.TABBING);
            phoneEmail.Add(Chunk.TABBING);
            phoneEmail.Add(Chunk.TABBING);
            phoneEmail.Add(new Chunk("Email: ", f11G));
            phoneEmail.Add(Chunk.TABBING);
            phoneEmail.Add(new Chunk("pxuanbach1412@gmail.com", f11));

            pdfDoc.Add(invoiceFor);
            pdfDoc.Add(phoneEmail);
            pdfDoc.Add(new Phrase("                         ", f11));
        }

        public void FormatInvoiceInformation(Document pdfDoc)
        {
            Paragraph invoiceId = new Paragraph();
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(new Chunk("Invoice Id: ", f11G));
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(new Chunk("101", f15B));
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(new Chunk("Date: ", f11G));
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(new Chunk(DateTime.Now.ToString(), f11));

            Paragraph timeStay = new Paragraph();
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(new Chunk("Arrival: ", f11G));
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(new Chunk(DateTime.Now.ToString(), f11));
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(new Chunk("Departure: ", f11G));
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(new Chunk(DateTime.Now.ToString(), f11));

            Paragraph totalDays = new Paragraph();
            totalDays.Add(Chunk.TABBING);
            totalDays.Add(Chunk.TABBING);
            totalDays.Add(Chunk.TABBING);
            totalDays.Add(Chunk.TABBING);
            totalDays.Add(Chunk.TABBING);
            totalDays.Add(Chunk.TABBING);
            totalDays.Add(Chunk.TABBING);
            totalDays.Add(Chunk.TABBING);
            totalDays.Add(Chunk.TABBING);
            totalDays.Add(new Chunk("Total No. of days: ", f11G));
            totalDays.Add(new Chunk("25", f11));

            pdfDoc.Add(invoiceId);
            pdfDoc.Add(timeStay);
            pdfDoc.Add(totalDays);
            pdfDoc.Add(Saperator());
        }

        public void FormatListRoom_Folio(Document pdfDoc)
        {
            float[] widths = new float[] { 1f, 3f, 1f, 1f, 1f, 1f }; //length = num of columns
            PdfPTable roomTable = new PdfPTable(widths);
            roomTable.WidthPercentage = 100;
            roomTable.HorizontalAlignment = Element.ALIGN_CENTER;

            //Title of Table
            roomTable.AddCell(CellCenterFormat("List Room", f15W, 1, widths.Length, true));

            //Column Header
            roomTable.AddCell(CellCenterFormat("Room Name", f11W, 2, 1, true));
            roomTable.AddCell(CellCenterFormat("Folio", f11W, 1, 3, true));
            roomTable.AddCell(CellCenterFormat("Price/Day", f11W, 2, 1, true));
            roomTable.AddCell(CellCenterFormat("Total", f11W, 2, 1, true));
            roomTable.AddCell(CellCenterFormat("Service Name", f11W, 1, 1, true));
            roomTable.AddCell(CellCenterFormat("Amount", f11W, 1, 1, true));
            roomTable.AddCell(CellCenterFormat("Price", f11W, 1, 1, true));

            //Datatable
            for (int i = 0; i < 3; i++)
            {
                roomTable.AddCell(CellLeftFormat("B03\n\nDeluxe (DLX)", f11, 3));

                roomTable.AddCell(CellLeftFormat("Dịch vụ tập thể hình theo ngày", f11));
                roomTable.AddCell(CellCenterFormat("2", f11));
                roomTable.AddCell(CellRightFormat("120,000", f11));

                roomTable.AddCell(CellRightFormat("750,000", f11, 3));
                roomTable.AddCell(CellRightFormat("2,630,000", f11, 3));

                roomTable.AddCell(CellLeftFormat("Nước lọc", f11));
                roomTable.AddCell(CellCenterFormat("5", f11));
                roomTable.AddCell(CellRightFormat("12,000", f11));

                roomTable.AddCell(CellLeftFormat("Dịch vụ giặt ủi", f11));
                roomTable.AddCell(CellCenterFormat("2", f11));
                roomTable.AddCell(CellRightFormat("40,000", f11));

            }

            pdfDoc.Add(roomTable);
        }
        #endregion

        #region Utilities
        public Paragraph Saperator()
        {
            Paragraph saperator = new Paragraph("---------------------------------------------------------------", f11G);
            saperator.Alignment = Element.ALIGN_CENTER;
            return saperator;
        }

        static PdfPCell CellCenterFormat(string content, Font font, int rowSpan = 1, int colSpan = 1, bool isBackground = false)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = colSpan;
            cell.Rowspan = rowSpan;

            if (isBackground)
                cell.BackgroundColor = BaseColor.GRAY;

            return cell;
        }

        static PdfPCell CellLeftFormat(string content, Font font, int rowSpan = 1, int colSpan = 1, bool isBackground = false)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, font));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = colSpan;
            cell.Rowspan = rowSpan;

            if (isBackground)
                cell.BackgroundColor = BaseColor.GRAY;

            return cell;
        }

        static PdfPCell CellRightFormat(string content, Font font, int rowSpan = 1, int colSpan = 1, bool isBackground = false)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, font));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = colSpan;
            cell.Rowspan = rowSpan;

            if (isBackground)
                cell.BackgroundColor = BaseColor.GRAY;

            return cell;
        }
        #endregion
    }
}
