using FT_WebAPIAjiya.Extensions;
using FT_WebAPIAjiya.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;

namespace FT_WebAPIAjiya.Controllers
{

    public class AjiyaActionController : Controller
    {

        private readonly IConfiguration configuration;

        public AjiyaActionController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [Authorize]
        [HttpPost]
        [Route("api/AjiyaAction/{mytype}")]
        public IActionResult AjiyaAction(string mytype, [FromBody] List<object> myobjects)
        {
            string conn_str = configuration.GetConnectionString("Default");
            
            using (var conn = new SqlConnection(conn_str))
            {
               try
                {
                    string myjson = JsonConvert.SerializeObject(myobjects);
                    string query = "exec sp_execaction @type, @json";

                    conn.Query(query, new { type = mytype, json = myjson });

                    //EXAMPLE JSON TO PARSE IN SWAGGER API: Format as following below:// dun miss the curly brackets
                    //[{"QuotationNo":"12312","CardCode":"C1231"}]
                    //^ above JSON has successfully added to the database
                }
                catch (Exception ex) 
                {
                   
                  return BadRequest(new {error= true ,errormessage = ex.Message });
                       
                }
                

            }

            //return Ok($"Inserted {quotation.QuotationNo} successfully");
            //return Ok($"Received {myobjects.Count} object ");
            return Ok(new { error = false, errormessage = "" });
        }

        

    }
}
