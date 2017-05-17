using System;
namespace WebApplication.Models
{
    public class BookingHistory
    {
        public int? page { get; set; }
        public int? forAjax { get; set; }
        public Int32? SNo { get; set; }
        public Int32 RecordCount { get; set; }
        public String Show_Id { get; set; }
        public String Movie_Name { get; set; }
        public Decimal Ticket_Id { get; set; }
        public Decimal Ticket_No { get; set; }
        public String User_Id { get; set; }
        public String Show_Date { get; set; }
        public String Booking_Date { get; set; }
        public String URL { get; set; }
        public String Show_Time { get; set; }
    }
}