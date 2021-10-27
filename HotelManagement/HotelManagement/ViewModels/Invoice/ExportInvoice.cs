using HotelManagement.Models;
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
    class ExportInvoice : BaseViewModel
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

        public void Export(string filePath, RESERVATION reservation)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                FormatHeader(pdfDoc);

                FormatInvoiceDetails(pdfDoc, reservation);

                FormatListRoom_Folio(pdfDoc, reservation);

                FormatChargesAndTotalMoney(pdfDoc, reservation);

                FormatFooter(pdfDoc);

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
        }

        public void FormatInvoiceDetails(Document pdfDoc, RESERVATION reservation)
        {
            float[] widths = new float[] { 1f, 1.5f, 1f, 1.5f }; //length = num of columns
            PdfPTable inforTable = new PdfPTable(widths);
            inforTable.WidthPercentage = 100;

            //row 1
            inforTable.AddCell(CellRightNoBorder("Reservation Id:", f11G));
            inforTable.AddCell(CellLeftNoBorder(reservation.id.ToString(), f15B));
            inforTable.AddCell(CellRightNoBorder("Invoice For:", f11G));
            inforTable.AddCell(CellLeftNoBorder(reservation.GUEST.name, f11B));

            //row 2
            inforTable.AddCell(CellRightNoBorder("Date Created:", f11G));
            inforTable.AddCell(CellLeftNoBorder(reservation.date_created.ToString(), f11));
            inforTable.AddCell(CellRightNoBorder("ID card:", f11G));
            inforTable.AddCell(CellLeftNoBorder(reservation.main_guest, f11));

            //row 3
            inforTable.AddCell(CellRightNoBorder("Arrival Date:", f11G));
            inforTable.AddCell(CellLeftNoBorder(reservation.arrival.ToString(), f11));
            inforTable.AddCell(CellRightNoBorder("Phone:", f11G));
            inforTable.AddCell(CellLeftNoBorder(reservation.GUEST.phone, f11));

            //row 4
            inforTable.AddCell(CellRightNoBorder("Departure Date:", f11G));
            inforTable.AddCell(CellLeftNoBorder(reservation.departure.ToString(), f11));
            inforTable.AddCell(CellRightNoBorder("Email:", f11G));
            inforTable.AddCell(CellLeftNoBorder(reservation.GUEST.email, f11));

            //row 5
            inforTable.AddCell(CellRightNoBorder("Total No. of days:", f11G));
            inforTable.AddCell(CellLeftNoBorder(CalculatorInvoice.TotalNumOfDays(reservation).ToString(), f11, 1, 3));

            pdfDoc.Add(inforTable);
            pdfDoc.Add(new Paragraph("                                  ", f11));
        }

        public void FormatListRoom_Folio(Document pdfDoc, RESERVATION reservation)
        {
            float[] widths = new float[] { 1.3f, 3f, 0.7f, 1f, 1f}; //length = num of columns
            PdfPTable roomTable = new PdfPTable(widths);
            roomTable.WidthPercentage = 100;
            roomTable.HorizontalAlignment = Element.ALIGN_CENTER;

            //Title of Table
            roomTable.AddCell(CellCenterFormat("List Room", f15W, 1, widths.Length, true));

            //Column Header
            roomTable.AddCell(CellCenterFormat("Room Name", f11W, 2, 1, true));
            roomTable.AddCell(CellCenterFormat("Folio", f11W, 1, 3, true));
            roomTable.AddCell(CellCenterFormat("Price/Day", f11W, 2, 1, true));
            roomTable.AddCell(CellCenterFormat("Service Name", f11W, 1, 1, true));
            roomTable.AddCell(CellCenterFormat("Amount", f11W, 1, 1, true));
            roomTable.AddCell(CellCenterFormat("Price", f11W, 1, 1, true));

            //Load Rooms + Folio
            foreach (ROOM_BOOKED obj in reservation.ROOM_BOOKED)
            {
                var folio = obj.FOLIOs.ToArray();
                if (folio.Length > 0)
                {
                    //Room Name
                    roomTable.AddCell(CellLeftFormat(obj.ROOM.name + "\n" + obj.ROOM.ROOMTYPE.name, f11, folio.Length + 1));

                    //Folio
                    int price = (int)folio[0].SERVICE.price;
                    roomTable.AddCell(CellLeftFormat(folio[0].SERVICE.name, f11));
                    roomTable.AddCell(CellCenterFormat(folio[0].amount.ToString(), f11));
                    roomTable.AddCell(CellRightFormat(SeparateThousands(price.ToString()), f11));

                    //List room type with the same name
                    var roomTypeList = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == obj.ROOM.ROOMTYPE.name).ToList();
                    int exactRoomPrice = CalculatorInvoice.ExactRoomPrice(roomTypeList, reservation.date_created.Value);
                    long roomTotal = exactRoomPrice * CalculatorInvoice.TotalNumOfDays(reservation);
                    long total = roomTotal + CalculatorInvoice.FolioTotalOfRoom(folio);

                    //Price/Day
                    roomTable.AddCell(CellRightFormat(SeparateThousands(
                        CalculatorInvoice.ExactRoomPrice(obj.room_id, reservation.date_created.Value).ToString()), f11, folio.Length));

                    for (int i = 1; i < folio.Length; i++)
                    {
                        //folio
                        roomTable.AddCell(CellLeftFormat(folio[i].SERVICE.name, f11));
                        roomTable.AddCell(CellCenterFormat(folio[i].amount.ToString(), f11));

                        price = (int)folio[i].SERVICE.price;
                        roomTable.AddCell(CellRightFormat(SeparateThousands(price.ToString()), f11));
                    }

                    roomTable.AddCell(CellLeftFormat("Total", f11B, 1, 2));
                    roomTable.AddCell(CellRightFormat(SeparateThousands(
                        CalculatorInvoice.FolioTotalOfRoom(folio).ToString()), f11B));
                    roomTable.AddCell(CellRightFormat(SeparateThousands(
                        CalculatorInvoice.RoomTotalMoney(obj.room_id, reservation).ToString()), f11B));
                }
                else
                {
                    roomTable.AddCell(CellLeftFormat(obj.ROOM.name + "\n" + obj.ROOM.ROOMTYPE.name, f11, 2));

                    //folio
                    roomTable.AddCell(CellLeftFormat("", f11));
                    roomTable.AddCell(CellCenterFormat("0", f11));
                    roomTable.AddCell(CellRightFormat("0", f11));

                    //List room type with the same name
                    var roomTypeList = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == obj.ROOM.ROOMTYPE.name).ToList();
                    int exactRoomPrice = CalculatorInvoice.ExactRoomPrice(roomTypeList, reservation.date_created.Value);

                    roomTable.AddCell(CellRightFormat(SeparateThousands(exactRoomPrice.ToString()), f11));

                    roomTable.AddCell(CellLeftFormat("Total", f11B, 1, 2));
                    roomTable.AddCell(CellRightFormat(SeparateThousands(CalculatorInvoice.FolioTotalOfRoom(folio).ToString()), f11B));
                    roomTable.AddCell(CellRightFormat(SeparateThousands(
                        CalculatorInvoice.RoomTotalMoney(obj.room_id, reservation).ToString()), f11B));
                }    
                
            }
            PdfPCell cell = new PdfPCell(new Phrase("Total  ", f11B));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = 0;
            cell.Colspan = 3;

            roomTable.AddCell(cell);
            roomTable.AddCell(CellRightFormat(SeparateThousands(CalculatorInvoice.FolioTotalMoney(reservation).ToString()), f11B));
            roomTable.AddCell(CellRightFormat(SeparateThousands(CalculatorInvoice.RoomsTotalMoney(reservation).ToString()), f11B));

            pdfDoc.Add(roomTable);
            pdfDoc.Add(new Paragraph("                                  ", f11));
        }

        public void FormatChargesAndTotalMoney(Document pdfDoc, RESERVATION reservation)
        {
            var invoice = DataProvider.Instance.DB.INVOICEs.SingleOrDefault(x => x.reservation_id == reservation.id);
            float[] widths = new float[] { 1.1f, 0.2f}; //length = num of columns
            PdfPTable chargesTable = new PdfPTable(widths);
            chargesTable.WidthPercentage = 100;

            int sumRoomPrice = CalculatorInvoice.TotalRoomPriceOfReservation(reservation);
            double earlyFeeMoney = 0;
            double lateFeeMoney = 0;
            double surchargeMoney = 0;
            long totalMoney = 0;

            if (invoice != null)
            {
                earlyFeeMoney = (invoice.early_checkin_fee.Value * sumRoomPrice) / 100;
                lateFeeMoney = (invoice.late_checkout_fee.Value * sumRoomPrice) / 100;
                surchargeMoney = (invoice.surcharge.Value * CalculatorInvoice.TotalMoneyNoFee(reservation)) / 100;

                totalMoney = (long)invoice.total_money;
            }
            else
            {
                var charges = DataProvider.Instance.DB.CHARGES.First();
                if (reservation.early_checkin.Value)
                {
                    earlyFeeMoney = (charges.early_checkin_fee.Value * sumRoomPrice) / 100;
                }
                if (reservation.late_checkout.Value)
                {
                    lateFeeMoney = (charges.late_checkout_fee.Value * sumRoomPrice) / 100;
                }

                surchargeMoney = (charges.surcharge.Value * CalculatorInvoice.TotalMoneyNoFee(reservation)) / 100;

                totalMoney = CalculatorInvoice.TotalMoneyWithFee(reservation);
            }

            chargesTable.AddCell(CellRightNoBorder("Early checkin fee:", f11G));
            chargesTable.AddCell(CellRightNoBorder(SeparateThousands(((int)earlyFeeMoney).ToString()), f11));

            chargesTable.AddCell(CellRightNoBorder("Late checkout fee:", f11G));
            chargesTable.AddCell(CellRightNoBorder(SeparateThousands(((int)lateFeeMoney).ToString()), f11));

            chargesTable.AddCell(CellRightNoBorder("Surcharge:", f11G));
            chargesTable.AddCell(CellRightNoBorder(SeparateThousands(((int)surchargeMoney).ToString()), f11));

            chargesTable.AddCell(CellRightNoBorder("Total Money:", f11G));
            chargesTable.AddCell(CellRightNoBorder(SeparateThousands(totalMoney.ToString()), f11));

            pdfDoc.Add(chargesTable);
            pdfDoc.Add(new Paragraph("                                  ", f11));
        }

        public void FormatFooter(Document pdfDoc)
        {
            float[] widths = new float[] { 1f, 1f, 1f }; //length = num of columns
            PdfPTable footerTable = new PdfPTable(widths);
            footerTable.WidthPercentage = 100;

            footerTable.AddCell(CellCenterNoBorder("Thank you for staying with us BTNQ Hotel!", f11, 1, 3));
            footerTable.AddCell(CellCenterNoBorder(" ", f11, 1, 3));
            footerTable.AddCell(CellCenterNoBorder(" ", f11, 1, 2));
            footerTable.AddCell(CellCenterNoBorder(DateTime.Now.ToString("dddd, dd MMMM yyyy"), f11));
            footerTable.AddCell(CellCenterNoBorder("Guest Signature", f11B));
            footerTable.AddCell(CellCenterNoBorder("", f11B));
            footerTable.AddCell(CellCenterNoBorder("Cashier Signature", f11B));
            footerTable.AddCell(CellCenterNoBorder(" ", f11, 1, 3));
            footerTable.AddCell(CellCenterNoBorder(" ", f11, 1, 3));
            footerTable.AddCell(CellCenterNoBorder(" ", f11, 1, 3));
            footerTable.AddCell(CellCenterNoBorder(" ", f11, 1, 3));
            footerTable.AddCell(CellCenterNoBorder(" ", f11, 1, 3));
            footerTable.AddCell(CellCenterNoBorder(" ", f11, 1, 3));

            pdfDoc.Add(footerTable);
            pdfDoc.Add(Saperator());
        }
        #endregion

        #region Utilities
        public Paragraph Saperator()
        {
            Paragraph saperator = new Paragraph("---------------------------------------------------------------", f11G);
            saperator.Alignment = Element.ALIGN_CENTER;
            return saperator;
        }

        //No border cell
        PdfPCell CellCenterNoBorder(string content, Font font, int rowSpan = 1, int colSpan = 1)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            cell.Colspan = colSpan;
            cell.Rowspan = rowSpan;

            return cell;
        }

        PdfPCell CellLeftNoBorder(string content, Font font, int rowSpan = 1, int colSpan = 1)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, font));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            cell.Colspan = colSpan;
            cell.Rowspan = rowSpan;

            return cell;
        }

        PdfPCell CellRightNoBorder(string content, Font font, int rowSpan = 1, int colSpan = 1)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, font));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            cell.Colspan = colSpan;
            cell.Rowspan = rowSpan;

            return cell;
        }

        //adjust background cell
        PdfPCell CellCenterFormat(string content, Font font, int rowSpan = 1, int colSpan = 1, bool isBackground = false)
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

        PdfPCell CellLeftFormat(string content, Font font, int rowSpan = 1, int colSpan = 1, bool isBackground = false)
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

        PdfPCell CellRightFormat(string content, Font font, int rowSpan = 1, int colSpan = 1, bool isBackground = false)
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
