using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quote.WebAPI.MongoDB.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }


        [Route("quote")]
        [Route("quote/index")]
        [Route("quote/create")]
        [Route("quote/edit")]
        [Route("quote/delete")]
        [Route("quote/detail")]
        public ActionResult Contact()
        {
            return View();
        }
    }
}