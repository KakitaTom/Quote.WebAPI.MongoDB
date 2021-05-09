using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Quote.WebAPI.MongoDB.Controllers
{
    public class QuoteController : Controller
    {
        private IMongoCollection<Models.Quote> _db;

        public QuoteController()
        {
            // Local Robo 3T
//            string constr = ConfigurationManager.AppSettings["connectionString"];
//            var Client = new MongoClient(constr);


            // MongoDB Cluster
            var Client = new MongoClient("mongodb+srv://Admin:Admin@quotecluster.xqmtl.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");

            // Object Data
            _db = Client.GetDatabase("QuoteAPI").GetCollection<Models.Quote>("Quote");
        }

        [System.Web.Mvc.Route("cdtn/index")]
        [System.Web.Mvc.Route("cdtn/all")]
        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            return View(_db.Find(new BsonDocument()).ToList());
        }

        [System.Web.Mvc.Route("cdtn/create")]
        [System.Web.Mvc.HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Create(Models.Quote quote)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            _db.InsertOneAsync(quote);

            return RedirectToAction("Index", "Quote", null);
        }

        [System.Web.Mvc.Route("cdtn/edit/{id}")]
        [System.Web.Mvc.HttpGet]
        public ActionResult Edit(string Id)
        {
            var quoteInDb = _db.Find(new BsonDocument()).ToList().SingleOrDefault(q => q.Id.Equals(Id));

            if (quoteInDb == null)
            {
                return HttpNotFound();
            }

            return View(quoteInDb);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Edit(Models.Quote quote)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            _db.FindOneAndUpdateAsync(Builders<Models.Quote>
                    .Filter
                    .Eq("Id", quote.Id),
                Builders<Models.Quote>
                    .Update
                    .Set("Text", quote.Text)
                    .Set("Author", quote.Author));

            return RedirectToAction("Index", "Quote", null);
        }

        [System.Web.Mvc.Route("cdtn/delete/{Id}")]
        public ActionResult Delete(string Id)
        {
            var quoteInDb = _db.Find(new BsonDocument()).ToList().SingleOrDefault(q => q.Id.Equals(Id));

            if (quoteInDb == null)
            {
                return HttpNotFound();
            }

            _db.DeleteOneAsync(Builders<Models.Quote>.Filter.Eq("Id", Id));

            return RedirectToAction("Index", "Quote", null);
        }
    }
}