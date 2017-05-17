using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using WebApplication.Services;
using WebApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;

namespace WebApplication.Controllers
{
    public class PublicController : BaseController
    {
        public PublicController(IOptions<GlobalOption> globalOptions) : base(globalOptions)
        {

        }
        public IActionResult Index()
        {
            ViewBag.SlidSowData = _dbAccess.SpGetMoviesImageURL();
            ViewBag.RunningMovies = _dbAccess.FillMovieList();
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            Session["UserInfo"] = null;
            Session["IsAdmin"] = null;
            Session.Clear();
            await HttpContext.Authentication.SignOutAsync("MyCookieMiddlewareInstance");
            ViewBag.Error = "Logout successfully !";
            return View("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact([FromForm]VMFeedback formData)
        {
            try
            {
                string mail = "";
                mail += "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title></title></head><body><div style=\"border: 1px solid #993333; width: 450px; height: 350px; background-color: #CCCCCC;\"><table  style=\"width:100%\"><tr><td style=\"width: 58px;\">Name :</td><td>";
                mail += formData.txtContact.Trim().ToUpper();
                mail += "</td></tr><tr><td style=\"width: 58px;\">Address :</td><td>";
                mail += formData.txtAddress.Trim().ToUpper();
                mail += "</td></tr><tr><td style=\"width: 58px;\">Contact :</td><td>";
                mail += formData.txtContact.Trim().ToUpper();
                mail += "</td></tr><tr><td style=\"width: 58px;\">Email :</td><td>";
                mail += formData.txtEmail.Trim().ToUpper();
                mail += "</td></tr><tr><td style=\"width: 58px;\">Views :</td><td>";
                mail += formData.txtViews.Trim().ToUpper();
                mail += "</td></tr></table></div></body></html>";
                try
                {
                    //MailGmail.SendEmail("cinemaol.asct@gmail.com", "CONCERNS/FEEDBACK", mail, "");
                    // MailGmail.SendEmail(obj.txtEmail"].Trim().ToLower(), "CinemaOL", "Thank you for visiting CinemaOl !", "");
                }
                catch (Exception)
                {
                }

                ViewBag.Msg = "Thanx for your feedback.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Msg = "Error. ## " + ex.Message;
                return View();
            }

        }

        [HttpPost]
        public IActionResult Login([FromForm]VMLogin obj)
        {
            UserInfo dataTable = _dbAccess.SpUserLogin(obj);
            if (dataTable != null)
            {
                LoginInner(dataTable);
                Session["UserInfo"] = dataTable;
                Session["IsAdmin"] = null;
            }
            else
            {
                ViewBag.Error = "Invalid user name or/ and password !";
                return View("Login");
            }
            //System.Web.Security.FormsAuthentication.RedirectFromLoginPage("LoggedUser", false);
            return RedirectToAction("Index");
        }
        public IActionResult LoginAdmin()
        {
            ViewBag.ForAdmin = true;
            return View("Login");
        }

        [HttpPost]
        public IActionResult LoginAdmin([FromForm]VMLogin obj)
        {
            UserInfo dataTable = _dbAccess.SpUserLogin(obj, true);
            if (dataTable != null)
            {
                LoginInner(dataTable,"Admin");
                Session["UserInfo"] = dataTable;
                Session["IsAdmin"] = true;
            }
            else
            {
                ViewBag.Error = "Invalid user name or/ and password !";
                ViewBag.ForAdmin = true;
                return View("Login");
            }
            //System.Web.Security.FormsAuthentication.RedirectFromLoginPage("AdminUser", false);
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult MakeShowSelection([FromForm]VMSelectShowPost obj)
        {
            //return Content(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
            try
            {
                if (Session["UserInfo"] == null)
                {
                    TempData["Msg"] = "You must have to looged in before you book a ticket.";
                    return RedirectToAction("Login");
                }
                else
                {
                    Session["ShowId"] = _dbAccess.GetShowId(obj);
                    return RedirectToAction("SelectSheats", "TicketBooking");
                }
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error. ## " + ex.Message;
                return RedirectToAction("Login");
            }
        }

        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            ViewBag.Error = feature?.Error.InnerException.StackTrace;
            return View();
        }

        public IActionResult Movie()
        {
            return View(_dbAccess.GetRunningMovies());
        }

        public IActionResult MovieUpComming()
        {
            return View(_dbAccess.GetUpCommingMovies());
        }

        public IActionResult Captcha()
        {
            string strString = "ABCDEFGHJKLMNOPQRSTUVWXYZ23456789";
            Random random = new Random();
            int randomCharIndex = 0;

            string captcha = "";
            for (int i = 0; i < 7; i++)
            {
                randomCharIndex = random.Next(0, strString.Length);
                captcha += Convert.ToString(strString[randomCharIndex]);
            }
            Session["CAPTCHA_CODE"] = captcha.Trim().ToUpper();
            return Content(captcha.Trim().ToUpper());
        }

        public IActionResult ResetPass()
        {
            ViewBag.Step = 1;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPass([FromForm]VMPwdReset formData)
        {
            UserInfo dt = null;
            switch (Convert.ToInt32(formData.Step))
            {
                case 1:
                    dt = _dbAccess.SpCheckUser(formData.txtUserName.Trim().ToUpper());
                    if (dt != null)
                    {
                        Session["UserCheck"] = dt;
                        ViewBag.SQ = dt.User_SQ.ToString().ToUpper();
                        ViewBag.Step = 2;
                    }
                    else
                    {
                        ViewBag.Step = 1;
                        ViewBag.Msg = "No user exists with this user name.";
                    }
                    break;
                case 2:
                    dt = Session.Get<UserInfo>("UserCheck");
                    if (dt.User_SA.ToString().ToUpper() == formData.txtSA.Trim().ToUpper())
                    {
                        ViewBag.Step = 3;
                    }
                    else
                    {
                        ViewBag.SQ = dt.User_SQ.ToString().ToUpper();
                        ViewBag.Step = 2;
                        ViewBag.Msg = "Security answer is wrong.";
                    }
                    break;
                case 3:
                    dt = Session.Get<UserInfo>("UserCheck");
                    dt.User_LoginPassword = formData.txtRePass.Trim();
                    if (_dbAccess.SpChangePassword(dt))
                    {
                        ViewBag.Msg = "Password reset successfully.";
                        ViewBag.Step = 1;
                    }
                    else
                    {
                        ViewBag.Step = 3;
                        ViewBag.Msg = "Error .## Oops some problems occured !";
                    }

                    break;
                default:
                    ViewBag.Step = 1;
                    break;
            }
            return View();
        }

        public IActionResult Register()
        {
            var data = _dbAccess.GetAllStates();
            List<SelectListItem> dataList = new List<SelectListItem>();
            dataList.Insert(0, new SelectListItem() { Value = "0", Text = "<-- SELECT STATE -->" });
            foreach (var item in data)
            {
                dataList.Add(new SelectListItem() { Text = item, Value = item });
            }
            return View(dataList);
        }

        [HttpPost]
        public IActionResult Register([FromForm]VMRegister obj)
        {
            try
            {
                if (!_dbAccess.IsUserExists(obj.txtUName))
                {
                    if (Session["CAPTCHA_CODE"].ToString().ToUpper().Equals(obj.txtVeriCode.Trim().ToUpper()))
                    {
                        if (_dbAccess.RegisterUser(obj))
                        {
                            var dt = _dbAccess.SpUserLogin(new VMLogin() { txtLoginId = obj.txtUName, txtLoginPass = obj.txtRePass });
                            Session["UserInfo"] = dt;
                            Session["IsAdmin"] = null;
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            throw new Exception("Some errors occurs, failed to register the user !");
                        }
                    }
                    else
                    {
                        TempData["Msg"] = "Verification code not macthed !";
                        return RedirectToAction("Register");
                    }
                }
                else
                {
                    TempData["Msg"] = "Sorry, this username is already taken !";
                    return RedirectToAction("Register");
                }
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error. ## " + ex.Message;
                return RedirectToAction("Register");
            }
        }

        private async void LoginInner(UserInfo dataTable,string role=null)
{
    const string Issuer = "https://contoso.com";
    var claims = new List<Claim>();
    claims.Add(new Claim(ClaimTypes.Name, dataTable.User_LoginName, ClaimValueTypes.String, Issuer));
    if(!String.IsNullOrWhiteSpace(role))
    {
    claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String, Issuer));
    }
    var userIdentity = new ClaimsIdentity("MyCookieMiddlewareInstance");
    userIdentity.AddClaims(claims);
    var userPrincipal = new ClaimsPrincipal(userIdentity);

    await HttpContext.Authentication.SignInAsync("MyCookieMiddlewareInstance", userPrincipal,
        new AuthenticationProperties
        {
            //ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
            IsPersistent = false,
            AllowRefresh = false
        });
		
		
		
	// System.Security.Principal.GenericIdentity i=new System.Security.Principal.GenericIdentity(dataTable.User_LoginName);
    //             System.Security.Principal.GenericPrincipal p = new System.Security.Principal.GenericPrincipal(i, "CasualUser".Split(','));
    //             System.Security.Claims.ClaimsPrincipal principal=new System.Security.Claims.ClaimsPrincipal(p);
    //             await HttpContext.Authentication.SignInAsync("MyCookieMiddlewareInstance", principal);
}
    }
}
