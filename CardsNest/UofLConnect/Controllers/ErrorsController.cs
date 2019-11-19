using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UofLConnect.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult General()
        {
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            ViewBag.ErrorMessage = "An unexpected error occured.";

            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;

            return View();
        }

        public ActionResult NotFound()
        {
            ViewBag.ErrorMessage = "The File or directory that you are looking for could not be found.";

            Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
            return View("Error");
        }

        public ActionResult NotAllowed( string message = "The action that is being attempted is not allowed")
        {
            ViewBag.ErrorMessage = message;

            Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
            return View("Error");
        }

        public ActionResult Internal()
        {
            ViewBag.ErrorMessage = "The system has experienced an unexpected error.";

            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return View("Error");
        }

        public ActionResult Forbidden()
        {
            ViewBag.ErrorMessage = "The file or directory you are accessing is not allowed on this web site.";

            Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            return View("Error");
        }
    }
}