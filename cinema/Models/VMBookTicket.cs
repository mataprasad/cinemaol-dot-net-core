using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
namespace WebApplication.Models
{
    public class VMBookTicket
    {
        public string Ticket_No { get; set; }
        public string Booking_Date { get; set; }
        public string Show_Date { get; set; }
        public string Show_Time { get; set; }
        public string Movie_Name { get; set; }
        public string Ticket_Id { get; set; }
        public List<SelectListItem> Sheats { get; set; }
    }
}