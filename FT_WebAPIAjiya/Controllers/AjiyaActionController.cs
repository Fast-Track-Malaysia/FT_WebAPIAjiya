using FT_WebAPIAjiya.Extensions;
using FT_WebAPIAjiya.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using FT_WebAPIAjiya.Helpers;

namespace FT_WebAPIAjiya.Controllers
{

    public class AjiyaActionController : Controller
    {
        private string logFileTitle { get; set; }
        private string logFilePath { get; set; }
        private Helpers.AjiyaLogFunction writeLog { get; set; }

        private readonly IConfiguration configuration;

        public AjiyaActionController(IConfiguration configuration)
        {
            this.configuration = configuration;
            
            //get the log file path from appsettings.json
            logFileTitle = configuration.GetSection("AjiyaLogFile").GetValue<string>("LogFileTitle");
            logFilePath = configuration.GetSection("AjiyaLogFile").GetValue<string>("LogFilePath");

            writeLog = new AjiyaLogFunction
            {
                logFileTitle = logFileTitle,
                logFilePath = logFilePath
            };

        }
        [Authorize]
        [HttpPost]
        [Route("api/AjiyaAction/{mytype}")]
        public IActionResult AjiyaAction(string mytype, [FromBody] List<object> myobjects)
        {
            string conn_str = configuration.GetConnectionString("Default");
            writeLog.WriteLog($"Log", $"Web API started", $"SENDING");
            string myjson = JsonConvert.SerializeObject(myobjects);

            using (var conn = new SqlConnection(conn_str))
            {
               try
                {
                    
                    string query = "exec sp_execaction @type, @json";

                    conn.Query(query, new { type = mytype, json = myjson });
                    writeLog.WriteLog($"Log", $"sending Type: [{mytype}] JSON: {myjson}", $"INPUT");

                    //EXAMPLE JSON TO PARSE IN SWAGGER API: Format as following below:// dun miss the curly brackets
                    //[{"QuotationNo":"12312","CardCode":"C1231"}]
                    //^ above JSON has successfully added to the database
                }
                catch (Exception ex) 
                {
                    writeLog.WriteLog($"Log", $"send failed Type: [{mytype}] JSON: {myjson}", $"FAILED. Error: True, Error Message: {ex.Message}");
                    return BadRequest(new {error= true ,errormessage = ex.Message });
                       
                }
                

            }
            writeLog.WriteLog($"Log", $"send failed Type: {mytype} JSON: {myjson}", $"SUCCESS");
            return Ok(new { error = false, errormessage = "" });
        }

        

    }
}
