using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
//using System.Web.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwaggerTest.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("{roll:bool}")]
        public string StartRoll(bool roll)
        {
            Random _random = new Random();
            if (roll == true)
                return _random.Next(0, 100).ToString();
            else
                return "🐱‍💻";
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
}
