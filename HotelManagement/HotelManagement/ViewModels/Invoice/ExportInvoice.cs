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

                FormatMainGuestInformation(pdfDoc, reservation);

                FormatInvoiceInformation(pdfDoc, reservation);

                FormatListRoom_Folio(pdfDoc, reservation);

                FormatChargesAndTotalMoney(pdfDoc, reservation);

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

        public void FormatMainGuestInformation(Document pdfDoc, RESERVATION reservation)
        {
            var mainGuest = DataProvider.Instance.DB.GUESTs.Where(x => x.id == reservation.main_guest).SingleOrDefault();

            Paragraph invoiceFor = new Paragraph();
            invoiceFor.Add(Chunk.TABBING);
            invoiceFor.Add(new Chunk("Invoice For: ", f11G));
            invoiceFor.Add(Chunk.TABBING);
            invoiceFor.Add(new Chunk(mainGuest.name, f11B));

            Paragraph idAndPhone = new Paragraph();
            idAndPhone.Add(Chunk.TABBING);
            idAndPhone.Add(new Chunk("ID Card: ", f11G));
            idAndPhone.Add(Chunk.TABBING);
            idAndPhone.Add(new Chunk(mainGuest.id, f11));
            idAndPhone.Add(Chunk.TABBING);
            idAndPhone.Add(Chunk.TABBING);
            idAndPhone.Add(Chunk.TABBING);
            idAndPhone.Add(Chunk.TABBING);
            idAndPhone.Add(Chunk.TABBING);
            idAndPhone.Add(new Chunk("Phone: ", f11G));
            idAndPhone.Add(Chunk.TABBING);
            idAndPhone.Add(new Chunk(mainGuest.phone, f11));

            Paragraph email = new Paragraph();
            email.Add(Chunk.TABBING);
            email.Add(new Chunk("Email: ", f11G));
            email.Add(Chunk.TABBING);
            email.Add(Chunk.TABBING);
            email.Add(new Chunk(mainGuest.email, f11));

            pdfDoc.Add(invoiceFor);
            pdfDoc.Add(idAndPhone);
            pdfDoc.Add(email);
            pdfDoc.Add(new Phrase("                         ", f11));
        }

        public void FormatInvoiceInformation(Document pdfDoc, RESERVATION reservation)
        {
            Paragraph invoiceId = new Paragraph();
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(new Chunk("Invoice Id: ", f11G));
            invoiceId.Add(Chunk.TABBING);
            invoiceId.Add(new Chunk(reservation.id.ToString(), f15B));
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
            timeStay.Add(new Chunk(reservation.arrival.Value.ToString(), f11));
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(new Chunk("Departure: ", f11G));
            timeStay.Add(Chunk.TABBING);
            timeStay.Add(new Chunk(reservation.departure.Value.ToString(), f11));

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
            totalDays.Add(new Chunk(CalculateTotalNumOfDays(reservation).ToString(), f11));

            pdfDoc.Add(invoiceId);
            pdfDoc.Add(timeStay);
            pdfDoc.Add(totalDays);
            pdfDoc.Add(Saperator());
        }

        public void FormatListRoom_Folio(Document pdfDoc, RESERVATION reservation)
        {
            float[] widths = new float[] { 1.3f, 3f, 0.7f, 1f, 1f, 1f }; //length = num of columns
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

            //Load Rooms + Folio
            foreach (ROOM_BOOKED obj in reservation.ROOM_BOOKED)
            {
                var folio = obj.FOLIOs.ToArray();
                if (folio.Length > 0)
                {
                    for (int i = 0; i < folio.Length; i++)
                    {
                        if (i == 0)
                        {
                            roomTable.AddCell(CellLeftFormat(obj.ROOM.name + "\n\n" + obj.ROOM.ROOMTYPE.name, f11, folio.Length));
                        }    

                        //folio
                        roomTable.AddCell(CellLeftFormat(folio[i].SERVICE.name, f11));
                        roomTable.AddCell(CellCenterFormat(folio[i].amount.ToString(), f11));

                        int price = (int)folio[i].SERVICE.price;
                        roomTable.AddCell(CellRightFormat(SeparateThousands(price.ToString()), f11));

                        if (i == 0)
                        {
                            //List room type with the same name
                            var roomTypeList = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == obj.ROOM.ROOMTYPE.name).ToList();
                            int exactRoomPrice = GetExactRoomPriceOfReservation(roomTypeList, reservation.date_created.Value);

                            roomTable.AddCell(CellRightFormat(SeparateThousands(exactRoomPrice.ToString()), f11, folio.Length));

                            long total = exactRoomPrice * CalculateTotalNumOfDays(reservation) + CalculateFolioTotalOfRoom(folio);
                            roomTable.AddCell(CellRightFormat(SeparateThousands(total.ToString()), f11, folio.Length));
                        }
                    }
                }
                else
                {
                    roomTable.AddCell(CellLeftFormat(obj.ROOM.name + "\n\n" + obj.ROOM.ROOMTYPE.name, f11));

                    //folio
                    roomTable.AddCell(CellLeftFormat("", f11));
                    roomTable.AddCell(CellCenterFormat("0", f11));
                    roomTable.AddCell(CellRightFormat("0", f11));

                    //List room type with the same name
                    var roomTypeList = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == obj.ROOM.ROOMTYPE.name).ToList();
                    int exactRoomPrice = GetExactRoomPriceOfReservation(roomTypeList, reservation.date_created.Value);

                    roomTable.AddCell(CellRightFormat(SeparateThousands(exactRoomPrice.ToString()), f11));
                    long total = exactRoomPrice * CalculateTotalNumOfDays(reservation);
                    roomTable.AddCell(CellRightFormat(SeparateThousands(total.ToString()), f11));
                }    
                
            }

            pdfDoc.Add(roomTable);
            pdfDoc.Add(Saperator());
        }

        public void FormatChargesAndTotalMoney(Document pdfDoc, RESERVATION reservation)
        {
            var invoice = DataProvider.Instance.DB.INVOICEs.SingleOrDefault(x => x.reservation_id == reservation.id);

            Paragraph earlyCheckinFee = new Paragraph();
            earlyCheckinFee.Add(Chunk.TABBING);
            earlyCheckinFee.Add(new Chunk("Early checkin fee: ", f11G));

            Paragraph lateCheckoutFee = new Paragraph();
            lateCheckoutFee.Add(Chunk.TABBING);
            lateCheckoutFee.Add(new Chunk("Late checkout fee: ", f11G));

            Paragraph surcharge = new Paragraph();
            surcharge.Add(Chunk.TABBING);
            surcharge.Add(new Chunk("Surcharge: ", f11G));
            surcharge.Add(Chunk.TABBING);

            Paragraph totalMoney = new Paragraph();
            totalMoney.Add(Chunk.TABBING);
            totalMoney.Add(new Chunk("Total Money: ", f11G));
            totalMoney.Add(Chunk.TABBING);
            totalMoney.Add(Chunk.TABBING);

            int sumRoomPrice = SumRoomPrice(reservation);

            //Insert values
            if (invoice != null)
            {
                double feeMoney = invoice.early_checkin_fee.Value * sumRoomPrice;
                earlyCheckinFee.Add(new Chunk(SeparateThousands(feeMoney.ToString()), f11));

                feeMoney = invoice.late_checkout_fee.Value * sumRoomPrice;
                lateCheckoutFee.Add(new Chunk(SeparateThousands(feeMoney.ToString()), f11));

                surcharge.Add(new Chunk(invoice.surcharge.ToString(), f11));

                long money = (long)invoice.total_money;
                totalMoney.Add(new Chunk(SeparateThousands(money.ToString()), f11));
            }
            else
            {
                if (reservation.early_checkin.Value)
                {
                    double feeMoney = DataProvider.Instance.DB.CHARGES.First().early_checkin_fee.Value * sumRoomPrice;
                    earlyCheckinFee.Add(new Chunk(SeparateThousands(feeMoney.ToString()), f11));
                }
                if (reservation.late_checkout.Value)
                {
                    double feeMoney = DataProvider.Instance.DB.CHARGES.First().late_checkout_fee.Value * sumRoomPrice;
                    lateCheckoutFee.Add(new Chunk(SeparateThousands(feeMoney.ToString()), f11));
                }

                double surMoney = DataProvider.Instance.DB.CHARGES.First().late_checkout_fee.Value * sumRoomPrice;
                surcharge.Add(new Chunk(DataProvider.Instance.DB.CHARGES.First().surcharge.ToString(), f11));
            }

            pdfDoc.Add(earlyCheckinFee);
            pdfDoc.Add(lateCheckoutFee);
            pdfDoc.Add(surcharge);
            pdfDoc.Add(totalMoney);
        }
        #endregion

        #region Utilities
        public Paragraph Saperator()
        {
            Paragraph saperator = new Paragraph("---------------------------------------------------------------", f11G);
            saperator.Alignment = Element.ALIGN_CENTER;
            return saperator;
        }

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

        int CalculateTotalNumOfDays(RESERVATION reservation)
        {
            return (int)(reservation.departure.Value - reservation.arrival.Value).TotalDays;
        }

        int GetExactRoomPriceOfReservation(List<ROOMTYPE> roomTypeList, DateTime dateCreated)
        {
            List<ROOMTYPE> sortList = roomTypeList.OrderBy(x => x.id).ToList();
            foreach (var item in sortList)
            {
                if (item.date_created <= dateCreated)
                {
                    if (item.date_updated.HasValue)
                    {
                        if (dateCreated <= item.date_updated)
                        {
                            return (int)item.price;
                        }
                    }
                    else
                    {
                        return (int)item.price;
                    }
                }
            }
            return 0;
        }

        int CalculateFolioTotalOfRoom(FOLIO[] folio)
        {
            int sum = 0;
            for (int i = 0; i < folio.Length; i++)
            {
                sum = sum + (int)folio[i].SERVICE.price * folio[i].amount.Value;
            }
            return sum;
        }

        int CalculateFolioTotal(RESERVATION reservation)
        {
            int sum = 0;
            foreach (var roomBooked in reservation.ROOM_BOOKED)
            {
                
            }
            return sum;
        }

        int SumRoomPrice(RESERVATION reservation)
        {
            int sum = 0;
            foreach(var roomBooked in reservation.ROOM_BOOKED)
            {
                //List room type with the same name
                var roomTypeList = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == roomBooked.ROOM.ROOMTYPE.name).ToList();
                int exactRoomPrice = GetExactRoomPriceOfReservation(roomTypeList, reservation.date_created.Value);
                sum = sum + exactRoomPrice;
            }
            return sum;
        }
        #endregion
    }
}
