using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwaggerTest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TestController(IConfiguration config)
        {
            this._configuration = config;
        }

        // GET: api/<TestController>
        /// <summary>
        /// Get Store Room Items
        /// </summary>
        /// <returns></returns>
        /// <remarks>List down all matched store room items</remarks>
        [HttpGet]
        public IEnumerable<StoreItem> Get()
        {
            string constr = _configuration.GetConnectionString("phadb");

            List<StoreItem> storeitems = new List<StoreItem>();
            string query = "select top 1000 item_description, item_code, new_item_code from dbo.storeroomitem$ where new_item_code is not null";
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            storeitems.Add(new StoreItem
                            {
                                item_name = Convert.ToString(sdr["item_description"]),
                                old_item_code = Convert.ToString(sdr["item_code"]),
                                new_item_code = Convert.ToString(sdr["new_item_code"])
                            });
                        }
                    }
                    con.Close();
                }
            }

            return storeitems.ToArray();
        }

        /// <summary>
        /// Get Store Room Items by Name
        /// </summary>
        /// <returns></returns>
        /// <remarks>Search matched store room items</remarks>
        /// <param name="name" example="ALSOFT">Store Room Item Description</param>
        [HttpGet("{name}")]
        public IEnumerable<StoreItem> GetByName(string name)
        {
            string constr = _configuration.GetConnectionString("phadb");

            List<StoreItem> storeitems = new List<StoreItem>();
            string query = "select top 100 item_description, item_code, new_item_code from dbo.storeroomitem$ where new_item_code is not null and item_description like '%" + name + "%'";
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            storeitems.Add(new StoreItem
                            {
                                item_name = Convert.ToString(sdr["item_description"]),
                                old_item_code = Convert.ToString(sdr["item_code"]),
                                new_item_code = Convert.ToString(sdr["new_item_code"])
                            });
                        }
                    }
                    con.Close();
                }
            }

            return storeitems.ToArray();
        }

        /// <summary>
        /// Get Store Room Items by Item Code
        /// </summary>
        /// <returns></returns>
        /// <remarks>Search matched store room items by item code</remarks>
        /// <param name="itemcode" example="MPHTC00489">Store Room Item New Code</param>
        /// <param name="active" example="Y">is Active (Y/N)</param>
        [HttpGet("{itemcode},{active}")]
        public IEnumerable<StoreItem> GetByCode(string itemcode, string active)
        {
            string constr = _configuration.GetConnectionString("phadb");

            List<StoreItem> storeitems = new List<StoreItem>();
            string query = "select top 1 item_description, item_code, new_item_code from dbo.storeroomitem$ where new_item_code is not null and new_item_code = '" + itemcode + "' and stock_active_ind = '" + active + "'";
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            storeitems.Add(new StoreItem
                            {
                                item_name = Convert.ToString(sdr["item_description"]),
                                old_item_code = Convert.ToString(sdr["item_code"]),
                                new_item_code = Convert.ToString(sdr["new_item_code"])
                            });
                        }
                    }
                    con.Close();
                }
            }

            return storeitems.ToArray();
        }

        // POST api/<TestController>
        /// <summary>
        /// Create To Do
        /// </summary>
        /// <returns></returns>
        /// <remarks>Create new To Do list</remarks>
        /// <param name="title" example="Title">Title</param>
        /// <param name="desc" example="Description">Description</param>
        /// <param name="isdone" example="0">is Done (1/0)</param>
        [HttpPost]
        public IActionResult Post(string title, string desc, int isdone)
        {
            string constr = _configuration.GetConnectionString("testdb");

            string query = "insert into todo(title, todo_desc, is_done) values('" + title + "','" + desc + "','" + isdone + "')";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                return Ok("Created!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<TestController>/5
        /// <summary>
        /// Update To Do
        /// </summary>
        /// <returns></returns>
        /// <remarks>Update To Do list</remarks>
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ToDo todo)
        {
            string constr = _configuration.GetConnectionString("testdb");

            string query = "update todo set title = '" + todo.title + "', todo_desc = '" + todo.desc + "', is_done = " + todo.isdone + " where id = " + id + "";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                return Ok("Updated!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<TestController>/5
        /// <summary>
        /// Delete To Do
        /// </summary>
        /// <returns></returns>
        /// <remarks>Delete To Do list</remarks>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string constr = _configuration.GetConnectionString("testdb");

            string query = "delete from todo where id = " + id + "";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                return Ok("Deleted!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Roll
        /// </summary>
        /// <returns></returns>
        /// <remarks>Roll</remarks>
        [HttpGet("{roll:bool}")]
        public string StartRoll(bool roll)
        {
            Random _random = new Random();
            if (roll == true)
                return _random.Next(0, 100).ToString();
            else
                return "🐱‍💻";
        }

        /// <summary>
        /// Get Weather Temperature
        /// </summary>
        /// <returns></returns>
        /// <remarks>Get Temperature by location name</remarks>
        [HttpGet("{location}")]
        public IActionResult getWeather(string location)
        {
            var url = $"https://www.metaweather.com/api/location/search/?query=" + location;
            var webClient = new WebClient();
            string jsonData = "";
            string jsonData2 = "";
            Location jsonObject = new Location();

            try
            {
                jsonData = webClient.DownloadString(url);
                jsonData = jsonData.Replace("[","");
                jsonData = jsonData.Replace("]","");
                jsonObject = JsonConvert.DeserializeObject<Location>(jsonData);

                if (jsonObject == null)
                    return BadRequest("Location not found! 🤦‍♂️");

                var url2 = $"https://www.metaweather.com/api/location/" + jsonObject.woeid;
                jsonData2 = webClient.DownloadString(url2);

                var jo = JObject.Parse(jsonData2);
                var id = jo["consolidated_weather"][0]["the_temp"].ToString();

                return Ok("✔ Temperature - " + id);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Currency Converter
        /// </summary>
        /// <returns></returns>
        /// <remarks>Convert value</remarks>
        [HttpGet("{base_currency}")]
        public IActionResult getCurrency(string base_currency, string convert_currency, double value)
        {
            var url = $"https://freecurrencyapi.net/api/v2/latest?apikey=fd9ac640-3d37-11ec-b8a0-a9d74dd0bb19&base_currency=" + base_currency;
            var webClient = new WebClient();
            string jsonData = "";

            try
            {
                jsonData = webClient.DownloadString(url);

                var jo = JObject.Parse(jsonData);
                var currency_rate = jo["data"][convert_currency].ToString();

                var convertvalue = value * Convert.ToDouble(currency_rate);

                return Ok("✔ Currency Rate - " + convertvalue);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get ISS location
        /// </summary>
        /// <returns></returns>
        /// <remarks>ISS current location</remarks>
        [HttpGet]
        public IActionResult getISSCurrentLocation()
        {
            var url = $"http://api.open-notify.org/iss-now.json";
            var webClient = new WebClient();
            string jsonData = "";

            try
            {
                jsonData = webClient.DownloadString(url);

                var jo = JObject.Parse(jsonData);
                var latitude = jo["iss_position"]["latitude"].ToString();
                var longitude = jo["iss_position"]["longitude"].ToString();

                string issurl = "http://maps.google.com/maps?q=" + latitude + "," + longitude;

                return Ok("✔ ISS Location - " + issurl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get IP Address
        /// </summary>
        /// <returns></returns>
        /// <remarks>Current IP Address</remarks>
        [HttpGet]
        public IActionResult getCurrentIPAddress()
        {
            var url = $"https://api.myip.com";
            var webClient = new WebClient();
            string jsonData = "";

            try
            {
                jsonData = webClient.DownloadString(url);

                var jo = JObject.Parse(jsonData);
                var ip = jo["ip"].ToString();
                var country = jo["country"].ToString();

                return Ok("🔑 IP Server Address - " + ip + "\n" + "🏠 Country - " + country);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class StoreItem
    {
        public string item_name { get; set; }
        public string old_item_code { get; set; }
        public string new_item_code { get; set; }
    }

    public class ToDo
    {
        public string title { get; set; }
        public string desc { get; set; }
        public int isdone { get; set; }
    }

    public class Location
    {
        public string title { get; set; }
        public string location_type { get; set; }
        public int woeid { get; set; }
        public string latt_long { get; set; }
    }
}
