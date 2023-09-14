using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FT_WebAPIAjiya.Helpers
{
    public class AjiyaLogFunction
    {
        public string logFileTitle { get; set; }
        public string logFilePath { get; set; }
        public void WriteLog(string lvl, string msg, string msgStatus)
        {
            if (string.IsNullOrWhiteSpace(logFileTitle) || string.IsNullOrWhiteSpace(logFilePath)) return;

            FileStream fileStream = null;
            string filePath = "";
            filePath = logFilePath;
            //set the log file creation name
            filePath = filePath + "[" + logFileTitle + "] Log_" + System.DateTime.Today.ToString("yyyyMMdd") + "." + "txt";

            //create file 
            FileInfo fileInfo = new FileInfo(filePath);
            DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);

            //check if file directory exist, if not create a new text file
            if (!dirInfo.Exists) dirInfo.Create();

            if (!fileInfo.Exists)
            {
                fileStream = fileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(filePath, FileMode.Append);
            }

            StreamWriter log = new StreamWriter(fileStream);
            if (string.IsNullOrWhiteSpace(lvl)) lvl = "Log";

            //write log line, the curly bracket numbers below represent the index of the parameters, basically how u want to arrange your log sequence
            log.WriteLine("[{0}-{2}][{3}]{1}", lvl, msg, DateTime.Now.ToString("yyyyMMdd HH:mm:ss"), msgStatus);

            log.Close();
        }
    }
}
