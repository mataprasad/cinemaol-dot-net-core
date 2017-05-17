using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication.Services;
using WebApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Controllers
{
    public class UserProfileController : BaseController
    {
        public UserProfileController(IOptions<GlobalOption> globalOptions) : base(globalOptions)
        {

        }

        [Authorize]
        public IActionResult UserHome()
        {
            var user_info = Session.Get<UserInfo>("UserInfo");
            return View(user_info);
        }
        [Authorize]
        public IActionResult EditProfile()
        {
            List<SelectListItem> dataList = new List<SelectListItem>();
            dataList.Insert(0, new SelectListItem() { Value = "0", Text = "<-- SELECT STATE -->" });
            foreach (var item in _dbAccess.GetAllStates())
            {
                dataList.Add(new SelectListItem() { Text = Convert.ToString(item), Value = Convert.ToString(item) });
            }
            ViewBag.States = dataList;
            var user_info = Session.Get<UserInfo>("UserInfo");
            return View(user_info);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditProfile([FromForm]UserInfo obj)
        {
            try
            {
                if (Session["UserInfo"] != null)
                {
                    var user_info = Session.Get<UserInfo>("UserInfo");
                    obj.User_LoginName = user_info.User_LoginName;
                    obj.User_LoginPassword = user_info.User_LoginPassword;
                    if (_dbAccess.SpUpdateUserInfo(obj))
                    {
                        UserInfo dataTable = _dbAccess.SpUserLogin(new VMLogin() { txtLoginId = obj.User_LoginName, txtLoginPass = obj.User_LoginPassword });
                        if (dataTable != null)
                        {
                            Session["UserInfo"] = dataTable;
                            Session["IsAdmin"] = null;
                        }
                        TempData["Msg"] = "Updated successfully.";
                        return RedirectToAction("EditProfile");
                    }
                    else
                    {
                        throw new Exception("Oops some error occured !");
                    }
                }
                else
                {
                    throw new Exception("No user is logged in !");
                }
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error. ## " + ex.Message;
                return RedirectToAction("EditProfile");
            }
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword([FromForm]VMPwdReset obj)
        {
            try
            {
                if (Session["UserInfo"] != null)
                {
                    var user_info = Session.Get<UserInfo>("UserInfo");
                    if (user_info.User_LoginPassword.ToString() == obj.txtCurrentPass.Trim())
                    {
                        user_info.User_LoginPassword = obj.txtNewRePass.Trim();
                        if (_dbAccess.SpChangePassword(user_info))
                        {
                            TempData["Msg"] = "Password Changed Successfully.";
                            return RedirectToAction("ChangePassword");
                        }
                        else
                        {
                            throw new Exception("Oops some problems occured !");
                        }
                    }
                    else
                    {
                        throw new Exception("Current password is not correct !");
                    }
                }
                else
                {
                    throw new Exception("No user is logged in !");
                }

            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error. ## " + ex.Message;
                return RedirectToAction("ChangePassword");
            }
        }

        [Authorize]
        public IActionResult BookingHistory([FromForm]BookingHistory obj)
        {
            return BookingHistoryInner(obj);
        }
   [Authorize]
        public IActionResult BookingHistoryAjax([FromQuery]BookingHistory obj)
        {
            return BookingHistoryInner(obj);
        }

        private IActionResult BookingHistoryInner(BookingHistory obj)
        {
            if (obj == null)
            {
                obj = new BookingHistory();
            }
            var user_info = Session.Get<UserInfo>("UserInfo");
            obj.page = (obj.page ?? 1);
            var data = _dbAccess.SpBookingHistoryWithPaging(user_info.User_Id, 10, obj.page.Value);
            ViewBag.RecordsCount = data.LastOrDefault().RecordCount;
            ViewBag.PageNo = (obj.page ?? 1);
            if (obj.forAjax == null)
            {
                return View(data.Take(data.Count - 1).ToList());
            }
            else
            {
                return PartialView("_MovieHistory", data.Take(data.Count - 1).ToList());
            }
        }

        [Authorize]
        public IActionResult BookingDetail(Int32 id)
        {
            var data = _dbAccess.SpGetTicketHistoryDetial(id);
            return View(data);
        }
    }
}