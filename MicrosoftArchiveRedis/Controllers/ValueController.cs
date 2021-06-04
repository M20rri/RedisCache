using MicrosoftArchiveRedis.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Threading.Tasks;
using MicrosoftArchiveRedis.Extension;

namespace MicrosoftArchiveRedis.Controllers
{
    public class ValueController : ApiController
    {
        #region Deal with strings

        [HttpGet, Route("api/GetValues")]
        public IHttpActionResult GetValues()
        {
            string result = "";

            //Step 1: Connect to redis server.
            using (ConnectionMultiplexer redisCon = ConnectionMultiplexer.Connect("10.10.1.235:6379,password=JYMe9j4sb7WNRvrG"))
            {
                //Step 2: Get the reference of the redis database using the redis connection.
                IDatabase redisDbConnection = redisCon.GetDatabase();

                if (!redisDbConnection.KeyExists("Name"))
                    redisDbConnection.StringSet("Name", "Mahmoud El Torri");

                result = redisDbConnection.StringGet("Name");

            }


            return Ok(result);
        }

        [HttpGet, Route("api/ChangeNameKey")]
        public IHttpActionResult ChangeNameKey([FromUri] string currentName)
        {

            using (ConnectionMultiplexer redisCon = ConnectionMultiplexer.Connect("10.10.1.235:6379,password=JYMe9j4sb7WNRvrG"))
            {
                //Step 2: Get the reference of the redis database using the redis connection.
                IDatabase redisDbConnection = redisCon.GetDatabase();


                redisDbConnection.StringSet("Name", currentName);
            }

            return Ok("Updated");
        }


        #endregion


        #region Deal With Lists (HashSet)


        [HttpGet, Route("api/SetAuthors")]
        public IHttpActionResult SetAuthors()
        {
            //Step 1: Connect to redis server.
            using (ConnectionMultiplexer redisCon = ConnectionMultiplexer.Connect("10.10.1.235:6379,password=JYMe9j4sb7WNRvrG"))
            {
                //Step 2: Get the reference of the redis database using the redis connection.
                IDatabase redisDbConnection = redisCon.GetDatabase();

                List<Author> authors = new List<Author>
            {
                    new Author { Id = 1, FirstName = "Joydip", LastName = "Kanjilal" },
                    new Author { Id = 2, FirstName = "Steve", LastName = "Smith" },
                    new Author { Id = 3, FirstName = "Anand", LastName = "Narayas"},
                    new Author { Id = 4, FirstName = "Dolores", LastName = "Abernathy"},
                    new Author { Id = 5, FirstName = "Maeve", LastName = "Millay"},
                    new Author { Id = 6, FirstName = "Bernard", LastName = "Lowe"},
                    new Author { Id = 7, FirstName = "ManIn", LastName = "Black"}
            };

                redisDbConnection.StringSet("authorsKey", JsonConvert.SerializeObject(authors));

            }


            return Ok("Data Saved");
        }

        [HttpGet, Route("api/GetAuthors")]
        public IHttpActionResult GetAuthors()
        {
            List<Author> result = new List<Author>();

            //Step 1: Connect to redis server.
            using (ConnectionMultiplexer redisCon = ConnectionMultiplexer.Connect("10.10.1.235:6379,password=JYMe9j4sb7WNRvrG"))
            {
                //Step 2: Get the reference of the redis database using the redis connection.
                IDatabase redisDbConnection = redisCon.GetDatabase();

                result = JsonConvert.DeserializeObject<List<Author>>(redisDbConnection.StringGet("authorsKey"));

            }

            return Ok(result);
        }


        [HttpPost, Route("api/SetWithTagsAuthors")]
        public async Task<IHttpActionResult> SetWithTagsAuthors([FromBody] Tag model)
        {
            string cacheKey = "cacheKeyWithTags";

            //Step 1: Connect to redis server.
            using (ConnectionMultiplexer redisCon = ConnectionMultiplexer.Connect("localhost"))
            {
                //Step 2: Get the reference of the redis database using the redis connection.
                IDatabase _redisDb = redisCon.GetDatabase();

                List<Author> authors = new List<Author>
            {
                    new Author { Id = 1, FirstName = "Joydip", LastName = "Kanjilal" },
                    new Author { Id = 2, FirstName = "Steve", LastName = "Smith" },
                    new Author { Id = 3, FirstName = "Anand", LastName = "Narayas"},
                    new Author { Id = 4, FirstName = "Dolores", LastName = "Abernathy"},
                    new Author { Id = 5, FirstName = "Maeve", LastName = "Millay"},
                    new Author { Id = 6, FirstName = "Bernard", LastName = "Lowe"},
                    new Author { Id = 7, FirstName = "ManIn", LastName = "Black"}
            };

                await _redisDb.KeyDeleteAsync(cacheKey);
                await _redisDb.StringSetAsync(cacheKey, JsonConvert.SerializeObject(authors), TimeSpan.FromMinutes(5));
                await _redisDb.SaveCacheKeyTags(cacheKey, model.organizationId, model.promotionId, model.organizationPromotionId);


            }

            return Ok();
        }

        [HttpPost, Route("api/GetWithTagsAuthors")]
        public async Task<IHttpActionResult> GetWithTagsAuthors([FromBody] TagWithOwnerEntityType model)
        {
            //Step 1: Connect to redis server.
            using (ConnectionMultiplexer redisCon = ConnectionMultiplexer.Connect("localhost"))
            {
                //Step 2: Get the reference of the redis database using the redis connection.
                IDatabase redisDbConnection = redisCon.GetDatabase();

                await redisDbConnection.FindAllCacheKeyByTag(model.ownerContextType, model.ownerEntityId);
            }

            return Ok();
        }

        #endregion
    }
}
