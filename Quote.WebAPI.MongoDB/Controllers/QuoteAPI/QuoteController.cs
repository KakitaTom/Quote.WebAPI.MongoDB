using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Quote.WebAPI.MongoDB.Controllers.QuoteAPI
{
    public class QuoteController : ApiController
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

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool isExistQuote(string Id)
        {
            var quote = _db.Find(new BsonDocument()).ToList().SingleOrDefault(q => q.Id.Equals(Id));

            if (quote != null) 
                return true;
            return false;
        }

        [Route("api/quote/all")]
        [HttpGet]
        public IHttpActionResult getAllQuotes()
        {
            return Ok(_db.Find(new BsonDocument()).ToList());
        }

        [Route("api/quote/random")]
        [HttpGet]
        public IHttpActionResult getRandomQuote()
        {
            return Ok(_db.Find(new BsonDocument()).ToList().OrderBy(q => Guid.NewGuid()).FirstOrDefault());
        }

        [Route("api/quote/random/{count}")]
        [HttpGet]
        public IHttpActionResult getRandomAmountQuotes(int count)
        {
            if (count == 1)
            {
                return getRandomQuote();
            }

            return Ok(_db.Find(new BsonDocument()).ToList().OrderBy(q => Guid.NewGuid()).Take(count));
        }



        //Danger Section



        //Tạo cho vui, không được sử dụng
        [Route("api/quote/detail/{id}")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult DetailQuote(string id)
        {
            if (!isExistQuote(id))
            {
                return NotFound();
            }

            return Ok(_db.Find(new BsonDocument()).ToList().SingleOrDefault(q => q.Id.Equals(id)));
        }



        //Tạo cho vui, không được sử dụng
        [Route("api/quote/create")]
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult CreateQuote(Models.Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _db.InsertOneAsync(quote);
            return Created(new Uri(Request.RequestUri + "/" + quote.Id), quote);
        }



        //Tạo cho vui, không được sử dụng
        [Route("api/quote/edit/{id}")]
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult EditQuote(Models.Quote quote)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!isExistQuote(quote.Id))
                return NotFound();

            _db.FindOneAndUpdateAsync(Builders<Models.Quote>.Filter.Eq("Id", quote.Id),
                Builders<Models.Quote>.Update.Set("Text", quote.Text).Set("Author", quote.Author));

            return Ok(_db.Find(new BsonDocument()).ToList().SingleOrDefault(q => q.Id.Equals(quote.Id)));
        }


        //Tạo cho vui, không được sử dụng
        [Route("api/quote/delete/{id}")]
        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult DeleteQuote(string id)
        {
            if (!isExistQuote(id))
                return NotFound();

            _db.DeleteOneAsync(Builders<Models.Quote>.Filter.Eq("Id", id));

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
