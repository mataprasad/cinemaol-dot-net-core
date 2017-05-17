using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication.Services;
using WebApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Controllers
{
    [Authorize("Admin")]
    public class AdminController : BaseController
    {
        public AdminController(IOptions<GlobalOption> globalOptions) : base(globalOptions)
        {

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddMovie()
        {
            Dictionary<String, List<SelectListItem>> modelData = new Dictionary<String, List<SelectListItem>>();
            //------------01            
            modelData.Add("ddlLanguage", _dbAccess.GetMovieLanguages());
            //-----------02
            modelData.Add("IndustryList", _dbAccess.GetMovieIndustries());
            //---------------03
            modelData.Add("StatusList", _dbAccess.GetMovieStatuses());
            return View(modelData);
        }

        [HttpPost]
        public IActionResult AddMovie([FromForm]VMMovieInfo obj1)
        {
            try
            {
                MovieInfo obj = new MovieInfo();
                obj.fuPoster = obj1.fuPoster;
                obj.Movie_Casts = obj1.txtCasts;
                obj.Movie_Director = obj1.txtDirector;
                obj.Movie_Industry = obj1.ddlIndustry;
                obj.Movie_Language = obj1.ddlLanguage;
                obj.Movie_ReleaseDate = obj1.txtReleaseDate;
                obj.Movie_Status = obj1.ddlStatus;
                obj.Movie_Title = obj1.txtTitle;
                obj.Movie_ImageURL = string.Empty;

                var _filePath = string.Empty;

                if (obj.fuPoster != null && obj.fuPoster.Length > 0)
                {
                    if (this.ValidatePoster(obj.fuPoster.FileName, out _filePath))
                    {
                        obj.Movie_ImageURL = _filePath;
                        if (_dbAccess.SpAddNewMovie(obj, out _filePath))
                        {
                            _globalOption.SaveFile(obj.fuPoster, _filePath);
                            TempData["Msg"] = "Movie Added successfully.";
                        }
                    }
                }
                return RedirectToAction("AddMovie");
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error.## " + ex.Message;
                return RedirectToAction("AddMovie");
            }
        }

        public IActionResult ManageShow()
        {
            Dictionary<String, List<SelectListItem>> modelData = new Dictionary<String, List<SelectListItem>>();
            //------------01            
            modelData.Add("HallList", _dbAccess.GetHallList());
            //-----------02
            modelData.Add("TimeList", _dbAccess.GetTimeList());
            //---------------03
            modelData.Add("MovieList", _dbAccess.GetMovieList());
            return View(modelData);
        }

        [HttpPost]
        public IActionResult ManageShow([FromForm] VMManageShow obj)
        {
            try
            {
                if (_dbAccess.SpAddShowInfo(obj))
                {
                    TempData["Msg"] = "Show added successfully.";

                }
                return RedirectToAction("ManageShow");
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error.##" + ex.Message;
                return RedirectToAction("ManageShow");
            }

        }

        public IActionResult RemoveMovie()
        {
            return View(_dbAccess.SpGetMoviesToRemove());
        }

        [HttpPost]
        public IActionResult RemoveMovie([FromForm]String[] selectedMovies)
        {
            try
            {
                if (_dbAccess.SpRemoveMovie(selectedMovies.ToList()))
                {
                    TempData["Msg"] = "Movie removed successfully.";

                }
                return RedirectToAction("RemoveMovie");
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error.##" + ex.Message;
                return RedirectToAction("RemoveMovie");
            }
        }


        private Boolean ValidatePoster(String pURL, out string _filePath)
        {
            try
            {
                pURL = System.IO.Path.GetExtension(pURL).ToUpper();
                if (pURL == ".JPG" || pURL == ".JPEG" || pURL == ".PNG")
                {
                    _filePath = pURL;
                }
                else
                {
                    throw new Exception("Poster file must have .jpg or .jpeg or .png file extention only !");
                }
                _filePath = _filePath.ToLower();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}