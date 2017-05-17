using System;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication.Services;
using WebApplication.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Controllers
{
    public class TicketBooking : BaseController
    {
        public TicketBooking(IOptions<GlobalOption> globalOptions) : base(globalOptions)
        {

        }

        private Hashtable _hashtableSelectSheats = new Hashtable();

        [Authorize]
        public ActionResult SelectSheats()
        {
            return View(_dbAccess.SelectSheats(Session.Get<Int32>("ShowId")));
        }

        [Authorize]
        public ActionResult ConfirmSelection([FromForm] VMSheatSelection formData)
        {
            ShowDetail dataTable = _dbAccess.SelectShow(formData.ShowId);
            ViewBag.SelectedSheats = formData.chkSheats;
            ViewBag.ShowId = formData.ShowId;
            var _hfDynamic = "";
            System.Collections.Hashtable _hashtableSelectSheats = new System.Collections.Hashtable();
            foreach (string sheat in formData.chkSheats.Split(','))
            {
                string initial = sheat.ToString().Substring(0, 1);
                if (initial == "A" || initial == "B" || initial == "C")
                {
                    _hashtableSelectSheats.Add(sheat, "100");
                }
                else if (initial == "D" || initial == "E" || initial == "F")
                {
                    _hashtableSelectSheats.Add(sheat, "175");
                }
                else if (initial == "G" || initial == "H" || initial == "I")
                {
                    _hashtableSelectSheats.Add(sheat, "225");
                }
            }
            decimal totalCost = 0;
            foreach (string item in _hashtableSelectSheats.Keys)
            {
                totalCost += Convert.ToDecimal(_hashtableSelectSheats[item]);
                _hfDynamic += "<tr><td align=\"center\" class=\"style2\" style=\"background-color: #FF9900\">" + item + "</td><td align=\"center\" style=\"background-color: #FF9900\">" + _hashtableSelectSheats[item].ToString() + " Rs.</td><td align=\"center\" style=\"background-color: #FF9900\">&nbsp;</td></tr>";
            }
            _hfDynamic += "<tr><td align=\"center\" class=\"style2\" style=\"background-color: #00FF00\"> Total Cost =  </td><td align=\"center\" style=\"background-color: #00FF00\">" + totalCost + " Rs.</td><td align=\"center\" style=\"background-color: #00FF00\">&nbsp;</td></tr>";
            ViewBag._hfDynamic = _hfDynamic;
            ViewBag.totalCost = totalCost;
            ViewBag.totalCount = _hashtableSelectSheats.Count;
            Session["_hashtableSelectSheats"] = _hashtableSelectSheats;
            return View(dataTable);
        }

        [Authorize]
        [HttpPost]
        public ActionResult BookTicket([FromForm] VMSheatSelection formData)
        {
            int tktId = -1;
            int tktNo = -1;
            var user = Session.Get<UserInfo>("UserInfo");
            _hashtableSelectSheats = Session.Get<Hashtable>("_hashtableSelectSheats");
            _dbAccess.BookTicket(Convert.ToDecimal(formData.totalCost), Convert.ToInt32(formData.ShowId), user.User_Id, Convert.ToInt32(formData.totalCount), out tktId, out tktNo);
            _dbAccess.AddTicketDetial(Convert.ToInt32(formData.ShowId), tktId, _hashtableSelectSheats);
            var data = _dbAccess.SpGetTicketHistoryDetial(tktId);
            return View(data);
        }


        [Authorize]
        public ActionResult PrintBookedTicket(String idT)
        {
            TempData["PrintOnLoad"] = true;
            return RedirectToAction("BookingDetail", "UserProfile", new { id = idT });
        }
    }
}